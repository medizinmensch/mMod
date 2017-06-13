using System;

namespace InvAddIn
{
	public static class Exporter
	{

		// Circle
		public static string exportCircle(SketchCircle circle, String varname)
		{
			double radius = circle.Radius;
			double x = circle.CenterSketchPoint.Geometry.X;
			double y = circle.CenterSketchPoint.Geometry.Y;

			String name = varname + "Radius";
			String xName = varname + "CenterX";
			String yName = varname + "CenterY";

			ParameterDef param = new ParameterDef(name, "radius of circle", "float",
				radius, 0.1, radius - 10 < 0 ? 0 : radius - 10, radius+10);
			ParameterDef paramX = new ParameterDef(xName, "center X of circle", "float",
				x, 0.1, x - 10 < 0 ? 0 : x - 10, x + 10);
			ParameterDef paramY = new ParameterDef(yName, "center Y of circle", "float",
				y, 0.1, y - 10 < 0 ? 0 : y - 10, y + 10);
			/*
			parameterList.Add(param);
			parameterList.Add(paramX);
			parameterList.Add(paramY);
			*/
			return ("var " + varname + "= CAG.circle({center: [params." + xName + ", params." + yName + "], radius: params." + name + "});");       
		}

		// Rectangle
		public static string exportRectangle(SketchLine[] lines, String varname)
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
			/*
			parameterList.Add(param1);
			parameterList.Add(param2);
			*/
			return("var " + varname + "= CSG.cube({radius: [params." + name1 +
				", params." + name2 + ", 0]});");
		}

		// Arc
		public static string exportArc(SketchArc arc, String varname)
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
			/*
			parameterList.Add(param1);
			parameterList.Add(param2);
			parameterList.Add(param3);
			*/
			return ("var " + varname + "= CSG.Path2D.arc({center: [params." + nameCenterX +
					", params." + nameCenterY + ",0], radius: params." + nameRadius +
					", startangle: 0,  endangle: 180}).close().innerToCAG();");
		}

		// Ellipsefull
		public static string exportEllipseFull(SketchEllipse ellipsefull, String varname)
		{        
			double majorradius = ellipsefull.MajorRadius;
			double minorradius = ellipsefull.MinorRadius;

			String nameMajor = varname + "Majorradius";
			String nameMinor = varname + "Minorradius";

			ParameterDef param1 = new ParameterDef(nameMajor, "Ellipse major radius", "float",
				majorradius, 0.1, majorradius - 10 < 0 ? 0 : majorradius - 10, majorradius + 10);
			ParameterDef param2 = new ParameterDef(nameMinor, "Ellipse minor radius", "float",
				minorradius, 0.1, minorradius - 10 < 0 ? 0 : minorradius - 10, minorradius + 10);
			/*
			parameterList.Add(param1);
			parameterList.Add(param2);
			*/
			return ("var " + varname + "= scale([params." + nameMajor + ", params." +
						nameMinor + "],circle(params." + nameMinor + "));");
		}

		// SPLines
		public static string exportSpline(SketchSpline spline)
		{
			double startX = spline.StartSketchPoint.Geometry.X;
			double startY = spline.StartSketchPoint.Geometry.Y;

			double endX = spline.EndSketchPoint.Geometry.X;
			double endY = spline.EndSketchPoint.Geometry.Y;

			return "blabla";
		}

		// EllipticalArc
		public static string exportEllipticalArc(SketchEllipticalArc ellipticalarc, String varname)
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
			/*
			parameterList.Add(param1);
			parameterList.Add(param2);
			parameterList.Add(param3);
			*/
			return ("var " + varname + "= CSG.Path2D.arc({center: [0,0,0]," +
					"radius: params." + nameRadius + ", startangle: params." +
					nameStartAngle + ", endangle: params." + nameSweepAngle + "}).close().innerToCAG();");
		}

		// Circle - Probe
		public static string exportCircle()
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
	}
}

