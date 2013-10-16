using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Graphic.Items
{
    public class Recycle: IEnumerable
    {
        ArrayList _Recycle;
        
        public Recycle()
        {
            
            _Recycle = new ArrayList();
        }

        // ����� ��� ���������� ������� Line
        public void Add(Object O) { _Recycle.Add(O); }
        // ����� ��� �������� ������� Line
        public void Remove(int IndexToRemove) { _Recycle.RemoveAt(IndexToRemove); }
        // ��������, ������������ ���������� �������� Line
        public int Count { get { return _Recycle.Count; } }
        // ����� ��� ������� ������� - �������� ���� �������� Line
        public void Clear() { _Recycle.Clear(); }
        // �����, ������� �������� �� ������ - ���� �� ��� � ������ ����� ������ Line
        public bool ObjectIsPresent(Object O) { return _Recycle.Contains(O); }
        public void Sort()
        {
            IComparer ObjCom = new ObjComparer();

            _Recycle.Sort(ObjCom);
        }

        public Object this[int index]
        {
            get { return _Recycle[index]; }
            set { _Recycle[index] = value; }
        }

        // � ��� ��� ������� � ����������� lEnumerator ������ �������������� � Lines
        public IEnumerator GetEnumerator() { return _Recycle.GetEnumerator(); }

    }
}
