using System;
using System.Drawing;

namespace Graphic.Items
{
    public class OnPage
    {
        ItemsObj _Items;
        Selected _Selected;
        Graphics _G;

        private int _LC;//
        private int _RC;//кол-во увелич при вставке объекта, но это не реальное их кол-во
        private int _PC;//
        private int _GC;//
        private int _ZOrdCurrent; //приоритет прорисовки ZOrder для последней нарисованной фигуры
        private int _ZOrderBeforeDoPos;//позиция выделенной фигуры до изменения позиции
        private bool _DoPos; //когда меняем позицию фигуры

        public OnPage()
        {
            _Selected = new Selected();
            _LC = 1;
            _RC = 1;
            _PC = 1;
            _GC = 1;
            _ZOrdCurrent = 0;
            _ZOrderBeforeDoPos = 0;
            _DoPos = false;
        }

        public ItemsObj Items
        {
            get { return _Items; }
            set { _Items = value; }
        }

        public Object FindShape(float X, float Y)
        {
            for (int i = _Items.Count - 1; i >= 0; i--)
            {
                switch (_Items.GetTypeOfObj(i))
                {
                    case Nows.Line:
                        Line L = (Line)_Items[i];
                        if (L.ContainsPoint(X, Y))
                          return _Items[i];
                        break;
                    case Nows.Rec:
                        Rec R = (Rec)_Items[i];
                        if (R.ContainsPoint(X, Y))
                            return _Items[i];
                        break;
                    case Nows.Ellip:
                    case Nows.Pie:
                        Pie P = (Pie)_Items[i];
                        if (P.ContainsPoint(X, Y))
                            return _Items[i];
                        break;
                    case Nows.Group:
                        GroupItem GI = (GroupItem)_Items[i];
                        if (GI.ContainsPoint(X, Y))
                            return _Items[i];
                        break;
                }
            }
            return null;
        }

        public bool Select(float X, float Y)
        {
            Object It = FindShape(X, Y);
            if (It == null) return false;
            _Selected.Add(It);
            return true;
        }

        public Selected Selected
        {
            get { return _Selected; }
            set { _Selected = value; }
        }

        public Graphics G
        {
            get { return _G; }
            set { _G = value; }
        }

        public int LC
        {
            get { return _LC; }
            set { _LC = value; }
        }

        public int RC
        {
            get { return _RC; }
            set { _RC = value; }
        }

        public int PC
        {
            get { return _PC; }
            set { _PC = value; }
        }

        public int GC
        {
            get { return _GC; }
            set { _GC = value; }
        }

        public int ZOrdCurrent
        {
            get { return _ZOrdCurrent; }
            set { _ZOrdCurrent = value; }
        }

        public int ZOrderBeforeDoPos
        {
            get { return _ZOrderBeforeDoPos; }
            set { _ZOrderBeforeDoPos = value; }
        }
        
        public bool DoPos
        {
            get { return _DoPos; }
            set { _DoPos = value; }
        }
    }
}
