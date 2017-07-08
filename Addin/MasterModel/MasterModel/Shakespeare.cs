using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using Inventor;
using System.IO;
using System.Text;
using File = System.IO.File;


namespace InvAddIn
{
    /*
        http://help.autodesk.com/view/INVNTOR/2018/ENU/?guid=GUID-311EC780-354E-4A58-9639-3E186755A498
        https://en.wikibooks.org/wiki/OpenJSCAD_User_Guide
    */

    public class Shakespeare
    {
        //main lists
        private List<string> listOfCodeLines = new List<string>();
        public static List<Parameter> ListOfParameter = new List<Parameter>();

        public static List<string> listOfEntityNamesOfOneSketch = new List<string>();
        private List<ExtrudeFeature> listOfObjects;
        private List<string> listOfObjectNames = new List<string>();

        //temporary lists
        private List<SketchLine> listOfSketchLines = new List<SketchLine>();

        //other variables:
        //setting culture to invariant so it prints 0.001 instead of german style: 0,001
        CultureInfo myCultureInfo = new CultureInfo("en-GB");
        public static int NumberOfSketchEntities;
        private static int _numberOfSketches;
        private static int _numberOfSubtractions;
        private static int _numberOfIntersections;
        private bool _needToInterpreteSketchLine = false;
        private string endVar = "OskarTheGreat";
        private static double factor = 10;

        //for testing purposes use desktop path, we can also use path chosen by user or path that directly saves the js-file into the web-app folder
        private static string desktopPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
        private static string _jscadPath = desktopPath + "\\example.js";


        //constructor
        public Shakespeare(MasterM masterModel, string savePathChosenByUser)
        {
            _jscadPath = savePathChosenByUser;

            //clear everything at start
            ListOfParameter.Clear();
            listOfEntityNamesOfOneSketch.Clear();
            listOfCodeLines.Clear();
            listOfObjectNames.Clear();

            //get list of partFeatures
            listOfObjects = masterModel.GetExtrudeFeatures();
            NumberOfSketchEntities = 1;
            _numberOfSketches = 0;
            _numberOfSubtractions = 0;
            _numberOfIntersections = 0;

            GenerateMainFunction();
            GenerateParameterFunction();
            WriteIntoJsFile();

        } //end of constructor 

        private void GenerateMainFunction()
        {
            listOfCodeLines.Add("function main(params) {");
            listOfCodeLines.Add("");

            InterpretePartFeatures();
            listOfCodeLines.Add(UnionAllObjects(endVar));

            listOfCodeLines.Add("");
            listOfCodeLines.Add("\t" + "return " + endVar + ";");
            listOfCodeLines.Add("}");
            listOfCodeLines.Add("");

        } //end of method GenerateMainFunction

        private void InterpretePartFeatures()
        {
            foreach (var partFeature in listOfObjects)
            {

                //create Sketch
                _numberOfSketches++;
                listOfCodeLines.Add("\t" + "//Sketch" + _numberOfSketches + ": ");
                Sketch actualSketch = partFeature.Profile.Parent;
                InterpreteSketch(actualSketch);
                listOfCodeLines.Add(UnionSketch());

                //do the magic -> make it threedimensional
                InterpretePartFeature(partFeature);

                listOfCodeLines.Add("");
                listOfEntityNamesOfOneSketch.Clear();
            }
        } //end of method InterpretePartFeatures

