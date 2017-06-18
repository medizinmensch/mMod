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
        private readonly Sherlock SherlockReader;
        public WhatWhereWhy(Sherlock sherlockReader)
        {
            InitializeComponent();
            SherlockReader = sherlockReader;
        }

        private void SaveMe_Click(object sender, EventArgs e)
        {
            // Displays a SaveFileDialog so the user can save the File
            // assigned to Button SaveMe.
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                Filter = "OpenJSCAD / JavaScript|*.js",
                Title = "Save as MasterModel",
                CheckPathExists = true
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(saveFileDialog.FileName))
                    SherlockReader.ShowShakespeare(saveFileDialog.FileName);

        }
    }
}
