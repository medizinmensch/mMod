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
        public PartDocument InventorDocument { get; }
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


        /// <summary>
        /// Everything you need to know about Extrusions
        /// </summary>
        /// <returns>List of "ExtrudeFeature"s</returns>
        public List<ExtrudeFeature> GetExtrudeFeatures()
        {
            PartComponentDefinition partComponentDefinition = InventorDocument.ComponentDefinition;

            List<ExtrudeFeature> toReturn = new List<ExtrudeFeature>();

            foreach (ExtrudeFeature extrudeFeature in partComponentDefinition.Features.ExtrudeFeatures)
            {
                toReturn.Add(extrudeFeature);
                
                if (extrudeFeature.ExtentType == PartFeatureExtentEnum.kDistanceExtent) //wir können nur Distance extend benutzen!
                {
                    Inventor.Parameter param = (extrudeFeature.Definition.Extent as DistanceExtent).Distance; //Value vorbereiten und unten dann abgreifen





                    List<string> msg = new List<string>
                    {
                        extrudeFeature.ExtendedName, //z.B. "Neuer Volumenkörper x 17mm"
                        extrudeFeature.Name, //z.B. "Extrusion1"
                        extrudeFeature.Profile.Type.ToString(),//enthält das Profil (Profil.Parent sollte die Skizze enthalten)
                        extrudeFeature.ExtentType.ToString(),// enthält die möglichen ExtentType typen. Z.B. kDistanceExtend, kThroughAllExtent, kFromToExtent, kToNextExtent. Wir gehen mal von kDistanceExtend aus - das ist das normale mit "17 mm" oder so.
                        extrudeFeature.Operation.ToString(),// z.B. kNewBodyOperation, kIntersectOperation, kCutOperation, kJoinOperation
                        extrudeFeature.Definition.IsTwoDirectional.ToString(),// bei der angabe kannste abbrechen da die Extrusion in beide richtungen geht. Es sind generell auch asyncrone Bidirektionale Extrude operationen möglich, ich weiß allerdings noch nicht inwiefern uns dieses eNum uns darüber informationen gibt
                        param._Value.ToString() // ENDLICH! die Extrusion-Distance als double Value!
                    };

                    if (extrudeFeature.Operation == PartFeatureOperationEnum.kIntersectOperation)
                    {
                        //extrudeFeature.ExtentTwo.Parent.
                    }
                }


                if (extrudeFeature.Definition.IsTwoDirectional)
                {
                    NotImplementedTypes.Add("extrudeFeature " + extrudeFeature.Name + ": IsTwoDirectional");
                }
                if (extrudeFeature.Profile.Count > 1)
                {
                    NotImplementedTypes.Add("extrudeFeature " + extrudeFeature.Name + ": Only 1 Profile per Sketch");
                }
                if (extrudeFeature.Operation != PartFeatureOperationEnum.kNewBodyOperation)
                {
                    NotImplementedTypes.Add("extrudeFeature " + extrudeFeature.Name + ": only kNewBodyOperation allowed");
                }
                if (extrudeFeature.ExtentType != PartFeatureExtentEnum.kDistanceExtent)
                {
                    NotImplementedTypes.Add("extrudeFeature " + extrudeFeature.Name + ": only kDistanceExtent allowed");
                }

            }

            return toReturn;

            #region old
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

            //partComponentDefinition.Features.ExtrudeFeatures.AddByDistanceExtent(
            //    partComponentDefinition.Sketches[1].Profiles[1], "20 mm",
            //    PartFeatureExtentDirectionEnum.kNegativeExtentDirection, PartFeatureOperationEnum.kNewBodyOperation);


            //foreach (PartFeature partFeature in partComponentDefinition.Features)
            //{
            //    foreach (Inventor.Parameter partFeatureParameter in partFeature.Parameters)
            //    {
            //        MessageBox.Show(string.Join(", ", partFeature.Name, partFeature.ExtendedName,partFeatureParameter.Value, partFeatureParameter.Name, partFeatureParameter.ParameterType,partFeatureParameter.Expression));
            //    }
            //}

            //partComponentDefinition.Features.ExtrudeFeatures.AddByToExtent(
            //    partComponentDefinition.Sketches[1].Profiles[1], "30 In", PartFeatureOperationEnum.kJoinOperation);

            #endregion

        }

        public enum ExtrudeDirection
        {
            Positive,
            Negative,
            Symetric
        }

        public static ExtrudeDirection GetDirection(ExtrudeFeature feature)
        {
            var extrDef = feature.Extent as DistanceExtent;
            PartFeatureExtentDirectionEnum direction = extrDef.Direction;

            if (direction == PartFeatureExtentDirectionEnum.kNegativeExtentDirection)
            {
                return ExtrudeDirection.Negative;
            }
            else if (direction == PartFeatureExtentDirectionEnum.kPositiveExtentDirection)
            {
                return ExtrudeDirection.Positive;
            }
            else
            {
                return ExtrudeDirection.Symetric;
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

        public List<EmbossFeature> GetEmbossFeatures()
        {
            List<EmbossFeature> bossyFeatures = new List<EmbossFeature>();

            if (InventorDocument.ComponentDefinition.Features.EmbossFeatures.Count > 0)
            {
               foreach (EmbossFeature boss in InventorDocument.ComponentDefinition.Features.EmbossFeatures)
                {
                    bossyFeatures.Add(boss);
                    //EmbossFeature.Depth returns the parameter controlling the depth
                    //EmbossFeature.parent returns the (text)sketch 
                } 
            }
            return bossyFeatures;

        }

        public List<RevolveFeature> GetRevolveFeatures()
        {
            PartComponentDefinition partComponentDefinition = InventorDocument.ComponentDefinition;

            List<RevolveFeature> toReturn = new List<RevolveFeature>();

            foreach (RevolveFeature revolveFeature in partComponentDefinition.Features.RevolveFeatures)
            {
                toReturn.Add(revolveFeature);

                //MessageBox.Show(revolveFeature.Parameters[revolveFeature.Parameters.Count].Value.ToString());
                if (revolveFeature.ExtentType == PartFeatureExtentEnum.kFullSweepExtent) //wenn volle Rotation
                {

                    List<string> msg = new List<string>
                    {
                        revolveFeature.ExtendedName, //z.B. "Neue Rotation 45°"
                        revolveFeature.Name, //z.B. "Umdrehung1"
                        revolveFeature.Profile.Type.ToString(), //enthält das Profil (Profil.Parent sollte die Skizze enthalten)
                        revolveFeature.ExtentType.ToString(), // enthält die möglichen ExtentType typen. Z.B. kAngleExtent oder kFullSweepExtent.
                        revolveFeature.Operation.ToString(), // z.B. kNewBodyOperation, kIntersectOperation, kCutOperation, kJoinOperation
                        revolveFeature.AxisEntity.ToString(), // gibt die Rotationsachse zurück
                        "360" // Der Rotationswinkel
                    };
                }
                if (revolveFeature.ExtentType == PartFeatureExtentEnum.kAngleExtent) //wenn drehung mit bestimmten Winkel
                {
                    double Rotationswinkel = 360 / (2 * Math.PI) * double.Parse(revolveFeature.Parameters[revolveFeature.Parameters.Count].Value.ToString());

                    List<string> msg = new List<string>
                    {
                        revolveFeature.ExtendedName, //z.B. "Neue Rotation 45°"
                        revolveFeature.Name, //z.B. "Umdrehung1"
                        revolveFeature.Profile.Type.ToString(), //enthält das Profil (Profil.Parent sollte die Skizze enthalten)
                        revolveFeature.ExtentType.ToString(), // enthält die möglichen ExtentType typen. Z.B. kAngleExtent oder kFullSweepExtent.
                        revolveFeature.Operation.ToString(), // z.B. kNewBodyOperation, kIntersectOperation, kCutOperation, kJoinOperation
                        revolveFeature.AxisEntity.ToString(), // gibt die Rotationsachse zurück
                        revolveFeature.Parameters[revolveFeature.Parameters.Count].Name, // Der Rotationswinkelparametername
                        Rotationswinkel.ToString() // Der Rotationswinkelwert
                    };
                }

                if (revolveFeature.Profile.Count > 1)
                {
                    NotImplementedTypes.Add("revolveFeature " + revolveFeature.Name + ": Only 1 Profile per Sketch");
                }

            }
            return toReturn;
        }

        private static List<object> GetFeatures(PartComponentDefinition partComponentDefinition)
        {
            List<object> myFeatures = new List<object>();

            PartFeatures partFeatures = partComponentDefinition.Features;
            foreach (PartFeature partFeature in partFeatures)
            {
                if (partFeature.Type == ObjectTypeEnum.kRevolveFeaturesObject)
                {
                    RevolveFeature feature = partFeature as RevolveFeature;
                    myFeatures.Add(feature);
                }
                else if (partFeature.Type == ObjectTypeEnum.kExtrudeFeatureObject)
                {
                    ExtendFeature feature = partFeature as ExtendFeature;
                    myFeatures.Add(feature);
                }
            }

            return myFeatures;
        }
    }
}
