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
        public void Investigate(PartDocument suspect)
        {
            List<string> test = new List<string>();
            //sketches
            foreach (Sketch sketchy in suspect.ComponentDefinition.Sketches)
            {
                partypart.SketchyList.Add(sketchy);
                //Test
                test.Add("    "+sketchy.Name+": ");
                foreach (Inventor.SketchEntity Ente in sketchy.SketchEntities)
                {
                    test.Add( Ente.Type.ToString());

                }
                //End Test
            }
            //Test
            var message = string.Join(",", test);
            MessageBox.Show(message);
            //End Test

            //Parameter
            partypart.param = suspect.ComponentDefinition.Parameters;
            //Test
            var message2 = string.Join(",", partypart.param.ToString());
            MessageBox.Show(message2);
            //End Test
        }
        public void ShowShakespeare()
        {
           //Shakespeare Shakey = new Shakespeare(partypart);   
        }
    }
}
