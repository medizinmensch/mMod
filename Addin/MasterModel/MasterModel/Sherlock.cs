using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventor;
using System.Windows.Forms;
using System.Diagnostics;
using System.Globalization;


namespace InvAddIn
{
    public class Sherlock
    {
        //public PartDocument suspect;
        private readonly MasterM Partypart;

        /// <summary>
        /// Sherlock lets you Read all relevant Information for the jsCad processing from a PartDocument Inventor File
        /// </summary>
        /// <param name="suspect">your PartDocument you want to get information about</param>
        public Sherlock(PartDocument suspect)
        {
            Partypart = new MasterM(suspect);

#if DEBUG
            ShowTestDialoges();
            Partypart.Get3DModelInformation();
#endif
        }

        /// <summary>
        /// This is for Testing / Debuging purpose only
        /// </summary>
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
            Parameters documentParameters = Partypart.InventorDocument.ComponentDefinition.Parameters;
            parameterList.Add("Name / Value / Comment");
            for (int i = 1; i < Partypart.InventorDocument.ComponentDefinition.Parameters.Count; i++)
            {
                try
                {
                    string p1 = documentParameters[i].Name;
                    string p2 = documentParameters[i]._Value.ToString(CultureInfo.InvariantCulture);
                    string p3 = documentParameters[i].Comment;
                    Inventor.Parameter test = documentParameters[i];
                    parameterList.Add(p1 + " - " + p2 + " - " + p3 + "\n");
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString());
                }
            }
            MessageBox.Show("ALLE PARAMETER: \n" + string.Join(",", parameterList));

        }
        public void ShowShakespeare(string pathypath)
        {
            Shakespeare shakey = new Shakespeare(Partypart, pathypath);
        }
    }
}
