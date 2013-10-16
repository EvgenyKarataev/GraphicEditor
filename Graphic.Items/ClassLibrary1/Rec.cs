using System.Drawing;

namespace Graphic.Items
{
    public class Rec : Item
    {
        RectangleF _Rec;

        public Rec(float Xn, float Yn, float Width, float Height, Color BorderCol, Color Col, int ZOrder, float BorderWidth, string Name)
        {
            _Rec.X = Xn;
            _Rec.Y = Yn;
            _Rec.Width = Width;
            _Rec.Height = Height;
            this.BorderCol = BorderCol;
            this.Col = Col;
            this.ZOrder = ZOrder;
            this.BorderWidth = BorderWidth;
// ReSharper disable RedundantThisQualifier
            this.TypeShape = Nows.Rec;
// ReSharper restore RedundantThisQualifier
            this.Name = Name;
          //  this.G = G;
            DoXY();
        }

        public Rec(Rec R)
        {
            _Rec.X = R.Xn;
            _Rec.Y = R.Yn;
            _Rec.Width = R.Width;
            _Rec.Height = R.Height;
            BorderCol = R.BorderCol;
            Col = R.Col;
            ZOrder = R.ZOrder;
            BorderWidth = R.BorderWidth;
            TypeShape = Nows.Rec;
            Name = R.Name;
         //   G = R.G;
            DoXY();
        }

        public void DoXY()
        {
            X1 = _Rec.X;
            Y1 = _Rec.Y;
            X2 = _Rec.X + _Rec.Width;
            Y2 = _Rec.Y + _Rec.Height;
        }

        public float Xn
        {
            get { return _Rec.X; }
            set
            {
                _Rec.X = value;
                X1 = _Rec.X;
                X2 = _Rec.X + _Rec.Width;
            }
        }

        public float Yn
        {
            get { return _Rec.Y; }
            set
            {
                _Rec.Y = value;
                Y1 = _Rec.Y;
                Y2 = _Rec.Y + _Rec.Height;
            }
        }

        public float Xk
        {
            get { return _Rec.X + _Rec.Width; }
            set { _Rec.Width = value - _Rec.X; }
            //_Rec.Width = Math.Abs(value - _Rec.X); }
        }

        public float Yk
        {
            get { return _Rec.Y + _Rec.Height; }
            set { _Rec.Height = value - _Rec.Y; }
            //_Rec.Height = Math.Abs(value - _Rec.Y); }
        }

        public float Width
        {
            get { return _Rec.Width; }
            set { _Rec.Width = value; X2 = _Rec.X + _Rec.Width; }
        }

        public float Height
        {
            get { return _Rec.Height; }
            set { _Rec.Height = value; Y2 = _Rec.Y + _Rec.Height; }
        }

        public RectangleF Rectan
        {
            get { return _Rec; }
            set { _Rec = value; }
        }

        public bool ContainsPoint(float X, float Y)
        {
            return _Rec.Contains(X, Y);
        }

        public void Move(float Dx, float Dy, float Dlg, float Dlv, Color BackColor, Graphics G)
        {
            RePain(Dlg, Dlv, BackColor, BorderWidth, 1, G);
            _Rec.X += Dx;
            _Rec.Y += Dy;
            DoXY();
            RePain(Dlg, Dlv, false, 1, G);
        }

        public void MovingX1(float NewX)
        {
            _Rec.X += NewX - _Rec.X;
            _Rec.Width -= NewX - _Rec.X;
            DoXY();
        }

        public void MoveX1(float NewX, float Dlg, float Dlv, Color BackColor, Graphics G)
        {
            RePain(Dlg, Dlv, BackColor, BorderWidth, 1, G);
            MovingX1(NewX);
            RePain(Dlg, Dlv, false, 1, G);
        }

        public void MovingY1(float NewY)
        {
            _Rec.Y += NewY - _Rec.Y;
            _Rec.Height -= NewY - _Rec.Y;
            DoXY();
        }

        public void MoveY1(float NewY, float Dlg, float Dlv, Color BackColor, Graphics G)
        {
            RePain(Dlg, Dlv, BackColor, BorderWidth, 1, G);
            MovingY1(NewY);
            RePain(Dlg, Dlv, false, 1, G);
        }

        public void MovingX2(float NewX)
        {
            _Rec.Width += NewX - _Rec.Width;
            DoXY();
        }

        public void MoveX2(float NewX, float Dlg, float Dlv, Color BackColor, Graphics G)
        {
            RePain(Dlg, Dlv, BackColor, BorderWidth, 1, G);
            MovingX2(NewX);
            RePain(Dlg, Dlv, false, 1, G);
        }

        public void MovingY2(float NewY)
        {
            _Rec.Height += NewY - _Rec.Height;
            DoXY();
        }

        public void MoveY2(float NewY, float Dlg, float Dlv, Color BackColor, Graphics G)
        {
            RePain(Dlg, Dlv, BackColor, BorderWidth, 1, G);
            MovingY2(NewY);
            RePain(Dlg, Dlv, false, 1, G);
        }

        public void Transformation(float Dx, float Dy, float Dlg, float Dlv, Corners Corner)
        {
            switch (Corner)
            {
                case Corners.TopLeft:
                    _Rec.X += Dx;
                    _Rec.Width -= Dx;
                    _Rec.Y += Dy;
                    _Rec.Height -= Dy;
                    break;
                case Corners.BottomRight:
                    _Rec.Width += Dx;
                    _Rec.Height += Dy;
                    break;
                case Corners.TopRight:
                    _Rec.Width += Dx;
                    _Rec.Y += Dy;
                    _Rec.Height -= Dy;
                    break;
                case Corners.BottomLeft:
                    _Rec.X += Dx;
                    _Rec.Width -= Dx;
                    _Rec.Height += Dy;
                    break;
                case Corners.Top:
                    _Rec.Y += Dy;
                    _Rec.Height -= Dy;
                    break;
                case Corners.Bottom:
                    _Rec.Height += Dy;
                    break;
                case Corners.Left:
                    _Rec.X += Dx;
                    _Rec.Width -= Dx;
                    break;
                case Corners.Right:
                    _Rec.Width += Dx;
                    break;
            }
            DoXY();
        }

        public void Transform(float Dx, float Dy, float Dlg, float Dlv, Color BackColor, Corners Corner, Graphics G)
        {
            RePain(Dlg, Dlv, BackColor, BorderWidth, 1, G);
            Transformation(Dx, Dy, Dlg, Dlv, Corner);
           
            RePain(Dlg, Dlv, false, 1, G);
        }

       public void RePain(float Dlg, float Dlv, bool Fill, float k, Graphics G)
        {
            if (Fill) G.FillRectangle(new SolidBrush(Col), (_Rec.X - Dlg) * k, (_Rec.Y - Dlv) * k, _Rec.Width * k, _Rec.Height * k);
            G.DrawRectangle(new Pen(BorderCol, BorderWidth), (_Rec.X - Dlg) * k, (_Rec.Y - Dlv) * k, _Rec.Width * k, _Rec.Height * k);
        }

        public void RePain(float Dlg, float Dlv, Color Cl, float BorWd, float k, Graphics G)
        {
            G.DrawRectangle(new Pen(Cl, BorWd), (_Rec.X - Dlg) * k, (_Rec.Y - Dlv) * k, _Rec.Width * k, _Rec.Height * k);
        }
    }
}
