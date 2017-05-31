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
        public Document suspect;
        MasterM partypart = new MasterM();
        public void Investigate()
        {
            //sketches
            foreach (Sketch sketchy in suspect.ComponentDefinition.Sketches)
            {
                partypart.SketchyList.Add(sketchy);
                MessageBox.Show(sketchy.AttributeSets.ToString());
            }
            //Parameter
            partypart.param = suspect.ComponentDefinition.Parameters;
        }
        public void ShowShakespeare()
        {
           Shakespeare Shakey = new Shakespeare(partypart);   
        }
    }
}
