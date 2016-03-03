using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Diagnostics;

namespace OntologyEditor
{
    public partial class Form1 : Form
    {
        public static Ontology Onto = new Ontology();
        TreeNode node;
        string _node;
        float GlobalScale;

        public Form1()
        {
            InitializeComponent();
            DoubleBuffered = true;
        }

        #region Интерфейс

        private TreeNode SearchNode(string SearchText, TreeNode StartNode)
        {
            TreeNode node = null;
            while (StartNode != null)
            {
                if (StartNode.Text.Contains(SearchText))
                {
                    node = StartNode;
                    break;
                }
                if (StartNode.Nodes.Count != 0)
                {
                    node = SearchNode(SearchText, StartNode.Nodes[0]);
                    if (node != null) break;
                }
                StartNode = StartNode.NextNode;
            };
            return node;
        }

        private void Panel1_Invalidate()
        {
            listBox12.Items.Clear();
            listBox1.Items.Clear();
            listBox2.Items.Clear();
            listBox3.Items.Clear();
            textBox1.Text = "";            
            node = treeView1.SelectedNode;
            if (node == null) node = treeView1.Nodes[0];
            _node = node.Text;
            label1.Text = node.FullPath;

            if (_node == "Thing") toolStripButton1.Enabled = toolStripButton5.Enabled = false;
            else toolStripButton1.Enabled = toolStripButton5.Enabled = true;

            ////////
            textBox1.Text = Onto.Classes[_node].annotation;
            foreach (ClassPropertie Cp in Onto.Classes[_node].listOfProperties) listBox12.Items.Add(Cp.objectPropertie + " (" + Cp.typeOfPropertie + ") " + Cp.destObject);
            foreach (string s in Onto.Classes[_node].equalClasses) listBox1.Items.Add(s);
            foreach (string s in Onto.Classes[_node].disjointClasses) listBox2.Items.Add(s);
            foreach (string s in Onto.Classes[_node].listOfIndividuals) listBox3.Items.Add(s);
        }

        private void Panel2_Invalidate()
        {
            listBox4.Items.Clear();
            listBox5.Items.Clear();
            listBox6.Items.Clear();
            listBox7.Items.Clear();
            textBox2.Text = "";
            node = treeView2.SelectedNode;
            if (node == null) node = treeView2.Nodes[0];
            _node = node.Text;
            label2.Text = node.FullPath;

            if (_node == "topObjectProperty") toolStripButton6.Enabled = toolStripButton10.Enabled = false;
            else toolStripButton6.Enabled = toolStripButton10.Enabled = true;

            ////////
            textBox2.Text = Onto.Propetries[_node].annotation;
            foreach (string s in Onto.Propetries[_node].diffArea) listBox4.Items.Add(s);
            foreach (string s in Onto.Propetries[_node].valueArea) listBox5.Items.Add(s);
            foreach (string s in Onto.Propetries[_node].equalProperties) listBox6.Items.Add(s);
            foreach (string s in Onto.Propetries[_node].disjointProperties) listBox7.Items.Add(s);
            checkBox1.Checked = Onto.Propetries[_node].transitive;
            checkBox2.Checked = Onto.Propetries[_node].symmetric;
            checkBox3.Checked = Onto.Propetries[_node].reflexive;
        }

        private void Panel3_Invalidate()
        {
            listBox9.Items.Clear();
            listBox10.Items.Clear();
            listBox11.Items.Clear();
            textBox3.Text = "";
            if (listBox8.SelectedIndex == -1) return;            
            _node = listBox8.SelectedItem.ToString();
            
            ////////
            textBox3.Text = Onto.Individuals[_node].annotation;
            foreach (string s in Onto.Individuals[_node].equalIndividuals) listBox9.Items.Add(s);
            foreach (string s in Onto.Individuals[_node].disjointIndividuals) listBox10.Items.Add(s);
            foreach (string s in Onto.Individuals[_node].listOfClasses) listBox11.Items.Add(s);
        }

        private void Panel41_Invalidate()
        {
            listBox13.Items.Clear();
            listBox16.Items.Clear();
            comboBox2.Items.Clear();

            foreach (OntoClass oc in Onto.Classes)
            {
                listBox13.Items.Add(oc.name);
                listBox16.Items.Add(oc.name);
                if (radioButton2.Checked || checkBox4.Checked) comboBox2.Items.Add(oc.name);
                else if (SearchNode(oc.name, treeView1.Nodes[0]).Nodes.Count > 0) comboBox2.Items.Add(oc.name); 
            }
        }

        private void Panel42_Invalidate()
        {
            listBox14.Items.Clear();
            listBox17.Items.Clear();

            foreach (OntoPropertie op in Onto.Propetries)
            {
                listBox14.Items.Add(op.name);
                listBox17.Items.Add(op.name);
            }
        }

        private void Panel43_Invalidate()
        {
            listBox15.Items.Clear();
            listBox18.Items.Clear();

            foreach (OntoIndividual oi in Onto.Individuals)
            {
                listBox15.Items.Add(oi.name);
                listBox18.Items.Add(oi.name);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            node = treeView1.Nodes[0];
            _node = "Thing";
            Onto.Classes.Add(new OntoClass() { name = "Thing", parentName = "", annotation = "" });
            Onto.Propetries.Add(new OntoPropertie() { name = "topObjectProperty", parentName = "", annotation = "", symmetric = false, reflexive = false, transitive = false });

            comboBox1.SelectedIndex = 0;
            diagramBox1.Diagram = Onto.diagram;
            GlobalScale = 1.0f;
            TransferData.IndividCollapse = false;
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            if (this.Height > 650)
            {
                groupBox1.Height = this.Height - 523;
                groupBox12.Height = this.Height - 523;
                groupBox20.Height = this.Height - 407;
                listBox8.Height = this.Height - 155;
                groupBox22.Height = this.Height - 586;
            }
            else
            {
                groupBox1.Height = 127;
                groupBox12.Height = 127;
                groupBox20.Height = 243;
                listBox8.Height = 495;
                groupBox22.Height = 64;
            }
        }

        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            if (e.TabPageIndex == 0) Panel1_Invalidate();
            else if (e.TabPageIndex == 1) Panel2_Invalidate();
            else if (e.TabPageIndex == 2) Panel3_Invalidate();
            else if (e.TabPageIndex == 3) diagramBox1.Invalidate();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (toolStripStatusLabel2.Text == "") toolStripStatusLabel2.Text = "   * изменен";
        }

        #endregion

        #region Tree1

        private void toolStripButton3_Click(object sender, EventArgs e) //развернуть
        {
            treeView1.ExpandAll();
        }

        private void toolStripButton4_Click(object sender, EventArgs e) //свернуть
        {
            treeView1.CollapseAll();
            treeView1.SelectedNode = null;
        }

        private void toolStripButton1_Click(object sender, EventArgs e) //добавить корень
        {
            List<string> StrMas = new List<string>();
            foreach (OntoClass c in Onto.Classes) StrMas.Add(c.name);
            foreach (string s in listBox8.Items) StrMas.Add(s);

            AddNode f = new AddNode(StrMas);
            f.ShowDialog();
            if (TransferData.StrValue == "") return;

            node.Parent.Nodes.Add(TransferData.StrValue);

            treeView1.ExpandAll();

            Onto.Classes.Add(new OntoClass() { name = TransferData.StrValue, parentName = node.Parent.Text, annotation = ""});

            if (toolStripStatusLabel2.Text == "") toolStripStatusLabel2.Text = "   * изменен";
            Panel41_Invalidate();
        }

        private void toolStripButton2_Click(object sender, EventArgs e) //добавить ветвь
        {
            List<string> StrMas = new List<string>();
            foreach (OntoClass c in Onto.Classes) StrMas.Add(c.name);
            foreach (string s in listBox8.Items) StrMas.Add(s);

            AddNode f = new AddNode(StrMas);
            f.ShowDialog();
            if (TransferData.StrValue == "") return;

            if (node == null) node = treeView1.Nodes[0];
            node.Nodes.Add(TransferData.StrValue);

            treeView1.ExpandAll();

            Onto.Classes.Add(new OntoClass() { name = TransferData.StrValue, parentName = node.Text, annotation = "" });

            if (toolStripStatusLabel2.Text == "") toolStripStatusLabel2.Text = "   * изменен";
            Panel41_Invalidate();
        }

        private void OntoClassesDelete (TreeNode node)
        {
            Onto.Classes.RemoveAt(Onto.Classes.FindIndex(0,f => f.name == node.Text));

            if (node.Nodes.Count != 0)
                for (int i = 0; i < node.Nodes.Count; i++) OntoClassesDelete(node.Nodes[i]);

        }

        private void toolStripButton5_Click(object sender, EventArgs e) //удалить
        {
            if (MessageBox.Show("Будет удален выделенный узел и все дочерние узлы. Удалить ?", "Внимание !", MessageBoxButtons.YesNo) == DialogResult.No) return;

            OntoClassesDelete(node);

            treeView1.Nodes.Remove(node);
            if (toolStripStatusLabel2.Text == "") toolStripStatusLabel2.Text = "   * изменен";
            Panel41_Invalidate();
        }

        private void button3_Click(object sender, EventArgs e) //изменить место
        {
            if (node == null || node == treeView1.Nodes[0]) return;

            TreeNodeCollection editNodes = node.Parent.Nodes;
            int indexSelectedNode = node.Index;
            TreeNode selectedNode = treeView1.SelectedNode;

            SelectNode f = new SelectNode(treeView1.Nodes);
            f.ShowDialog();
            if (TransferData.StrValue == "" || TransferData.StrValue == _node) return;
            Onto.Classes[_node].parentName = TransferData.StrValue;

            editNodes.RemoveAt(indexSelectedNode);
            SearchNode(TransferData.StrValue, treeView1.Nodes[0]).Nodes.Add(selectedNode);

            treeView1.SelectedNode = selectedNode;

            label1.Text = node.FullPath;
            if (toolStripStatusLabel2.Text == "") toolStripStatusLabel2.Text = "   * изменен";
        }

