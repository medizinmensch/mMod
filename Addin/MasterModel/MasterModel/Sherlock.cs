using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventor;
using System.Windows.Forms;

namespace InvAddIn
{
    public class Sherlock
    {
        //public PartDocument suspect;
        MasterM partypart = new MasterM();
        PartDocument Suspect { get; set; }

        public Sherlock(PartDocument suspect)
        {
            Suspect = suspect;
            Investigate();
        }
        public void Investigate()
        {
            //sketches
            foreach (Sketch sketchy in Suspect.ComponentDefinition.Sketches)
            {
                partypart.SketchyList.Add(sketchy);
            }
            //Parameter
            partypart.param = Suspect.ComponentDefinition.Parameters;

            ShowTestDialoges();

        }

        public void ShowTestDialoges()
        {
            List<string> entetyNames = new List<string>();
            foreach (var sketchy in partypart.SketchyList)
            {
                entetyNames.Add(sketchy.Name + ":\n");
                foreach (Inventor.SketchEntity Ente in sketchy.SketchEntities)
                {
                    entetyNames.Add(Ente.Type.ToString() + ", ");
                }
                entetyNames.Add("\n");
            }
            MessageBox.Show(string.Join("", entetyNames));

            //List<string> parameterList = new List<string>();
            //for (int i = 0; i < partypart.param.Count-1; i++)
            //{
            //    string p1 = "Test";//partypart.param[i].Name;
            //    string p2 = partypart.param[i].Expression;
            //    parameterList.Add(p1 + " - " + p2 + "\n");
            //}
            //MessageBox.Show("ALLE PARAMETER: \n" + string.Join(",", parameterList));

            //List<string> userParameterList = new List<string>();
            //for (int i = 0; i < partypart.param.UserParameters.Count; i++)
            //{
            //    userParameterList.Add(partypart.param.UserParameters[i] + " - " + partypart.param.UserParameters[i].Expression + "\n");
            //}
            //MessageBox.Show("ALLE PARAMETER: \n" + string.Join(",", userParameterList));

        }
        public void ShowShakespeare(string pathypath)
        {
            Shakespeare Shakey = new Shakespeare(partypart, pathypath);
        }
    }
}
