using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OntologyEditor
{
    public partial class SelectIndivid : Form
    {
        public SelectIndivid(List<string> LC)
        {
            InitializeComponent();
            ListCopy = LC;
        }
        List<string> ListCopy = new List<string>();

        private void SelectIndivid_Load(object sender, EventArgs e)
        {
            foreach (string s in ListCopy) listBox1.Items.Add(s);
            TransferData.StrValue = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex > -1) TransferData.StrValue = listBox1.SelectedItem.ToString();
            this.Close();
        }


    }
}
