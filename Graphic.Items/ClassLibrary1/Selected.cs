using System;
using System.Collections;
using System.Drawing;

namespace Graphic.Items
{
    public class Selected
    {
        private int _ZOrder;
        private float _X1;
        private float _X2;
        private float _Y1;
        private float _Y2;
        private readonly ArrayList _Item;
        private SelectedCorners _Corners;

        public Selected()
        {
            _ZOrder = 0;
            _Corners = new SelectedCorners();
            _Item = new ArrayList();
        }

        public void Add(Object O)
        {
            if (_Item == null || O == null) return;
            _Item.Add(O);
            ReSelect();
            Sort();
            Item It = (Item) _Item[_Item.Count - 1];
            _ZOrder = It.ZOrder;
        }

        // Метод для удаления объекта Line
        public void Remove(int IndexToRemove)
        {
            _Item.RemoveAt(IndexToRemove);
        }

        public void RemoveObj(Object Obj)
        {
            _Item.Remove(Obj);
        }
        // Свойство, возвращающее количество объектов Line
        public int Count { get { return _Item.Count; } }
        // Метод для очистки объекта - удаления всех объектов Line
        public void Clear()
        {
            _Item.Clear();
            _ZOrder = -1;
        }

        public bool ObjectIsPresent(Object O)
        {
            return _Item.Contains(O);
        }

        public void Sort()
        {
            IComparer ObjCom = new ObjComparer();

            _Item.Sort(ObjCom);
        }

        public Object this[int index]
        {
            get { return _Item[index]; }
            set { _Item[index] = value; }
        }

        // А все что связано с реализацией lEnumerator просто перенаправляем в Lines
        public IEnumerator GetEnumerator() { return _Item.GetEnumerator(); }

        public void ReSelect()
        {
            if (_Item.Count == 0) return;
            Item It = (Item)_Item[0];
            float Xmin = It.X1;
            float Xmax = It.X2;
            float Ymin = It.Y1;
            float Ymax = It.Y2;
            _ZOrder = 0;
            for (int i = 0; i < _Item.Count; i++)
            {
                It = (Item)_Item[i];
                Xmin = Xmin >= It.X1 ? It.X1 : Xmin;
                Xmin = Xmin >= It.X2 ? It.X2 : Xmin;
                Xmax = Xmax <= It.X1 ? It.X1 : Xmax;
                Xmax = Xmax <= It.X2 ? It.X2 : Xmax;

                Ymin = Ymin >= It.Y1 ? It.Y1 : Ymin;
                Ymin = Ymin >= It.Y2 ? It.Y2 : Ymin;
                Ymax = Ymax <= It.Y1 ? It.Y1 : Ymax;
                Ymax = Ymax <= It.Y2 ? It.Y2 : Ymax;

                _ZOrder = _ZOrder <= It.ZOrder ? It.ZOrder : _ZOrder;
            }
            _Corners.SetSelection(Xmin, Ymin, Xmax, Ymax);
            _X1 = Xmin;
            _X2 = Xmax;
            _Y1 = Ymin;
            _Y2 = Ymax;

            // _Coners.Show(It.G);
        }

        public int ZOrder
        {
            get { return _ZOrder; }
            set { _ZOrder = value; }
        }

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

        public SelectedCorners Corners
        {
            get { return _Corners; }
            set { _Corners = value; }
        }

        public bool ContainsPoint(float X, float Y, bool CtrlPressed)
        {
            //bool result = false;
            for (int i = _Item.Count - 1; i >= 0; i--)
            {
              //  if (result) return result;
                Item It = (Item) _Item[i];
                switch (It.TypeShape)
                {
                    case Nows.Line:
                        Line L = (Line) _Item[i];
                        if (L.ContainsPoint(X, Y))
                        {
                            if (CtrlPressed) 
                              _Item.RemoveAt(i);
                            return true;
                        }
                        break;
                    case Nows.Rec:
                        Rec R = (Rec) _Item[i];
                        if (R.ContainsPoint(X, Y))
                        {
                            if (CtrlPressed)
                                _Item.RemoveAt(i);
                            return true;
                        }
                        break;
                    case Nows.Ellip:
                    case Nows.Pie:
                        Pie P = (Pie) _Item[i];
                        if (P.ContainsPoint(X, Y))
                        {
                            if (CtrlPressed)
                                _Item.RemoveAt(i);
                            return true;
                        }
                        break;
                    case Nows.Group:
                        GroupItem GI = (GroupItem)_Item[i];
                        if (GI.ContainsPoint(X, Y))
                        {
                            if (CtrlPressed)
                                _Item.RemoveAt(i);
                            return true;
                        }
                        break;
                    default:
                        break;
                }
            }
            return false;
        }

        public void Move(float Dx, float Dy, float Dlg, float Dlv, Color BackColor, Graphics G)
        {
            for (int i = 0; i < _Item.Count; i++)
            {
                Item It = (Item) _Item[i];
                switch (It.TypeShape)
                {
                    case Nows.Line:
                        Line L = (Line) _Item[i];
                        L.Move(Dx, Dy, Dlg, Dlv, BackColor, G);
                        break;
                    case Nows.Rec:
                        Rec R = (Rec) _Item[i];
                        R.Move(Dx, Dy, Dlg, Dlv, BackColor, G);
                        break;
                    case Nows.Ellip:
                    case Nows.Pie:
                        Pie P = (Pie) _Item[i];
                        P.Move(Dx, Dy, Dlg, Dlv, BackColor, G);
                        break;
                    case Nows.Group:
                        GroupItem GI = (GroupItem)_Item[i];
                        GI.Move(Dx, Dy, Dlg, Dlv, BackColor, G);
                        break;
                }
            }
        }

        public void Transform(float Dx, float Dy, float Dlg, float Dlv, Color BackColor, Corners Corner, Graphics G)
        {
            for (int i = 0; i < _Item.Count; i++)
            {
                Item It = (Item) _Item[i];
                switch (It.TypeShape)
                {
                    case Nows.Line:
                        Line L = (Line) _Item[i];
                        L.Transform(Dx, Dy, Dlg, Dlv, BackColor, Corner, G);
                        break;
                    case Nows.Rec:
                        Rec R = (Rec) _Item[i];
                        R.Transform(Dx, Dy, Dlg, Dlv, BackColor, Corner, G);
                        break;
                    case Nows.Ellip:
                    case Nows.Pie:
                        Pie P = (Pie) _Item[i];
                        P.Transform(Dx, Dy, Dlg, Dlv, BackColor, Corner, G);
                        break;
                    case Nows.Group:
                        GroupItem GI = (GroupItem)_Item[i];
                        GI.Transform(Dx, Dy, Dlg, Dlv, BackColor, Corner, G);
                        break;
                }
            }
        }
    }
}
