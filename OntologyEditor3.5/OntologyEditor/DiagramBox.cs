using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace OntologyEditor
{
    public partial class DiagramBox : UserControl
    {
        Diagram diagram;
        public Figure selectedFigure = null;
        public List<Marker> markers = new List<Marker>();
        public float GlobalScale = 1;
        Figure draggedFigure = null;        
        Pen selectRectPen;
        Point startDragPoint;

        public DiagramBox()
        {
            InitializeComponent();

            DoubleBuffered = true;
            ResizeRedraw = true;

            selectRectPen = new Pen(Color.Red, 1);
            selectRectPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
        }

        public Diagram Diagram
        {
            get { return diagram; }
            set
            {
                diagram = value;
                selectedFigure = null;
                draggedFigure = null;
                markers.Clear();
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Draw(e.Graphics);
        }

        private void Draw(Graphics gr)
        {
            gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            if (diagram != null)
            {                
                foreach (Figure f in diagram.figures) if (f is LineFigure && f.visible) f.Draw(gr);
                foreach (Figure f in diagram.figures) if (f is SolidFigure && f.visible) f.Draw(gr);
            }

            if (selectedFigure is SolidFigure)
            {
                SolidFigure figure = selectedFigure as SolidFigure;
                RectangleF bounds = figure.Bounds;
                gr.DrawRectangle(selectRectPen, bounds.Left - 2, bounds.Top - 2, bounds.Width + 4, bounds.Height + 4);
            }

            foreach (Marker m in markers) m.Draw(gr);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            draggedFigure = FindFigureByPoint(e.Location);
            if (!(draggedFigure is Marker))
            {
                selectedFigure = draggedFigure;
                CreateMarkers();
            }
            startDragPoint = e.Location;
            Invalidate();

            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                if (selectedFigure != null)
                {
                    if (selectedFigure is LineFigure) { развернутьToolStripMenuItem.Enabled = false; свернутьВУзелToolStripMenuItem.Enabled = false; }
                    if (selectedFigure is SolidFigure)
                    {
                        if ((selectedFigure as SolidFigure).collapse) { развернутьToolStripMenuItem.Enabled = true; свернутьВУзелToolStripMenuItem.Enabled = false; }
                        else { развернутьToolStripMenuItem.Enabled = false; свернутьВУзелToolStripMenuItem.Enabled = true; }
                    }
                    contextMenuStrip1.Show(PointToScreen(e.Location));
                }
            }
        }

        private void CreateMarkers()
        {
            if (selectedFigure == null) markers = new List<Marker>();
            else
            {
                markers = selectedFigure.CreateMarkers(diagram);
                UpdateMarkers();
            }
        }

        private void UpdateMarkers()
        {
            foreach (Marker m in markers) if (draggedFigure != m) m.UpdateLocation();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (draggedFigure != null && (draggedFigure is SolidFigure) && !(selectedFigure is LineFigure))
                {
                    (draggedFigure as SolidFigure).Offset(e.Location.X - startDragPoint.X, e.Location.Y - startDragPoint.Y);
                    UpdateMarkers();
                    Invalidate();
                }
            }
            else
            {
                Figure figure = FindFigureByPoint(e.Location);
                if (figure != null) Cursor = Cursors.Hand;
                else Cursor = Cursors.Default;
            }

            startDragPoint = e.Location;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            draggedFigure = null;
            UpdateMarkers();
            Invalidate();
        }

        Figure FindFigureByPoint(Point p)
        {
            foreach (Marker m in markers) if (m.IsInsidePoint(p)) return m;
            for (int i = diagram.figures.Count - 1; i >= 0; i--)
                if (diagram.figures[i] is SolidFigure && diagram.figures[i].IsInsidePoint(p) && diagram.figures[i].visible) return diagram.figures[i];
            for (int i = diagram.figures.Count - 1; i >= 0; i--)
                if (diagram.figures[i] is LineFigure && diagram.figures[i].IsInsidePoint(p) && diagram.figures[i].visible) return diagram.figures[i];
            return null;
        }

        #region Menu

        private void наПереднийПланToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedFigure != null)
            {
                diagram.figures.Remove(selectedFigure);
                diagram.figures.Add(selectedFigure);
                Invalidate();
            }
        }

        private void наЗаднийПланToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedFigure != null)
            {
                diagram.figures.Remove(selectedFigure);
                diagram.figures.Insert(0, selectedFigure);
                Invalidate();
            }
        }

        public Bitmap GetImage()
        {
            selectedFigure = null;
            draggedFigure = null;
            CreateMarkers();

            Bitmap bmp = new Bitmap(Bounds.Width, Bounds.Height);
            DrawToBitmap(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height));

            return bmp;
        }

        private void информацияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            panel1.Visible = true;

            string ss = "", str;
            textBox10.ScrollBars = ScrollBars.None;

            OntoClass oc;
            if ((oc = Form1.Onto.Classes.FirstOrDefault(f => f.name == selectedFigure.name)) != null)
            {
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
            }
            else
            {
                OntoIndividual oi;
                if ((oi = Form1.Onto.Individuals.FirstOrDefault(f => f.name == selectedFigure.name)) != null)
                {
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
                }
                else
                {
                    OntoPropertie op;
                    if ((op = Form1.Onto.Propetries.FirstOrDefault(f => f.name == selectedFigure.name)) != null)
                    {
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
                    }
                    else ss = "Cвязь";
                }
            }
            textBox10.Text = ss;
            if (textBox10.Lines.Length > 8) textBox10.ScrollBars = ScrollBars.Vertical;
        }

        private void CollapseTree(Figure StartFigure)
        {
            foreach (Figure f in Form1.Onto.diagram.figures)
            {
                if ((f is LineFigure) && (f as LineFigure).To == StartFigure)
                {
                    f.visible = false;
                    if ((f as LineFigure).type == 1)
                    {
                        (f as LineFigure).From.visible = false;
                        CollapseTree((f as LineFigure).From);
                    }
                }
                else if ((f is LineFigure) && (f as LineFigure).type == 2 && (f as LineFigure).From == StartFigure) f.visible = false;
            }
            (StartFigure as SolidFigure).collapse = true;
        }

        private void deCollapseTree(Figure StartFigure)
        {
            foreach (Figure f in Form1.Onto.diagram.figures)
            {
                if ((f is LineFigure) && (f as LineFigure).To == StartFigure)
                {
                    if ((TransferData.IndividCollapse && (f as LineFigure).From is RectFigure) || (f as LineFigure).type == 2 && (f as LineFigure).From.collapse) continue;
                    f.visible = true;
                    if ((f as LineFigure).type == 1)
                    {
                        (f as LineFigure).From.visible = true;
                        deCollapseTree((f as LineFigure).From);
                    }
                }
                else if ((f is LineFigure) && (f as LineFigure).type == 2 && (f as LineFigure).From == StartFigure && !(f as LineFigure).To.collapse) f.visible = true;
            }
            (StartFigure as SolidFigure).collapse = false;
        }

        private void свернутьВУзелToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CollapseTree(selectedFigure);
            Invalidate();            
        }

        private void развернутьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            deCollapseTree(selectedFigure);
            Invalidate();
        }

        private void DiagramBox_Click(object sender, EventArgs e)
        {
            panel1.Visible = false;
        }

        private void DiagramBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            panel1.Visible = false;
        }

        bool isInfoDrag = false;
        Point InfoLocation;

        private void textBox10_MouseMove(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
            if (isInfoDrag)
            {
                panel1.Left += e.X - InfoLocation.X;
                panel1.Top += e.Y - InfoLocation.Y;
            }
        }

        private void textBox10_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isInfoDrag = true;
                InfoLocation.X = e.X;
                InfoLocation.Y = e.Y;
            }
        }

        private void textBox10_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) isInfoDrag = false;
        }

        #endregion
    }
}
