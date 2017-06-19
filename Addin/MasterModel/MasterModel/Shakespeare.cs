using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using Inventor;
using System.IO;
using File = System.IO.File;


namespace InvAddIn
{
    /*
        http://help.autodesk.com/view/INVNTOR/2018/ENU/?guid=GUID-311EC780-354E-4A58-9639-3E186755A498
        https://en.wikibooks.org/wiki/OpenJSCAD_User_Guide

        /TODO:
        - export interprete methods, can parameter be added outside of class when public?   check
        - seperate between 2d and 3d entities. need two list for entities?
        - use polygon for SketchLine, write SketchLine case
        - polygones have to be closed by itself not combined with pther entities...
            modify sketchLineInterpretation
            concat all entities

        hints, questions:
        - do we need SketchPoint? at this time they get ignored and it works
        - try out: in one sketch draw two different polylines and check sketch entities (sketchpoint as separation between?)
            profile has information which entities belong to each other in one sketch
    
     */
    public class Shakespeare
    {
        //main lists
        private List<String> listOfCodeLines = new List<string>();
        public static List<Parameter> ListOfParameter = new List<Parameter>();

            
        
        private List<Sketch> listOfSketches;
        public static List<String> listOfEntityNamesOfOneSketch = new List<string>();
        private List<ExtrudeFeature> listOfExtrusions;

        //temporary lists
        private List<SketchLine> listOfSketchLines = new List<SketchLine>();

        //other variables:
        //setting culture to invariant so it prints 0.001 instead of german style: 0,001
        CultureInfo myCultureInfo = new CultureInfo("en-GB");
        public static int numberOfSketchEntities;
        public static int numberOfSketches;
        private bool needToInterpreteSketchLine = false;
        private string endVar = "OskarTheGreat";

        private static string desktopPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
        private static string jscadPath = desktopPath + "\\example.js";


        

        


        //constructor
        public Shakespeare(MasterM masterModel, string savePathChosenByUser)
        {
            //for testing purposes use desktop path, we can also use path chosen by user or path that directly saves the js-file into the web-app folder
            jscadPath = savePathChosenByUser;

            //clear everything at start
            ListOfParameter.Clear();
            listOfEntityNamesOfOneSketch.Clear();
            listOfCodeLines.Clear();

            listOfSketches = masterModel.SketchyList;
            listOfExtrusions = masterModel.GetExtrudeFeatures();
            numberOfSketchEntities = 1;
            numberOfSketches = 0;

            GenerateMainFunction();
            GenerateParameterFunction();
            WriteIntoJsFile();

        } //end of constructor 

        private void GenerateMainFunction()
        {
            listOfCodeLines.Add("function main(params) {");

            InterpreteExtrusions();
            listOfCodeLines.Add(unionAllExtrusion(endVar));
            //return 
            listOfCodeLines.Add("");
            listOfCodeLines.Add("\t" + "return " + endVar + ";");
            listOfCodeLines.Add("}");

        } //end of method GenerateMainFunction

        private void InterpreteExtrusions()
        {
            foreach (var extrusion in listOfExtrusions)
            {
                numberOfSketches++;
                listOfCodeLines.Add("\t" + "//Sketch" + numberOfSketches + ": ");
                //get Sketch
                Sketch actualSketch = extrusion.Profile.Parent;
                InterpreteSketch(actualSketch);

                listOfCodeLines.Add(UnionSketch());
                //extrude sketch (need height here)
                //extrusion.Extent.Distance.Value;                  dont work?!
                //extrusion.Definition.Extent.Distance.Value;       dont work?!
                double distance = getDistance(extrusion);
                listOfCodeLines.Add(ExtrudeSketch(distance));
                listOfCodeLines.Add("");

                listOfEntityNamesOfOneSketch.Clear();
            }
        } //end of method InterpreteExtrusions

