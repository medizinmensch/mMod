using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventor;
using System.IO;

namespace InvAddIn
{
    public class Shakespeare
    {
        private List<String> varList = new List<string>();
        private List<String> listOfCodeLines = new List<string>();
        private List<Sketch> listOfSketches;
        private List<SketchLine> rectangleLines = new List<SketchLine>();
        private List<ParameterDef> parameterList = new List<ParameterDef>();

        private int numberOfParameter;

        private static string desktopPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
        private static string jscadPath = desktopPath + "\\example.js";

        // save directly into Web-App jscad_scripts folder - not yet working
        /*
        private static string mModDirectory = Directory.GetCurrentDirectory();
        private static string path1 = System.Reflection.Assembly.GetExecutingAssembly().Location;
        private static string path2 = System.IO.Path.GetDirectoryName(path1);
        private static string jscadPath = mModDirectory + "Web-App\\src\\scripts\\jscad_scripts\\JavaScriptExampleFile.js";
        */

        


        //constructor
        public Shakespeare(MasterM masterModel, string savePathChosenByUser)
        {
            //for testing purposes use desktop path, we can also use path chosen by user or path that directly saves the js-file into the web-app folder
            //jscadPath = savePathChosenByUser;

            listOfSketches = masterModel.SketchyList;
            numberOfParameter = 1;

            GenerateMainFunction();
            GenerateParameterFunction();
            WriteIntoJSFile();

        } //end of constructor 

        private void GenerateMainFunction()
        {
            listOfCodeLines.Add("function main(params) {");

            foreach (Sketch sketch in listOfSketches)
            {
                List<SketchEntity> sketchEntities = MasterM.GetSketchParts(sketch);
                foreach (SketchEntity sketchEntity in sketchEntities)
                {
                    InterpreteSketch(sketchEntity);
                }
            }

            listOfCodeLines.Add("");
            listOfCodeLines.Add("\treturn [ ");

            double len = varList.Count();
            int count = 0;
            foreach (String name in varList)
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
            listOfCodeLines.Add("\t]; ");
            listOfCodeLines.Add("}");
            listOfCodeLines.Add("");
            listOfCodeLines.Add("");
        } //end of method GenerateMainFunction

        public void InterpreteSketch(SketchEntity sketchEntity)
        {
            String entityType = "";
            if (sketchEntity is SketchCircle)
            {
                entityType = "circle";
                listOfCodeLines.Add(exportCircle((SketchCircle) sketchEntity, entityType + numberOfParameter));
                varList.Add(entityType + numberOfParameter);
                numberOfParameter++;
            }
            else if (sketchEntity is SketchArc)
            {
                entityType = "arc";
                listOfCodeLines.Add(exportArc((SketchArc)sketchEntity, entityType + numberOfParameter));
                varList.Add(entityType + numberOfParameter);
                numberOfParameter++;
            }
            else if (sketchEntity is SketchEllipse)
            {
                entityType = "ellipse";
                listOfCodeLines.Add(exportEllipseFull((SketchEllipse)sketchEntity, entityType + numberOfParameter));
                varList.Add(entityType + numberOfParameter);
                numberOfParameter++;
            }
            else if (sketchEntity is SketchEllipticalArc)
            {
                entityType = "ellipseArc";
                listOfCodeLines.Add(exportEllipticalArc((SketchEllipticalArc)sketchEntity, entityType + numberOfParameter));
                varList.Add(entityType + numberOfParameter);
                numberOfParameter++;

            }
            else if (sketchEntity is SketchLine)
            {
                // Angenommen: Rectangle besteht aus 4 SketchLine
                rectangleLines.Add((SketchLine)sketchEntity);
                if (rectangleLines.Count == 4)
                {
                    entityType = "ellipseArc";
                    listOfCodeLines.Add(exportRectangle(rectangleLines.ToArray(), entityType + numberOfParameter));
                    varList.Add(entityType + numberOfParameter);
                    numberOfParameter++;
                }
            }
            else if (sketchEntity is SketchPoint)
            {
                //do code
            }

        } //end of method InterpreteSketch