        private void button2_Click(object sender, EventArgs e) //уровень вверх
        {
            if (node == null || node.Level == 0 || node.PrevNode == null) return;

            TreeNodeCollection editNodes = node.Parent.Nodes;

            int indexSelectedNode = node.Index;
            int indexPreviousNode = node.PrevNode.Index;

            TreeNode selectedNode = treeView1.SelectedNode;

            editNodes.RemoveAt(indexSelectedNode);
            editNodes.Insert(indexPreviousNode, selectedNode);

            treeView1.SelectedNode = selectedNode;


            label1.Text = node.FullPath;
            if (toolStripStatusLabel2.Text == "") toolStripStatusLabel2.Text = "   * изменен";
        }

        private void button1_Click(object sender, EventArgs e) //уровень вниз
        {
            if (node == null || node.NextNode == null) return;

            TreeNodeCollection editNodes = node.Parent.Nodes;

            int indexSelectedNode = node.Index;
            int indexNextNode = node.NextNode.Index;

            TreeNode selectedNode = treeView1.SelectedNode;

            editNodes.RemoveAt(indexSelectedNode);
            editNodes.Insert(indexNextNode, selectedNode);

            treeView1.SelectedNode = selectedNode;

            label1.Text = node.FullPath;
            if (toolStripStatusLabel2.Text == "") toolStripStatusLabel2.Text = "   * изменен";
        }   
  
        #endregion

        #region Tree2

        private void toolStripButton8_Click(object sender, EventArgs e) //развернуть
        {
            treeView2.ExpandAll();
        }

        private void toolStripButton9_Click(object sender, EventArgs e) //свернуть
        {
            treeView2.CollapseAll();
            treeView1.SelectedNode = null;
        }

        private void toolStripButton6_Click(object sender, EventArgs e) //добавить корень
        {
            List<string> StrMas = new List<string>();
            foreach (OntoPropertie c in Onto.Propetries) StrMas.Add(c.name);

            AddNode f = new AddNode(StrMas);
            f.ShowDialog();
            if (TransferData.StrValue == "") return;

            node.Parent.Nodes.Add(TransferData.StrValue);

            treeView2.ExpandAll();
            
            Onto.Propetries.Add(new OntoPropertie() { name = TransferData.StrValue, parentName = node.Parent.Text, annotation = "", transitive = false, reflexive = false, symmetric = false });

            if (toolStripStatusLabel2.Text == "") toolStripStatusLabel2.Text = "   * изменен";
            Panel42_Invalidate();
        }

        private void toolStripButton7_Click(object sender, EventArgs e) //добавить ветвь
        {
            List<string> StrMas = new List<string>();
            foreach (OntoPropertie c in Onto.Propetries) StrMas.Add(c.name);

            AddNode f = new AddNode(StrMas);
            f.ShowDialog();
            if (TransferData.StrValue == "") return;

            if (node == null) node = treeView2.Nodes[0];
            node.Nodes.Add(TransferData.StrValue);

            treeView2.ExpandAll();

            Onto.Propetries.Add(new OntoPropertie() { name = TransferData.StrValue, parentName = node.Text, annotation = "", transitive = false, reflexive = false, symmetric = false });

            if (toolStripStatusLabel2.Text == "") toolStripStatusLabel2.Text = "   * изменен";
            Panel42_Invalidate();
        }

        private void OntoPropertieDelete(TreeNode node)
        {
            Onto.Propetries.RemoveAt(Onto.Propetries.FindIndex(0, f => f.name == node.Text));

            if (node.Nodes.Count != 0)
                for (int i = 0; i < node.Nodes.Count; i++) OntoPropertieDelete(node.Nodes[i]);
        }

        private void toolStripButton10_Click(object sender, EventArgs e) //удалить
        {
            if (MessageBox.Show("Будет удален выделенный узел и все дочерние узлы. Удалить ?", "Внимание !", MessageBoxButtons.YesNo) == DialogResult.No) return;

            OntoPropertieDelete(node);

            treeView1.Nodes.Remove(node);
            if (toolStripStatusLabel2.Text == "") toolStripStatusLabel2.Text = "   * изменен";
            Panel42_Invalidate();
        }

        private void button20_Click(object sender, EventArgs e) //изменить место
        {
            if (node == null || node == treeView2.Nodes[0]) return;

            TreeNodeCollection editNodes = node.Parent.Nodes;
            int indexSelectedNode = node.Index;
            TreeNode selectedNode = treeView2.SelectedNode;

            SelectNode f = new SelectNode(treeView2.Nodes);
            f.ShowDialog();
            if (TransferData.StrValue == "" || TransferData.StrValue == _node) return;
            Onto.Propetries[_node].parentName = TransferData.StrValue;

            editNodes.RemoveAt(indexSelectedNode);
            SearchNode(TransferData.StrValue, treeView2.Nodes[0]).Nodes.Add(selectedNode);

            treeView2.SelectedNode = selectedNode;

            label2.Text = node.FullPath;
            if (toolStripStatusLabel2.Text == "") toolStripStatusLabel2.Text = "   * изменен";
        }

        private void button21_Click(object sender, EventArgs e) //уровень вверх
        {
            if (node == null || node.Level == 0 || node.PrevNode == null) return;

            TreeNodeCollection editNodes = node.Parent.Nodes;

            int indexSelectedNode = node.Index;
            int indexPreviousNode = node.PrevNode.Index;

            TreeNode selectedNode = treeView2.SelectedNode;

            editNodes.RemoveAt(indexSelectedNode);
            editNodes.Insert(indexPreviousNode, selectedNode);

            treeView2.SelectedNode = selectedNode;

            label2.Text = node.FullPath;
            if (toolStripStatusLabel2.Text == "") toolStripStatusLabel2.Text = "   * изменен";
        }

        private void button22_Click(object sender, EventArgs e) //уровень вниз
        {
            if (node == null || node.NextNode == null) return;

            TreeNodeCollection editNodes = node.Parent.Nodes;

            int indexSelectedNode = node.Index;
            int indexNextNode = node.NextNode.Index;

            TreeNode selectedNode = treeView2.SelectedNode;

            editNodes.RemoveAt(indexSelectedNode);
            editNodes.Insert(indexNextNode, selectedNode);

            treeView2.SelectedNode = selectedNode;

            label2.Text = node.FullPath;
            if (toolStripStatusLabel2.Text == "") toolStripStatusLabel2.Text = "   * изменен";
        }

        #endregion

        #region Tree3

        private void toolStripButton11_Click(object sender, EventArgs e) //добавить
        {
            List<string> StrMas = new List<string>();
            foreach (string s in listBox8.Items) StrMas.Add(s);
            foreach (OntoClass c in Onto.Classes) StrMas.Add(c.name);

            AddNode f = new AddNode(StrMas);
            f.ShowDialog();
            if (TransferData.StrValue == "") return;

            listBox8.Items.Add(TransferData.StrValue);
            Onto.Individuals.Add(new OntoIndividual() { name = TransferData.StrValue});

            if (toolStripStatusLabel2.Text == "") toolStripStatusLabel2.Text = "   * изменен";
            Panel43_Invalidate();
        }

        private void toolStripButton12_Click(object sender, EventArgs e) //удалить
        {            
            Onto.Individuals.RemoveAt(Onto.Individuals.FindIndex(0, f => f.name == _node));
            listBox8.Items.RemoveAt(listBox8.SelectedIndex);

            if (toolStripStatusLabel2.Text == "") toolStripStatusLabel2.Text = "   * изменен";
            Panel43_Invalidate();
        }

        #endregion

        #region Panel1

        private void textBox1_Leave(object sender, EventArgs e)
        {
            Onto.Classes[_node].annotation = textBox1.Text;
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            Panel1_Invalidate();
        }

        private void button36_Click(object sender, EventArgs e) //добавить отношение
        {
            if (treeView1.SelectedNode == null) return;
            SelectProperty f = new SelectProperty(treeView2.Nodes, treeView1.Nodes);
            f.ShowDialog();
            if (TransferData.StrValue == "" || TransferData.StrValue2 == "") return;

            ///если объект отношения совпадает с текущим классом, то отношение должно быть рефлексивным
            if (TransferData.StrValue2 == _node && !Onto.Propetries[TransferData.StrValue].reflexive)
            {
                MessageBox.Show("Объект отношения совпадает с выбранным классом, отношение должно быть рефлексивным.", "Внимание !");
                return;
            }

            Onto.Classes[_node].listOfProperties.Add(new ClassPropertie() { objectPropertie = TransferData.StrValue, typeOfPropertie = TransferData.StrValue3, destObject = TransferData.StrValue2 });

            ///если отношение симметрично, добавим его ко второму объекту
            if (Onto.Propetries[TransferData.StrValue].symmetric && TransferData.StrValue2 != _node)
                Onto.Classes[TransferData.StrValue2].listOfProperties.Add(new ClassPropertie() { objectPropertie = TransferData.StrValue, typeOfPropertie = TransferData.StrValue3, destObject = _node });

            listBox12.Items.Add(TransferData.StrValue + " (" + TransferData.StrValue3 + ") " + TransferData.StrValue2);
            if (toolStripStatusLabel2.Text == "") toolStripStatusLabel2.Text = "   * изменен";
        }

        private void button37_Click(object sender, EventArgs e) //удалить отношение
        {
            if (listBox12.SelectedIndex == -1) return;
            string[] lines = listBox12.SelectedItem.ToString().Split(' ');
            lines[1] = lines[1].Substring(1, lines[1].Length - 2);
                        
            Onto.Classes[_node].listOfProperties.RemoveAll(f => f.objectPropertie == lines[0] && f.typeOfPropertie == lines[1] && f.destObject == lines[2]);
            if (Onto.Propetries[lines[0]].symmetric) Onto.Classes[lines[2]].listOfProperties.RemoveAll(f => f.objectPropertie == lines[0] && f.typeOfPropertie == lines[1] && f.destObject == _node);

            listBox12.Items.RemoveAt(listBox12.SelectedIndex);
            listBox12.ClearSelected();
            if (toolStripStatusLabel2.Text == "") toolStripStatusLabel2.Text = "   * изменен";
        }

        private void button28_Click(object sender, EventArgs e) //перейти к отношению
        {
            if (listBox12.SelectedIndex == -1) return;
            string line = listBox12.SelectedItem.ToString();
            string name = line.Substring(0, line.IndexOf(' '));
            tabControl1.SelectedIndex = 1;
            treeView2.SelectedNode = SearchNode(name, treeView2.Nodes[0]);
            Panel2_Invalidate();
        }

