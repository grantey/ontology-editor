using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Runtime.Serialization;

namespace OntologyEditor
{
    [Serializable]
    public class Diagram
    {
        public readonly FigureList figures = new FigureList();
    }

    [Serializable]
    public abstract class Figure
    {
        readonly SerializableGraphicsPath serializablePath = new SerializableGraphicsPath();
        protected GraphicsPath Path { get { return serializablePath.path; } }
        public static Pen pen = Pens.Black;
        public abstract bool IsInsidePoint(Point p);
        public abstract void Draw(Graphics gr);
        public abstract List<Marker> CreateMarkers(Diagram diagram);
        public string name;
        public bool visible;
    }

    [Serializable]
    public class FigureList : List<Figure>
    {
        public Figure this[string name]
        {
            get
            {
                return this.First(f => f.name == name);
            }
        }
    }

    [Serializable]
    public abstract class SolidFigure : Figure
    {
        protected static int defaultSize = 40;       
        public static Brush brush = Brushes.White;
        public Point location;
        protected RectangleF textRect;
        public bool collapse = false;

        protected virtual StringFormat StringFormat //отображение строки внутри объекта
        {
            get
            {
                StringFormat stringFormat = new StringFormat();
                stringFormat.Alignment = StringAlignment.Center;
                stringFormat.LineAlignment = StringAlignment.Center;
                return stringFormat;
            }
        } 

        public override bool IsInsidePoint(Point p) //точка внутри объекта?
        {
            return Path.IsVisible(p.X - location.X, p.Y - location.Y);
        }

        public RectangleF Bounds //рамка вокруг фигуры
        {
            get
            {
                RectangleF bounds = Path.GetBounds();
                return new RectangleF(bounds.Left + location.X, bounds.Top + location.Y, bounds.Width, bounds.Height);
            }
        }

        public Rectangle TextBounds //прямоугольник под текст
        {
            get
            {
                return new Rectangle((int)textRect.Left + location.X, (int)textRect.Top + location.Y, (int)textRect.Width, (int)textRect.Height);
            }
        }

        public SizeF Size //размер прямоугольника вокруг объекта
        {
            get { return Path.GetBounds().Size; }
            set
            {
                SizeF oldSize = Path.GetBounds().Size;
                SizeF newSize = new SizeF(Math.Max(1, value.Width), Math.Max(1, value.Height));
                float kx = newSize.Width / oldSize.Width;
                float ky = newSize.Height / oldSize.Height;
                Scale(kx, ky);
            }
        }

        public void Scale(float scaleX, float scaleY) //изменение масштаба фигуры
        {
            Matrix m = new Matrix();
            m.Scale(scaleX, scaleY);
            Path.Transform(m);
            textRect = new RectangleF(textRect.Left * scaleX, textRect.Top * scaleY, textRect.Width * scaleX, textRect.Height * scaleY);
        }

        public virtual void Offset(int dx, int dy) //сдвиг
        {
            location.Offset(dx, dy);
            if (location.X < 0) location.X = 20;
            if (location.Y < 0) location.Y = 20;
        }

        public override void Draw(Graphics gr) //рисование объекта
        {
            gr.TranslateTransform(location.X, location.Y);
            gr.FillPath(brush, Path);
            gr.DrawPath(pen, Path);
            if (!string.IsNullOrEmpty(name))
            {
                if (!collapse) gr.DrawString(name, SystemFonts.DefaultFont, Brushes.Black, textRect, StringFormat);
                else gr.DrawString(name+"+", SystemFonts.DefaultFont, Brushes.Black, textRect, StringFormat);
            }
            gr.ResetTransform();
        }

        public override List<Marker> CreateMarkers(Diagram diagram) //создание маркера для изменения размера
        {
            List<Marker> markers = new List<Marker>();
            Marker m = new SizeMarker();
            m.targetFigure = this;
            markers.Add(m);
            return markers;
        }
    }

    [Serializable]
    public class RectFigure : SolidFigure
    {        
        public RectFigure()
        {
            Path.AddRectangle(new RectangleF(-defaultSize, -defaultSize / 2, 2 * defaultSize, defaultSize));
            textRect = new RectangleF(-defaultSize + 3, -defaultSize / 2 + 2, 2 * defaultSize - 6, defaultSize - 4);
        }
    }

    [Serializable]
    public class RoundRectFigure : SolidFigure
    {
        public RoundRectFigure()
        {
            float diameter = 16f;
            SizeF sizeF = new SizeF(diameter, diameter);
            RectangleF arc = new RectangleF(-defaultSize, -defaultSize / 2, sizeF.Width, sizeF.Height);
            Path.AddArc(arc, 180, 90);
            arc.X = defaultSize - diameter;
            Path.AddArc(arc, 270, 90);
            arc.Y = defaultSize / 2 - diameter;
            Path.AddArc(arc, 0, 90);
            arc.X = -defaultSize;
            Path.AddArc(arc, 90, 90);
            Path.CloseFigure();
            textRect = new RectangleF(-defaultSize + 3, -defaultSize / 2 + 2, 2 * defaultSize - 6, defaultSize - 4);
        }
    }

