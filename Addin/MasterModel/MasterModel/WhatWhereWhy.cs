using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace InvAddIn
{
    public partial class WhatWhereWhy : Form
    {
        public string Path;
        private Sherlock sherly;
        public WhatWhereWhy(Sherlock Lock)
        {
            InitializeComponent();
            sherly = Lock;
        }

        private void SaveMe_Click(object sender, EventArgs e)
        {
            // Displays a SaveFileDialog so the user can save the File
            // assigned to Button SaveMe.
            SaveFileDialog SFD = new SaveFileDialog();
            SFD.Filter = "OpenJSCAD|*.jscad|txt file |*.txt";
            SFD.Title = "Save as MasterModel";
            SFD.ShowDialog();

            // If the file name is not an empty string open it for saving.
            if (SFD.FileName != "")
            {
                SFD.CheckFileExists = true;
                SFD.CheckPathExists = true;
                Path = SFD.FileName;
            }
            sherly.ShowShakespeare(Path);
        }
    }
}
