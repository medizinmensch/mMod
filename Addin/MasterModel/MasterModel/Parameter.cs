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
        string name;
        string caption;
        string type;    //float, slider

        double initial;
        double step;


        //optional parameters

        //when slider is used
        double min;
        double max;

        public ParameterDef() { }

        public ParameterDef(String name, String caption, String type, double initial,
            double step, double min, double max){
            this.name = name;
            this.caption = caption;
            this.type = type;
            this.initial = initial;
            this.step = step;
            this.min = min;
            this.max = max;
        }


        public string getParameterString() {

            string completeString = "";

            completeString += "{ ";
            completeString += "name: '" + name + "', ";
            completeString += "caption: '" + caption + "', ";
            completeString += "type: '" + type + "', ";
            completeString += "initial: '" + initial + "', ";
            
            if(type == "float")
            {
                completeString += "step: " + step;
            }

            if(type == "slider") {
                completeString += "step: " + step + ", ";     
                completeString += "max: " + max + ", ";
                completeString += "min: " + min;               
            }
            completeString += " }";

            return completeString;

        }
    }


}