        private void button4_Click(object sender, EventArgs e) //добавить эквивалентный класс
        {
            SelectNode f = new SelectNode(treeView1.Nodes);
            f.ShowDialog();
            if (TransferData.StrValue == "" || TransferData.StrValue == _node) return;
            if (listBox2.Items.Contains(TransferData.StrValue)) { MessageBox.Show("Класс " + TransferData.StrValue + " указан как непересекающийся"); return; }

            if (Onto.Classes[_node].equalClasses.Contains(TransferData.StrValue)) return;

            listBox1.Items.Add(TransferData.StrValue);

            Onto.Classes[_node].equalClasses.Add(TransferData.StrValue);
            Onto.Classes[TransferData.StrValue].equalClasses.Add(_node);
            if (toolStripStatusLabel2.Text == "") toolStripStatusLabel2.Text = "   * изменен";
        }

        private void button5_Click(object sender, EventArgs e) //удалить эквивалентный класс
        {
            if (listBox1.SelectedIndex == -1) return;
            Onto.Classes[_node].equalClasses.Remove(listBox1.SelectedItem.ToString());
            Onto.Classes[listBox1.SelectedItem.ToString()].equalClasses.Remove(_node);
            listBox1.Items.RemoveAt(listBox1.SelectedIndex);
            listBox1.ClearSelected();
            if (toolStripStatusLabel2.Text == "") toolStripStatusLabel2.Text = "   * изменен";
        }

        private void button6_Click(object sender, EventArgs e) //перейти к эквивалентному классу
        {
            if (listBox1.SelectedIndex == -1) return;
            treeView1.SelectedNode = SearchNode(listBox1.SelectedItem.ToString(), treeView1.Nodes[0]);
            Panel1_Invalidate();
        }

        private void button7_Click(object sender, EventArgs e)  //добавить непересекающийся класс
        {
            SelectNode f = new SelectNode(treeView1.Nodes);
            f.ShowDialog();
            if (TransferData.StrValue == "" || TransferData.StrValue == _node) return;
            if (listBox1.Items.Contains(TransferData.StrValue)) { MessageBox.Show("Класс " + TransferData.StrValue + " указан как эквивалентный"); return; }

            if (Onto.Classes[_node].disjointClasses.Contains(TransferData.StrValue)) return;

            listBox2.Items.Add(TransferData.StrValue);

            Onto.Classes[_node].disjointClasses.Add(TransferData.StrValue);
            Onto.Classes[TransferData.StrValue].disjointClasses.Add(_node);
            if (toolStripStatusLabel2.Text == "") toolStripStatusLabel2.Text = "   * изменен";
        }

        private void button8_Click(object sender, EventArgs e) //удалить непересекающийся класс
        {
            if (listBox2.SelectedIndex == -1) return;
            Onto.Classes[_node].disjointClasses.Remove(listBox2.SelectedItem.ToString());
            Onto.Classes[listBox2.SelectedItem.ToString()].disjointClasses.Remove(_node);
            listBox2.Items.RemoveAt(listBox2.SelectedIndex);
            listBox2.ClearSelected();
            if (toolStripStatusLabel2.Text == "") toolStripStatusLabel2.Text = "   * изменен";
        }

        private void button9_Click(object sender, EventArgs e) //перейти к непересекающемуся классу
        {
            if (listBox1.SelectedIndex == -1) return;
            treeView1.SelectedNode = SearchNode(listBox2.SelectedItem.ToString(), treeView1.Nodes[0]);
            Panel1_Invalidate();
        }

        private void button38_Click(object sender, EventArgs e) //добавить экземпляр
        {
            List<string> S = new List<string>();
            for (int i = 0; i < listBox8.Items.Count; i++) S.Add(listBox8.Items[i].ToString());
            SelectIndivid f = new SelectIndivid(S);
            f.ShowDialog();

            if (TransferData.StrValue == "") return;            
            if (Onto.Classes[_node].listOfIndividuals.Contains(TransferData.StrValue)) return;

            listBox3.Items.Add(TransferData.StrValue);

            Onto.Classes[_node].listOfIndividuals.Add(TransferData.StrValue);
            Onto.Individuals[TransferData.StrValue].listOfClasses.Add(_node);
            if (toolStripStatusLabel2.Text == "") toolStripStatusLabel2.Text = "   * изменен";
        }

        private void button10_Click(object sender, EventArgs e) //удалить экземпляр
        {
            if (listBox3.SelectedIndex == -1) return;
                        
            Onto.Classes[_node].listOfIndividuals.Remove(listBox3.SelectedItem.ToString());
            Onto.Individuals[listBox3.SelectedItem.ToString()].listOfClasses.Remove(_node);

            listBox3.Items.RemoveAt(listBox3.SelectedIndex);
            listBox3.ClearSelected();
            if (toolStripStatusLabel2.Text == "") toolStripStatusLabel2.Text = "   * изменен";
        }

        private void button11_Click(object sender, EventArgs e) //перейти к экземпляру
        {
            if (listBox3.SelectedIndex == -1) return;
            tabControl1.SelectedIndex = 2;
            listBox8.SelectedIndex = listBox8.FindString(listBox3.SelectedItem.ToString());
            Panel3_Invalidate();
        }

        #endregion

        #region Panel2

        private void checkBox1_CheckStateChanged(object sender, EventArgs e)
        {
            Onto.Propetries[_node].transitive = checkBox1.Checked;
        }

        private void checkBox2_CheckStateChanged(object sender, EventArgs e)
        {
            Onto.Propetries[_node].symmetric = checkBox2.Checked;
        }

        private void checkBox3_CheckStateChanged(object sender, EventArgs e)
        {
            Onto.Propetries[_node].reflexive = checkBox3.Checked;
        }

        private void textBox2_Leave(object sender, EventArgs e)
        {            
            Onto.Propetries[_node].annotation = textBox2.Text;
        }

        private void treeView2_AfterSelect(object sender, TreeViewEventArgs e)
        {
            Panel2_Invalidate();
        }

        private void button13_Click(object sender, EventArgs e) //область определения добавить
        {
            SelectNode f = new SelectNode(treeView1.Nodes);
            f.ShowDialog();

            if (TransferData.StrValue == "") return;
            if (listBox5.Items.Contains(TransferData.StrValue) && !Onto.Propetries[_node].reflexive) 
            { 
                MessageBox.Show("Отношение " + _node + " должно быть рефлексивным"); 
                return; 
            }

            if (Onto.Propetries[_node].diffArea.Contains(TransferData.StrValue)) return;

            listBox4.Items.Add(TransferData.StrValue);

            Onto.Propetries[_node].diffArea.Add(TransferData.StrValue);
            if (toolStripStatusLabel2.Text == "") toolStripStatusLabel2.Text = "   * изменен";
        }

        private void button23_Click(object sender, EventArgs e) //область определения удалить
        {
            if (listBox4.SelectedIndex == -1) return;
            Onto.Propetries[_node].diffArea.Remove(listBox4.SelectedItem.ToString());

            listBox4.Items.RemoveAt(listBox4.SelectedIndex);
            listBox4.ClearSelected();
            if (toolStripStatusLabel2.Text == "") toolStripStatusLabel2.Text = "   * изменен";
        }

        private void button12_Click(object sender, EventArgs e) //область определения перейти
        {
            if (listBox4.SelectedIndex == -1) return;
            tabControl1.SelectedIndex = 0;
            treeView1.SelectedNode = SearchNode(listBox4.SelectedItem.ToString(), treeView1.Nodes[0]);
            Panel1_Invalidate();
        }

        private void button25_Click(object sender, EventArgs e) //область значений добавить
        {
            SelectNode f = new SelectNode(treeView1.Nodes);
            f.ShowDialog();

            if (TransferData.StrValue == "") return;
            if (listBox4.Items.Contains(TransferData.StrValue) && !Onto.Propetries[_node].reflexive)
            {
                MessageBox.Show("Отношение " + _node + " должно быть рефлексивным");
                return;
            }

            if (Onto.Propetries[_node].valueArea.Contains(TransferData.StrValue)) return;

            listBox5.Items.Add(TransferData.StrValue);

            Onto.Propetries[_node].valueArea.Add(TransferData.StrValue);
            if (toolStripStatusLabel2.Text == "") toolStripStatusLabel2.Text = "   * изменен";
        }

        private void button26_Click(object sender, EventArgs e) //область значений удалить
        {
            if (listBox5.SelectedIndex == -1) return;
            Onto.Propetries[_node].valueArea.Remove(listBox5.SelectedItem.ToString());

            listBox5.Items.RemoveAt(listBox5.SelectedIndex);
            listBox5.ClearSelected();
            if (toolStripStatusLabel2.Text == "") toolStripStatusLabel2.Text = "   * изменен";
        }

        private void button24_Click(object sender, EventArgs e) //область значений перейти
        {
            if (listBox5.SelectedIndex == -1) return;
            tabControl1.SelectedIndex = 0;
            treeView1.SelectedNode = SearchNode(listBox5.SelectedItem.ToString(), treeView1.Nodes[0]);
            Panel1_Invalidate();
        }

        private void button15_Click(object sender, EventArgs e) //эквивалентные отношения добавить
        {
            SelectNode f = new SelectNode(treeView2.Nodes);
            f.ShowDialog();
            if (TransferData.StrValue == "" || TransferData.StrValue == _node) return;
            if (listBox7.Items.Contains(TransferData.StrValue)) { MessageBox.Show("Отношение " + TransferData.StrValue + " указано как противоположное"); return; }

            if (Onto.Propetries[_node].equalProperties.Contains(TransferData.StrValue)) return;

            listBox6.Items.Add(TransferData.StrValue);

            Onto.Propetries[_node].equalProperties.Add(TransferData.StrValue);
            Onto.Propetries[TransferData.StrValue].equalProperties.Add(_node);
            if (toolStripStatusLabel2.Text == "") toolStripStatusLabel2.Text = "   * изменен";
        }

        private void button16_Click(object sender, EventArgs e) //эквивалентные отношения удалить
        {
            if (listBox6.SelectedIndex == -1) return;
            Onto.Propetries[_node].equalProperties.Remove(listBox6.SelectedItem.ToString());
            Onto.Propetries[listBox6.SelectedItem.ToString()].equalProperties.Remove(_node);

            listBox6.Items.RemoveAt(listBox6.SelectedIndex);
            listBox6.ClearSelected();
            if (toolStripStatusLabel2.Text == "") toolStripStatusLabel2.Text = "   * изменен";
        }

