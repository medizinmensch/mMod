using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventor;
using System.IO;

namespace InvAddIn
{
    public class ParameterDef
    {
        //https://en.wikibooks.org/wiki/OpenJSCAD_Quick_Reference#Parameter_Types

        //essential parameters
        public string Name { get; set; }
        public string Caption { get; set; }
        public string ParapeterType { get; set; }

        public double Initial { get; set; }
        public double Step { get; set; }

        //optional parameters

        //when slider is used
        public double Min { get; set; }
        public double Max { get; set; }

        public ParameterDef() { }

        public ParameterDef(String name, String caption, String parapeterType, double initial,
            double step, double min, double max){
            this.Name = name;
            this.Caption = caption;
            this.ParapeterType = parapeterType;
            this.Initial = initial;
            this.Step = step;
            this.Min = min;
            this.Max = max;
        }


        public string GetParameterString() {

            string completeString = "";

            completeString += "{ ";
            completeString += "name: '" + Name + "', ";
            completeString += "caption: '" + Caption + "', ";
            completeString += "ParapeterType: '" + ParapeterType + "', ";
            completeString += "initial: '" + Initial + "', ";
            
            if(ParapeterType == "float")
            {
                completeString += "step: " + Step;
            }

            if(ParapeterType == "slider") {
                completeString += "step: " + Step + ", ";     
                completeString += "max: " + Max + ", ";
                completeString += "min: " + Min;               
            }
            completeString += " }";

            return completeString;

        }
    }


}
