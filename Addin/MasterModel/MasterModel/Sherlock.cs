using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventor;

namespace InvAddIn
{
    class Sherlock
    {
        PartDocument suspect;
        MasterM partypart = new MasterM();
        public void Investigate()
        {
            //sketchy
            foreach (Sketch sketchy in suspect.ComponentDefinition.Sketches)
            {
                partypart.SketchyList.Add(sketchy);
            }
        }
        public void ShowShakespeare()
        {
            
        }
    }
}