    [Serializable]
    public abstract class Marker : SolidFigure
    {
        protected static new int defaultSize = 2;
        public Figure targetFigure;

        public override bool IsInsidePoint(Point p)
        {
            if (p.X < location.X - defaultSize || p.X > location.X + defaultSize) return false;
            if (p.Y < location.Y - defaultSize || p.Y > location.Y + defaultSize) return false;
            return true;
        }

        public override void Draw(Graphics gr)
        {
            gr.DrawRectangle(Pens.Black, location.X - defaultSize, location.Y - defaultSize, defaultSize * 2, defaultSize * 2);
            gr.FillRectangle(Brushes.Red, location.X - defaultSize, location.Y - defaultSize, defaultSize * 2, defaultSize * 2);
        }

        public abstract void UpdateLocation();
    }

    public class SizeMarker : Marker
    {
        public override void UpdateLocation()
        {
            RectangleF bounds = (targetFigure as SolidFigure).Bounds;
            location = new Point((int)Math.Round(bounds.Right) + defaultSize / 2, (int)Math.Round(bounds.Bottom) + defaultSize / 2);
        }

        public override void Offset(int dx, int dy)
        {
            base.Offset(dx, dy);
            (targetFigure as SolidFigure).Size = SizeF.Add((targetFigure as SolidFigure).Size, new SizeF(dx * 2, dy * 2));
        }
    }

    [Serializable]
    public class EndLineMarker : Marker
    {
        int pointIndex;
        Diagram diagram;

        public EndLineMarker(Diagram diagram, int pointIndex)
        {
            this.diagram = diagram;
            this.pointIndex = pointIndex;
        }

        public override void UpdateLocation()
        {
            LineFigure line = (targetFigure as LineFigure);
            if (line.From == null || line.To == null) return;            
            SolidFigure figure = pointIndex == 0 ? line.From : line.To;
            location = figure.location;
        }

        public override void Offset(int dx, int dy)
        {
            base.Offset(dx, dy);

            //ищем фигуру под маркером
            SolidFigure figure = null;
            for (int i = diagram.figures.Count - 1; i >= 0; i--)
                if (diagram.figures[i] is SolidFigure && diagram.figures[i].IsInsidePoint(location))
                {
                    figure = (SolidFigure)diagram.figures[i];
                    break;
                }

            LineFigure line = (targetFigure as LineFigure);
            if (figure == null) figure = this;

            if (line.From == figure || line.To == figure) return;
            
            if (pointIndex == 0) line.From = figure;
            else line.To = figure;
        }
    }

    [Serializable]
    public class LineFigure : Figure
    {
        public int type;
        public SolidFigure From;
        public SolidFigure To;
        static Pen clickPen = new Pen(Color.Transparent, 3);

        public override void Draw(Graphics gr)
        {
            if (From == null || To == null) return;
            RecalcPath();
            if (type == 1) gr.DrawPath(pen, Path);
            else
            {
                Pen pen1 = new Pen(Color.Red, 2);
                pen1.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                gr.DrawPath(pen1, Path);
            }
        }

        public override bool IsInsidePoint(Point p)
        {
            if (From == null || To == null) return false;
            RecalcPath();
            return Path.IsOutlineVisible(p, clickPen);
        }

        protected virtual void RecalcPath()
        {
            PointF[] points = null;
            if (Path.PointCount > 0) points = Path.PathPoints;
            if (Path.PointCount != 2 || points[0] != From.location || points[1] != To.location)
            {
                Path.Reset();
                Path.AddLine(From.location, To.location);
            }
        }

        public override List<Marker> CreateMarkers(Diagram diagram)
        {
            List<Marker> markers = new List<Marker>();
            EndLineMarker m1 = new EndLineMarker(diagram, 0);
            m1.targetFigure = this;
            EndLineMarker m2 = new EndLineMarker(diagram, 1);
            m2.targetFigure = this;

            markers.Add(m1);
            markers.Add(m2);

            return markers;
        }
    }

    [Serializable]
    public class SerializableGraphicsPath : ISerializable
    {
        public GraphicsPath path = new GraphicsPath();

        public SerializableGraphicsPath()
        {
        }

        private SerializableGraphicsPath(SerializationInfo info, StreamingContext context)
        {
            if (info.MemberCount > 0)
            {
                PointF[] points = (PointF[])info.GetValue("p", typeof(PointF[]));
                byte[] types = (byte[])info.GetValue("t", typeof(byte[]));
                path = new GraphicsPath(points, types);
            }
            else
                path = new GraphicsPath();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (path.PointCount > 0)
            {
                info.AddValue("p", path.PathPoints);
                info.AddValue("t", path.PathTypes);
            }
        }
    }
}
