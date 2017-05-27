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

        private static String jscadPath = "C:\\Users\\Davin\\test.jscad";

        StreamWriter outputFile;

        //static int Main(string[] args)
        //{
        //    exportCircle();
        //    return 0;
        //}

        public Shakespeare(MasterM MM)
        {
            sketchyList = MM.SketchyList;
            outputFile = new StreamWriter(jscadPath, true);

            using (outputFile)
            {
                outputFile.WriteLine("function main(){");
                outputFile.WriteLine("return ");
            }

            foreach (Sketch sketch in sketchyList)
            {
                List<SketchEntity> sketchParts = MM.SketchyParts(sketch);
                foreach (SketchEntity part in sketchyList)
                {
                    if (part is SketchCircle) {
                        exportCircle((SketchCircle)part);
                    } else if (part is SketchArc)
                    {
                        exportArc((SketchArc)part);
                    }
                }
            }
        }
        // Circle
        public void exportCircle(SketchCircle circle)
        {
            double radius = circle.Radius;
            using (StreamWriter outputFile = new StreamWriter(jscadPath, true))
            {
                outputFile.WriteLine("circle({r: " + convertCommaToDot(radius) + "}); ");
            }
        }

        // Arc
        public void exportArc(SketchArc arc)
        {
            double centerX = arc.CenterSketchPoint.Geometry.X;
            double centerY = arc.CenterSketchPoint.Geometry.Y;
        
            double startX = arc.StartSketchPoint.Geometry.X;
            double startY = arc.StartSketchPoint.Geometry.Y;

            double endX = arc.EndSketchPoint.Geometry.X;
            double endY = arc.EndSketchPoint.Geometry.Y;

            double startAngle = arc.StartAngle * (180 / Math.PI);
            double sweepAngle = arc.SweepAngle * (180 / Math.PI);
            double radius = arc.Radius;
        
            using (StreamWriter outputFile = new StreamWriter(jscadPath, true))
            {
                outputFile.WriteLine("CSG.Path2D.arc({");
                outputFile.WriteLine("center: [" + convertCommaToDot(centerX) + "," + convertCommaToDot(centerY) + ",0],");
                outputFile.WriteLine("radius: " + convertCommaToDot(radius) + ",");
                outputFile.WriteLine("startangle: " + convertCommaToDot(startAngle) + ",");
                outputFile.WriteLine("endangle: " + convertCommaToDot(sweepAngle));
                outputFile.WriteLine("}).close().innerToCAG();");
            }
        }
        // Line 
        public void exportLine(SketchLine line)
        {
            double length = line.Length;

            using (StreamWriter outputFile = new StreamWriter(jscadPath, true))
            {
                outputFile.WriteLine("length: " + convertCommaToDot(length) + ",");
                outputFile.WriteLine("function main(){");
                outputFile.WriteLine("return ");
            }
        }

        // Ellipsefull
        public void exportEllipseFull(SketchEllipse ellipsefull)
        {
            double centerX = ellipsefull.CenterSketchPoint.Geometry.X;
            double centerY = ellipsefull.CenterSketchPoint.Geometry.Y;
            
            double majorradius = ellipsefull.MajorRadius;
            double minorradius = ellipsefull.MinorRadius;

            using (StreamWriter outputFile = new StreamWriter(jscadPath, true))
            {
                outputFile.WriteLine("majorradius: " + convertCommaToDot(majorradius) + ",");
                outputFile.WriteLine("minorradius: " + convertCommaToDot(minorradius) + ",");
                outputFile.WriteLine("center: [" + convertCommaToDot(centerX) + "," + convertCommaToDot(centerY) + ",0],");
                outputFile.WriteLine("function main(){");
                outputFile.WriteLine("return ");
                outputFile.WriteLine("ellipsefull({r: " + majorradius.ToString().Replace(",", ".") + "});");
                outputFile.WriteLine("ellipsefull({r: " + minorradius.ToString().Replace(",", ".") + "});");
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
        public void exportEllipticalArc(SketchEllipticalArc ellipticalarc)
        {
            double centerX = ellipticalarc.CenterSketchPoint.Geometry.X;
            double centerY = ellipticalarc.CenterSketchPoint.Geometry.Y;

            double startX = ellipticalarc.StartSketchPoint.Geometry.X;
            double startY = ellipticalarc.StartSketchPoint.Geometry.Y;

            double endX = ellipticalarc.EndSketchPoint.Geometry.X;
            double endY = ellipticalarc.EndSketchPoint.Geometry.Y;

            double startAngle = ellipticalarc.StartAngle * (180 / Math.PI);
            double sweepAngle = ellipticalarc.SweepAngle * (180 / Math.PI);

            double majorradius = ellipticalarc.MajorRadius;
            double minorradius = ellipticalarc.MinorRadius;

            using (StreamWriter outputFile = new StreamWriter(jscadPath, true))
            {

                outputFile.WriteLine("majorradius: " + convertCommaToDot(majorradius) + ",");
                outputFile.WriteLine("minorradius: " + convertCommaToDot(minorradius) + ",");
                outputFile.WriteLine("center: [" + convertCommaToDot(centerX) + "," + convertCommaToDot(centerY) + ",0],");
                outputFile.WriteLine("function main(){");
                outputFile.WriteLine("return ");
                outputFile.WriteLine("ellipticalarc({r: " + majorradius.ToString().Replace(",", ".") + "});");
                outputFile.WriteLine("ellipticalarc({r: " + minorradius.ToString().Replace(",", ".") + "});");
                outputFile.WriteLine("CSG.Path2D.arc({");
                outputFile.WriteLine("center: [" + convertCommaToDot(centerX) + "," + convertCommaToDot(centerY) + ",0],");
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

    }
}
