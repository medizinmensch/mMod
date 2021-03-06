﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using Inventor;
using System.IO;
using System.Text;


namespace InvAddIn
{
	public static class Exporter
	{
	    private static double factor = 10;
        //setting culture to invariant so it prints 0.001 instead of german style: 0,001
        static CultureInfo myCultureInfo = new CultureInfo("en-GB");

        // Circle (finished)
        public static string ExportCircle(SketchCircle circle, string entityName)
        {
            double radius = Math.Round(circle.Radius, 4) * factor;
            double x = Math.Round(circle.CenterSketchPoint.Geometry.X, 4) * factor;
            double y = Math.Round(circle.CenterSketchPoint.Geometry.Y, 4) * factor;

            string radiusString = entityName + "_Radius";
            string xCoordinate = entityName + "_CenterX";
            string yCoordinate = entityName + "_CenterY";

            //create parameter
            Parameter param1 = new Parameter(radiusString, "radius of " + entityName, "float", radius, 0.1);
            Parameter param2 = new Parameter(xCoordinate, "X-Coordinate of " + entityName, "float", x, 0.1);
            Parameter param3 = new Parameter(yCoordinate, "Y-Coordinate of " + entityName, "float", y, 0.1);

            Shakespeare.ListOfParameter.Add(param1);
            Shakespeare.ListOfParameter.Add(param2);
            Shakespeare.ListOfParameter.Add(param3);

            string javaScriptVariable = "var " + entityName + " = CAG.circle (" +
                                        "{ center:[params." + xCoordinate + ", params." + yCoordinate + "], " +
                                        "radius: params." + radiusString + "});";

            return ("\t" + javaScriptVariable);
        }

        // Arc (finished)
        public static string ExportArc(SketchArc arc, string entityName)
        {
            double centerX = Math.Round(arc.CenterSketchPoint.Geometry.X, 4) * factor;
            double centerY = Math.Round(arc.CenterSketchPoint.Geometry.Y, 4) * factor;

            double radius = Math.Round(arc.Radius, 4) * factor;

            double startAngle = Math.Round(arc.StartAngle * (180 / Math.PI), 4);
            double sweepAngle = Math.Round(arc.SweepAngle * (180 / Math.PI), 4);

            string nameCenterX = entityName + "_CenterX";
            string nameCenterY = entityName + "_CenterY";
            string nameRadius = entityName + "_Radius";
            string nameStartAngle = entityName + "_StartAngle";
            string nameSweepAngle = entityName + "_SweepAngle";

            Parameter param1 = new Parameter(nameCenterX, "Center X of " + entityName, "float",
               centerX, 0.1);
            Parameter param2 = new Parameter(nameCenterY, "Center Y of " + entityName, "float",
               centerY, 0.1);
            Parameter param3 = new Parameter(nameRadius, "Radius of " + entityName, "float",
               radius, 0.1);
            //Parameter param4 = new Parameter(nameStartAngle, "Start angle of " + entityName, "float",startAngle, 1);
            //Parameter param5 = new Parameter(nameSweepAngle, "Sweep angle of " + entityName, "float",sweepAngle, 1);

            Shakespeare.ListOfParameter.Add(param1);
            Shakespeare.ListOfParameter.Add(param2);
            Shakespeare.ListOfParameter.Add(param3);
            //Shakespeare.ListOfParameter.Add(param4);
            //Shakespeare.ListOfParameter.Add(param5);

            string javaScriptVariable = "var " + entityName + " = CSG.Path2D.arc({center: [params." + nameCenterX +
                    ", params." + nameCenterY + ",0], radius: params." + nameRadius +
                    ", startangle: " + startAngle.ToString(myCultureInfo) + ",  endangle: " + sweepAngle.ToString(myCultureInfo) + "}).close().innerToCAG();";

            return ("\t" + javaScriptVariable);
        }

