using System.Drawing;
using System.Drawing.Drawing2D;

namespace Graphic.Items
{
    public class Pie : Rec
    {
        float _startAngle;
        float _sweepAngle;


        public Pie(float Xn, float Yn, float Width, float Height, Color BorderCol, Color Col, int ZOrder, float BorderWidth,
          Nows TypeShape, float startAngle, float sweepAngle, string Name)
            : base(Xn, Yn, Width, Height, BorderCol, Col, ZOrder, BorderWidth, Name)
        {
            this.startAngle = startAngle;
            this.sweepAngle = sweepAngle;
            this.TypeShape = TypeShape;
        }

        public Pie(Pie P)
            : base(P.Xn, P.Yn, P.Width, P.Height, P.BorderCol, P.Col, P.ZOrder, P.BorderWidth, P.Name)
        {
            startAngle = P.startAngle;
            sweepAngle = P.sweepAngle;
            TypeShape = P.TypeShape;
        }

        public float startAngle
        {
            get { return _startAngle; }
            set { _startAngle = value; }
        }

        public float sweepAngle
        {
            get { return _sweepAngle; }
            set { _sweepAngle = value; }
        }

        public new bool ContainsPoint(float X, float Y)
        {
            GraphicsPath Path = new GraphicsPath();
            Path.StartFigure();
            if (TypeShape == Nows.Ellip)
                Path.AddEllipse(Rectan);
            else
                Path.AddPie(Rectan.X, Rectan.Y, Rectan.Width, Rectan.Height, startAngle, sweepAngle);
            Path.CloseFigure();

            return Path.IsVisible(X, Y);
        }

        public new void Move(float Dx, float Dy, float Dlg, float Dlv, Color BackColor, Graphics G)
        {
            RePain(Dlg, Dlv, BackColor, BorderWidth, 1, G);
            Xn += Dx;
            Yn += Dy;
            RePain(Dlg, Dlv, false, 1, G);
        }

        public new void MoveX1(float NewX, float Dlg, float Dlv, Color BackColor, Graphics G)
        {
            RePain(Dlg, Dlv, BackColor, BorderWidth, 1, G);
            MovingX1(NewX);
            RePain(Dlg, Dlv, false, 1, G);
        }

        public new void MoveY1(float NewY, float Dlg, float Dlv, Color BackColor, Graphics G)
        {
            RePain(Dlg, Dlv, BackColor, BorderWidth, 1, G);
            MovingY1(NewY);
            RePain(Dlg, Dlv, false, 1, G);
        }

        public new void MoveX2(float NewX, float Dlg, float Dlv, Color BackColor, Graphics G)
        {
            RePain(Dlg, Dlv, BackColor, BorderWidth, 1, G);
            MovingX2(NewX);
            RePain(Dlg, Dlv, false, 1, G);
        }

        public new void MoveY2(float NewY, float Dlg, float Dlv, Color BackColor, Graphics G)
        {
            RePain(Dlg, Dlv, BackColor, BorderWidth, 1, G);
            MovingY2(NewY);
            RePain(Dlg, Dlv, false, 1, G);
        }

        public new void Transform(float Dx, float Dy, float Dlg, float Dlv, Color BackColor, Corners Corner, Graphics G)
        {
            RePain(Dlg, Dlv, BackColor, BorderWidth, 1, G);
            Transformation(Dx, Dy, Dlg, Dlv, Corner);
            
            RePain(Dlg, Dlv, false, 1, G);
        }

        public new void RePain(float Dlg, float Dlv, bool Fill, Color BackCol, float k, Graphics G)
        {
            if (TypeShape == Nows.Ellip)
            {
                if (Fill) G.FillEllipse(new SolidBrush(BackCol), (Xn - Dlg) * k, (Yn - Dlv) * k, Width * k, Height * k);
                G.DrawEllipse(new Pen(BackCol, BorderWidth), (Xn - Dlg) * k, (Yn - Dlv) * k, Width * k, Height * k);
            }
            else
            {
                if (Fill) G.FillPie(new SolidBrush(BackCol), (Xn - Dlg) * k, (Yn - Dlv) * k, Width * k, Height * k, startAngle, sweepAngle);
                G.DrawPie(new Pen(BackCol, BorderWidth), (Xn - Dlg) * k, (Yn - Dlv) * k, Width * k, Height * k, startAngle, sweepAngle);
            }
        }

        public new void RePain(float Dlg, float Dlv, bool Fill, float k, Graphics G)
        {
            if (TypeShape == Nows.Ellip)
            {
                if (Fill) G.FillEllipse(new SolidBrush(Col), (Xn - Dlg) * k, (Yn - Dlv) * k, Width * k, Height * k);
                G.DrawEllipse(new Pen(BorderCol, BorderWidth), (Xn - Dlg) * k, (Yn - Dlv) * k, Width * k, Height * k);
            }
            else
            {
                if (Fill) G.FillPie(new SolidBrush(Col), (Xn - Dlg) * k, (Yn - Dlv ) * k, Width * k, Height * k, startAngle, sweepAngle);
                G.DrawPie(new Pen(BorderCol, BorderWidth), (Xn - Dlg) * k, (Yn - Dlv) * k, Width * k, Height * k, startAngle, sweepAngle);
            }
        }

        public new void RePain(float Dlg, float Dlv, Color Cl, float BorWd, float k, Graphics G)
        {
            if (TypeShape == Nows.Ellip)
                G.DrawEllipse(new Pen(Cl, BorWd), (Xn - Dlg) * k, (Yn - Dlv) * k, Width * k, Height * k);
            else
                G.DrawPie(new Pen(Cl, BorWd), (Xn - Dlg) * k, (Yn - Dlv) * k, Width * k, Height * k, startAngle, sweepAngle);
        }
    }
}
