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
    public partial class SelectNode : Form
    {
        public SelectNode(TreeNodeCollection TC)
        {
            InitializeComponent();
            TreeCopy = TC;
        }
        TreeNodeCollection TreeCopy;

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

        private void SelectNode_Load(object sender, EventArgs e)
        {
            foreach (TreeNode node in TreeCopy) CopyNode(node, treeView1.Nodes);
            treeView1.ExpandAll();
            TransferData.StrValue = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null) TransferData.StrValue = treeView1.SelectedNode.Text;
            this.Close();
        }


    }
}
