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

        public List<string> NotImplementedTypes { get; } = new List<string>();

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


        public List<ExtrudeFeature> GetExtrudeFeatures()
        {
            PartComponentDefinition partComponentDefinition = InventorDocument.ComponentDefinition;

            //foreach (PartFeature partFeature in partComponentDefinition.Features)
            //{
            //    foreach (Inventor.Parameter partFeatureParameter in partFeature.Parameters)
            //    {
            //        MessageBox.Show(string.Join(", ", partFeature.Name, partFeature.ExtendedName,partFeatureParameter.Value, partFeatureParameter.Name, partFeatureParameter.ParameterType,partFeatureParameter.Expression));
            //    }
            //}

            //partComponentDefinition.Features.ExtrudeFeatures.AddByToExtent(
            //    partComponentDefinition.Sketches[1].Profiles[1], "30 In", PartFeatureOperationEnum.kJoinOperation);

            partComponentDefinition.Features.ExtrudeFeatures.AddByDistanceExtent(
                partComponentDefinition.Sketches[1].Profiles[1], "20 mm",
                PartFeatureExtentDirectionEnum.kNegativeExtentDirection, PartFeatureOperationEnum.kNewBodyOperation);

            List<ExtrudeFeature> toReturn = new List<ExtrudeFeature>();
            
            foreach (ExtrudeFeature extrudeFeature in partComponentDefinition.Features.ExtrudeFeatures)
            {
                toReturn.Add(extrudeFeature);
                List<string> msg = new List<string>();
                msg.Add(extrudeFeature.ExtendedName); //z.B. "Neuer Volumenkörper x 17mm"
                msg.Add(extrudeFeature.Name); //z.B. "Extrusion1"
                msg.Add(extrudeFeature.Profile.Type.ToString()); //enthält das Profil (Profil.Parent sollte die Skizze enthalten)
                msg.Add(extrudeFeature.ExtentType.ToString()); // enthält die möglichen ExtentType typen. Z.B. kDistanceExtend, kThroughAllExtent, kFromToExtent, kToNextExtent. Wir gehen mal von kDistanceExtend aus - das ist das normale mit "17 mm" oder so.
                msg.Add(extrudeFeature.Operation.ToString()); // z.B. kNewBodyOperation, kIntersectOperation, kCutOperation, kJoinOperation
                msg.Add(extrudeFeature.Definition.IsTwoDirectional.ToString()); // bei der angabe 

                foreach (Inventor.Parameter parameter in extrudeFeature.Parameters)
                {
                    msg.Add(parameter._Value.ToString());
                }
                //msg.Add(extrudeFeature.Definition.Extent.Distance.);

                if (extrudeFeature.Definition.IsTwoDirectional)
                {
                    NotImplementedTypes.Add("extrudeFeature " + extrudeFeature.ExtendedName + ": IsTwoDirectional");
                }
                if (extrudeFeature.Profile.Count>1)
                {
                    NotImplementedTypes.Add("extrudeFeature " + extrudeFeature.ExtendedName + ": Only 1 Profile per Sketch");
                }
                if (extrudeFeature.Operation != PartFeatureOperationEnum.kNewBodyOperation)
                {
                    NotImplementedTypes.Add("extrudeFeature " + extrudeFeature.ExtendedName + ": only kNewBodyOperation allowed");
                }
                if (extrudeFeature.ExtentType != PartFeatureExtentEnum.kDistanceExtent)
                {
                    NotImplementedTypes.Add("extrudeFeature " + extrudeFeature.ExtendedName + ": only kDistanceExtent allowed");
                }

                MessageBox.Show(string.Join("\n", msg));

            }

            return toReturn;


            //foreach (PlanarSketch planarSketch in partComponentDefinition.Sketches)
            //{
            //    foreach (Profile planarSketchProfile in planarSketch.Profiles)
            //    {
            //        string b = "planarSketchProfile.AttributeSets.Type: " + planarSketchProfile.AttributeSets.Type + "\n";
            //        string c = "planarSketchProfile.Count: " + planarSketchProfile.Count + "\n";
            //        string d = "planarSketchProfile.Parent: " + planarSketchProfile.Parent.Type + "\n";
            //        string e = "planarSketchProfile.RegionProperties: " + planarSketchProfile.RegionProperties.Type+ "\n";
            //        string f = "planarSketchProfile.Type: " + planarSketchProfile.Type + "\n";
            //        string g = "planarSketchProfile.Wires: " + planarSketchProfile.Wires.Type + "\n";

            //        foreach (AttributeSet attributeSet in planarSketchProfile.AttributeSets)
            //        {
            //            attributeSet.
            //        }
            //        MessageBox.Show(b + c + d + e + f + g);
            //    }
            //}
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
