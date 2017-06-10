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

        private List<Sketch> sketchyList;

        private List<String> varList = new List<string>();

        private List<ParameterDef> parameterList = new List<ParameterDef>();

        private static String jscadPath = "C:\\Users\\Davin\\test.jscad";

        StreamWriter outputFile;

        private String outputMainFunction = "";

        private String outputParamDef = "";

        //static int Main(string[] args)
        //{
        //    exportCircle();
        //    return 0;
        //}

        public Shakespeare(MasterM MM, string path)
        {
            jscadPath = path;
            sketchyList = MM.SketchyList;
           
            int i = 1;
            List<SketchLine> rectangleLines = new List<SketchLine>();
            foreach (Sketch sketch in sketchyList)
            {
                List<SketchEntity> sketchParts = MM.SketchyParts(sketch);
                foreach (SketchEntity part in sketchParts)
                {
                    String var = "";
                    if (part is SketchCircle)
                    {
                        exportCircle((SketchCircle)part, "circle"+i);
                        var = "circle";
                    } else if (part is SketchArc)
                    {
                        exportArc((SketchArc)part, "arc"+i);
                        var = "arc";
                    }
                    else if (part is SketchEllipse)
                    {
                        exportEllipseFull((SketchEllipse)part, "ellipse" + i);
                        var = "ellipse";
                    }
                    else if (part is SketchEllipticalArc)
                    {
                        exportEllipticalArc((SketchEllipticalArc)part, "ellipseArc" + i);
                        var = "ellipseArc";
                    }
                    // Angenommen: Rectangle besteht aus 4 SketchLine
                    else if (part is SketchLine)
                    {
                        rectangleLines.Add((SketchLine)part);
                        if(rectangleLines.Count == 4)
                        {
                            exportRectangle(rectangleLines.ToArray(), "rectangle" + i);
                            var = "ellipseArc";
                        }else
                        {
                            continue;
                        }                                           
                    }
                    varList.Add(var + i);
                    i++;
                }
            }

            writeFullParameterFunction();

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
                
        }

        // Circle
        public void exportCircle(SketchCircle circle, String varname)
        {
            double radius = circle.Radius;
            String name = varname + "Radius";
            ParameterDef param = new ParameterDef(name, "radius of circle", "float",
               radius, 0.1, radius - 10 < 0 ? 0 : radius - 10, radius+10);
            parameterList.Add(param);
            outputMainFunction += "var " + varname + "= circle({r: params." + name + "}); \n";       
        }

        // Rectangle
        public void exportRectangle(SketchLine[] lines, String varname)
        {          
            double side1 = lines[0].Length;
            double side2 = lines[1].Length;

            if(side1 == side2)
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

            outputMainFunction += "var " + varname + "= CSG.cube({radius: [params." + name1 +
                ", params." + name2 + ", 0]}); \n";
        }

        // Arc
        public void exportArc(SketchArc arc, String varname)
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

            outputMainFunction += "var " + varname + "= CSG.Path2D.arc({center: [params." + nameCenterX +
                ", params." + nameCenterY + ",0], radius: params." + nameRadius +
               ", startangle: 0,  endangle: 180}).close().innerToCAG();\n";
        }

        // Ellipsefull
        public void exportEllipseFull(SketchEllipse ellipsefull, String varname)
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

            outputMainFunction += "var " + varname + "= scale([params." + nameMajor + ", params." +
                nameMinor + "],circle(params." + nameMinor + "));\n";
        }

        // SPLines
        public void exportSpline(SketchSpline spline)
        {
            double startX = spline.StartSketchPoint.Geometry.X;
            double startY = spline.StartSketchPoint.Geometry.Y;

            double endX = spline.EndSketchPoint.Geometry.X;
            double endY = spline.EndSketchPoint.Geometry.Y;

            using(StreamWriter outputFile = new StreamWriter(jscadPath, true))
            {
                

            }
        }

        // EllipticalArc
        public void exportEllipticalArc(SketchEllipticalArc ellipticalarc, String varname)
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

            outputMainFunction += "var " + varname + "= CSG.Path2D.arc({center: [0,0,0]," +
                "radius: params." + nameRadius + ", startangle: params." +
                nameStartAngle + ", endangle: params." + nameSweepAngle + "}).close().innerToCAG();\n";
        }

        // ConvertCommaToDot
        private static string convertCommaToDot(double val)
        {
            return val.ToString().Replace(",", ".");
        }

        // Circle - Probe
        public static void exportCircle()
        {
            double radius = 3.5;
            
            using (StreamWriter outputFile = new StreamWriter(jscadPath, true))
            {
                outputFile.WriteLine("function main(){");
                outputFile.WriteLine("return ");
                outputFile.WriteLine("circle({r: " + radius.ToString().Replace(",", ".") + "});");
            }
        }

        //oskar edit:
        public void writeFullParameterFunction() {
            outputParamDef += "function getParameterDefinitions() {\n";
            outputParamDef += "return [\n";

            foreach (ParameterDef parameter in parameterList) {
                outputParamDef += parameter.getParameterString() + ",";
            }
            //remove the last comma
            outputParamDef.Remove(outputParamDef.Length - 1, 1);
            outputParamDef += "];";
            outputParamDef += "}\n";
           

        }

    }
}
