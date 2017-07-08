﻿using System;
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
                    MessageBox.Show("Hep Text");
                }
                else
                {
                    Sherlock sherlockReader = new Sherlock(wholeDocument, transGeo);
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