        public void GenerateParameterFunction()
        {
            listOfCodeLines.Add("function getParameterDefinitions() {");
            listOfCodeLines.Add("\treturn [");

            foreach (ParameterDef parameter in parameterList)
            {
                listOfCodeLines.Add(parameter.GetParameterString() + ",");
            }

            //remove the last comma of last element of list
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
                        double len = varList.Count();
                        int count = 0;
                        foreach(String name in varList)
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

        private static string convertCommaToDot(double val)
        {
            return val.ToString().Replace(",", ".");
        } //end of method convertCommaToDot

        //exporter

        // Circle
        public string exportCircle(SketchCircle circle, String varname)
        {
            double radius = circle.Radius;
            double x = circle.CenterSketchPoint.Geometry.X;
            double y = circle.CenterSketchPoint.Geometry.Y;

            String name = varname + "Radius";
            String xName = varname + "CenterX";
            String yName = varname + "CenterY";

            ParameterDef param = new ParameterDef(name, "radius of circle", "float",
                radius, 0.1, radius - 10 < 0 ? 0 : radius - 10, radius + 10);
            ParameterDef paramX = new ParameterDef(xName, "center X of circle", "float",
                x, 0.1, x - 10 < 0 ? 0 : x - 10, x + 10);
            ParameterDef paramY = new ParameterDef(yName, "center Y of circle", "float",
                y, 0.1, y - 10 < 0 ? 0 : y - 10, y + 10);
            
			parameterList.Add(param);
			parameterList.Add(paramX);
			parameterList.Add(paramY);
			
            return ("\t" + "var " + varname + " = CAG.circle( {center: [params." + xName + ", params." + yName + "], radius: params." + name + " } );");
        }

        // Rectangle
        public string exportRectangle(SketchLine[] lines, String varname)
        {
            double side1 = lines[0].Length;
            double side2 = lines[1].Length;

            if (side1 == side2)
            {
                side2 = lines[3].Length;
            }

            String name1 = varname + "Side1";
            String name2 = varname + "Side2";
            ParameterDef param1 = new ParameterDef(name1, "Width of retangle", "float",
                side1, 0.1, side1 - 10 < 0 ? 0 : side1 - 10, side1 + 10);
            ParameterDef param2 = new ParameterDef(name2, "Length of retangle", "float",
                side2, 0.1, side2 - 10 < 0 ? 0 : side2 - 10, side2 + 10);
            
			parameterList.Add(param1);
			parameterList.Add(param2);
			
            return ("\t" + "var " + varname + "= CSG.cube({radius: [params." + name1 +
                ", params." + name2 + ", 0]});");
        }

        // Arc
        public string exportArc(SketchArc arc, String varname)
        {
            double centerX = arc.CenterSketchPoint.Geometry.X;
            double centerY = arc.CenterSketchPoint.Geometry.Y;

            double radius = arc.Radius;

            String nameCenterX = varname + "CenterX";
            String nameCenterY = varname + "CenterY";
            String nameRadius = varname + "Radius";

            ParameterDef param1 = new ParameterDef(nameCenterX, "Arc Center X", "float",
                centerX, 0.1, centerX - 10 < 0 ? 0 : centerX - 10, centerX + 10);
            ParameterDef param2 = new ParameterDef(nameCenterY, "Arc Center Y", "float",
                centerY, 0.1, centerY - 10 < 0 ? 0 : centerY - 10, centerY + 10);
            ParameterDef param3 = new ParameterDef(nameRadius, "Arc Radius", "float",
                radius, 0.1, radius - 10 < 0 ? 0 : radius - 10, radius + 10);
            
			parameterList.Add(param1);
			parameterList.Add(param2);
			parameterList.Add(param3);
			
            return ("\t" + "var " + varname + "= CSG.Path2D.arc({center: [params." + nameCenterX +
                    ", params." + nameCenterY + ",0], radius: params." + nameRadius +
                    ", startangle: 0,  endangle: 180}).close().innerToCAG();");
        }

        // Ellipsefull
        public string exportEllipseFull(SketchEllipse ellipsefull, String varname)
        {
            double majorradius = ellipsefull.MajorRadius;
            double minorradius = ellipsefull.MinorRadius;

            String nameMajor = varname + "Majorradius";
            String nameMinor = varname + "Minorradius";

            ParameterDef param1 = new ParameterDef(nameMajor, "Ellipse major radius", "float",
                majorradius, 0.1, majorradius - 10 < 0 ? 0 : majorradius - 10, majorradius + 10);
            ParameterDef param2 = new ParameterDef(nameMinor, "Ellipse minor radius", "float",
                minorradius, 0.1, minorradius - 10 < 0 ? 0 : minorradius - 10, minorradius + 10);
            
			parameterList.Add(param1);
			parameterList.Add(param2);
			
            return ("\t" + "var " + varname + "= scale([params." + nameMajor + ", params." +
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
        public string exportEllipticalArc(SketchEllipticalArc ellipticalarc, String varname)
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

            String nameRadius = varname + "Radius";
            String nameStartAngle = varname + "StartAngle";
            String nameSweepAngle = varname + "SweepAngle";

            ParameterDef param1 = new ParameterDef(nameRadius, "Ellipse arc radius", "float",
                radius, 0.1, radius - 10 < 0 ? 0 : radius - 10, radius + 10);
            ParameterDef param2 = new ParameterDef(nameStartAngle, "Ellipse arc start angle", "float",
                startAngle, 0.1, startAngle - 10 < 0 ? 0 : startAngle - 10, startAngle + 10);
            ParameterDef param3 = new ParameterDef(nameSweepAngle, "Ellipse arc sweep angle", "float",
                sweepAngle, 0.1, sweepAngle - 10 < 0 ? 0 : sweepAngle - 10, sweepAngle + 10);
            
			parameterList.Add(param1);
			parameterList.Add(param2);
			parameterList.Add(param3);
			
            return ("\t" + "var " + varname + "= CSG.Path2D.arc({center: [0,0,0]," +
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

    } //end of class Shakespeare
} //end of namespace InvAddIn
