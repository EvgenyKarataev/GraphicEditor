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

        // Метод для удаления объекта Line
        public void Remove(int IndexToRemove) { _Buffer.RemoveAt(IndexToRemove); }
        // Свойство, возвращающее количество объектов Line
        public int Count { get { return _Buffer.Count; } }
        // Метод для очистки объекта - удаления всех объектов Line
        public void Clear() { _Buffer.Clear(); }
        // Метод, который отвечает на вопрос - есть ли уже в наборе такой объект Line
        public bool ObjectIsPresent(Object Obj) { return _Buffer.Contains(Obj); }

        public Object this[int index]
        {
            get { return _Buffer[index]; }
            set { _Buffer[index] = value; }
        }

        // А все что связано с реализацией lEnumerator просто перенаправляем в Lines
        public IEnumerator GetEnumerator() { return _Buffer.GetEnumerator(); }
    }
}
