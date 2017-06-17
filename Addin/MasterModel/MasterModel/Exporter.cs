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
            double radius = Math.Round(circle.Radius, 4);
            double x = Math.Round(circle.CenterSketchPoint.Geometry.X, 4);
            double y = Math.Round(circle.CenterSketchPoint.Geometry.Y, 4);

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
	    public static string ExportPolygon(List<SketchLine> listOfSketchLines, int numberOfSketches)
	    {
            /*
            - create array of x and y coordinates?
            - check if endpoint of last line is same then startpoint of next line
            - 
            
            */
	        int numberOfSketchesInExporter = numberOfSketches;
	        string javaScriptVariable = "";
            double tempX = 0;
            double tempY = 0;

            List<double> xCoordinates = new List<double>();
            List<double> yCoordinates = new List<double>();

            int counter = 0;
            bool first = true;

            foreach (var sketchLine in listOfSketchLines)
            {
                //first time will be executed, without comparing start and endpoint
                if (first)
                {
                    xCoordinates.Add(Math.Round(sketchLine.Geometry.StartPoint.X, 4));
                    yCoordinates.Add(Math.Round(sketchLine.Geometry.StartPoint.Y, 4));

                    tempX = Math.Round(sketchLine.Geometry.EndPoint.X, 4);
                    tempY = Math.Round(sketchLine.Geometry.EndPoint.Y, 4);
                    first = false;
                }
                //compare endpoints of last SketchLine with startpoint of new SketchLine
                else if (tempX == Math.Round(sketchLine.Geometry.StartPoint.X, 4) && tempY == Math.Round(sketchLine.Geometry.StartPoint.Y, 4)) 
                {
                    xCoordinates.Add(Math.Round(sketchLine.Geometry.StartPoint.X, 4));
                    yCoordinates.Add(Math.Round(sketchLine.Geometry.StartPoint.Y, 4));

                    tempX = Math.Round(sketchLine.Geometry.EndPoint.X, 4);
                    tempY = Math.Round(sketchLine.Geometry.EndPoint.Y, 4);
                    first = false;

                }
                else
                {
                    //next line does not belong to "old system", create new polygon
                    javaScriptVariable += CreatePolygonVariable(numberOfSketchesInExporter, xCoordinates, yCoordinates);
                    javaScriptVariable += "\n\t";
                    numberOfSketchesInExporter++;
                    xCoordinates.Clear();
                    yCoordinates.Clear();
                    first = true;

                    //add point of new polygon to lists
                    xCoordinates.Add(Math.Round(sketchLine.Geometry.StartPoint.X, 4));
                    yCoordinates.Add(Math.Round(sketchLine.Geometry.StartPoint.Y, 4));

                    tempX = Math.Round(sketchLine.Geometry.EndPoint.X, 4);
                    tempY = Math.Round(sketchLine.Geometry.EndPoint.Y, 4);


                    //reset arrays

                }
                
                counter++;
            }

	        javaScriptVariable += CreatePolygonVariable(numberOfSketchesInExporter, xCoordinates, yCoordinates);

            return ("\t" + javaScriptVariable);
	    }

	    private static string CreatePolygonVariable(int numberOfSketch, List<double> xCoordinates, List<double> yCoordinates)
	    {
            Shakespeare.listOfEntityNames.Add("polygon" + numberOfSketch);
            Shakespeare.numberOfSketches++;


            string javaScriptVariable = "var polygon" + numberOfSketch + " = CAG.fromPoints ( [";

            //insert points: CAG.fromPoints([ [0,0],[5,0],[3,5],[0,5] ]);
            for (int i = 0; i < xCoordinates.Count; i++)
            {
                if (i == xCoordinates.Count - 1)
                {
                    javaScriptVariable += "[" + SubstituteCommaWithDot(xCoordinates[i]) + "," + SubstituteCommaWithDot(yCoordinates[i]) + "] ";
                }
                else
                {
                    javaScriptVariable += "[" + SubstituteCommaWithDot(xCoordinates[i]) + "," + SubstituteCommaWithDot(yCoordinates[i]) + "], ";
                }
            }
            javaScriptVariable += "] );";

	        return javaScriptVariable;
	    }

        public static string SubstituteCommaWithDot(double value)
        {
            string valueString = value.ToString();
            string variable = "";

            foreach (var character in valueString)
            {
                if (character == ',')
                {
                    variable += '.';
                }
                else
                {
                    variable += character;
                }
            }
            return variable;
        }
    }
}

