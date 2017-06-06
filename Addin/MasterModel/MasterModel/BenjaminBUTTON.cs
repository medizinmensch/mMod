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
    internal class BenjaminBUTTON : Button
    {
        public PartDocument HeySherlock;
        #region "Methods"

        public BenjaminBUTTON(string displayName, string internalName, CommandTypesEnum commandType, string clientId, string description, string tooltip, stdole.IPictureDisp standardIcon, stdole.IPictureDisp largeIcon, ButtonDisplayEnum buttonDisplayType)
			: base(displayName, internalName, commandType, clientId, description, tooltip, standardIcon, largeIcon, buttonDisplayType)
		{

        }
        public BenjaminBUTTON(string displayName, string internalName, CommandTypesEnum commandType, string clientId, string description, string tooltip, ButtonDisplayEnum buttonDisplayType)
			: base(displayName, internalName, commandType, clientId, description, tooltip, buttonDisplayType)
		{

        }

        override protected void ButtonDefinition_OnExecute(NameValueMap context)
        {
            try
            {
                HeySherlock = (PartDocument) Button.InventorApplication.ActiveDocument;
                Sherlock sher = new Sherlock();
                sher.Investigate(HeySherlock);
                WhatWhereWhy www =new WhatWhereWhy();
                www.Show();
                sher.ShowShakespeare(www.Path);

            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        #endregion
    }
}