        private void InterpreteSketch(Sketch actualSketch) 
        {
                List<SketchEntity> sketchEntities = MasterM.GetSketchParts(actualSketch);
                foreach (SketchEntity sketchEntity in sketchEntities)
                {
                    InterpreteSketchEntity(sketchEntity);
                }

                //interprete sketchLine list of one sketch
                if (needToInterpreteSketchLine)
                {
                    listOfCodeLines.Add(Exporter.ExportPolygon(listOfSketchLines, numberOfSketchEntities));
                    needToInterpreteSketchLine = false;
                    listOfSketchLines.Clear();
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
            else if (sketchEntity is SketchLine)
            {
                //get multiple sketchLine's, put them in list, at end of sketch, interprete all sketchlines
                needToInterpreteSketchLine = true;
                listOfSketchLines.Add((SketchLine) sketchEntity);
                return;

            }
            else if (sketchEntity is SketchCircle)
            {
                entityType = "circle";
                listOfCodeLines.Add(Exporter.ExportCircle((SketchCircle) sketchEntity, entityType + numberOfSketchEntities));
            }
            else if (sketchEntity is SketchArc)
            {
                entityType = "arc";
                listOfCodeLines.Add(Exporter.ExportArc((SketchArc)sketchEntity, entityType + numberOfSketchEntities));
            }
            else if (sketchEntity is SketchEllipse)
            {
                entityType = "ellipse";
                listOfCodeLines.Add(Exporter.ExportEllipseFull((SketchEllipse)sketchEntity, entityType + numberOfSketchEntities));
            }
            else if (sketchEntity is SketchEllipticalArc)
            {
                entityType = "ellipseArc";
                listOfCodeLines.Add(Exporter.ExportEllipticalArc((SketchEllipticalArc)sketchEntity, entityType + numberOfSketchEntities));

            }
            listOfEntityNamesOfOneSketch.Add(entityType + numberOfSketchEntities);
            numberOfSketchEntities++;


        } //end of method InterpreteSketchEntity

        private void GenerateParameterFunction()
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

        } //end of method GenerateParameterFunction

        private void WriteIntoJsFile()
        {
            //check for existing file
            if (File.Exists(jscadPath))
                File.Delete(jscadPath);

            using (StreamWriter outputFile = new StreamWriter(jscadPath, true))
            {
                foreach (var codeLine in listOfCodeLines)
                {
                    outputFile.WriteLine(codeLine);
                }
            }
        } //end of method WriteIntoJSFile



        private string UnionSketch()
        {
            string unionLine = "\t" + "var sketch" + numberOfSketches + " = union(";
            var lastEntity = listOfEntityNamesOfOneSketch.Last();

            if (listOfEntityNamesOfOneSketch.Count == 0)
            {
                unionLine = "";
            }
            else if (listOfEntityNamesOfOneSketch.Count == 1)
            {
                unionLine = "\t" + "var sketch" + numberOfSketches + " = " + lastEntity + ";";
            }
            else
            {
                foreach (var entityName in listOfEntityNamesOfOneSketch)
                {
                    if (entityName == lastEntity)
                    {
                        unionLine += entityName;
                    }
                    else
                    {
                        unionLine += entityName + ", ";
                    }
                }
                unionLine += ");";    
            }
            return unionLine;

        } //end of method UnionSketch


        private string ExtrudeSketch(double height) 
        {
            string extrusionLine = "\t" + "var extrusion" + numberOfSketches + " = sketch" + numberOfSketches + ".extrude({ offset: [0,0," + height.ToString(myCultureInfo) + "] });";
            return extrusionLine;

        } //end of method ExtrudeSketch

        private string unionAllExtrusion(string varName)
        {
            string extrusionLine = "";
            if (numberOfSketches == 0)
            {
                extrusionLine = "";
            }
            else if (numberOfSketches == 1)
            {
                extrusionLine = "\t" + "var " + varName + " = extrusion" + numberOfSketches + "";
            }
            else
            {
                extrusionLine = "\t" + "var " + varName + " = union(";
                for (int i = 1; i <= numberOfSketches; i++)
                {
                    if (i == numberOfSketches)
                    {
                        extrusionLine += "extrusion" + i;
                    }
                    else
                    {
                        extrusionLine += "extrusion" + i + ", ";
                    }

                }
                extrusionLine += ");";
                
            }
            return extrusionLine;

        } //end of method unionAllExtrusion

        private double getDistance(ExtrudeFeature extrusion)
        {
            //find out direction 20993 is one, 20994 is other direction
            //if (extrusion.Extent.Direction == 20993)
            //cant get it...

            //current method to take a parameter out of the list which fits the best...
            List<double> listOfParameters = new List<double>();
            foreach (Inventor.Parameter parameter in extrusion.Parameters)
            {
                double temp;
                Double.TryParse(parameter.Value.ToString(), out temp);
                listOfParameters.Add(temp);
            }

            for (int i = 0; i < listOfParameters.Count; i++)
            {
                if (i==0)
                {
                    continue;
                }
                else if (listOfParameters[i] == 0.0)
                {
                    continue;
                }
                else
                {
                    return listOfParameters[i];
                }
            }
            return 10;
        }
    } //end of class Shakespeare
} //end of namespace InvAddIn