        private void InterpretePartFeature(ExtrudeFeature partFeature)
        {
            string firstObjectName = partFeature.Name;
            string secondObjectName = "";

            switch (partFeature.Operation)
            {
                //this is new extrusion or revolve
                case PartFeatureOperationEnum.kNewBodyOperation:
                    CreateObject(partFeature);
                    listOfObjectNames.Add(firstObjectName);
                    break;

                case PartFeatureOperationEnum.kJoinOperation:
                    CreateObject(partFeature);
                    listOfObjectNames.Add(firstObjectName);
                    break;

                //this case will create a new element aswell but it gets then cutted with another object
                case PartFeatureOperationEnum.kCutOperation:
                    CreateObject(partFeature);

                    //TODO get other object name
                    //secondObjectName = ...
                    _numberOfSubtractions++;
                    listOfObjectNames.Remove(secondObjectName);
                    listOfObjectNames.Add("Subtraction" + _numberOfSubtractions);
                    CutOperation("Extrusion1", firstObjectName);
                    //TODO remove both object names out of listOfObjectNames, add one name
                    break;

                //this case will create a new element aswell but it gets then intersected with another object
                case PartFeatureOperationEnum.kIntersectOperation:
                    CreateObject(partFeature);

                    //TODO get other object name
                    //secondObjectName = ...
                    _numberOfIntersections++;
                    listOfObjectNames.Remove(secondObjectName);
                    listOfObjectNames.Add("Intersection" + _numberOfIntersections);
                    IntersectOperation(partFeature.Name, firstObjectName);
                    break;
            }





            //TODO
            //find out in which layer the sketch is orientated, eg. XY, XZ or YZ
            //then rotate sketch if possible
            /*
            //check for axis
            //if yes then rotate
            if (_numberOfSketches == 1)
            {
                RotateObject("x");
            }


            //check for translation
            bool needToTranslate = true;
            if (needToTranslate)
            {
                StringBuilder lastLineOfCode = new StringBuilder(listOfCodeLines.Last());
                listOfCodeLines.RemoveAt(listOfCodeLines.Count - 1);

                //remove semicolon at end of line
                lastLineOfCode.Remove(lastLineOfCode.Length - 1, 1);

                //TODO
                //get value out of sketch or partFeature?!

                lastLineOfCode.Append(TranslateObject("z", value));
                listOfCodeLines.Add(lastLineOfCode.ToString());
            }
            */

        } //end of method InterpretePartFeature

        private void InterpreteSketch(Sketch actualSketch) 
        {
                List<SketchEntity> sketchEntities = MasterM.GetSketchParts(actualSketch);
                foreach (SketchEntity sketchEntity in sketchEntities)
                {
                    InterpreteSketchEntity(sketchEntity);
                }

                //interprete sketchLine list of one sketch
                if (_needToInterpreteSketchLine)
                {
                    listOfCodeLines.Add(Exporter.ExportPolygon(listOfSketchLines, NumberOfSketchEntities));
                    _needToInterpreteSketchLine = false;
                    listOfSketchLines.Clear();
                }

                // for text
                foreach(TextBox tbox in actualSketch.TextBoxes)
                {
                    string entityType = "text";
                    listOfCodeLines.Add(Exporter.ExportText(tbox, entityType + NumberOfSketchEntities));
                    listOfEntityNamesOfOneSketch.Add(entityType + NumberOfSketchEntities);
                    NumberOfSketchEntities++;
                }


        } //end of method InterpreteSketch

        private void InterpreteSketchEntity(SketchEntity sketchEntity)
        {

            String entityType = "";
            if (sketchEntity is SketchPoint)
            {
                //sketchPoints are just optional?
                //skip it
                return;
            }

            if (sketchEntity is SketchLine)
            {
                //get multiple sketchLine's, put them in list, at end of sketch, interprete all sketchlines
                _needToInterpreteSketchLine = true;
                listOfSketchLines.Add((SketchLine) sketchEntity);
                return;

            }

            if (sketchEntity is SketchCircle)
            {
                entityType = "circle";
                listOfCodeLines.Add(Exporter.ExportCircle((SketchCircle) sketchEntity, entityType + NumberOfSketchEntities));
            }
            else if (sketchEntity is SketchArc)
            {
                entityType = "arc";
                listOfCodeLines.Add(Exporter.ExportArc((SketchArc)sketchEntity, entityType + NumberOfSketchEntities));
            }
            else if (sketchEntity is SketchEllipse)
            {
                entityType = "ellipse";
                listOfCodeLines.Add(Exporter.ExportEllipseFull((SketchEllipse)sketchEntity, entityType + NumberOfSketchEntities));
            }
            else if (sketchEntity is SketchEllipticalArc)
            {
                entityType = "ellipseArc";
                listOfCodeLines.Add(Exporter.ExportEllipticalArc((SketchEllipticalArc)sketchEntity, entityType + NumberOfSketchEntities));

            }
            listOfEntityNamesOfOneSketch.Add(entityType + NumberOfSketchEntities);
            NumberOfSketchEntities++;


        } //end of method InterpreteSketchEntity

        private void GenerateParameterFunction()
        {
            if (ListOfParameter.Count == 0)
            {
                return;
            }
            else
            {
                listOfCodeLines.Add("function getParameterDefinitions() {");
                listOfCodeLines.Add("\treturn [");

                foreach (Parameter parameter in ListOfParameter)
                {
                    listOfCodeLines.Add(parameter.GetParameterString() + ",");
                }

                //optional: remove the last comma of last element of list
                //outputParamDef.Remove(outputParamDef.Length - 1, 1);

                listOfCodeLines.Add("\t];");
                listOfCodeLines.Add("}");
            }


        } //end of method GenerateParameterFunction

