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
        private string ParameterType { get; set; }

        public double Initial { get; set; }
        private double Step { get; set; }

        //optional parameters

        //when slider is used
        private double Min { get; set; }
        private double Max { get; set; }

        //constructor for float
        public Parameter(string name, string caption, string parameterType, double initial, double step)          
        {
            this.Name = name;
            this.Caption = caption;
            this.ParameterType = parameterType;
            this.Initial = initial;
            this.Step = step;
        }
        
        public Parameter() { }

        //constructor for slider
        public Parameter(string name, string caption, string parameterType, double initial,
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
            completeString += "initial: '" + SubstituteCommaWithDot(Initial) + "', ";
            
            if(ParameterType == "float")
            {
                completeString += "step: " + SubstituteCommaWithDot(Step);
            }

            if(ParameterType == "slider") {
                completeString += "step: " + SubstituteCommaWithDot(Step) + ", ";     
                completeString += "max: " + SubstituteCommaWithDot(Max) + ", ";
                completeString += "min: " + SubstituteCommaWithDot(Min);               
            }
            completeString += " }";

            return completeString;

        }

        private string SubstituteCommaWithDot(double value)
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
