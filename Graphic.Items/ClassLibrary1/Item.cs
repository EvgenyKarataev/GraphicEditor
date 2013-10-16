using System.Drawing;

namespace Graphic.Items
{
    public class Item
    {
        private Color _BorderCol;
        private Color _Col;
        private int _ZOrder;
        private float _BorderWidth;
        private Nows _TypeShape;
        private float _X1;
        private float _X2;
        private float _Y1;
        private float _Y2;
        private string _Name;
        
     //   private Graphics _G;

        public Color BorderCol
        {
            get { return _BorderCol; }
            set { _BorderCol = value; }
        }

        public Color Col
        {
            get { return _Col; }
            set { _Col = value; }
        }

        public int ZOrder
        {
            get { return _ZOrder; }
            set { _ZOrder = value; }
        }

        public float BorderWidth
        {
            get { return _BorderWidth; }
            set { _BorderWidth = value; }
        }

        public Nows TypeShape
        {
            get { return _TypeShape; }
            set { _TypeShape = value; }
        }

      /*  public Graphics G
        {
            get { return _G; }
            set { _G = value; }
        }*/

        public float X1
        {
            get { return _X1; }
            set { _X1 = value; }
        }
        public float X2
        {
            get { return _X2; }
            set { _X2 = value; }
        }
        public float Y1
        {
            get { return _Y1; }
            set { _Y1 = value; }
        }
        public float Y2
        {
            get { return _Y2; }
            set { _Y2 = value; }
        }

        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }
    }
}
