using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventor;

namespace InvAddIn
{
    public class MasterM
    {
        public PartDocument InventorDocument { get; set; }
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