        private void button14_Click(object sender, EventArgs e) //эквивалентные отношения перейти
        {
            if (listBox6.SelectedIndex == -1) return;
            treeView2.SelectedNode = SearchNode(listBox6.SelectedItem.ToString(), treeView2.Nodes[0]);
            Panel2_Invalidate();
        }

        private void button18_Click(object sender, EventArgs e) //противоположные отношения добавить
        {
            SelectNode f = new SelectNode(treeView2.Nodes);
            f.ShowDialog();
            if (TransferData.StrValue == "" || TransferData.StrValue == _node) return;
            if (listBox6.Items.Contains(TransferData.StrValue)) { MessageBox.Show("Отношение " + TransferData.StrValue + " указано как эквивалентное"); return; }

            if (Onto.Propetries[_node].disjointProperties.Contains(TransferData.StrValue)) return;

            listBox7.Items.Add(TransferData.StrValue);

            Onto.Propetries[_node].disjointProperties.Add(TransferData.StrValue);
            Onto.Propetries[TransferData.StrValue].disjointProperties.Add(_node);
            if (toolStripStatusLabel2.Text == "") toolStripStatusLabel2.Text = "   * изменен";
        }

        private void button19_Click(object sender, EventArgs e) //противоположные отношения удалить
        {
            if (listBox7.SelectedIndex == -1) return;
            Onto.Propetries[_node].disjointProperties.Remove(listBox7.SelectedItem.ToString());
            Onto.Propetries[listBox7.SelectedItem.ToString()].disjointProperties.Remove(_node);

            listBox7.Items.RemoveAt(listBox7.SelectedIndex);
            listBox7.ClearSelected();
            if (toolStripStatusLabel2.Text == "") toolStripStatusLabel2.Text = "   * изменен";
        }

        private void button17_Click(object sender, EventArgs e) //противоположные отношения перейти
        {
            if (listBox7.SelectedIndex == -1) return;
            treeView2.SelectedNode = SearchNode(listBox7.SelectedItem.ToString(), treeView2.Nodes[0]);
            Panel2_Invalidate();
        }

        #endregion

        #region Panel3

        private void textBox3_Leave(object sender, EventArgs e)
        {
            if (listBox8.SelectedIndex == -1) return;
            Onto.Individuals[_node].annotation = textBox3.Text;                 
        }

        private void listBox8_Click(object sender, EventArgs e)
        {
            Panel3_Invalidate();
        }

        private void button33_Click(object sender, EventArgs e) //эквивалентный экземпляр добавить
        {
            if (listBox8.SelectedIndex == -1) return;
            List<string> S = new List<string>();
            for (int i = 0; i < listBox8.Items.Count; i++) S.Add(listBox8.Items[i].ToString());
            SelectIndivid f = new SelectIndivid(S);
            f.ShowDialog();

            if (TransferData.StrValue == "") return;
            if (listBox10.Items.Contains(TransferData.StrValue)) { MessageBox.Show("Экземпляр " + TransferData.StrValue + " указан как противопложный"); return; }


            if (Onto.Individuals[_node].equalIndividuals.Contains(TransferData.StrValue)) return;

            listBox9.Items.Add(TransferData.StrValue);

            Onto.Individuals[_node].equalIndividuals.Add(TransferData.StrValue);
            Onto.Individuals[TransferData.StrValue].equalIndividuals.Add(_node);
            if (toolStripStatusLabel2.Text == "") toolStripStatusLabel2.Text = "   * изменен";
        }

        private void button34_Click(object sender, EventArgs e) //эквивалентный экземпляр удалить
        {
            if (listBox9.SelectedIndex == -1) return;
            
            Onto.Individuals[_node].equalIndividuals.Remove(listBox9.SelectedItem.ToString());
            Onto.Individuals[listBox9.SelectedItem.ToString()].equalIndividuals.Remove(_node);

            listBox9.Items.RemoveAt(listBox9.SelectedIndex);
            listBox9.ClearSelected();
            if (toolStripStatusLabel2.Text == "") toolStripStatusLabel2.Text = "   * изменен";
        }

        private void button32_Click(object sender, EventArgs e) //эквивалентный экземпляр перейти
        {
            if (listBox9.SelectedIndex == -1) return;
            listBox8.SelectedIndex = listBox8.FindString(listBox9.SelectedItem.ToString());
            Panel3_Invalidate();
        }

        private void button30_Click(object sender, EventArgs e) //противоположный экземпляр добавить
        {
            if (listBox8.SelectedIndex == -1) return;
            List<string> S = new List<string>();
            for (int i = 0; i < listBox8.Items.Count; i++) S.Add(listBox8.Items[i].ToString());
            SelectIndivid f = new SelectIndivid(S);
            f.ShowDialog();

            if (TransferData.StrValue == "") return;
            if (listBox9.Items.Contains(TransferData.StrValue)) { MessageBox.Show("Экземпляр " + TransferData.StrValue + " указан как эквивалентный"); return; }

            if (Onto.Individuals[_node].disjointIndividuals.Contains(TransferData.StrValue)) return;

            listBox10.Items.Add(TransferData.StrValue);

            Onto.Individuals[_node].disjointIndividuals.Add(TransferData.StrValue);
            Onto.Individuals[TransferData.StrValue].disjointIndividuals.Add(_node);
            if (toolStripStatusLabel2.Text == "") toolStripStatusLabel2.Text = "   * изменен";
        }

        private void button31_Click(object sender, EventArgs e) //противоположный экземпляр удалить
        {
            if (listBox10.SelectedIndex == -1) return;

            Onto.Individuals[_node].disjointIndividuals.Remove(listBox10.SelectedItem.ToString());
            Onto.Individuals[listBox10.SelectedItem.ToString()].disjointIndividuals.Remove(_node);

            listBox10.Items.RemoveAt(listBox10.SelectedIndex);
            listBox10.ClearSelected();

            if (toolStripStatusLabel2.Text == "") toolStripStatusLabel2.Text = "   * изменен";
        }

        private void button29_Click(object sender, EventArgs e) //противоположный экземпляр перейти
        {
            if (listBox10.SelectedIndex == -1) return;
            listBox8.SelectedIndex = listBox8.FindString(listBox10.SelectedItem.ToString());
            Panel3_Invalidate();
        }

        private void button39_Click(object sender, EventArgs e) //принадлежность классам добавить
        {
            if (listBox8.SelectedIndex == -1) return;
            SelectNode f = new SelectNode(treeView1.Nodes);
            f.ShowDialog();

            if (TransferData.StrValue == "") return;
            if (listBox11.Items.Contains(TransferData.StrValue)) return;

            listBox11.Items.Add(TransferData.StrValue);

            Onto.Individuals[_node].listOfClasses.Add(TransferData.StrValue);
            Onto.Classes[TransferData.StrValue].listOfIndividuals.Add(_node);            

            if (toolStripStatusLabel2.Text == "") toolStripStatusLabel2.Text = "   * изменен";
        }

        private void button40_Click(object sender, EventArgs e) //принадлежность классам удалить
        {
            if (listBox11.SelectedIndex == -1) return;

            Onto.Individuals[_node].listOfClasses.Remove(listBox11.SelectedItem.ToString());
            Onto.Classes[listBox11.SelectedItem.ToString()].listOfIndividuals.Remove(_node);

            listBox11.Items.RemoveAt(listBox11.SelectedIndex);
            listBox11.ClearSelected();

            if (toolStripStatusLabel2.Text == "") toolStripStatusLabel2.Text = "   * изменен";
        }

        private void button27_Click(object sender, EventArgs e) //принадлежность классам перейти
        {
            if (listBox11.SelectedIndex == -1) return;
            tabControl1.SelectedIndex = 0;
            treeView1.SelectedNode = SearchNode(listBox11.SelectedItem.ToString(), treeView1.Nodes[0]);
            Panel1_Invalidate();
        }        
        
        #endregion

        #region Panel4

        private void listBox13_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox13.SelectedIndex == -1) return;
            OntoClass oc = Onto.Classes[listBox13.SelectedItem.ToString()];
            string ss, str;

            ss = "Класс \"" + oc.name + "\", подкласс класса \"" + oc.parentName + "\"";
            ss += "\r\nАннотация: " + oc.annotation;

            str = "";
            foreach (string s in oc.equalClasses) str += " " + s;
            ss += "\r\nЭквивалентные классы:" + str;

            str = "";
            foreach (string s in oc.disjointClasses) str += " " + s;
            ss += "\r\nПротивоположные классы:" + str;

            str = "";
            foreach (string s in oc.listOfIndividuals) str += " " + s;
            ss += "\r\nЭкземпляры:" + str;

            str = "";
            foreach (ClassPropertie cp in oc.listOfProperties) str += "  " + cp.objectPropertie + " (" + cp.typeOfPropertie + ") " + cp.destObject;
            ss += "\r\nОтношения класса:" + str;

