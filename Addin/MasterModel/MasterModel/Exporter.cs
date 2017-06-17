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
               startAngle, 1, 0, 180);
            Parameter param5 = new Parameter(nameSweepAngle, "Sweep angle of " + entityName, "float",
               sweepAngle, 1, 0, 180);

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
	    public static string ExportPolygon(List<SketchLine> listOfSketchLines, String entityName)
	    {
            /*
            - create array of x and y coordinates?
            - check if endpoint of last line is same then startpoint of next line
            - 
            
            */

            int length = listOfSketchLines.Count();
            double tempX = 0;
            double tempY = 0;

            double[] xCoordinates = new double[length];
            double[] yCoordinates = new double[length];

            int counter = 0;
            bool notFirst = false;

            foreach (var sketchLine in listOfSketchLines)
            {
                //pseudocode:

                //first time will be executed no matter what goes on
                if (notFirst == false)
                {
                    xCoordinates[counter] = sketchLine.Geometry.StartPoint.X;
                    yCoordinates[counter] = sketchLine.Geometry.StartPoint.Y;

                    tempX = sketchLine.Geometry.EndPoint.X;
                    tempY = sketchLine.Geometry.EndPoint.Y;
                    notFirst = true;
                }

                //all other times, compare endpoints of last SketchLine with startpoint of new SketchLine
                if (notFirst && tempX == sketchLine.Geometry.StartPoint.X && tempY == sketchLine.Geometry.StartPoint.Y) 
                {
                    xCoordinates[counter] = sketchLine.Geometry.StartPoint.X;
                    yCoordinates[counter] = sketchLine.Geometry.StartPoint.Y;

                    tempX = sketchLine.Geometry.EndPoint.X;
                    tempY = sketchLine.Geometry.EndPoint.Y;
                    
                }
                else
                {
                    //error, destroy universe
                    //throw error message
                }
                counter++;
            }


            string javaScriptVariable = "var " + entityName + " = CAG.fromPoints ( [";

            //insert points: CAG.fromPoints([ [0,0],[5,0],[3,5],[0,5] ]);
            for (int i = 0; i < xCoordinates.Length; i++)
            {
                if (i == xCoordinates.Length - 1)
                {
                    javaScriptVariable += "[" + SubstituteCommaWithDot(xCoordinates[i]) + "," + SubstituteCommaWithDot(yCoordinates[i]) + "] ";
                }
                else
                {
                    javaScriptVariable += "[" + SubstituteCommaWithDot(xCoordinates[i]) + "," + SubstituteCommaWithDot(yCoordinates[i]) + "], ";
                }
            }
            javaScriptVariable += "] );";

            return ("\t" + javaScriptVariable);
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

