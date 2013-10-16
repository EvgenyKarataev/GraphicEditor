using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Graphic.Items
{
    public class GroupItem : Item 
    {
        private readonly ArrayList _GroupItem;
        
        public GroupItem()
        {
            _GroupItem = new ArrayList();
            TypeShape = Nows.Group;
        }

        public GroupItem(GroupItem GI)
        {
            _GroupItem = new ArrayList();
            TypeShape = Nows.Group;
            Item It;
            for (int i = 0; i < GI.Count; i++)
            {
                It = (Item) GI[i];
                switch (It.TypeShape)
                {
                    case Nows.Line:
                        Line L = (Line) GI[i];
                        _GroupItem.Add(new Line(L));
                        break;
                    case Nows.Rec:
                        Rec R = (Rec)GI[i];
                        _GroupItem.Add(new Rec(R));
                        break;
                    case Nows.Ellip:
                    case Nows.Pie:
                        Pie P = (Pie)GI[i];
                        _GroupItem.Add(new Pie(P));
                        break;
                    case Nows.Group:
                        GroupItem Gr = (GroupItem)GI[i];
                        _GroupItem.Add(new GroupItem(Gr));
                        break;
                }
            }
            X1 = GI.X1;
            X2 = GI.X2;
            Y1 = GI.Y1;
            Y2 = GI.Y2;
            Name = GI.Name;
            ZOrder = GI.ZOrder;
        }

        public void Add(Object Obj)
        {
            if (_GroupItem == null || Obj == null) return;
            _GroupItem.Add(Obj);
            Item It = (Item) Obj;
            if (_GroupItem.Count == 1)
            {
                X1 = It.X1;
                X2 = It.X2;
                Y1 = It.Y1;
                Y2 = It.Y2;
            }
            else
            {
                X1 = Math.Min(X1, It.X1);
                X2 = Math.Max(X2, It.X2);
                Y1 = Math.Min(Y1, It.Y1);
                Y2 = Math.Max(Y2, It.Y2);
            }
            Sort();
            It = (Item) _GroupItem[0];
            ZOrder = It.ZOrder;
        }

        // Метод для удаления объекта Line
        public void Remove(int IndexToRemove)
        {
            _GroupItem.RemoveAt(IndexToRemove);
        }

        public void RemoveObj(Object Obj)
        {
            _GroupItem.Remove(Obj);
        }
        // Свойство, возвращающее количество объектов Line
        public int Count { get { return _GroupItem.Count; } }
        // Метод для очистки объекта - удаления всех объектов Line
        public void Clear()
        {
            _GroupItem.Clear();
            ZOrder = -1;
        }

        public bool ObjectIsPresent(Object O)
        {
            return _GroupItem.Contains(O);
        }

        public void Sort()
        {
            IComparer ObjCom = new ObjComparer();

            _GroupItem.Sort(ObjCom);
        }

        public Object this[int index]
        {
            get { return _GroupItem[index]; }
            set { _GroupItem[index] = value; }
        }

        // А все что связано с реализацией lEnumerator просто перенаправляем в Lines
        public IEnumerator GetEnumerator() { return _GroupItem.GetEnumerator(); }

        public bool ContainsPoint(float X, float Y)
        {
            for (int i = _GroupItem.Count - 1; i >= 0; i--)
            {
                Item It = (Item) _GroupItem[i];
                switch (It.TypeShape)
                {
                    case Nows.Line:
                        Line L = (Line) _GroupItem[i];
                        if (L.ContainsPoint(X, Y))
                            return true;
                        break;
                    case Nows.Rec:
                        Rec R = (Rec) _GroupItem[i];
                        if (R.ContainsPoint(X, Y))
                            return true;
                        break;
                    case Nows.Ellip:
                    case Nows.Pie:
                        Pie P = (Pie) _GroupItem[i];
                        if (P.ContainsPoint(X, Y))
                            return true;
                        break;
                    case Nows.Group:
                        GroupItem GI = (GroupItem)_GroupItem[i];
                        if (GI.ContainsPoint(X, Y))
                            return true;
                        break;
                    default:
                        break;
                }
            }
            return false;
        }

        public void Move(float Dx, float Dy, float Dlg, float Dlv, Color BackColor, Graphics G)
        {
            for (int i = 0; i < _GroupItem.Count; i++)
            {
                Item It = (Item) _GroupItem[i];
                switch (It.TypeShape)
                {
                    case Nows.Line:
                        Line L = (Line) _GroupItem[i];
                        L.Move(Dx, Dy, Dlg, Dlv, BackColor, G);
                        break;
                    case Nows.Rec:
                        Rec R = (Rec) _GroupItem[i];
                        R.Move(Dx, Dy, Dlg, Dlv, BackColor, G);
                        break;
                    case Nows.Ellip:
                    case Nows.Pie:
                        Pie P = (Pie) _GroupItem[i];
                        P.Move(Dx, Dy, Dlg, Dlv, BackColor, G);
                        break;
                    case Nows.Group:
                        GroupItem GI = (GroupItem)_GroupItem[i];
                        GI.Move(Dx, Dy, Dlg, Dlv, BackColor, G);
                        break;
                }
            }
            DoXY();
        }

        private void DoXY()
        {
            Item It = (Item) _GroupItem[0];
            X1 = It.X1;
            X2 = It.X2;
            Y1 = It.Y1;
            Y2 = It.Y2;
            for (int i = 1; i < _GroupItem.Count; i++)
            {
                It = (Item) _GroupItem[i];
                X1 = Math.Min(X1, It.X1);
                X2 = Math.Max(X2, It.X2);
                Y1 = Math.Min(Y1, It.Y1);
                Y2 = Math.Max(Y2, It.Y2);
            }
        }

        public void Transform(float Dx, float Dy, float Dlg, float Dlv, Color BackColor, Corners Corner, Graphics G)
        {
            for (int i = 0; i < _GroupItem.Count; i++)
            {
                Item It = (Item) _GroupItem[i];
                switch (It.TypeShape)
                {
                    case Nows.Line:
                        Line L = (Line) _GroupItem[i];
                        L.Transform(Dx, Dy, Dlg, Dlv, BackColor, Corner, G);
                        break;
                    case Nows.Rec:
                        Rec R = (Rec) _GroupItem[i];
                        R.Transform(Dx, Dy, Dlg, Dlv, BackColor, Corner, G);
                        break;
                    case Nows.Ellip:
                    case Nows.Pie:
                        Pie P = (Pie) _GroupItem[i];
                        P.Transform(Dx, Dy, Dlg, Dlv, BackColor, Corner, G);
                        break;
                    case Nows.Group:
                        GroupItem GI = (GroupItem)_GroupItem[i];
                        GI.Transform(Dx, Dy, Dlg, Dlv, BackColor, Corner, G);
                        break;
                }
            }
            DoXY();
        }

        public void RePain(float Dlg, float Dlv, bool Fill, float k, Graphics G)
        {
            Line L;
            Rec R;
            Pie P;
            GroupItem GI;
            Item It;
            for (int i = 0; i < _GroupItem.Count; i++)
            {
                It = (Item) _GroupItem[i];
                switch (It.TypeShape)
                {
                    case Nows.Line:
                        L = (Line)_GroupItem[i];
                        L.RePain(Dlg, Dlv, k, G);
                        break;
                    case Nows.Rec:
                        R = (Rec)_GroupItem[i];
                        R.RePain(Dlg, Dlv, Fill, k, G);
                        break;
                    case Nows.Ellip:
                    case Nows.Pie:
                        P = (Pie)_GroupItem[i];
                        P.RePain(Dlg, Dlv, Fill, k, G);
                        break;
                    case Nows.Group:
                        GI = (GroupItem)_GroupItem[i];
                        GI.RePain(Dlg, Dlv, Fill, k, G);
                        break;
                }
            }
        }
    }
}
