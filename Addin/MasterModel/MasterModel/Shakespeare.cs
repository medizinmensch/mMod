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
                    if(part is SketchCircle){
                        exportCircle((SketchCircle)part);
                    }else if(part is SketchArc)
                    {
                        exportArc((SketchArc)part);
                    }
                }
            }
        }



        public void exportCircle(SketchCircle circle)
        {
            double radius = circle.Radius;
            using (StreamWriter outputFile = new StreamWriter(jscadPath, true))
            {
                outputFile.WriteLine("circle({r: " + convertCommaToDot(radius) + "}); ");
            }
        }

        public void exportArc(SketchArc arc)
        {
            double centerX = arc.CenterSketchPoint.Geometry.X;
            double centerY = arc.CenterSketchPoint.Geometry.Y;

            double startX = arc.StartSketchPoint.Geometry.X;
            double startY = arc.StartSketchPoint.Geometry.Y;

            double endX = arc.EndSketchPoint.Geometry.X;
            double endY = arc.EndSketchPoint.Geometry.Y;

            double startAngle = arc.StartAngle * (180/Math.PI);
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

        private static string convertCommaToDot(double val)
        {
            return val.ToString().Replace(",", ".");
        }

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
