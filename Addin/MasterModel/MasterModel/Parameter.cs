using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventor;
using System.IO;
using System.Globalization;

namespace InvAddIn
{
    public class Parameter
    {
        //setting culture to invariant so it prints 0.001 instead of german style: 0,001
        CultureInfo myCultureInfo = new CultureInfo("en-GB");

        //https://en.wikibooks.org/wiki/OpenJSCAD_Quick_Reference#Parameter_Types

        //essential parameters
        public string Name { get; set; }
        public string Caption { get; set; }
        public string ParameterType { get; set; }

        public double Initial { get; set; }
        public double Step { get; set; }

        public string Initial_str { get; set; }
        public double Size { get; set; }

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
        
        public Parameter() { }

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

        // for text
        public Parameter(String name, String caption, String parameterType, string initial,
            double size)
        {
            this.Name = name;
            this.Caption = caption;
            this.ParameterType = parameterType;
            this.Initial_str = initial;
            this.Size = size;
        }

        public string GetParameterString() {

            string completeString = "\t\t";

            completeString += "{ ";

            completeString += "name: '" + Name + "', ";
            completeString += "caption: '" + Caption + "', ";
            completeString += "type: '" + ParameterType + "', ";
                  
            if(ParameterType == "text")
            {
                completeString += "initial: '" + Initial_str + "', ";
                completeString += "size: '" + Size;
            }else if(ParameterType == "float")
            {
                completeString += "initial: '" + Initial.ToString(myCultureInfo) + "', ";
                completeString += "step: " + Step.ToString(myCultureInfo);
            }else if(ParameterType == "slider")
            {
                completeString += "step: " + Step.ToString(myCultureInfo) + ", ";
                completeString += "max: " + Max.ToString(myCultureInfo) + ", ";
                completeString += "min: " + Min.ToString(myCultureInfo);
            }
            completeString += " }";

            return completeString;

        }


    }


}