        private void WriteIntoJsFile()
        {
            //check for existing file
            if (File.Exists(_jscadPath))
                File.Delete(_jscadPath);

            using (StreamWriter outputFile = new StreamWriter(_jscadPath, true))
            {
                foreach (var codeLine in listOfCodeLines)
                {
                    outputFile.WriteLine(codeLine);
                }
            }
        } //end of method WriteIntoJSFile


        private string UnionSketch()
        {
            StringBuilder unionLine = new StringBuilder();
            var lastEntity = listOfEntityNamesOfOneSketch.Last();

            if (listOfEntityNamesOfOneSketch.Count == 0)
            {
                unionLine.Clear();
            }
            else if (listOfEntityNamesOfOneSketch.Count == 1)
            {
                unionLine.Append("\t" + "var sketch" + _numberOfSketches + " = " + lastEntity + ";");
            }
            else
            {
                unionLine.Append("\t" + "var sketch" + _numberOfSketches + " = union(");

                foreach (var entityName in listOfEntityNamesOfOneSketch)
                {
                    if (entityName == lastEntity)
                    {
                        unionLine.Append(entityName);
                    }
                    else
                    {
                        unionLine.Append(entityName + ", ");
                    }
                }
                unionLine.Append(");");    
            }
            return unionLine.ToString();

        } //end of method UnionSketch

        private void CutOperation(string actualVar, string oldVar)
        {
            //sphere1 = sphere1.subtract(cube1); 
            listOfCodeLines.Add("\t" + "var Subtraction" + _numberOfSubtractions + " = " + actualVar + ".subtract(" + oldVar + ");");
        } //end of method CutOperation

        private void IntersectOperation(string actualVar, string oldVar)
        {
            //sphere 1 = sphere1.intersect(cube1);
            listOfCodeLines.Add("\t" + "var " + _numberOfIntersections + " = "+ actualVar + ".intersect(" + oldVar + ");");
        } //end of method IntersectOperation

        private void CreateObject(ExtrudeFeature partFeature)
        {
            //check if partFeature is revolveFeature or extrudeFeature
            if (partFeature.Type == ObjectTypeEnum.kExtrudeFeatureObject)
            {
                listOfCodeLines.Add(ExtrudeSketch((ExtrudeFeature)partFeature));
            }
            else if (partFeature.Type == ObjectTypeEnum.kRevolveFeatureObject)
            {
                listOfCodeLines.Add(RevolveSketch((RevolveFeature)partFeature));
            }
        } //end of method CreateObject

        private string ExtrudeSketch(ExtrudeFeature extrudeFeature)
        {
            StringBuilder extrusionLine = new StringBuilder();
            MasterM.ExtrudeDirection direction = MasterM.GetDirection(extrudeFeature);
            string objectName = extrudeFeature.Name;

            if (direction == MasterM.ExtrudeDirection.Positive)
            {
                if (extrudeFeature.ExtentType == PartFeatureExtentEnum.kDistanceExtent)
                {
                    Inventor.Parameter param = (extrudeFeature.Definition.Extent as DistanceExtent).Distance;
                    double height = param._Value * factor;
                    string name = "Extrusion" + _numberOfSketches;
                    string paramString = "params." + name;
                    Parameter extrusionParameter = new Parameter(name, "Length of " + name, "float", height, 0.1);
                    ListOfParameter.Add(extrusionParameter);

                    //extrusionLine.Append("\t" + "var " + objectName + " = sketch" + _numberOfSketches + ".extrude({ offset: [0,0," + height.ToString(myCultureInfo) + "] });");
                    extrusionLine.Append("\t" + "var " + objectName + " = sketch" + _numberOfSketches + ".extrude({ offset: [0,0," + paramString + "] });");

                }
                else
                {
                    //hopefully we wont come to this point
                    //extruding with 10
                    extrusionLine.Append("\t" + "var " + objectName + " = sketch" + _numberOfSketches + ".extrude({ offset: [0,0,10] });");
                }
            }
            else if (direction == MasterM.ExtrudeDirection.Negative)
            {
                if (extrudeFeature.ExtentType == PartFeatureExtentEnum.kDistanceExtent)
                {
                    //because you cant choose a direction we extrude the same direction but then translating in the reversed direction
                    Inventor.Parameter param = (extrudeFeature.Definition.Extent as DistanceExtent).Distance;

                    double height = param._Value * factor;
                    string name = "Extrusion" + _numberOfSketches;
                    string paramString = "params." + name;
                    Parameter extrusionParameter = new Parameter(name, "Length of " + name, "float", height, 0.1);
                    ListOfParameter.Add(extrusionParameter);

                    //extrusionLine.Append("\t" + "var " + objectName + " = sketch" + _numberOfSketches + ".extrude({ offset: [0,0," + height.ToString(myCultureInfo) + "] });");
                    extrusionLine.Append("\t" + "var " + objectName + " = sketch" + _numberOfSketches + ".extrude({ offset: [0,0," + paramString + "] })");

                    //parameter:
                    extrusionLine.Append(TranslateObject("z", paramString, true));
                    //without parameter
                    //extrusionLine.Append(TranslateObject("z", height.ToString(myCultureInfo), true));
                }
                else
                {
                    //hopefully we wont come to this point :D
                    //extruding with 10
                    extrusionLine.Append("\t" + "var " + objectName + " = sketch" + _numberOfSketches + ".extrude({ offset: [0,0,10] });");
                }
            }
            else //if(direction == ExtrudeDirection.Symetric)
            {
                //translate distance/2
                //extrude distance
            }

            return extrusionLine.ToString();
        } //end of method ExtrudeSketch

