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
        public List<Sketch> SketchyList = new List<Sketch>();
        public Parameters param;
        public List<SketchEntity> SketchyParts(Sketch sketchy)
        {
            List<SketchEntity> SketchyP = new List<SketchEntity>();
            foreach (SketchEntity Part in sketchy.SketchEntities)
            {
                SketchyP.Add(Part);
            }
            return SketchyP;
        }
    }
}
