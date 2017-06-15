using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using Inventor;
using System.IO;


namespace InvAddIn
{
	public static class Exporter
	{

        // Circle (finished)
        public static string ExportCircle(SketchCircle circle, String entityName)
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

            Shakespeare.ListOfParameter.Add(param1);
            Shakespeare.ListOfParameter.Add(param2);
            Shakespeare.ListOfParameter.Add(param3);

            string javaScriptVariable = "var " + entityName + " = CAG.circle ( " +
                                        "{ center: [ params." + xCoordinate + ", params." + yCoordinate + "], " +
                                        "radius: params." + varName + " " +
                                        "} );";

            return ("\t" + javaScriptVariable);
        }

        // Rectangle (todo)
        public static string exportRectangle(SketchLine[] lines, String entityName)
        {
            double side1 = lines[0].Length;
            double side2 = lines[1].Length;

            if (side1 == side2)
            {
                side2 = lines[3].Length;
            }

            String creationString = entityName + "= CSG.cube({radius: [params." + side1 +
                ", params." + side2 + ", 0]});";
            return creationString;
        }

        // Arc (todo)
        public static string exportArc(SketchArc arc, String entityName)
        {
            double centerX = arc.CenterSketchPoint.Geometry.X;
            double centerY = arc.CenterSketchPoint.Geometry.Y;

            double radius = arc.Radius;

            String creationString = entityName + "= CSG.Path2D.arc({center: [params." + centerX +
                    ", params." + centerY + ",0], radius: params." + radius +
                    ", startangle: 0,  endangle: 180}).close().innerToCAG();";
            return creationString;
        }

        // Ellipsefull(todo)
        public static string exportEllipseFull(SketchEllipse ellipsefull, String entityName)
        {
            double majorradius = ellipsefull.MajorRadius;
            double minorradius = ellipsefull.MinorRadius;

            String creationString = entityName + "= scale([params." + majorradius + ", params." +
                        minorradius + "],circle(params." + minorradius + "));";
            return creationString;
        }

        // SPLines(todo)
        public static string exportSpline(SketchSpline spline)
        {
            double startX = spline.StartSketchPoint.Geometry.X;
            double startY = spline.StartSketchPoint.Geometry.Y;

            double endX = spline.EndSketchPoint.Geometry.X;
            double endY = spline.EndSketchPoint.Geometry.Y;

            return "blabla";
        }

        // EllipticalArc(todo)
        public static string exportEllipticalArc(SketchEllipticalArc ellipticalarc, String entityName)
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

            String creationString = entityName + "= CSG.Path2D.arc({center: [0,0,0]," +
                    "radius: params." + radius + ", startangle: params." +
                    startAngle + ", endangle: params." + sweepAngle + "}).close().innerToCAG();";
            return creationString;
        }

        // Circle - Probe (needed?)
        public static string ExportCircle()
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

        //polygon (todo)
	    public static string ExportPolygon(List<SketchLine> listOfSketchLines, String entityName)
	    {
	        return "blabla";
	    }
    }
}

