using System;
using System.Drawing;

namespace Graphic.Items
{
    public class Line : Item
    {
        float _Xn;
        float _Yn;
        float _Xk;
        float _Yk;

        public Line(float Xn, float Yn, float Xk, float Yk, Color BorderCol, Color Col, int ZOrder, float BorderWidth, string Name)
        {
            _Xn = Xn;
            _Yn = Yn;
            _Xk = Xk;
            _Yk = Yk;
            this.BorderCol = BorderCol;
            this.Col = Col;
            this.ZOrder = ZOrder;
            this.BorderWidth = BorderWidth;
// ReSharper disable RedundantThisQualifier
            this.TypeShape = Nows.Line;
// ReSharper restore RedundantThisQualifier
            this.Name = Name;
      //      this.G = G;

            DoXY();
        }

        public Line(Line L)
        {
            _Xn = L.Xn;
            _Yn = L.Yn;
            _Xk = L.Xk;
            _Yk = L.Yk;
            BorderCol = L.BorderCol;
            Col = L.Col;
            ZOrder = L.ZOrder;
            BorderWidth = L.BorderWidth;
            TypeShape = Nows.Line;
            Name = L.Name;
         //   G = L.G;

            DoXY();
        }

        void DoXY()
        {
            if (_Xk > _Xn)
            {
                X1 = _Xn;
                X2 = _Xk;
            }
            else
            {
                X1 = _Xk;
                X2 = _Xn;
            }

            if (_Yk > _Yn)
            {
                Y1 = _Yn;
                Y2 = _Yk;
            }
            else
            {
                Y1 = _Yk;
                Y2 = _Yn;
            }
        }

        public float Xn
        {
            get { return _Xn; }
            set { _Xn = value; }
        }

        public float Yn
        {
            get { return _Yn; }
            set { _Yn = value; }
        }

        public float Xk
        {
            get { return _Xk; }
            set { _Xk = value; }
        }

        public float Yk
        {
            get { return _Yk; }
            set { _Yk = value; }
        }

        public bool ContainsPoint(float X, float Y)
        {
            if (X < X1 || X > X2) return false;
            if (Y < Y1 || Y > Y2) return false;
            return Math.Abs(Math.Abs((X - Xn)/(Xk - Xn)) - Math.Abs((Y - Yn)/(Yk - Yn))) <= 0.03;
        }

        public void Move(float Dx, float Dy, float Dlg, float Dlv, Color BackColor, Graphics G)
        {
            RePain(Dlg, Dlv, BackColor, BorderWidth, 1, G);
            _Xn += Dx;
            _Xk += Dx;
            _Yn += Dy;
            _Yk += Dy;
            DoXY();
            RePain(Dlg, Dlv, BorderCol, 1, 1, G);
        }

        public void MoveX1(float NewX, float Dlg, float Dlv, Color BackColor, Graphics G)
        {
            RePain(Dlg, Dlv, BackColor, BorderWidth, 1, G);
            if (_Xn <= _Xk)
            {
                _Xk += NewX - _Xn;
                _Xn += NewX - _Xn;
                
            }
            else
            {
                _Xn += NewX - _Xk;
                _Xk += NewX - _Xk;
                
            }
            DoXY();
            RePain(Dlg, Dlv, BorderCol, BorderWidth, 1, G);
        }

        public void MoveY1(float NewY, float Dlg, float Dlv, Color BackColor, Graphics G)
        {
            RePain(Dlg, Dlv, BackColor, BorderWidth, 1, G);
            if (_Xn <= _Xk)
            {
                _Yk += NewY - _Yn;
                _Yn += NewY - _Yn;
            }
            else
            {
                _Yn += NewY - _Yk;
                _Yk += NewY - _Yk;
            }
            DoXY();
            RePain(Dlg, Dlv, BorderCol, BorderWidth, 1, G);
        }

        public void MoveX2(float NewX, float Dlg, float Dlv, Color BackColor, Graphics G)
        {
            RePain(Dlg, Dlv, BackColor, BorderWidth, 1, G);
            if (_Xn >= _Xk)
                _Xn += NewX - _Xn;
            else
                _Xk += NewX - _Xk;
            DoXY();
            RePain(Dlg, Dlv, BorderCol, BorderWidth, 1, G);
        }

        public void MoveY2(float NewY, float Dlg, float Dlv, Color BackColor, Graphics G)
        {
            RePain(Dlg, Dlv, BackColor, BorderWidth, 1, G);
            if (_Xn >= _Xk)
                _Yn += NewY - _Yn;
            else
                _Yk += NewY - _Yk;
            DoXY();
            RePain(Dlg, Dlv, BorderCol, BorderWidth, 1, G);
        }

        public void Transform(float Dx, float Dy, float Dlg, float Dlv, Color BackColor, Corners Corner, Graphics G)
        {
            RePain(Dlg, Dlv, BackColor, BorderWidth, 1, G);
            switch (Corner)
            {
                case Corners.TopLeft:
                    _Xn += Dx;
                    _Yn += Dy;
                    break;
                case Corners.BottomRight:
                    _Xk += Dx;
                    _Yk += Dy; 
                    break;
                case Corners.TopRight:
                    _Xk += Dx;
                    _Yn += Dy;
                    break;
                case Corners.BottomLeft:
                    _Xn += Dx;
                    _Yk += Dy;
                    break;
                case Corners.Top:
                    if (_Yn < _Yk)
                        _Yn += Dy;
                    else
                        _Yk += Dy;
                    break;
                case Corners.Bottom:
                    _Yk += Dy;
                    break;
            }
            DoXY();
            RePain(Dlg, Dlv, BorderCol, 1, 1, G);
        }

        public void RePain(float Dlg, float Dlv, float k, Graphics G)
        {
            G.DrawLine(new Pen(BorderCol, BorderWidth), (_Xn - Dlg) * k, (_Yn - Dlv) * k, (_Xk - Dlg) * k, (_Yk - Dlv) * k);
        }

        public void RePain(float Dlg, float Dlv, Color Cl, float BorWd, float k, Graphics G)
        {
            G.DrawLine(new Pen(Cl, BorWd), (_Xn - Dlg) * k, (_Yn - Dlv) * k, (_Xk - Dlg) * k, (_Yk - Dlv) * k);
        }
    }
}
