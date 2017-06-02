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
        string name;
        string caption;
        string type;    //float, slider

        float initial;
        float step;


        //optional parameters

        //when slider is used
        float min;
        float max;


        public string getParameterString() {

            string completeString = "";

            completeString += "{ ";
            completeString += "name: '" + name + "', ";
            completeString += "caption: '" + caption + "', ";
            completeString += "type: '" + type + "', ";
            completeString += "initial: " + initial + ", ";
            completeString += "step: " + step + ", ";

            if(type == "slider") {
                completeString += "max: " + max + ", ";
                completeString += "min: " + min + ", ";

            }

            completeString += " }";

            return completeString;

        }
    }


}
