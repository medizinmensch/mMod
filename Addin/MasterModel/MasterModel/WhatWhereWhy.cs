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
        Sherlock _sherlockReader;
        public WhatWhereWhy(Sherlock sherlockReader)
        {
            InitializeComponent();
            _sherlockReader = sherlockReader;
        }

        private void SaveMe_Click(object sender, EventArgs e)
        {
            // Displays a SaveFileDialog so the user can save the File
            // assigned to Button SaveMe.
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                Filter = "OpenJSCAD|*.jscad|txt file |*.txt",
                Title = "Save as MasterModel"
            };

            saveFileDialog.ShowDialog();

            // If the file name is not an empty string open it for saving.
            if (saveFileDialog.FileName != "")
            {
                saveFileDialog.CheckFileExists = true;
                saveFileDialog.CheckPathExists = true;
                _sherlockReader.ShowShakespeare(saveFileDialog.FileName);
            }

        }
    }
}