        private string RevolveSketch(RevolveFeature revolveFeature)
        {
            //rotation is always around the z-axis
            //var object = rotate_extrude({fn:4}, sketch);

            //do we need revolveFeature as parameter here?
            //we can vary fn?

            string objectName = revolveFeature.Name;

            StringBuilder rotationLine = new StringBuilder();
            rotationLine.Append("\t" + "var " + objectName + " = rotate_extrude(sketch" + _numberOfSketches + ");");
            rotationLine.Append("\n");
            rotationLine.Append("\t" + objectName + " = " + objectName + ".rotateX(-90);");

            return rotationLine.ToString();

        } //end of method RevolveSketch

        private string UnionAllObjects(string varName)
        {
            StringBuilder extrusionLine = new StringBuilder();
            int length = listOfObjectNames.Count;

            if (length == 0)
            {
                extrusionLine.Clear();
            }
            else if (length == 1)
            {
                extrusionLine.Append("\t" + "var " + varName + " = " + listOfObjectNames[0] + ";");
            }
            else
            {
                extrusionLine.Append("\t" + "var " + varName + " = union(");

                for (int i = 0; i < length; i++)
                {
                    if (i == length-1)
                    {
                        extrusionLine.Append(listOfObjectNames[i]);
                    }
                    else
                    {
                        extrusionLine.Append(listOfObjectNames[i] + ", ");
                    }

                }
                extrusionLine.Append(");");
                
            }
            return extrusionLine.ToString();

        } //end of method UnionAllObjects

        private void RotateObject(string layer)
        {
            //todo
            //this method switches the layer in which the sketch is positioned

            StringBuilder lastLine = new StringBuilder(listOfCodeLines.Last());
            listOfCodeLines.RemoveAt(listOfCodeLines.Count - 1);

            //remove semicolon at end of line
            lastLine.Remove(lastLine.Length - 1, 1);

            //switching XY -> XZ layer, rotating along x-axis 
            if (layer == "x")
            {
                lastLine.Append(".rotateX(-90);");
            }
            //switching XY -> YZ layer, rotating along x-axis 
            else if (layer == "y")
            {
                lastLine.Append(".rotateY(-90);");
            }
            //switching XZ -> YZ layer, rotating along x-axis 
            else if (layer == "z")
            {
                lastLine.Append(".rotateZ(-90);");
            }

            listOfCodeLines.Add(lastLine.ToString());
        }

        private string TranslateObject(string axis, string value, bool withNegativeFactor)
        {
            //translate only works for 3D objects, after sketches got extruded/revolved

            if (withNegativeFactor)
            {
                switch (axis)
                {
                    case "z":
                        return (".translate([0,0,-1*" + value + "]);");
                    case "y":
                        return (".translate([0,-1*" + value + ",0]);");
                    case "x":
                        return (".translate([-1*" + value + ",0,0]);");
                    default:
                        return "";
                }
            }
            else
            {
                switch (axis)
                {
                    case "z":
                        return (".translate([0,0," + value + "]);");
                    case "y":
                        return (".translate([0," + value + ",0]);");
                    case "x":
                        return (".translate([" + value + ",0,0]);");
                    default:
                        return "";
                }
            }



        }


    } //end of class Shakespeare
} //end of namespace InvAddIn
