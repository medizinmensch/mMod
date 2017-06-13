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
        private List<String> listOfCodeLines = new List<string>();

        private string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        private string jscadPath = desktopPath + "\\JavaScriptExampleFile.js";

        /* save directly into Web-App jscad_scripts folder
        string mModDirectory = System.IO.Path.GetFullPath(@"..\..\..\..\..\..\")
        string jsFilePath = directory + "Web-App\\src\\scripts\\jscad_scripts\\JavaScriptExampleFile.js";

        */

        private String outputMainFunction = "";

        private String outputParamDef = "";

        //constructor
        public Shakespeare(MasterM MM, string savePathChosenByUser)
        {
            //for testing purposes use desktop path, we can also use path chosen by user or path that directly saves the js-file into the web-app folder
            //jscadPath = path;
            sketchyList = MM.SketchyList;

            interpreteSketches();
            writeFullParameterFunction();
            writeIntoJSFile;     
        }

        public void interpreteSketches() 
		{
			int numberOfParameter = 1;
        	List<SketchLine> rectangleLines = new List<SketchLine>();
            foreach (Sketch sketch in sketchyList)
            {
				
            	List<SketchEntity> sketchParts = MM.SketchyParts(sketch);
                foreach (SketchEntity part in sketchParts)
                {
                	String sketchType = "";
                    if (part is SketchCircle)
                    {
						sketchType = "circle";
						listOfCodeLines.Add(Exporter.exportCircle((SketchCircle) part, sketchType + numberOfParameter));
                    } else if (part is SketchArc)
                    {
						sketchType = "arc";
						listOfCodeLines.Add(Exporter.exportArc((SketchArc) part, sketchType + numberOfParameter));	
                    } else if (part is SketchEllipse)
                    {
						sketchType = "ellipse";
						listOfCodeLines.Add(Exporter.exportEllipseFull((SketchEllipse) part, sketchType + numberOfParameter));
                    } else if (part is SketchEllipticalArc)
                    {
						sketchType = "ellipseArc";
						listOfCodeLines.Add(Exporter.exportEllipticalArc((SketchEllipticalArc)part, sketchType + numberOfParameter));

                    } else if (part is SketchLine)
                    {
						// Angenommen: Rectangle besteht aus 4 SketchLine
                        rectangleLines.Add((SketchLine)part);
                        if(rectangleLines.Count == 4)
                        {
							sketchType = "ellipseArc";
							listOfCodeLines.Add(Exporter.exportRectangle(rectangleLines.ToArray(), sketchType + numberOfParameter));

                        } else
                        {
                        	continue;
                        }
                    } 
					varList.Add(sketchType + numberOfParameter);
                    numberOfParameter++;
				}
			}

			listOfCodeLines.Add("");
			listOfCodeLines.Add("");


        }

        

        // ConvertCommaToDot
        private static string convertCommaToDot(double val)
        {
            return val.ToString().Replace(",", ".");
        }
			
        public void writeFullParameterFunction() 
		{
            listOfCodeLines.Add("function getParameterDefinitions() {");
            listOfCodeLines.Add("return [");

            foreach (ParameterDef parameter in parameterList) {
                listOfCodeLines.Add(parameter.getParameterString() + ",");
            }

            //remove the last comma of last element of list
            //outputParamDef.Remove(outputParamDef.Length - 1, 1);
            listOfCodeLines.Add("];");
            listOfCodeLines.Add("}");


        }

        public void writeIntoJSFile() {
			using (StreamWriter outputFile = new StreamWriter (jscadPath, true)) 
			{
				foreach (var codeLine in listOfCodeLines) 
				{
					outputFile.WriteLine (codeLine);
				}
			}

			/*
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
        	*/
		}



		//exporter funktionen


	}
}
