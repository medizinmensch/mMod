using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public List<Parameter> listOfParameter = new List<Parameter>();

        private int numberOfSketches;
        private bool sketchPoints = false;

        private static string desktopPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
        private static string jscadPath = desktopPath + "\\example.js";

        //setting culture invariant so it prints 0.001 instead of german style: 0,001
        CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

        


        //constructor
        public Shakespeare(MasterM masterModel, string savePathChosenByUser)
        {
            //for testing purposes use desktop path, we can also use path chosen by user or path that directly saves the js-file into the web-app folder
            //jscadPath = savePathChosenByUser;

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
            listOfCodeLines.Add(union2DPrimitives());
            //put it into one var?

            //extrude 2D Primitive
            extrude2DPrimitive("2DPrimitive", 10);
            //union 3D Primitives


            //return var
            listOfCodeLines.Add("");
            listOfCodeLines.Add("\t" + "return 2DPrimitive;");



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
            if (sketchEntity is SketchCircle)
            {
                entityType = "circle";
                listOfCodeLines.Add(exportCircle((SketchCircle) sketchEntity, entityType + numberOfSketches));
            }
            else if (sketchEntity is SketchArc)
            {
                entityType = "arc";
                listOfCodeLines.Add(exportArc((SketchArc)sketchEntity, entityType + numberOfSketches));
            }
            else if (sketchEntity is SketchEllipse)
            {
                entityType = "ellipse";
                listOfCodeLines.Add(exportEllipseFull((SketchEllipse)sketchEntity, entityType + numberOfSketches));
            }
            else if (sketchEntity is SketchEllipticalArc)
            {
                entityType = "ellipseArc";
                listOfCodeLines.Add(exportEllipticalArc((SketchEllipticalArc)sketchEntity, entityType + numberOfSketches));

            }
            else if (sketchEntity is SketchLine)
            {
                // Angenommen: Rectangle besteht aus 4 SketchLine
                rectangleLines.Add((SketchLine)sketchEntity);
                if (rectangleLines.Count == 4)
                {
                    entityType = "ellipseArc";
                    listOfCodeLines.Add(exportRectangle(rectangleLines.ToArray(), entityType + numberOfSketches));
                }
            }
            else if (sketchEntity is SketchPoint)
            {
                //do code
                sketchPoints = true;
            }

            listOfEntityNames.Add(entityType + numberOfSketches);
            numberOfSketches++;

        } //end of method InterpreteSketchEntity

        public void GenerateParameterFunction()
        {
            listOfCodeLines.Add("function getParameterDefinitions() {");
            listOfCodeLines.Add("\treturn [");

            foreach (Parameter parameter in listOfParameter)
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

        // Circle
        public string exportCircle(SketchCircle circle, String entityName)
        {
            double radius = circle.Radius;
            double x = circle.CenterSketchPoint.Geometry.X;
            double y = circle.CenterSketchPoint.Geometry.Y;

            String varName = entityName + "_Radius";
            String xCoordinate = entityName + "_CenterX";
            String yCoordinate = entityName + "_CenterY";

            //create parameter
            Parameter param1 = new Parameter(varName, "radius of " + entityName, "float", radius, 0.1);
            Parameter param2 = new Parameter(xCoordinate, "X-Coordinate of " + entityName, "float", x, 0.1);
            Parameter param3 = new Parameter(yCoordinate, "Y-Coordinate of " + entityName, "float", y, 0.1);
            
			listOfParameter.Add(param1);
			listOfParameter.Add(param2);
			listOfParameter.Add(param3);
			
            string javaScriptVariable = "var " + entityName + " = CAG.circle ( " + 
                                        "{ center: [ params." + xCoordinate + ", params." + yCoordinate + "], " + 
                                        "radius: params." + varName + " " + 
                                        "} );"

            return ("\t" + javaScriptVariable);
        }

        // Rectangle
        public string exportRectangle(SketchLine[] lines, String entityName)
        {
            double side1 = lines[0].Length;
            double side2 = lines[1].Length;

            if (side1 == side2)
            {
                side2 = lines[3].Length;
            }

            String name1 = entityName + "= CSG.cube({radius: [params." + name1 +
                ", params." + name2 + ", 0]});");
        }

        // Arc
        public string exportArc(SketchArc arc, String entityName)
        {
            double centerX = arc.CenterSketchPoint.Geometry.X;
            double centerY = arc.CenterSketchPoint.Geometry.Y;

            double radius = arc.Radius;

            String nameCenterX = entityName + "= CSG.Path2D.arc({center: [params." + nameCenterX +
                    ", params." + nameCenterY + ",0], radius: params." + nameRadius +
                    ", startangle: 0,  endangle: 180}).close().innerToCAG();");
        }

        // Ellipsefull
        public string exportEllipseFull(SketchEllipse ellipsefull, String entityName)
        {
            double majorradius = ellipsefull.MajorRadius;
            double minorradius = ellipsefull.MinorRadius;

            String nameMajor = entityName + "= scale([params." + nameMajor + ", params." +
                        nameMinor + "],circle(params." + nameMinor + "));");
        }

        // SPLines
        public string exportSpline(SketchSpline spline)
        {
            double startX = spline.StartSketchPoint.Geometry.X;
            double startY = spline.StartSketchPoint.Geometry.Y;

            double endX = spline.EndSketchPoint.Geometry.X;
            double endY = spline.EndSketchPoint.Geometry.Y;

            return "blabla";
        }

        // EllipticalArc
        public string exportEllipticalArc(SketchEllipticalArc ellipticalarc, String entityName)
        {
            double centerX = ellipticalarc.CenterSketchPoint.Geometry.X;
            double centerY = ellipticalarc.CenterSketchPoint.Geometry.Y;

            double startAngle = ellipticalarc.StartAngle * (180 / Math.PI);
            double sweepAngle = ellipticalarc.SweepAngle * (180 / Math.PI);

            double startX = ellipticalarc.StartSketchPoint.Geometry.X;
            double startY = ellipticalarc.StartSketchPoint.Geometry.Y;

            double majorradius = ellipticalarc.MajorRadius;
            double minorradius = ellipticalarc.MinorRadius;

            double radius = (majorradius / 2) / Math.Cos(sweepAngle);

            String nameRadius = entityName + "= CSG.Path2D.arc({center: [0,0,0]," +
                    "radius: params." + nameRadius + ", startangle: params." +
                    nameStartAngle + ", endangle: params." + nameSweepAngle + "}).close().innerToCAG();");
        }

        // Circle - Probe
        public string exportCircle()
        {
            double radius = 3.5;
            return "blabla";
            /*
			using (StreamWriter outputFile = new StreamWriter(jscadPath, true))
			{
				outputFile.WriteLine("function main(){");
				outputFile.WriteLine("return ");
				outputFile.WriteLine("circle({r: " + radius.ToString().Replace(",", ".") + "});");
			}
			*/
        }

        public string union2DPrimitives() 
        {
            string unionLine = "var 2DPrimitive = union( ";
            foreach (var entityName in listOfEntityNames) 
            {
                unionLine += entityName + ", ";
            }
            unionLine += ");";

            return unionLine;
        }


        public string extrude2DPrimitive(string varName, int height) 
        {
            string extrusion = varName " = " + varName + ".extrude({ offset: [0,0," + height + "] })";
            return extrusion;

        }
    } //end of class Shakespeare
} //end of namespace InvAddIn
