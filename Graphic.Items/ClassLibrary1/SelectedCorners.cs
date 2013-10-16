using System.Drawing;
using System.Drawing.Drawing2D;


namespace Graphic.Items
{
    public class SelectedCorners
    {
        const int _Side = 10;
        const int _Standoff = 10;

        RectangleF _TopLeft;
        RectangleF _Left;
        RectangleF _BottomLeft;
        RectangleF _Bottom;
        RectangleF _BottomRight;
        RectangleF _Right;
        RectangleF _TopRight;
        RectangleF _Top;
        
        public SelectedCorners()
        {
            _TopLeft.Width = _TopLeft.Height = _Side;
            _Left.Width = _Left.Height = _Side;
            _BottomLeft.Width = _BottomLeft.Height = _Side;
            _Bottom.Width = _Bottom.Height = _Side;
            _BottomRight.Width = _BottomRight.Height = _Side;
            _Right.Width = _Right.Height = _Side;
            _TopRight.Width = _TopRight.Height = _Side;
            _Top.Width = _Top.Height = _Side;
        }

        public void SetSelection(float X1, float Y1, float X2, float Y2)
        {
            _TopLeft.X = X1 - _Standoff - _Side;
            _TopLeft.Y = Y1 - _Standoff - _Side;

            _Left.X = X1 - _Standoff - _Side;
// ReSharper disable PossibleLossOfFraction
            _Left.Y = Y1 + (Y2 - Y1) / 2 - _Side / 2;
// ReSharper restore PossibleLossOfFraction

            _BottomLeft.X = X1 - _Standoff - _Side;
            _BottomLeft.Y = Y2 + _Standoff;

            _Bottom.X = X1 + (X2 - X1) / 2 - _Side / 2;
            _Bottom.Y = Y2 + _Standoff;

            _BottomRight.X = X2 + _Standoff;
            _BottomRight.Y = Y2 + _Standoff;

            _Right.X = X2 + _Standoff;
            _Right.Y = Y1 + (Y2 - Y1) / 2 - _Side / 2;

            _TopRight.X = X2 + _Standoff;
            _TopRight.Y = Y1 - _Standoff - _Side;

            _Top.X = X1 + (X2 - X1) / 2 - _Side / 2;
            _Top.Y = Y1 - _Standoff - _Side;
        }

        public void Show(float Dlg, float Dlv, Graphics g)
        {
         //   g.CompositingMode CompositingMode = CopyPixelOperation.PatInvert;
            SolidBrush SB = new SolidBrush(Color.Black);
           // g.CompositingMode //g.CopyFromScreen();
            g.FillRectangle(SB, _TopLeft.X - Dlg, _TopLeft.Y - Dlv, _TopLeft.Width, _TopLeft.Height);
            g.FillRectangle(SB, _Left.X - Dlg, _Left.Y - Dlv, _Left.Width, _Left.Height);
            g.FillRectangle(SB, _BottomLeft.X - Dlg, _BottomLeft.Y - Dlv, _BottomLeft.Width, _BottomLeft.Height);
            g.FillRectangle(SB, _Bottom.X - Dlg, _Bottom.Y - Dlv, _Bottom.Width, _Bottom.Height);
            g.FillRectangle(SB, _BottomRight.X - Dlg, _BottomRight.Y - Dlv, _BottomRight.Width, _BottomRight.Height);
            g.FillRectangle(SB, _Right.X - Dlg, _Right.Y - Dlv, _Right.Width, _Right.Height);
            g.FillRectangle(SB, _TopRight.X - Dlg, _TopRight.Y - Dlv, _TopRight.Width, _TopRight.Height);
            g.FillRectangle(SB, _Top.X - Dlg, _Top.Y - Dlv, _Top.Width, _Top.Height);
        }

        public void Hide(float Dlg, float Dlv, Graphics g, Color Col)
        {
            SolidBrush SB = new SolidBrush(Col);
            g.FillRectangle(SB, _TopLeft.X - Dlg, _TopLeft.Y - Dlv, _TopLeft.Width, _TopLeft.Height);
            g.FillRectangle(SB, _Left.X - Dlg, _Left.Y - Dlv, _Left.Width, _Left.Height);
            g.FillRectangle(SB, _BottomLeft.X - Dlg, _BottomLeft.Y - Dlv, _BottomLeft.Width, _BottomLeft.Height);
            g.FillRectangle(SB, _Bottom.X - Dlg, _Bottom.Y - Dlv, _Bottom.Width, _Bottom.Height);
            g.FillRectangle(SB, _BottomRight.X - Dlg, _BottomRight.Y - Dlv, _BottomRight.Width, _BottomRight.Height);
            g.FillRectangle(SB, _Right.X - Dlg, _Right.Y - Dlv, _Right.Width, _Right.Height);
            g.FillRectangle(SB, _TopRight.X - Dlg, _TopRight.Y - Dlv, _TopRight.Width, _TopRight.Height);
            g.FillRectangle(SB, _Top.X - Dlg, _Top.Y - Dlv, _Top.Width, _Top.Height);
        }

        public Corners Contains(float X, float Y)
        {
            if (_TopLeft.Contains(X, Y)) return Corners.TopLeft;
            if (_Left.Contains(X, Y)) return Corners.Left;
            if (_BottomLeft.Contains(X, Y)) return Corners.BottomLeft;
            if (_Bottom.Contains(X, Y)) return Corners.Bottom;
            if (_BottomRight.Contains(X, Y)) return Corners.BottomRight;
            if (_Right.Contains(X, Y)) return Corners.Right;
            if (_TopRight.Contains(X, Y)) return Corners.TopRight;
            if (_Top.Contains(X, Y)) return Corners.Top;
            return Corners.None;
        }
    }
}