        // Ellipsefull (finished)
        public static string ExportEllipseFull(SketchEllipse ellipsefull, string entityName)
        {
            double majorradius = Math.Round(ellipsefull.MajorRadius, 4) * factor;
            double minorradius = Math.Round(ellipsefull.MinorRadius, 4) * factor;

            double centerX = Math.Round(ellipsefull.CenterSketchPoint.Geometry.X, 4) * factor;
            double centerY = Math.Round(ellipsefull.CenterSketchPoint.Geometry.Y, 4) * factor;

            string nameMajor = entityName + "_MajorRadius";
            string nameMinor = entityName + "_MinorRadius";
            string nameCenterX = entityName + "_CenterX";
            string nameCenterY = entityName + "_CenterY";

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

        // EllipticalArc (todo)
        public static string ExportEllipticalArc(SketchEllipticalArc ellipticalarc, string entityName)
        {
            double centerX = Math.Round(ellipticalarc.CenterSketchPoint.Geometry.X, 4) * factor;
            double centerY = Math.Round(ellipticalarc.CenterSketchPoint.Geometry.Y, 4) * factor;

            double startAngle = ellipticalarc.StartAngle * (180 / Math.PI);
            double sweepAngle = ellipticalarc.SweepAngle * (180 / Math.PI);

            double majorradius = Math.Round(ellipticalarc.MajorRadius, 4) * factor;
            double minorradius = Math.Round(ellipticalarc.MinorRadius, 4) * factor;

            double radius = (majorradius / 2) / Math.Cos(sweepAngle);

            string nameRadius = entityName + "_Radius";
            string nameStartAngle = entityName + "_StartAngle";
            string nameSweepAngle = entityName + "_SweepAngle";
            string nameCenterX = entityName + "_CenterX";
            string nameCenterY = entityName + "_CenterY";

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

        //polygon (finished)
        public static string ExportPolygon(List<SketchLine> listOfSketchLines, int numberOfSketches)
	    {
            //we can only draw polygons if the lines are created on the endpoint of the last line
            //else we wont create a polygon because

	        int numberOfSketchesInExporter = numberOfSketches;
            StringBuilder javaScriptVariable = new StringBuilder();
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
                    xCoordinates.Add(Math.Round(sketchLine.Geometry.StartPoint.X, 4) * factor);
                    yCoordinates.Add(Math.Round(sketchLine.Geometry.StartPoint.Y, 4) * factor);

                    tempX = Math.Round(sketchLine.Geometry.EndPoint.X, 4);
                    tempY = Math.Round(sketchLine.Geometry.EndPoint.Y, 4);
                    first = false;
                }
                //compare endpoints of last SketchLine with startpoint of new SketchLine
                else if (tempX == Math.Round(sketchLine.Geometry.StartPoint.X, 4) && tempY == Math.Round(sketchLine.Geometry.StartPoint.Y, 4)) 
                {
                    xCoordinates.Add(Math.Round(sketchLine.Geometry.StartPoint.X, 4) * factor);
                    yCoordinates.Add(Math.Round(sketchLine.Geometry.StartPoint.Y, 4) * factor);

                    tempX = Math.Round(sketchLine.Geometry.EndPoint.X, 4);
                    tempY = Math.Round(sketchLine.Geometry.EndPoint.Y, 4);

                }
                else
                {
                    //next line does not belong to "old system", create new polygon
                    //add endpoint of last SketchLine
                    xCoordinates.Add(tempX * factor);
                    yCoordinates.Add(tempY * factor);

                    //polygon can only be created with at least 3 points
                    if (xCoordinates.Count >= 3 && yCoordinates.Count >= 3)
                    {
                        javaScriptVariable.Append(CreatePolygonVariable(numberOfSketchesInExporter, xCoordinates, yCoordinates));
                        javaScriptVariable.Append("\n\t");
                        numberOfSketchesInExporter++;
                    }


                    //reset arrays
                    xCoordinates.Clear();
                    yCoordinates.Clear();
                    first = true;

                    //add point of new polygon to lists
                    xCoordinates.Add(Math.Round(sketchLine.Geometry.StartPoint.X, 4) * factor);
                    yCoordinates.Add(Math.Round(sketchLine.Geometry.StartPoint.Y, 4) * factor);

                    tempX = Math.Round(sketchLine.Geometry.EndPoint.X, 4);
                    tempY = Math.Round(sketchLine.Geometry.EndPoint.Y, 4);

                }
                
                if (sketchLine == listOfSketchLines.Last())
                {
                    xCoordinates.Add(tempX * factor);
                    yCoordinates.Add(tempY * factor);
                }

                counter++;
            }

            //polygon can only be created with at least 3 points
            if (xCoordinates.Count >= 3 && yCoordinates.Count >= 3)
            {
                javaScriptVariable.Append(CreatePolygonVariable(numberOfSketchesInExporter, xCoordinates, yCoordinates));
            }

            return "\t" + javaScriptVariable;
	    }