            textBox4.Text = ss;
            listBox14.SelectedIndex = -1;
            listBox15.SelectedIndex = -1;
            if (listBox16.SelectedIndex > -1 || listBox17.SelectedIndex > -1 || listBox18.SelectedIndex > -1) Correlation();
        }

        private void listBox16_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox16.SelectedIndex == -1) return;
            OntoClass oc = Onto.Classes[listBox16.SelectedItem.ToString()];
            string ss, str;

            ss = "Класс \"" + oc.name + "\", подкласс класса \"" + oc.parentName + "\"";
            ss += "\r\nАннотация: " + oc.annotation;

            str = "";
            foreach (string s in oc.equalClasses) str += " " + s;
            ss += "\r\nЭквивалентные классы:" + str;

            str = "";
            foreach (string s in oc.disjointClasses) str += " " + s;
            ss += "\r\nПротивоположные классы:" + str;

            str = "";
            foreach (string s in oc.listOfIndividuals) str += " " + s;
            ss += "\r\nЭкземпляры:" + str;

            str = "";
            foreach (ClassPropertie cp in oc.listOfProperties) str += "  " + cp.objectPropertie + " (" + cp.typeOfPropertie + ") " + cp.destObject;
            ss += "\r\nОтношения класса:" + str;

            textBox5.Text = ss;
            listBox17.SelectedIndex = -1;
            listBox18.SelectedIndex = -1;
            if (listBox13.SelectedIndex > -1 || listBox14.SelectedIndex > -1 || listBox15.SelectedIndex > -1) Correlation();
        }

        private void listBox14_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox14.SelectedIndex == -1) return;
            OntoPropertie op = Onto.Propetries[listBox14.SelectedItem.ToString()];
            string ss, str;

            ss = "Отношение \"" + op.name + "\", подкласс отношения \"" + op.parentName + "\"";
            ss += "\r\nАннотация: " + op.annotation;

            str = "";
            if (op.transitive) str += " транзитивно";
            if (op.symmetric) str += " симметрично";
            if (op.reflexive) str += " рефлексивно";
            if (op.transitive || op.reflexive || op.symmetric) ss += "\nОтношение " + str;

            str = "";
            foreach (string s in op.diffArea) str += " " + s;
            ss += "\r\nКлассы области определения:" + str;

            str = "";
            foreach (string s in op.valueArea) str += " " + s;
            ss += "\r\nКлассы области значений:" + str;

            str = "";
            foreach (string s in op.equalProperties) str += " " + s;
            ss += "\r\nЭквивалентные отношения:" + str;

            str = "";
            foreach (string s in op.disjointProperties) str += " " + s;
            ss += "\r\nПротивоположные отношения:" + str;

            textBox4.Text = ss;
            listBox13.SelectedIndex = -1;
            listBox15.SelectedIndex = -1;
            if (listBox16.SelectedIndex > -1 || listBox17.SelectedIndex > -1 || listBox18.SelectedIndex > -1) Correlation();
        }

        private void listBox17_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox17.SelectedIndex == -1) return;
            OntoPropertie op = Onto.Propetries[listBox17.SelectedItem.ToString()];
            string ss, str;

            ss = "Отношение \"" + op.name + "\", подкласс отношения \"" + op.parentName + "\"";
            ss += "\r\nАннотация: " + op.annotation;

            str = "";
            if (op.transitive) str += " транзитивно";
            if (op.symmetric) str += " симметрично";
            if (op.reflexive) str += " рефлексивно";
            if (op.transitive || op.reflexive || op.symmetric) ss += "\nОтношение " + str;

            str = "";
            foreach (string s in op.diffArea) str += " " + s;
            ss += "\r\nКлассы области определения:" + str;

            str = "";
            foreach (string s in op.valueArea) str += " " + s;
            ss += "\r\nКлассы области значений:" + str;

            str = "";
            foreach (string s in op.equalProperties) str += " " + s;
            ss += "\r\nЭквивалентные отношения:" + str;

            str = "";
            foreach (string s in op.disjointProperties) str += " " + s;
            ss += "\r\nПротивоположные отношения:" + str;

            textBox5.Text = ss;
            listBox16.SelectedIndex = -1;
            listBox18.SelectedIndex = -1;
            if (listBox13.SelectedIndex > -1 || listBox14.SelectedIndex > -1 || listBox15.SelectedIndex > -1) Correlation();
        }

        private void listBox15_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox15.SelectedIndex == -1) return;
            OntoIndividual oi = Onto.Individuals[listBox15.SelectedItem.ToString()];
            string ss, str;

            ss = "Экземпляр \"" + oi.name + "\"";

            str = "";
            foreach (string s in oi.listOfClasses) str += " " + s;
            ss += "\r\nПринадлежит классам:" + str;

            ss += "\r\nАннотация: " + oi.annotation;

            str = "";
            foreach (string s in oi.equalIndividuals) str += " " + s;
            ss += "\r\nЭквивалентные экземпляры:" + str;

            str = "";
            foreach (string s in oi.disjointIndividuals) str += " " + s;
            ss += "\r\nПротивоположные экземпляры:" + str;

            textBox4.Text = ss;
            listBox13.SelectedIndex = -1;
            listBox14.SelectedIndex = -1;
            if (listBox16.SelectedIndex > -1 || listBox17.SelectedIndex > -1 || listBox18.SelectedIndex > -1) Correlation();
        }

        private void listBox18_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox18.SelectedIndex == -1) return;
            OntoIndividual oi = Onto.Individuals[listBox18.SelectedItem.ToString()];
            string ss, str;

            ss = "Экземпляр \"" + oi.name + "\"";

            str = "";
            foreach (string s in oi.listOfClasses) str += " " + s;
            ss += "\r\nПринадлежит классам:" + str;

            ss += "\r\nАннотация: " + oi.annotation;

            str = "";
            foreach (string s in oi.equalIndividuals) str += " " + s;
            ss += "\r\nЭквивалентные экземпляры:" + str;

            str = "";
            foreach (string s in oi.disjointIndividuals) str += " " + s;
            ss += "\r\nПротивоположные экземпляры:" + str;

            textBox5.Text = ss;
            listBox16.SelectedIndex = -1;
            listBox17.SelectedIndex = -1;
            if (listBox13.SelectedIndex > -1 || listBox14.SelectedIndex > -1 || listBox15.SelectedIndex > -1) Correlation();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e) //выбран класс
        {
            if (radioButton2.Checked) return;
            comboBox3.Items.Clear();
            SearchPropertyInTree(SearchNode(comboBox2.SelectedItem.ToString(), treeView1.Nodes[0]));
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e) //выбран тип поиска
        {
            comboBox2.Items.Clear();
            comboBox3.Items.Clear();
            foreach (OntoClass oc in Onto.Classes)
            {
                if (radioButton2.Checked || checkBox4.Checked) comboBox2.Items.Add(oc.name);
                else if (SearchNode(oc.name, treeView1.Nodes[0]).Nodes.Count > 0) comboBox2.Items.Add(oc.name); 
            }
            comboBox2.SelectedIndex = 0;
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e) //поиск через отношение
        {
            object s1 = comboBox2.SelectedItem;
            object s2 = comboBox3.SelectedItem;
            comboBox2.Items.Clear();
            foreach (OntoClass oc in Onto.Classes)
            {
                if (radioButton2.Checked || checkBox4.Checked) comboBox2.Items.Add(oc.name);
                else if (SearchNode(oc.name, treeView1.Nodes[0]).Nodes.Count > 0) comboBox2.Items.Add(oc.name);
            }
            comboBox2.SelectedIndex = comboBox2.Items.IndexOf(s1);
            if (s2 != null) comboBox3.SelectedIndex = comboBox3.Items.IndexOf(s2);
        }

        private void SearchInTree(TreeNode StartNode, int type, string str1, string str2, double f1, double f2)
        {
            double r;
            foreach (TreeNode t in StartNode.Nodes)
            {
                if (comboBox1.SelectedIndex == 0)
                {
                    try { r = Convert.ToDouble(t.Text); }
                    catch (FormatException) { SearchInTree(t, type, str1, str2, f1, f2); continue; }
                    switch (type)
                    {
                        case 1: if (r <= f2) textBox7.Text += t.Text + "\r\n"; break;
                        case 2: if (r <= f2 && r >= f1) textBox7.Text += t.Text + "\r\n"; break;
                        case 3: if (r >= f1) textBox7.Text += t.Text + "\r\n"; break;
                    }
                }
                else
                {
                    switch (type)
                    {
                        case 1: if (!(string.Compare(t.Text, str2) > 0)) textBox7.Text += t.Text + "\r\n"; break;
                        case 2: if (!(string.Compare(t.Text, str2) > 0) && !(string.Compare(str1, t.Text) > 0)) textBox7.Text += t.Text + "\r\n"; break;
                        case 3: if (!(string.Compare(str1, t.Text) > 0)) textBox7.Text += t.Text + "\r\n"; break;
                    }
                }
                SearchInTree(t, type, str1, str2, f1, f2);
            };
        }

        private void SearchInTree2(TreeNode StartNode, int type, string str1, string str2, double f1, double f2)
        {
            double r;
            foreach (TreeNode t in StartNode.Nodes)
            {
                foreach (ClassPropertie cp in Onto.Classes[t.Text].listOfProperties)
                    if (string.Equals(cp.objectPropertie, comboBox3.SelectedItem.ToString()))
                    {
                        if (comboBox1.SelectedIndex == 0)
                        {
                            try { r = Convert.ToDouble(cp.destObject); }
                            catch (FormatException) { SearchInTree2(t, type, str1, str2, f1, f2); continue; }
                            switch (type)
                            {
                                case 1: if (r <= f2) textBox7.Text += t.Text + " " + cp.objectPropertie + " " + cp.destObject + "\r\n"; break;
                                case 2: if (r <= f2 && r >= f1) textBox7.Text += t.Text + " " + cp.objectPropertie + " " + cp.destObject + "\r\n"; break;
                                case 3: if (r >= f1) textBox7.Text += t.Text + " " + cp.objectPropertie + " " + cp.destObject + "\r\n"; break;
                            }
                        }
                        else
                            switch (type)
                            {
                                case 1: if (!(string.Compare(cp.destObject, str2) > 0)) textBox7.Text += t.Text + " " + cp.objectPropertie + " " + cp.destObject + "\r\n"; break;
                                case 2: if (!(string.Compare(cp.destObject, str2) > 0) && !(string.Compare(str1, cp.destObject) > 0)) textBox7.Text += t.Text + " " + cp.objectPropertie + " " + cp.destObject + "\r\n"; break;
                                case 3: if (!(string.Compare(str1, cp.destObject) > 0)) textBox7.Text += t.Text + " " + cp.objectPropertie + " " + cp.destObject + "\r\n"; break;
                            }
                    }
                SearchInTree2(t, type, str1, str2, f1, f2);
            };
        }

        private void SearchPropertyInTree(TreeNode StartNode)
        {
            foreach (ClassPropertie cp in Onto.Classes[StartNode.Text].listOfProperties)
                if (!comboBox3.Items.Contains(cp.objectPropertie)) comboBox3.Items.Add(cp.objectPropertie);

            foreach (TreeNode t in StartNode.Nodes) SearchPropertyInTree(t);
        }

        private void button35_Click(object sender, EventArgs e) //поиск
        {
            if (textBox8.Text.Length == 0 && textBox9.Text.Length == 0)
            {
                MessageBox.Show("Не указан диапазон значений");
                return;
            }

            textBox7.Text = "";
            string str1 = "", str2 = "";
            double f1 = 0, f2 = 0, r;
            int type; //тип поиска 1 - до, 2 - от и до, 3 - от

            if (textBox8.Text.Length == 0)
            {
                type = 1;
                if (comboBox1.SelectedIndex == 0)
                    try { f2 = Convert.ToDouble(textBox9.Text); } 
                    catch (FormatException) { MessageBox.Show("Неверная граница поиска"); return; }
                else str2 = textBox9.Text;
            }
            else if (textBox9.Text.Length == 0)
            {
                type = 3;
                if (comboBox1.SelectedIndex == 0) 
                    try { f1 = Convert.ToDouble(textBox8.Text); } 
                    catch (FormatException) { MessageBox.Show("Неверная граница поиска"); return; }
                else str1 = textBox8.Text;
            }
            else
            {
                type = 2;
                if (comboBox1.SelectedIndex == 0)
                    try { f1 = Convert.ToDouble(textBox8.Text); f2 = Convert.ToDouble(textBox9.Text); }
                    catch (FormatException) { MessageBox.Show("Неверные границы поиска"); return; }
                else
                {
                    str1 = textBox8.Text;
                    str2 = textBox9.Text;
                }
            }

            if (radioButton1.Checked)
            {
                if (!checkBox4.Checked) SearchInTree(SearchNode(comboBox2.SelectedItem.ToString(), treeView1.Nodes[0]), type, str1, str2, f1, f2);
                else SearchInTree2(SearchNode(comboBox2.SelectedItem.ToString(), treeView1.Nodes[0]), type, str1, str2, f1, f2);
            }
            else
            {
                if (comboBox1.SelectedIndex == 0)
                    foreach (string s in Onto.Classes[comboBox2.SelectedItem.ToString()].listOfIndividuals)
                    {
                        try { r = Convert.ToDouble(s); }
                        catch (FormatException) { continue; }
                        switch (type)
                        {
                            case 1: if (r <= f2) textBox7.Text += s + "\r\n"; break;
                            case 2: if (r <= f2 && r >= f1) textBox7.Text += s + "\r\n"; break;
                            case 3: if (r >= f1) textBox7.Text += s + "\r\n"; break;
                        }
                    }
                else
                {
                    switch (type)
                    {
                        case 1: foreach (string s in Onto.Classes[comboBox2.SelectedItem.ToString()].listOfIndividuals)
                                if (!(string.Compare(s,str2) > 0)) textBox7.Text += s + "\r\n"; break;
                        case 2: foreach (string s in Onto.Classes[comboBox2.SelectedItem.ToString()].listOfIndividuals)
                                if (!(string.Compare(s,str2) > 0) && !(string.Compare(str1,s) > 0)) textBox7.Text += s + "\r\n"; break;
                        case 3: foreach (string s in Onto.Classes[comboBox2.SelectedItem.ToString()].listOfIndividuals)
                                if (!(string.Compare(str1,s) > 0)) textBox7.Text += s + "\r\n"; break;
                    }
                }
            }

            if (textBox7.Text.Length == 0) textBox7.Text = "Поиск завершен: нет результатов";
        }

        private void textBox8_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '.') e.KeyChar = ',';
        }

        private void Correlation()
        {            
            textBox6.Text = "";
            string str1, str2;
            if (tabControl2.SelectedIndex == 0)
            {
                str1 = listBox13.SelectedItem.ToString();
                if (tabControl3.SelectedIndex == 0) //класс-класс
                {
                    str2 = listBox16.SelectedItem.ToString();
                    if (Onto.Classes[str1].parentName == str2) textBox6.Text += "Класс " + str1 + " является подклассом класса " + str2 + "\r\n";
                    else if (Onto.Classes[str2].parentName == str1) textBox6.Text += "Класс " + str2 + " является подклассом класса " + str1 + "\r\n";

                    if (Onto.Classes[str1].equalClasses.Contains(str2) || Onto.Classes[str2].equalClasses.Contains(str1)) textBox6.Text += "Классы " + str1 + " и " + str2 + " эквивалентны\r\n";
                    else if (Onto.Classes[str1].disjointClasses.Contains(str2) || Onto.Classes[str2].disjointClasses.Contains(str1)) textBox6.Text += "Классы " + str1 + " и " + str2 + " противоположны\r\n";

                    foreach (ClassPropertie cp in Onto.Classes[str1].listOfProperties) if (cp.destObject == str2) textBox6.Text += str1 + " " + cp.objectPropertie + " " + str2 + "\r\n";
                    foreach (ClassPropertie cp in Onto.Classes[str2].listOfProperties) if (cp.destObject == str1) textBox6.Text += str2 + " " + cp.objectPropertie + " " + str1 + "\r\n";
                }
                else if (tabControl3.SelectedIndex == 1) //класс-отношение
                {
                    str2 = listBox17.SelectedItem.ToString();
                    if (Onto.Propetries[str2].diffArea.Contains(str1)) textBox6.Text += "Класс " + str1 + " лежит в области определения отношения " + str2 + "\r\n";
                    if (Onto.Propetries[str2].valueArea.Contains(str1)) textBox6.Text += "Класс " + str1 + " лежит в области значений отношения " + str2 + "\r\n";

                    foreach (ClassPropertie cp in Onto.Classes[str1].listOfProperties) if (cp.objectPropertie == str2) textBox6.Text += str1 + " " + str2 + " " + cp.destObject + "\r\n";
                    foreach (OntoClass oc in Onto.Classes)
                        foreach (ClassPropertie cp in oc.listOfProperties) if (cp.objectPropertie == str2 && cp.destObject == str1) textBox6.Text += oc.name + " " + str2 + " " + str1 + "\r\n";
                }
                else if (tabControl3.SelectedIndex == 2) //класс-экземпляр
                {
                    str2 = listBox18.SelectedItem.ToString();
                    if (Onto.Classes[str1].listOfIndividuals.Contains(str2)) textBox6.Text += "Класс " + str1 + " содержит экземпляр " + str2 + "\r\n";
                    if (Onto.Individuals[str2].listOfClasses.Contains(str1)) textBox6.Text += "Экземпляр " + str2 + " принадлежит классу " + str1 + "\r\n";
                }
            }
            else if (tabControl2.SelectedIndex == 1)
            {
                str1 = listBox14.SelectedItem.ToString();
                if (tabControl3.SelectedIndex == 0) //отношение-класс
                {
                    str2 = listBox16.SelectedItem.ToString();
                    if (Onto.Propetries[str1].diffArea.Contains(str2)) textBox6.Text += "Класс " + str2 + " лежит в области определения отношения " + str1 + "\r\n";
                    if (Onto.Propetries[str1].valueArea.Contains(str2)) textBox6.Text += "Класс " + str2 + " лежит в области значений отношения " + str1 + "\r\n";

                    foreach (ClassPropertie cp in Onto.Classes[str2].listOfProperties) if (cp.objectPropertie == str1) textBox6.Text += str2 + " " + str1 + " " + cp.destObject + "\r\n";
                    foreach (OntoClass oc in Onto.Classes)
                        foreach (ClassPropertie cp in oc.listOfProperties) if (cp.objectPropertie == str1 && cp.destObject == str2) textBox6.Text += oc.name + " " + str1 + " " + str2 + "\r\n";
                }
                else if (tabControl3.SelectedIndex == 1) //отношение-отношение
                {
                    str2 = listBox17.SelectedItem.ToString();
                    if (Onto.Propetries[str1].parentName == str2) textBox6.Text += "Отношение " + str1 + " является подчененным отношению " + str2 + "\r\n";
                    else if (Onto.Propetries[str2].parentName == str1) textBox6.Text += "Отношение " + str2 + " является подчененным отношению " + str1 + "\r\n";

                    if (Onto.Propetries[str1].equalProperties.Contains(str2) || Onto.Propetries[str2].equalProperties.Contains(str1)) textBox6.Text += "Отношения " + str1 + " и " + str2 + " эквивалентны\r\n";
                    else if (Onto.Propetries[str1].disjointProperties.Contains(str2) || Onto.Propetries[str2].disjointProperties.Contains(str1)) textBox6.Text += "Отношения " + str1 + " и " + str2 + " противоположны\r\n";                    
                }
            }
            else if (tabControl2.SelectedIndex == 2)
            {
                str1 = listBox15.SelectedItem.ToString();
                if (tabControl3.SelectedIndex == 0) //экземпляр-класс
                {
                    str2 = listBox16.SelectedItem.ToString();
                    if (Onto.Classes[str2].listOfIndividuals.Contains(str1)) textBox6.Text += "Класс " + str2 + " содержит экземпляр " + str1 + "\r\n";
                    if (Onto.Individuals[str1].listOfClasses.Contains(str2)) textBox6.Text += "Экземпляр " + str1 + " принадлежит классу " + str2 + "\r\n";
                }
                else if (tabControl3.SelectedIndex == 2) //экземпляр-экземпляр
                {
                    str2 = listBox18.SelectedItem.ToString();

                    if (Onto.Individuals[str1].equalIndividuals.Contains(str2) || Onto.Individuals[str2].equalIndividuals.Contains(str1)) textBox6.Text += "Экземпляры " + str1 + " и " + str2 + " эквивалентны\r\n";
                    else if (Onto.Individuals[str1].disjointIndividuals.Contains(str2) || Onto.Individuals[str2].disjointIndividuals.Contains(str1)) textBox6.Text += "Экземпляры " + str1 + " и " + str2 + " противоположны\r\n";
                }
            }
            if (textBox6.Text.Length == 0) textBox6.Text = "Связей не найдено";
        }

        #endregion

        #region DiagramBox

        private void DiagramBox_Invalidate()
        {
            SolidFigure figure;
            LineFigure line;

            foreach (OntoClass oc in Onto.Classes)
            {
                figure = new RoundRectFigure();
                figure.location = new Point(100, 100);
                figure.name = oc.name;
                figure.visible = true;
                Onto.diagram.figures.Add(figure);
            }

            foreach (OntoClass oc in Onto.Classes)
            {
                if (oc.parentName != "")
                {
                    line = new LineFigure();
                    line.From = Onto.diagram.figures[oc.name] as SolidFigure;
                    line.To = Onto.diagram.figures[oc.parentName] as SolidFigure;
                    line.type = 1;
                    line.visible = true;
                    Onto.diagram.figures.Add(line);
                }
                if (oc.listOfProperties.Count > 0)
                    foreach (ClassPropertie cp in oc.listOfProperties)
                    {
                        line = new LineFigure();
                        line.From = Onto.diagram.figures[oc.name] as SolidFigure;
                        line.To = Onto.diagram.figures[cp.destObject] as SolidFigure;
                        line.type = 2;
                        line.visible = true;
                        line.name = cp.objectPropertie;
                        Onto.diagram.figures.Add(line);
                    }
            }

            foreach (OntoIndividual oi in Onto.Individuals)
            {
                figure = new RectFigure();
                figure.location = new Point(100, 100);
                figure.name = oi.name;
                figure.visible = true;
                Onto.diagram.figures.Add(figure);
                if (oi.listOfClasses.Count > 0)                
                    foreach (string s in oi.listOfClasses)
                    {
                        line = new LineFigure();
                        line.From = Onto.diagram.figures[oi.name] as SolidFigure;
                        line.To = Onto.diagram.figures[s] as SolidFigure;
                        line.type = 1;
                        line.visible = true;
                        Onto.diagram.figures.Add(line);
                    }
            }

            diagramBox1.Invalidate();
        }

        private void случайноToolStripMenuItem_Click(object sender, EventArgs e) //случайно
        {
            diagramBox1.markers.Clear();
            diagramBox1.selectedFigure = null;

            Random rnd = new Random();
            foreach (Figure sf in Onto.diagram.figures) if (sf is SolidFigure) (sf as SolidFigure).location = new Point(rnd.Next(diagramBox1.Width - 200) + 100, rnd.Next(diagramBox1.Height - 200) + 100);

            diagramBox1.Invalidate();
        }

        private void TreeScaning(TreeNode StartNode, List<string> nodes, List<int> levels)
        {
            nodes.Add(StartNode.Text);
            levels.Add(StartNode.Level);
            foreach (TreeNode t in StartNode.Nodes) TreeScaning(t, nodes, levels);
        }

        private void иерархияToolStripMenuItem_Click(object sender, EventArgs e) // дерево вертикально
        {
            diagramBox1.markers.Clear();
            diagramBox1.selectedFigure = null;

            List<string> nodes = new List<string>();
            List<int> levels = new List<int>();
            TreeScaning(treeView1.Nodes[0], nodes, levels);
            List<int> LevelsCount = new List<int>();            
            foreach (int x in levels) 
            {
                if (LevelsCount.Count <= x) LevelsCount.Add(0);
                LevelsCount[x]++;
            }

            int[] LevelsCountAdded = new int[LevelsCount.Count];
            int[] distance = new int[LevelsCount.Count];

            for (int i = 0; i < LevelsCount.Count; i++)
            {
                distance[i] = (int)diagramBox1.Width / (LevelsCount[i] + 1);
                LevelsCountAdded[i] = 1;
            }

            for (int i = 0; i < nodes.Count; i++)
            {
                (Onto.diagram.figures[nodes[i]] as SolidFigure).location = new Point(LevelsCountAdded[levels[i]] * distance[levels[i]], 25 + 80 * levels[i]);
                LevelsCountAdded[levels[i]]++;
            }

            diagramBox1.Invalidate();

            nodes.Clear();
            foreach (string s in listBox8.Items) nodes.Add(s);
            distance[0] = (int)diagramBox1.Width / (nodes.Count + 1);
            distance[1] = diagramBox1.Height - 25;
            for (int i = 0; i < nodes.Count; i++) (Onto.diagram.figures[nodes[i]] as SolidFigure).location = new Point((i + 1) * distance[0], distance[1]);
        }

        private void иерархияГоризонтальноToolStripMenuItem_Click(object sender, EventArgs e) //горизонтально
        {
            diagramBox1.markers.Clear();
            diagramBox1.selectedFigure = null;

            List<string> nodes = new List<string>();
            List<int> levels = new List<int>();
            TreeScaning(treeView1.Nodes[0], nodes, levels);
            List<int> LevelsCount = new List<int>();
            foreach (int x in levels)
            {
                if (LevelsCount.Count <= x) LevelsCount.Add(0);
                LevelsCount[x]++;
            }

            int[] LevelsCountAdded = new int[LevelsCount.Count];
            int[] distance = new int[LevelsCount.Count];

            for (int i = 0; i < LevelsCount.Count; i++)
            {
                distance[i] = (int)diagramBox1.Height / (LevelsCount[i] + 1);
                LevelsCountAdded[i] = 1;
            }

            for (int i = 0; i < nodes.Count; i++)
            {
                (Onto.diagram.figures[nodes[i]] as SolidFigure).location = new Point(60 + 160 * levels[i], LevelsCountAdded[levels[i]] * distance[levels[i]]);
                LevelsCountAdded[levels[i]]++;
            }

            diagramBox1.Invalidate();

            nodes.Clear();
            foreach (string s in listBox8.Items) nodes.Add(s);
            distance[0] = (int)diagramBox1.Height / (nodes.Count + 1);
            distance[1] = diagramBox1.Width - 45;
            for (int i = 0; i < nodes.Count; i++) (Onto.diagram.figures[nodes[i]] as SolidFigure).location = new Point(distance[1], (i + 1) * distance[0]);
        }

        private void toolStripButton13_Click(object sender, EventArgs e) //скриншот
        {
            if (saveFileDialog1.ShowDialog(this) != DialogResult.OK) return;
            string Path = saveFileDialog1.FileName;
            int index = Path.LastIndexOf('.');
            if (index < 0 || Path.Substring(index, Path.Length - index) != ".png") Path += ".png";
            diagramBox1.GetImage().Save(Path);
        }

        private void toolStripButton17_Click(object sender, EventArgs e) //обновить диаграмму
        {            
            Onto.diagram.figures.Clear();
            diagramBox1.markers.Clear();
            diagramBox1.selectedFigure = null;
            toolStripMenuItem1.Checked = true;
            toolStripMenuItem2.Checked = true;
            TransferData.IndividCollapse = false;
            отображатьОтношенияToolStripMenuItem.Checked = true;
            DiagramBox_Invalidate();
        }

        private void toolStripButton14_Click(object sender, EventArgs e) //увеличить масштаб
        {
            if (GlobalScale > 1.6f) return;
            diagramBox1.markers.Clear();
            diagramBox1.selectedFigure = null;            
            GlobalScale += 0.2f;
            float ds = GlobalScale / (GlobalScale - 0.2f);
            foreach (Figure f in Onto.diagram.figures) if (f is SolidFigure) (f as SolidFigure).Scale(ds, ds);
            diagramBox1.Invalidate();
        }

        private void toolStripButton16_Click(object sender, EventArgs e) //масштаб по умолчанию
        {
            diagramBox1.markers.Clear();
            diagramBox1.selectedFigure = null;
            foreach (Figure f in Onto.diagram.figures) if (f is SolidFigure) (f as SolidFigure).Scale(1 / GlobalScale, 1 / GlobalScale);
            GlobalScale = 1.0f;
            diagramBox1.Invalidate();
        }

        private void toolStripButton15_Click(object sender, EventArgs e) //уменьшить масштаб
        {
            if (GlobalScale < 0.6f) return;
            diagramBox1.markers.Clear();
            diagramBox1.selectedFigure = null;
            GlobalScale -= 0.2f;
            float ds = GlobalScale / (GlobalScale + 0.2f);
            foreach (Figure f in Onto.diagram.figures) if (f is SolidFigure) (f as SolidFigure).Scale(ds, ds);
            diagramBox1.Invalidate();            
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e) //отображать экземпяры
        {
            ToolStripMenuItem.Checked = !ToolStripMenuItem.Checked;
            if (ToolStripMenuItem.Checked)
            {
                TransferData.IndividCollapse = false;
                foreach (OntoIndividual oi in Onto.Individuals)
                {
                    Figure ff = Onto.diagram.figures[oi.name];
                    if ((ff as RectFigure).collapse == true) continue;
                    Onto.diagram.figures[oi.name].visible = true;
                    if (отображатьСвязиToolStripMenuItem.Checked) foreach (Figure f in Onto.diagram.figures) if ((f is LineFigure) && ((f as LineFigure).From == ff)) f.visible = true;
                }
            }
            else
            {
                TransferData.IndividCollapse = true;
                foreach (OntoIndividual oi in Onto.Individuals) Onto.diagram.figures[oi.name].visible = false;
                foreach (Figure f in Onto.diagram.figures) if ((f is LineFigure) && ((f as LineFigure).From is RectFigure)) f.visible = false;
            }

            diagramBox1.Invalidate();
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e) //отображать связи
        {
            отображатьСвязиToolStripMenuItem.Checked = !отображатьСвязиToolStripMenuItem.Checked;
            if (отображатьСвязиToolStripMenuItem.Checked)
            {
                if (ToolStripMenuItem.Checked)
                {
                    foreach (Figure f in Onto.diagram.figures) if ((f is LineFigure) && (f as LineFigure).type == 1) f.visible = true;
                }
                else
                {
                    foreach (Figure f in Onto.diagram.figures) if ((f is LineFigure) && (f as LineFigure).type == 1 && ((f as LineFigure).From is RoundRectFigure)) f.visible = true;
                }
            }
            else
            {
                foreach (Figure f in Onto.diagram.figures) if ((f is LineFigure) && (f as LineFigure).type == 1) f.visible = false;
            }

            diagramBox1.Invalidate();
        }

        private void отображатьОтношенияToolStripMenuItem_Click(object sender, EventArgs e) //отображать отношения
        {
            отображатьОтношенияToolStripMenuItem.Checked = !отображатьОтношенияToolStripMenuItem.Checked;
            if (отображатьОтношенияToolStripMenuItem.Checked)
            {
                foreach (Figure f in Onto.diagram.figures) if ((f is LineFigure) && (f as LineFigure).type == 2) f.visible = true;
            }
            else
            {
                foreach (Figure f in Onto.diagram.figures) if ((f is LineFigure) && (f as LineFigure).type == 2) f.visible = false;
            }

            diagramBox1.Invalidate();
        }

        #endregion

        #region Меню

        private void toolStripMenuItem5_Click(object sender, EventArgs e) //сохранить
        {
            if (toolStripStatusLabel1.Text == "Новый проект") сохранитьКакToolStripMenuItem_Click(this, e);
            else
            {
                BinaryFormatter binFormat = new BinaryFormatter();
                using (Stream fstream = new FileStream(toolStripStatusLabel1.Text, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    binFormat.Serialize(fstream, Onto);
                }

                toolStripStatusLabel2.Text = "";
            }
        }

        private void сохранитьКакToolStripMenuItem_Click(object sender, EventArgs e) //сохранить как
        {
            if (saveFileDialog1.ShowDialog(this) != DialogResult.OK) return;
            string Path = saveFileDialog1.FileName;
            int index = Path.LastIndexOf('.');
            if (index < 0 || Path.Substring(index, Path.Length-index) != ".onto") Path += ".onto";

            BinaryFormatter binFormat = new BinaryFormatter();
            using (Stream fstream = new FileStream(Path, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                binFormat.Serialize(fstream, Onto);
            }

            toolStripStatusLabel1.Text = Path;
            toolStripStatusLabel2.Text = "";
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e) //открыть
        {
            if (toolStripStatusLabel2.Text != "")
                if (MessageBox.Show("Изменения не сохранены. Продолжить ?", "Внимание !", MessageBoxButtons.YesNo) == DialogResult.No) return;

            if (openFileDialog1.ShowDialog(this) != DialogResult.OK) return;

            BinaryFormatter binFormat = new BinaryFormatter();

            Onto.Classes.Clear();
            Onto.Propetries.Clear();
            Onto.Individuals.Clear();

            Onto.diagram.figures.Clear();
            diagramBox1.markers.Clear();
            diagramBox1.selectedFigure = null;
            GlobalScale = 1.0f;

            treeView1.Nodes.Clear();
            treeView2.Nodes.Clear();
            listBox8.Items.Clear();
            comboBox2.Items.Clear();
            comboBox3.Items.Clear();

            treeView1.Nodes.Add("Thing");
            treeView2.Nodes.Add("topObjectProperty"); 

            using (Stream fstream = File.OpenRead(openFileDialog1.FileName))
            {
                try
                {
                    Onto = (Ontology)binFormat.Deserialize(fstream);
                }
                catch (System.Runtime.Serialization.SerializationException)
                {
                    MessageBox.Show("Неверный файл");
                    return;
                }
            }

            toolStripStatusLabel1.Text = openFileDialog1.FileName;
            toolStripStatusLabel2.Text = "";

            ///Panel1
            string _parent = "Thing";
            TreeNodeCollection editNodes = treeView1.Nodes[0].Nodes;
            int i = 1;
            while (true)
            {
                while (i < Onto.Classes.Count && Onto.Classes[i].parentName == _parent) editNodes.Add(Onto.Classes[i++].name);
                if (i >= Onto.Classes.Count) break;
                _parent = Onto.Classes[i].parentName;
                editNodes = SearchNode(_parent, treeView1.Nodes[0]).Nodes;
            }            
            treeView1.ExpandAll();
            Panel1_Invalidate();
            ///Panel2
            _parent = "topObjectProperty";
            editNodes = treeView2.Nodes[0].Nodes;
            i = 1;
            while (true)
            {
                while (i < Onto.Propetries.Count && Onto.Propetries[i].parentName == _parent) editNodes.Add(Onto.Propetries[i++].name);
                if (i >= Onto.Propetries.Count) break;
                _parent = Onto.Propetries[i].parentName;
                editNodes = SearchNode(_parent, treeView2.Nodes[0]).Nodes;
            }
            treeView2.ExpandAll();
            Panel2_Invalidate();            
            ///Panel3
            foreach (OntoIndividual oi in Onto.Individuals) listBox8.Items.Add(oi.name);
            Panel3_Invalidate();
            ///Panel4
            Panel41_Invalidate();
            Panel42_Invalidate();
            Panel43_Invalidate();
            comboBox1.SelectedIndex = 0;
            ///
            if (Onto.diagram == null) Onto.diagram = new Diagram(); //для совместимости сохранений
            diagramBox1.Diagram = Onto.diagram;
            diagramBox1.Invalidate();
            node = treeView1.Nodes[0];
            _node = "Thing";
        }

        private void новыйToolStripMenuItem_Click(object sender, EventArgs e) //новый файл
        {
            if (MessageBox.Show("Очистить всю имеющуюся информацию ?", "Внимание !", MessageBoxButtons.YesNo) == DialogResult.No) return;

            Onto.Classes.Clear();
            Onto.Propetries.Clear();
            Onto.Individuals.Clear();

            Onto.diagram.figures.Clear();
            diagramBox1.markers.Clear();
            diagramBox1.selectedFigure = null;
            GlobalScale = 1.0f;

            treeView1.Nodes.Clear();
            treeView2.Nodes.Clear();
            listBox8.Items.Clear();
            comboBox2.Items.Clear();
            comboBox3.Items.Clear();

            treeView1.Nodes.Add("Thing");
            treeView2.Nodes.Add("topObjectProperty");  
            toolStripStatusLabel1.Text = "Новый проект";
            Onto.Classes.Add(new OntoClass() { name = "Thing", parentName = "", annotation = "" });
            Onto.Propetries.Add(new OntoPropertie() { name = "topObjectProperty", parentName = "", annotation = "", symmetric = false, reflexive = false, transitive = false });
            node = treeView1.Nodes[0];
            _node = "Thing";

            Panel3_Invalidate();            
            Panel2_Invalidate();
            Panel1_Invalidate();
            if (tabControl1.SelectedIndex == 0) node = treeView1.Nodes[0];
            else if (tabControl1.SelectedIndex == 1) node = treeView2.Nodes[0];
            Panel41_Invalidate();
            Panel42_Invalidate();
            Panel43_Invalidate();
            diagramBox1.Invalidate();
            toolStripStatusLabel2.Text = "";
            comboBox1.SelectedIndex = 0;
            TransferData.IndividCollapse = false;
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)  //выход
        {
            if (toolStripStatusLabel2.Text == "") this.Close();
            else if (MessageBox.Show("Выйти без сохранения новых данных ?", "Внимание !", MessageBoxButtons.YesNo) == DialogResult.Yes) this.Close();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e) //выход
        {
            if (toolStripStatusLabel2.Text != "" && MessageBox.Show("Выйти без сохранения новых данных ?", "Внимание !", MessageBoxButtons.YesNo) == DialogResult.No) e.Cancel = true;
        }

        private void создатьОтчетToolStripMenuItem_Click(object sender, EventArgs e) //текстовый отчет
        {
            if (saveFileDialog1.ShowDialog(this) != DialogResult.OK) return;
            string Path = saveFileDialog1.FileName;
            int index = Path.LastIndexOf('.');
            if (index < 0 || Path.Substring(index, Path.Length - index) != ".txt") saveFileDialog1.FileName += ".txt";

            FileInfo fi = new FileInfo(saveFileDialog1.FileName);
            StreamWriter sw;
            sw = fi.CreateText();
            string str;

            sw.Write("Онтология  " + toolStripStatusLabel1.Text);
            sw.Write("\r\n\r\n================================");
            sw.Write("\r\n============ Классы ============");
            sw.Write("\r\n================================");
            foreach (OntoClass oc in Onto.Classes)
            {
                sw.Write("\r\n\r\nКласс \"" + oc.name + "\", подкласс класса \"" + oc.parentName + "\"");
                sw.Write("\r\nАннотация: " + oc.annotation);

                str = "";
                foreach (string s in oc.equalClasses) str += " " + s;
                sw.Write("\r\nЭквивалентные классы:" + str);

                str = "";
                foreach (string s in oc.disjointClasses) str += " " + s;
                sw.Write("\r\nПротивоположные классы:" + str);

                str = "";
                foreach (string s in oc.listOfIndividuals) str += " " + s;
                sw.Write("\r\nЭкземпляры:" + str);

                str = "";                
                foreach (ClassPropertie cp in oc.listOfProperties) str += "  " + cp.objectPropertie + " (" + cp.typeOfPropertie + ") " + cp.destObject;
                sw.Write("\r\nОтношения класса:" + str);        
            }

            sw.Write("\r\n\r\n===================================");
            sw.Write("\r\n============ Отношения ============");
            sw.Write("\r\n===================================");

            foreach (OntoPropertie op in Onto.Propetries)
            {
                sw.Write("\r\n\r\nОтношение \"" + op.name + "\", подкласс отношения \"" + op.parentName + "\"");
                sw.Write("\r\nАннотация: " + op.annotation);

                str = "";
                if (op.transitive) str += " транзитивно";
                if (op.symmetric) str += " симметрично";
                if (op.reflexive) str += " рефлексивно";
                if (op.transitive || op.reflexive || op.symmetric) sw.Write("\nОтношение " + str);

                str = "";
                foreach (string s in op.diffArea) str += " " + s;
                sw.Write("\r\nКлассы области определения:" + str);

                str = "";
                foreach (string s in op.valueArea) str += " " + s;
                sw.Write("\r\nКлассы области значений:" + str);

                str = "";
                foreach (string s in op.equalProperties) str += " " + s;
                sw.Write("\r\nЭквивалентные отношения:" + str);

                str = "";
                foreach (string s in op.disjointProperties) str += " " + s;
                sw.Write("\r\nПротивоположные отношения:" + str);
            }

            sw.Write("\r\n\r\n====================================");
            sw.Write("\r\n============ Экземпляры ============");
            sw.Write("\r\n====================================");

            foreach (OntoIndividual oi in Onto.Individuals)
            {
                sw.Write("\r\n\r\nЭкземпляр \"" + oi.name + "\"");

                str = "";
                foreach (string s in oi.listOfClasses) str += " " + s;
                sw.Write("\r\nПринадлежит классам:" + str);

                sw.Write("\r\nАннотация: " + oi.annotation);


                str = "";
                foreach (string s in oi.equalIndividuals) str += " " + s;
                sw.Write("\r\nЭквивалентные экземпляры:" + str);

                str = "";
                foreach (string s in oi.disjointIndividuals) str += " " + s;
                sw.Write("\r\nПротивоположные экземпляры:" + str);
            }
            sw.Dispose();
            MessageBox.Show("Отчет создан");
        }

        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e) //о программе
        {
            About f = new About();
            f.ShowDialog();
        }

        private void справкаToolStripMenuItem_Click(object sender, EventArgs e) //справка
        {
            Process proc = new Process();
            proc.StartInfo.FileName = "OntoHelp.chm";
            try
            {
                proc.Start();
            }
            catch (Win32Exception)
            {
                MessageBox.Show("Файл справки не найден");
            }
        }

        #endregion



    }

    static class TransferData
    {
        public static string StrValue { get; set; }

        public static string StrValue2 { get; set; }

        public static string StrValue3 { get; set; }

        public static bool IndividCollapse { get; set; }
    }
}
