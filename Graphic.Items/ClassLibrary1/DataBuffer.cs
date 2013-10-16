using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Graphic.Items
{
    public class DataBuffer
    {
        ArrayList _Buffer;

        public DataBuffer()
        {
            _Buffer = new ArrayList();
        }

        public void Add(Object Obj) { _Buffer.Add(Obj); }

        // ����� ��� �������� ������� Line
        public void Remove(int IndexToRemove) { _Buffer.RemoveAt(IndexToRemove); }
        // ��������, ������������ ���������� �������� Line
        public int Count { get { return _Buffer.Count; } }
        // ����� ��� ������� ������� - �������� ���� �������� Line
        public void Clear() { _Buffer.Clear(); }
        // �����, ������� �������� �� ������ - ���� �� ��� � ������ ����� ������ Line
        public bool ObjectIsPresent(Object Obj) { return _Buffer.Contains(Obj); }

        public Object this[int index]
        {
            get { return _Buffer[index]; }
            set { _Buffer[index] = value; }
        }

        // � ��� ��� ������� � ����������� lEnumerator ������ �������������� � Lines
        public IEnumerator GetEnumerator() { return _Buffer.GetEnumerator(); }
    }
}
