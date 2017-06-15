using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using Inventor;
using System.IO;


namespace InvAddIn
{
    //TODO:
    /*
        - export interprete methods, can parameter be added outside of class when public?
        - seperate between 2d and 3d entities. need two list for entities?
        - use polygon for sketchPoints, write sketchPoint case
    
    
     */
    public class Shakespeare
    {
        private List<String> listOfEntityNames = new List<string>();
        private List<String> listOfCodeLines = new List<string>();
        private List<Sketch> listOfSketches;
        private List<SketchLine> rectangleLines = new List<SketchLine>();
        public static List<Parameter> ListOfParameter = new List<Parameter>();

        private int numberOfSketches;
        private bool sketchPoints = false;

        private static string desktopPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
        private static string jscadPath = desktopPath + "\\example.js";


        

        


        //constructor
        public Shakespeare(MasterM masterModel, string savePathChosenByUser)
        {
            //for testing purposes use desktop path, we can also use path chosen by user or path that directly saves the js-file into the web-app folder
            //jscadPath = savePathChosenByUser;

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

            //create 2D Primitives
            foreach (Sketch sketch in listOfSketches)
            {
                List<SketchEntity> sketchEntities = MasterM.GetSketchParts(sketch);
                foreach (SketchEntity sketchEntity in sketchEntities)
                {
                    InterpreteSketchEntity(sketchEntity);
                }

            }


            //union all 2D Primitives
            listOfCodeLines.Add("");
            listOfCodeLines.Add(UnionSketches());
            //put it into one var?

            //extrude 2D Primitive
            listOfCodeLines.Add(ExtrudeSketches("sketches", 10));

            //union 3D Primitives


            //return var
            listOfCodeLines.Add("");
            listOfCodeLines.Add("\t" + "return sketches;");
            listOfCodeLines.Add("}");
            listOfCodeLines.Add("");



            /*
            double len = listOfEntityNames.Count();
            int count = 0;
            foreach (String name in listOfEntityNames)
            {
                if (count == len - 1)
                {
                    listOfCodeLines.Add("\t" + name);
                }
                else
                {
                    listOfCodeLines.Add("\t" + name + ",");
                }

                count++;
            }

            listOfCodeLines.Add("\t" + "]; ");
            listOfCodeLines.Add("}");
            listOfCodeLines.Add("");
            listOfCodeLines.Add("");
            */
        } //end of method GenerateMainFunction

        public void InterpreteSketchEntity(SketchEntity sketchEntity)
        {
            String entityType = "";
            if (sketchEntity is SketchPoint)
            {
                //sketchPoints are just optional?
                //skip it
                //sketchPoints = true;
                return;
            }
            if (sketchEntity is SketchCircle)
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
            else if (sketchEntity is SketchLine)
            {
                // Angenommen: Rectangle besteht aus 4 SketchLine
                rectangleLines.Add((SketchLine)sketchEntity);
                if (rectangleLines.Count == 4)
                {
                    entityType = "ellipseArc";
                    listOfCodeLines.Add(Exporter.exportRectangle(rectangleLines.ToArray(), entityType + numberOfSketches));
                }
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
        }


        public string ExtrudeSketches(string varName, int height) 
        {
            string extrusion = "\t" + varName + " = " + varName + ".extrude({ offset: [0,0," + height + "] });";
            return extrusion;

        }
    } //end of class Shakespeare
} //end of namespace InvAddIn
