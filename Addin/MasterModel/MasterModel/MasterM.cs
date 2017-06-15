using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventor;

namespace InvAddIn
{
    public class MasterM
    {
        public PartDocument InventorDocument { get;}
        public List<Sketch> SketchyList
        {
            get
            {
                List<Sketch> tmp = new List<Sketch>();
                foreach (Sketch sketchy in InventorDocument.ComponentDefinition.Sketches)
                {
                    tmp.Add(sketchy);
                }
                return tmp;

            }
        }

        public MasterM(PartDocument inventorDocument)
        {
            InventorDocument = inventorDocument;
        }

        public List<Parameter> GetCustomParameters
        {
            get
            {   
                List<Parameter> toReturn = new List<Parameter>();
                Parameters inventorParameters = InventorDocument.ComponentDefinition.Parameters;
                for (int i = 1; i < inventorParameters.Count; i++)
                {
                    if (inventorParameters[i].ParameterType == ParameterTypeEnum.kUserParameter)
                    {
                        Parameter tmpParam =
                            new Parameter
                            {
                                Name = inventorParameters[i].Name,
                                Initial = inventorParameters[i]._Value,
                                Caption = inventorParameters[i].Comment
                            };

                        toReturn.Add(tmpParam);
                    }
                }
                return toReturn;
            }
        }

        public void Get3DModelInformation()
        {
            PartComponentDefinition partComponentDefinition = InventorDocument.ComponentDefinition;
            foreach (PlanarSketch planarSketch in partComponentDefinition.Sketches)
            {
                foreach (Profile planarSketchProfile in planarSketch.Profiles)
                {
                    string a = "planarSketchProfile.Application: " + planarSketchProfile.Application + "\n";
                    string b = "planarSketchProfile.AttributeSets: " + planarSketchProfile.AttributeSets + "\n";
                    string c = "planarSketchProfile.Count: " + planarSketchProfile.Count + "\n";
                    string d = "planarSketchProfile.Parent: " + planarSketchProfile.Parent + "\n";
                    string e = "planarSketchProfile.RegionProperties: " + planarSketchProfile.RegionProperties + "\n";
                    string f = "planarSketchProfile.Type: " + planarSketchProfile.Type + "\n";
                    string g = "planarSketchProfile.Wires: " + planarSketchProfile.Wires+ "\n";

                    MessageBox.Show(a + b + c + d + e + f + g);
                }
            }
        }


        /// <summary>
        /// Gives you all the Entitys of one Sketch
        /// </summary>
        /// <param name="sketchy">The Sketch you want the Entitys from</param>
        /// <returns>List of Entitys (Containing stuff like Arcs, )</returns>
        public static List<SketchEntity> GetSketchParts(Sketch sketchy)
        {
            List<SketchEntity> sketchParts = new List<SketchEntity>();
            foreach (SketchEntity part in sketchy.SketchEntities)
            {
                sketchParts.Add(part);
            }
            return sketchParts;
        }
    }
}
