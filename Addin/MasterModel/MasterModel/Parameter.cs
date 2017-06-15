using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventor;
using System.IO;

namespace InvAddIn
{
    public class Parameter
    {
        //https://en.wikibooks.org/wiki/OpenJSCAD_Quick_Reference#Parameter_Types

        //essential parameters
        public string Name { get; set; }
        public string Caption { get; set; }
        public string ParameterType { get; set; }

        public double Initial { get; set; }
        public double Step { get; set; }

        //optional parameters

        //when slider is used
        public double Min { get; set; }
        public double Max { get; set; }

        //constructor for float
        public Parameter(String name, String caption, String parameterType, double initial, double step)          
        {
            this.Name = name;
            this.Caption = caption;
            this.ParameterType = parameterType;
            this.Initial = initial;
            this.Step = step;
        }

        //constructor for slider
        public Parameter(String name, String caption, String parameterType, double initial,
            double step, double min, double max)          
        {
            this.Name = name;
            this.Caption = caption;
            this.ParameterType = parameterType;
            this.Initial = initial;
            this.Step = step;
            this.Min = min;
            this.Max = max;
        }


        public string GetParameterString() {

            string completeString = "\t\t";

            completeString += "{ ";
            completeString += "name: '" + Name + "', ";
            completeString += "caption: '" + Caption + "', ";
            completeString += "type: '" + ParameterType + "', ";
            completeString += "initial: '" + Initial + "', ";
            
            if(ParameterType == "float")
            {
                completeString += "step: " + Step;
            }

            if(ParameterType == "slider") {
                completeString += "step: " + Step + ", ";     
                completeString += "max: " + Max + ", ";
                completeString += "min: " + Min;               
            }
            completeString += " }";

            return completeString;

        }
    }


}