	    private static string CreatePolygonVariable(int numberOfSketch, List<double> xCoordinates, List<double> yCoordinates)
	    {
            //TODO if xcoordinates.count = 1 oder = 0
            Shakespeare.listOfEntityNamesOfOneSketch.Add("polygon" + numberOfSketch);
            Shakespeare.NumberOfSketchEntities++;

	        StringBuilder javaScriptVariable = new StringBuilder();
	        javaScriptVariable.Append("var polygon" + numberOfSketch + " = CAG.fromPoints ( [");

            //insert points: CAG.fromPoints([ [0,0],[5,0],[3,5],[0,5] ]);

            for (int i = 0; i < xCoordinates.Count; i++)
            {
                if (i == xCoordinates.Count - 1)
                {
                    javaScriptVariable.Append("[" + xCoordinates[i].ToString(myCultureInfo) + "," + yCoordinates[i].ToString(myCultureInfo) + "] ");
                }
                else
                {
                    javaScriptVariable.Append("[" + xCoordinates[i].ToString(myCultureInfo) + "," + yCoordinates[i].ToString(myCultureInfo) + "], ");
                }
            }
            javaScriptVariable.Append("] );");

	        return javaScriptVariable.ToString();
	    }

        public static string ExportText(TextBox textbox, string entityName)
        {
            string label = textbox.Text;
            double size = 1; //TODO inventor uses height-width, jscad uses one size value
            double posX = textbox.Origin.X;
            double posY = textbox.Origin.Y - textbox.Height; // inventor uses upper-left, jscad lower-left
            double extrudeWidth = 1; //TODO
            double extrudeHeight = 1; //TODO

            string nameLabel = entityName + "_Label";
            string nameSize = entityName + "_Size";
            string namePosX = entityName + "_PositionX";
            string namePosY = entityName + "_PositionY";

            Parameter param1 = new Parameter(nameLabel, "Label of " + entityName, "text", label, size);
            Parameter param2 = new Parameter(nameSize, "Size of " + entityName, "float", size, 1);
            Parameter param3 = new Parameter(namePosX, "Position X of " + entityName, "float", extrudeWidth, 0);
            Parameter param4 = new Parameter(namePosY, "Position Y of " + entityName, "float", extrudeHeight, 0);

            // TODO size 

            string javaScriptVariable = "var " + entityName + " = vector_text(params." + namePosX + ", params." +
                namePosY + ", params. " + nameLabel + "); \n";

            // rotation?
            // TODO extrude
            string extrude = "var o = []\n";
            extrude += entityName + ".forEach(function(s) {\n";
            extrude += "\t o.push(rectangular_extrude(s, {w: " +  extrudeWidth + ", h: " + extrudeHeight + "}));\n";
            extrude += "});\n";
            extrude += "return union(o);";
            javaScriptVariable += extrude;

            return javaScriptVariable;
        }
    }
}

