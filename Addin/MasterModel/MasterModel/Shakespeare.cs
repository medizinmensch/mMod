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

        private static String jscadPath = "C:\\Users\\Davin\\test.jscad";

        StreamWriter outputFile;

        //static int Main(string[] args)
        //{
        //    exportCircle();
        //    return 0;
        //}

        public Shakespeare(MasterM MM, string path)
        {
            jscadPath = path;
            sketchyList = MM.SketchyList;
            outputFile = new StreamWriter(jscadPath, true);

            using (outputFile)
            {
                outputFile.WriteLine("function main(){");     
            }

            int i = 1;
            foreach (Sketch sketch in sketchyList)
            {
                List<SketchEntity> sketchParts = MM.SketchyParts(sketch);
                foreach (SketchEntity part in sketchyList)
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
                    varList.Add(var + i);
                    i++;
                }
            }
            using (outputFile)
            {
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
            using (StreamWriter outputFile = new StreamWriter(jscadPath, true))
            {
                outputFile.WriteLine("var " + varname + "= circle({r: " + convertCommaToDot(radius) + "})); ");
            }
        }

        // Rectangle
        public void exportRectangle(SketchLine[] lines, String varname)
        {
            
        }

        // Arc
        public void exportArc(SketchArc arc, String varname)
        {
            double centerX = arc.CenterSketchPoint.Geometry.X;
            double centerY = arc.CenterSketchPoint.Geometry.Y;

            double radius = arc.Radius;
        
            using (StreamWriter outputFile = new StreamWriter(jscadPath, true))
            {
                outputFile.WriteLine("var " + varname + "= CSG.Path2D.arc({");
                outputFile.WriteLine("center: [" + convertCommaToDot(centerX) + "," + convertCommaToDot(centerY) + ",0],");
                outputFile.WriteLine("radius: " + convertCommaToDot(radius) + ",");
                outputFile.WriteLine("startangle: 0,");
                outputFile.WriteLine("endangle: 180");
                outputFile.WriteLine("}).close().innerToCAG();");
            }
        }

        // Ellipsefull
        public void exportEllipseFull(SketchEllipse ellipsefull, String varname)
        {        
            double majorradius = ellipsefull.MajorRadius;
            double minorradius = ellipsefull.MinorRadius;

            using (StreamWriter outputFile = new StreamWriter(jscadPath, true))
            {
                outputFile.WriteLine(("var " + varname + "= scale([" + convertCommaToDot(majorradius) + ","));
                outputFile.WriteLine(convertCommaToDot(minorradius) + "],");
                outputFile.WriteLine("circle(" + convertCommaToDot(minorradius) + "));");
            }
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

            using (StreamWriter outputFile = new StreamWriter(jscadPath, true))
            {
                outputFile.WriteLine("var " + varname + "= CSG.Path2D.arc({");
                outputFile.WriteLine("center: [0,0,0],");
                outputFile.WriteLine("radius: " + convertCommaToDot(radius) + ",");
                outputFile.WriteLine("startangle: " + convertCommaToDot(startAngle) + ",");
                outputFile.WriteLine("endangle: " + convertCommaToDot(sweepAngle));
                outputFile.WriteLine("}).close().innerToCAG();");
            }
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
        public string writeFullParameterFunction() {
            outputFile.WriteLine("function getParameterDefinitions() {");
            outputFile.WriteLine("return [");
            //Fehlermeldung
            List<Parameter> parameterList = new List<Parameter>();
            //Fehlermeldung ende

            foreach (var parameter in parameterList) {
                outputFile.WriteLine(parameter.getParameterString() + ",");
            }

            outputFile.WriteLine("];");
            outputFile.WriteLine("}");

            //Fehlermeldung 
            return " ";
            //Fehlermeldung ende

        }

    }
}
