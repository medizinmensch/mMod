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

        // Arc (finished)
        public static string exportArc(SketchArc arc, String entityName)
        {
            double centerX = arc.CenterSketchPoint.Geometry.X;
            double centerY = arc.CenterSketchPoint.Geometry.Y;

            double radius = arc.Radius;

            double startAngle = arc.StartAngle * (180 / Math.PI);
            double sweepAngle = arc.SweepAngle * (180 / Math.PI);

            String nameCenterX = entityName + "_CenterX";
            String nameCenterY = entityName + "_CenterY";
            String nameRadius = entityName + "_Radius";
            String nameStartAngle = entityName + "_StartAngle";
            String nameSweepAngle = entityName + "_SweepAngle";

            Parameter param1 = new Parameter(nameCenterX, "Center X of " + entityName, "float",
               centerX, 0.1);
            Parameter param2 = new Parameter(nameCenterY, "Center Y of " + entityName, "float",
               centerY, 0.1);
            Parameter param3 = new Parameter(nameRadius, "Radius of " + entityName, "float",
               radius, 0.1);
            Parameter param4 = new Parameter(nameStartAngle, "Start angle of " + entityName, "float",
               startAngle, 1);
            Parameter param5 = new Parameter(nameSweepAngle, "Sweep angle of " + entityName, "float",
               sweepAngle, 1);

            Shakespeare.ListOfParameter.Add(param1);
            Shakespeare.ListOfParameter.Add(param2);
            Shakespeare.ListOfParameter.Add(param3);
            Shakespeare.ListOfParameter.Add(param4);
            Shakespeare.ListOfParameter.Add(param5);

            string javaScriptVariable = "var " + entityName + "= CSG.Path2D.arc({center: [params." + nameCenterX +
                    ", params." + nameCenterY + ",0], radius: params." + nameRadius +
                    ", startangle: params." + nameStartAngle + ",  endangle: params." + nameSweepAngle + "}).close().innerToCAG();";

            return ("\t" + javaScriptVariable);
        }

        // Ellipsefull (finished)
        public static string exportEllipseFull(SketchEllipse ellipsefull, String entityName)
        {
            double majorradius = ellipsefull.MajorRadius;
            double minorradius = ellipsefull.MinorRadius;

            double centerX = ellipsefull.CenterSketchPoint.Geometry.X;
            double centerY = ellipsefull.CenterSketchPoint.Geometry.Y;

            String nameMajor = entityName + "_MajorRadius";
            String nameMinor = entityName + "_MinorRadius";
            String nameCenterX = entityName + "_CenterX";
            String nameCenterY = entityName + "_CenterY";

            Parameter param1 = new Parameter(nameMajor, "Major radius of " + entityName, "float",
               majorradius, 0.1);
            Parameter param2 = new Parameter(nameMinor, "Minor radius of " + entityName, "float",
               minorradius, 0.1);
            Parameter param3 = new Parameter(nameCenterX, "Center X of " + entityName, "float",
               centerX, 0.1);
            Parameter param4 = new Parameter(nameCenterY, "Center Y of " + entityName, "float",
               centerY, 0.1);

            Shakespeare.ListOfParameter.Add(param1);
            Shakespeare.ListOfParameter.Add(param2);
            Shakespeare.ListOfParameter.Add(param3);
            Shakespeare.ListOfParameter.Add(param4);

            string javaScriptVariable = "var " + entityName + " = scale([params." + nameMajor + ", params." +
                nameMinor + "], CAG.circle ({ center: [ params." + nameCenterX + ", params." + nameCenterY +
                "], " + "radius: params." + nameMinor + "} )  );";
             
            return ("\t" + javaScriptVariable);
        }

        // EllipticalArc(todo)
        public static string exportEllipticalArc(SketchEllipticalArc ellipticalarc, String entityName)
        {
            double centerX = ellipticalarc.CenterSketchPoint.Geometry.X;
            double centerY = ellipticalarc.CenterSketchPoint.Geometry.Y;

            double startAngle = ellipticalarc.StartAngle * (180 / Math.PI);
            double sweepAngle = ellipticalarc.SweepAngle * (180 / Math.PI);

            double majorradius = ellipticalarc.MajorRadius;
            double minorradius = ellipticalarc.MinorRadius;

            double radius = (majorradius / 2) / Math.Cos(sweepAngle);

            String nameRadius = entityName + "_Radius";
            String nameStartAngle = entityName + "_StartAngle";
            String nameSweepAngle = entityName + "_SweepAngle";
            String nameCenterX = entityName + "_CenterX";
            String nameCenterY = entityName + "_CenterY";

            Parameter param1 = new Parameter(nameRadius, "Radius of " + entityName, "float",
               radius, 0.1);
            Parameter param2 = new Parameter(nameStartAngle, "Start angle of " + entityName, "float",
               startAngle, 0.1);
            Parameter param3 = new Parameter(nameSweepAngle, "Sweep angle of " + entityName, "float",
               sweepAngle, 0.1);
            Parameter param4 = new Parameter(nameCenterX, "Center X of " + entityName, "float",
               centerX, 0.1);
            Parameter param5 = new Parameter(nameCenterY, "Center Y of " + entityName, "float",
               centerY, 0.1);

            Shakespeare.ListOfParameter.Add(param1);
            Shakespeare.ListOfParameter.Add(param2);
            Shakespeare.ListOfParameter.Add(param3);
            Shakespeare.ListOfParameter.Add(param4);
            Shakespeare.ListOfParameter.Add(param5);

            // das hier funktioniert nicht in jscad :/
            // https://joostn.github.io/OpenJsCad/
                  //var ellipsearc = CSG.Path2D.arc({
                  //    center: [0, 0, 0], 
                  // xradius: 5, 
                  // yradius: 20, 
                  // xaxisrotation: 100,
                  // clockwise: true,
                  //    large: false,
                  //    resolution: 48,
                  //    startangle: 0,  
                  // endangle: 60
                  //}).close().innerToCAG();

            string javaScriptVariable = "var " + entityName + "= CSG.Path2D.arc({center: [0,0,0]," +
                "radius: params." + nameRadius + ", startangle: params." +
                nameStartAngle + ", endangle: params." + nameSweepAngle + "}).close().innerToCAG();";

            return ("\t" + javaScriptVariable);
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

