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
    public partial class SelectProperty : Form
    {
        public SelectProperty(TreeNodeCollection TL, TreeNodeCollection TR)
        {
            InitializeComponent();
            TreeCopyLeft = TL;
            TreeCopyRight = TR;

        }
        TreeNodeCollection TreeCopyLeft;
        TreeNodeCollection TreeCopyRight;

        public void CopyNode(TreeNode node, TreeNodeCollection dest)
        {
            TreeNode copy = new TreeNode(node.Name);
            copy.Tag = node.Tag;
            copy.Text = node.Text;
            dest.Add(copy);
            foreach (TreeNode child in node.Nodes)
            {
                CopyNode(child, copy.Nodes);
            }
        }

        private void SelectProperty_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;

            foreach (TreeNode node in TreeCopyLeft) CopyNode(node, treeView1.Nodes);
            treeView1.ExpandAll();
            foreach (TreeNode node in TreeCopyRight) CopyNode(node, treeView2.Nodes);
            treeView2.ExpandAll();

            TransferData.StrValue = "";
            TransferData.StrValue2 = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null) TransferData.StrValue = treeView1.SelectedNode.Text;
            if (treeView2.SelectedNode != null) TransferData.StrValue2 = treeView2.SelectedNode.Text;
            TransferData.StrValue3 = comboBox1.SelectedItem.ToString();
            this.Close();
        }

        private void SelectProperty_SizeChanged(object sender, EventArgs e)
        {
            toolStripTextBox1.Width = menuStrip1.Width-7;
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (treeView2.SelectedNode == null) toolStripTextBox1.Text = treeView1.SelectedNode.Text + " (" + comboBox1.SelectedItem.ToString() + ") ";
            else toolStripTextBox1.Text = treeView1.SelectedNode.Text + " (" + comboBox1.SelectedItem.ToString() + ") " + treeView2.SelectedNode.Text;   
        }

        private void treeView2_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (treeView1.SelectedNode == null) toolStripTextBox1.Text = " (" + comboBox1.SelectedItem.ToString() + ") " + treeView2.SelectedNode.Text;
            else toolStripTextBox1.Text = treeView1.SelectedNode.Text + " (" + comboBox1.SelectedItem.ToString() + ") " + treeView2.SelectedNode.Text;           
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode == null && treeView2.SelectedNode == null) return;
            else if (treeView2.SelectedNode == null) toolStripTextBox1.Text = treeView1.SelectedNode.Text + " (" + comboBox1.SelectedItem.ToString() + ") ";
            else if (treeView1.SelectedNode == null) toolStripTextBox1.Text = " (" + comboBox1.SelectedItem.ToString() + ") " + treeView2.SelectedNode.Text;
            else toolStripTextBox1.Text = treeView1.SelectedNode.Text + " (" + comboBox1.SelectedItem.ToString() + ") " + treeView2.SelectedNode.Text;           
        }

    }
}
