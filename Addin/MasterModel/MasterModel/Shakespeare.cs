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
        //consists of extrudeFeatures and revolveFeatures
        private List<object> listOfObjects;
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

            //get list of partFeatures (extrudeFeatures & RevolveFeature)
            listOfObjects = masterModel.GetFeatures();
            NumberOfSketchEntities = 1;
            _numberOfSketches = 1;
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
                Sketch actualSketch;

                //could not find a simpler method to determine which type the object is
                if (Microsoft.VisualBasic.Information.TypeName(partFeature) == "ExtrudeFeature")
                {
                    var extrudeFeature = (ExtrudeFeature) partFeature;
                    actualSketch = extrudeFeature.Profile.Parent;
                }
                else if (Microsoft.VisualBasic.Information.TypeName(partFeature) == "RevolveFeature")
                {
                    var revolveFeature = (RevolveFeature) partFeature;
                    actualSketch = revolveFeature.Profile.Parent;
                }
                else
                {
                    //do not interprete because we just cant...
                    continue;
                }

                //create Sketch
                listOfCodeLines.Add("\t" + "//Sketch" + _numberOfSketches + ": ");
                InterpreteSketch(actualSketch);
                listOfCodeLines.Add(UnionSketch());
                listOfEntityNamesOfOneSketch.Clear();

                //do the magic -> make the sketch threedimensional
                InterpretePartFeature(partFeature);

                listOfCodeLines.Add("");
                _numberOfSketches++;
            }
        } //end of method InterpretePartFeatures

        private void InterpretePartFeature(object partFeature)
        {
            switch (Microsoft.VisualBasic.Information.TypeName(partFeature))
            {
                case "ExtrudeFeature":
                {
                    ExtrudeFeature extrudeFeature = (ExtrudeFeature)partFeature;
                    string firstObjectName = extrudeFeature.Name;
                    string secondObjectName;

                    //no matter which operation will be executed, object gets created
                    listOfCodeLines.Add(ExtrudeSketch(extrudeFeature));

                    switch (extrudeFeature.Operation)
                    {
                        //this is new extrusion or revolve
                        case PartFeatureOperationEnum.kNewBodyOperation:
                            listOfObjectNames.Add(firstObjectName);
                            break;

                        case PartFeatureOperationEnum.kJoinOperation:
                            listOfObjectNames.Add(firstObjectName);
                            break;

                        //this case uses new created object and will cut it with another object
                        case PartFeatureOperationEnum.kCutOperation:

                            //TODO get other object name(s), which interfere with object
                            //method kinda works. not returning the name i am using
                            secondObjectName = string.Join(",", MasterM.GetAffectedBodyNames(extrudeFeature));

                            //old object gets deleted, new subtraction gets created which includs both objects
                            _numberOfSubtractions++;
                            listOfObjectNames.Remove(secondObjectName);
                            listOfObjectNames.Add("Subtraction" + _numberOfSubtractions);
                            CutOperation(secondObjectName, firstObjectName);
                            break;

                        //this case uses new created object and will intersect it with another object
                        case PartFeatureOperationEnum.kIntersectOperation:

                            //method kinda works. not returning the name i am using
                            secondObjectName = string.Join(",", MasterM.GetAffectedBodyNames(extrudeFeature));

                            //old object gets deleted, new intersection gets created which includs both objects
                            _numberOfIntersections++;
                            listOfObjectNames.Remove(secondObjectName);
                            listOfObjectNames.Add("Intersection" + _numberOfIntersections);
                            IntersectOperation(secondObjectName, firstObjectName);
                            break;
                    }
                }
                break;
                case "RevolveFeature":
                {
                    //revolving is not really working like in inventor. openjscad is always revolving around y-axis
                    //so when revolveFeature is used we will create it but it will be at a different spot
                    //and maybe interfere with another object
                    var revolveFeature = (RevolveFeature)partFeature;
                    string firstObjectName = revolveFeature.Name;

                    //no matter which operation will be executed, object gets created
                    listOfCodeLines.Add(RevolveSketch(revolveFeature));

                    switch (revolveFeature.Operation)
                    {
                        case PartFeatureOperationEnum.kNewBodyOperation:
                            listOfObjectNames.Add(firstObjectName);
                            break;

                        case PartFeatureOperationEnum.kJoinOperation:
                            listOfObjectNames.Add(firstObjectName);
                            break;

                        case PartFeatureOperationEnum.kCutOperation:
                            listOfObjectNames.Add(firstObjectName);
                            break;
                        case PartFeatureOperationEnum.kIntersectOperation:
                            listOfObjectNames.Add(firstObjectName);
                            break;
                    }
                }
                break;
            }

            /*
            //TODO
            //find out in which layer the sketch is orientated, eg. XY, XZ or YZ
            //then rotate sketch if possible

            bool needToRotate = true;
            if (rotationIsNeeded)
            {
                AppendToLastLineOfCode(RotateObject("x"));
                
            }

            //if sketch is not on BasePlane it has to be translated
            bool needToTranslate = true;
            if (needToTranslate)
            {
                AppendToLastLineOfCode(TranslateObject("z", value, false));
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
            //intersection1 = sphere1.intersect(cube1);
            listOfCodeLines.Add("\t" + "var Intersection" + _numberOfIntersections + " = "+ actualVar + ".intersect(" + oldVar + ");");
        } //end of method IntersectOperation

        private string ExtrudeSketch(ExtrudeFeature extrudeFeature)
        {
            StringBuilder extrusionLine = new StringBuilder();
            MasterM.ExtrudeDirection direction = MasterM.GetDirection(extrudeFeature);
            string objectName = extrudeFeature.Name;
            string name = "Extrusion" + _numberOfSketches;

            if (direction == MasterM.ExtrudeDirection.Positive)
            {
                if (extrudeFeature.ExtentType == PartFeatureExtentEnum.kDistanceExtent)
                {
                    //get length of extrusion
                    Inventor.Parameter param = (extrudeFeature.Definition.Extent as DistanceExtent).Distance;
                    double length = param._Value * factor;
                    string paramString = "params." + name;
                    Parameter extrusionParameter = new Parameter(name, "Length of " + name, "float", length, 0.1);
                    ListOfParameter.Add(extrusionParameter);

                    //with parameter:
                    extrusionLine.Append("\t" + "var " + objectName + " = sketch" + _numberOfSketches + ".extrude({ offset: [0,0," + paramString + "] });");
                    //without parameter:
                    //extrusionLine.Append("\t" + "var " + objectName + " = sketch" + _numberOfSketches + ".extrude({ offset: [0,0," + height.ToString(myCultureInfo) + "] });");
                }
                else //default: extruding with 10
                { 
                    extrusionLine.Append("\t" + "var " + objectName + " = sketch" + _numberOfSketches + ".extrude({ offset: [0,0,10] });");
                }
            }
            else if (direction == MasterM.ExtrudeDirection.Negative)
            {
                if (extrudeFeature.ExtentType == PartFeatureExtentEnum.kDistanceExtent)
                {
                    //because when extruding in openjscad you cant choose a direction 
                    //we extrude in the same direction but then translating same length in reversed direction

                    //get length of extrusion
                    Inventor.Parameter param = (extrudeFeature.Definition.Extent as DistanceExtent).Distance;
                    double length = param._Value * factor;
                    string paramString = "params." + name;
                    Parameter extrusionParameter = new Parameter(name, "Length of " + name, "float", length, 0.1);
                    ListOfParameter.Add(extrusionParameter);

                    //with parameter:
                    extrusionLine.Append("\t" + "var " + objectName + " = sketch" + _numberOfSketches + ".extrude({ offset: [0,0," + paramString + "] })");
                    extrusionLine.Append(TranslateObject("z", paramString, true));

                    //without parameter:
                    //extrusionLine.Append("\t" + "var " + objectName + " = sketch" + _numberOfSketches + ".extrude({ offset: [0,0," + height.ToString(myCultureInfo) + "] });");
                    //extrusionLine.Append(TranslateObject("z", height.ToString(myCultureInfo), true));
                }
                else //default: extruding with 10
                {
                    extrusionLine.Append("\t" + "var " + objectName + " = sketch" + _numberOfSketches + ".extrude({ offset: [0,0,10] });");
                }
            }
            else //if(direction == ExtrudeDirection.Symetric)
            {
                //extrude distance
                //translate distance/2
            }

            return extrusionLine.ToString();
        } //end of method ExtrudeSketch

        private string RevolveSketch(RevolveFeature revolveFeature)
        {
            //revolving a sketch in openjscad will result in a rotation around the z-axis (y-axis in inventor)
            //so the resulting object will be orientated differently then the one in inventor (probably)

            string objectName = revolveFeature.Name;
            StringBuilder rotationLine = new StringBuilder();

            //var object = rotate_extrude({fn:4}, sketch);
            rotationLine.Append("\t" + "var " + objectName + " = rotate_extrude(sketch" + _numberOfSketches + ");");

            //if we could find out on which plane the sketch was orientated originally
            //we could rotate the new object so it is an the right plane again
            //rotationLine.Append("\n");
            //rotationLine.Append("\t" + objectName + " = " + objectName + RotateObject("x"));

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

        private string RotateObject(string layer)
        {
            //this method switches the layer in which the sketch is positioned

            switch (layer)
            {
                //switching XY -> XZ layer, rotating along x-axis 
                case "x":
                    return(".rotateX(-90);");
                //switching XY -> YZ layer, rotating along y-axis 
                case "y":
                    return (".rotateY(-90);");
                //switching XZ -> YZ layer, rotating along z-axis 
                case "z":
                    return (".rotateZ(-90);");
                default:
                    return ";";
            }
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
                        return ";";
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
                        return ";";
                }
            }



        } //end of method TranslateObject

        private void AppendToLastLineOfCode(string stringToAppend)
        {
            //this method removes lastElement of list
            //the lastElement can then be edited (remove semicolon and append "transformation")
            //and then gets added to list again

            StringBuilder lastLine = new StringBuilder(listOfCodeLines.Last());
            listOfCodeLines.RemoveAt(listOfCodeLines.Count - 1);

            //remove semicolon at end of line
            lastLine.Remove(lastLine.Length - 1, 1);

            lastLine.Append(stringToAppend);

            listOfCodeLines.Add(lastLine.ToString());
        }


    } //end of class Shakespeare
} //end of namespace InvAddIn


/*
old check:
//check if partFeature is revolveFeature or extrudeFeature
            if (partFeature.Type == ObjectTypeEnum.kExtrudeFeatureObject)
            {
                listOfCodeLines.Add(ExtrudeSketch((ExtrudeFeature)partFeature));
            }
            else if (partFeature.Type == ObjectTypeEnum.kRevolveFeatureObject)
            {
                listOfCodeLines.Add(RevolveSketch((RevolveFeature)partFeature));
            }
*/
