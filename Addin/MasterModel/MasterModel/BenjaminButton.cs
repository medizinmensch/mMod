using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventor;
using System.Drawing;

namespace InvAddIn
{
    internal class BenjaminButton : Button
    {
        private bool IsHelp;
        #region "Methods"

        public BenjaminButton(string displayName, string internalName, CommandTypesEnum commandType, string clientId, string description, string tooltip, stdole.IPictureDisp standardIcon, stdole.IPictureDisp largeIcon, ButtonDisplayEnum buttonDisplayType, bool Ishelp)
			: base(displayName, internalName, commandType, clientId, description, tooltip, standardIcon, largeIcon, buttonDisplayType)
        {
            IsHelp = Ishelp;
        }
        public BenjaminButton(string displayName, string internalName, CommandTypesEnum commandType, string clientId, string description, string tooltip, ButtonDisplayEnum buttonDisplayType, bool Ishelp)
			: base(displayName, internalName, commandType, clientId, description, tooltip, buttonDisplayType)
		{
		    IsHelp = Ishelp;
        }

        protected override void ButtonDefinition_OnExecute(NameValueMap context)
        {
            try
            {
                PartDocument wholeDocument = Button.InventorApplication.ActiveDocument as PartDocument;
                var transGeo = Button.InventorApplication.TransientGeometry;
                if (IsHelp)
                {
                    MessageBox.Show("-2D Skizze ohne Splines erstellen\r\n-Nur ein Profil pro Skizze nutzen\r\n-Profile nur für Extrusion oder Drehung nutzen\r\n-Für Drehungen ausschließlich die Z-Achse als Rotationsachse nutzen");
                }
                else
                {
                    Sherlock sherlockReader = new Sherlock(wholeDocument, transGeo);

                    // Displays a SaveFileDialog so the user can save the File
                    SaveFileDialog saveFileDialog = new SaveFileDialog()
                    {
                        Filter = "OpenJSCAD / JavaScript|*.js",
                        Title = "Save as MasterModel",
                        CheckPathExists = true
                    };

                    if (saveFileDialog.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(saveFileDialog.FileName))
                        sherlockReader.ShowShakespeare(saveFileDialog.FileName);
                }

            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        #endregion
    }
}