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

        hints, questions:
        - do we need SketchPoint? at this time they get ignored and it works
        - try out: in one sketch draw two different polylines and check sketch entities (sketchpoint as separation between?)
            profile has information which entities belong to each other in one sketch
    
     */
    public class Shakespeare
    {
        //main lists
        public static List<String> listOfEntityNames = new List<string>();
        private List<String> listOfCodeLines = new List<string>();
        public static List<Parameter> ListOfParameter = new List<Parameter>();
        private List<Sketch> listOfSketches;

        //temporary lists
        private List<SketchLine> listOfSketchLines = new List<SketchLine>();



        public static int numberOfSketches;
        private bool needToInterpreteSketchLine = false;

        private static string desktopPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
        private static string jscadPath = desktopPath + "\\example.jscad";


        

        


        //constructor
        public Shakespeare(MasterM masterModel, string savePathChosenByUser)
        {
            //for testing purposes use desktop path, we can also use path chosen by user or path that directly saves the js-file into the web-app folder
            jscadPath = savePathChosenByUser;

            //clear everything at start
            ListOfParameter.Clear();
            listOfEntityNames.Clear();
            listOfCodeLines.Clear();

            listOfSketches = masterModel.SketchyList;
            numberOfSketches = 1;

            GenerateMainFunction();
            GenerateParameterFunction();
            WriteIntoJSFile();

        } //end of constructor 

        private void GenerateMainFunction()
        {
            listOfCodeLines.Add("function main(params) {");

            //interprete sketches
            InterpreteSketches();


            //union all sketches
            listOfCodeLines.Add("");
            listOfCodeLines.Add(UnionSketches());
            //put it into one var?

            //extrude sketches (need height here)
            listOfCodeLines.Add(ExtrudeSketches("sketches", 10));

            //interprete 3d entities

            //union 3d entities (also with extruded sketches)


            //return var
            listOfCodeLines.Add("");
            listOfCodeLines.Add("\t" + "return sketches;");
            listOfCodeLines.Add("}");
            listOfCodeLines.Add("");

        } //end of method GenerateMainFunction

        public void InterpreteSketches() 
        {
            foreach (Sketch sketch in listOfSketches)
            {
                List<SketchEntity> sketchEntities = MasterM.GetSketchParts(sketch);
                foreach (SketchEntity sketchEntity in sketchEntities)
                {
                    InterpreteSketchEntity(sketchEntity);
                }

                //interprete sketchLine list of one sketch
                if (needToInterpreteSketchLine)
                {
                    listOfCodeLines.Add(Exporter.ExportPolygon(listOfSketchLines, numberOfSketches));
                    needToInterpreteSketchLine = false;
                    listOfSketchLines.Clear();
                }

            }

        }

        public void InterpreteSketchEntity(SketchEntity sketchEntity)
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
                //get multiple sketchLine's, put them in list, when next element is different interprete all sketchlines
                needToInterpreteSketchLine = true;
                listOfSketchLines.Add((SketchLine) sketchEntity);
                return;

            }
            else if (sketchEntity is SketchCircle)
            {
                entityType = "circle";
                listOfCodeLines.Add(Exporter.ExportCircle((SketchCircle) sketchEntity, entityType + numberOfSketches));
            }
            else if (sketchEntity is SketchArc)
            {
                entityType = "arc";
                listOfCodeLines.Add(Exporter.exportArc((SketchArc)sketchEntity, entityType + numberOfSketches));
            }
            else if (sketchEntity is SketchEllipse)
            {
                entityType = "ellipse";
                listOfCodeLines.Add(Exporter.exportEllipseFull((SketchEllipse)sketchEntity, entityType + numberOfSketches));
            }
            else if (sketchEntity is SketchEllipticalArc)
            {
                entityType = "ellipseArc";
                listOfCodeLines.Add(Exporter.exportEllipticalArc((SketchEllipticalArc)sketchEntity, entityType + numberOfSketches));

            }
            listOfEntityNames.Add(entityType + numberOfSketches);
            numberOfSketches++;

        } //end of method InterpreteSketchEntity

        public void GenerateParameterFunction()
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

        public void WriteIntoJSFile()
        {
            //check for existing file
            //overwrite it? give hint? create copy?

            if (File.Exists(jscadPath))
                File.Delete(jscadPath);

            using (StreamWriter outputFile = new StreamWriter(jscadPath, true))
            {
                //setting culture invariant so it prints 0.001 instead of german style: 0,001
                //System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
                Thread.CurrentThread.CurrentCulture = new CultureInfo("de-DE");

                foreach (var codeLine in listOfCodeLines)
                {
                    outputFile.WriteLine(codeLine);
                }
            }

            /*
                    using (outputFile = new StreamWriter(jscadPath, true))
                    {
                        outputFile.WriteLine(outputParamDef);

                        outputFile.WriteLine("function main(params){");
                        outputFile.WriteLine(outputMainFunction);

                        outputFile.WriteLine("return [ ");
                        double len = listOfEntityNames.Count();
                        int count = 0;
                        foreach(String name in listOfEntityNames)
                        {
                            if(count == len - 1)
                            {
                                outputFile.WriteLine(name);
                            }else
                            {
                                outputFile.WriteLine(name + ",");
                            }

                            count++;
                        }
                        outputFile.WriteLine("]; ");
                    }
        	*/
        } //end of method WriteIntoJSFile



        public string UnionSketches()
        {
            if (listOfEntityNames.Count <= 1)
            {
                return "";
            }

            var lastEntity = listOfEntityNames.Last();
            string unionLine = "\t" + "var sketches = union(";
            foreach (var entityName in listOfEntityNames) 
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

            return unionLine;
        } //end of method UnionSketches


        public string ExtrudeSketches(string varName, int height) 
        {
            string extrusion = "\t" + varName + " = " + varName + ".extrude({ offset: [0,0," + height + "] });";
            return extrusion;

        } //end of method ExtrudeSketches
    } //end of class Shakespeare
} //end of namespace InvAddIn
