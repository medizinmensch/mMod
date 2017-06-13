using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventor;
using System.Windows.Forms;
using System.Diagnostics;

namespace InvAddIn
{
    public class Sherlock
    {
        //public PartDocument suspect;
        private readonly MasterM Partypart = new MasterM();
        private PartDocument Suspect { get; set; }

        public Sherlock(PartDocument suspect)
        {
            Suspect = suspect;
            Investigate();
        }
        private void Investigate()
        {
            //sketches
            foreach (Sketch sketchy in Suspect.ComponentDefinition.Sketches)
            {
                Partypart.SketchyList.Add(sketchy);
            }
            //Parameter
            Partypart.param = Suspect.ComponentDefinition.Parameters;

            ShowTestDialoges();

        }

        private void ShowTestDialoges()
        {
            List<string> entetyNames = new List<string>();
            foreach (var sketchy in Partypart.SketchyList)
            {
                entetyNames.Add(sketchy.Name + ":\n");
                foreach (Inventor.SketchEntity ente in sketchy.SketchEntities)
                {
                    entetyNames.Add(ente.Type.ToString() + ", ");
                }
                entetyNames.Add("\n");
            }
            Debug.WriteLine(string.Join("", entetyNames));
            MessageBox.Show(string.Join("", entetyNames));

            List<string> parameterList = new List<string>();
            for (int i = 0; i < Partypart.param.Count - 1; i++)
            {
                try
                {
                    string p1 = Partypart.param[i].Name;
                    string p2 = Partypart.param[i].Expression;
                    parameterList.Add(p1 + " - " + p2 + "\n");
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString());
                }
            }
            MessageBox.Show("ALLE PARAMETER: \n" + string.Join(",", parameterList));

            //List<string> userParameterList = new List<string>();
            //for (int i = 0; i < partypart.param.UserParameters.Count; i++)
            //{
            //    userParameterList.Add(partypart.param.UserParameters[i] + " - " + partypart.param.UserParameters[i].Expression + "\n");
            //}
            //MessageBox.Show("ALLE PARAMETER: \n" + string.Join(",", userParameterList));

        }
        public void ShowShakespeare(string pathypath)
        {
            Shakespeare shakey = new Shakespeare(Partypart, pathypath);
        }
    }
}
