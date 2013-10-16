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

        // Метод для добавления объекта Line
        public void Add(Object O) { _Recycle.Add(O); }
        // Метод для удаления объекта Line
        public void Remove(int IndexToRemove) { _Recycle.RemoveAt(IndexToRemove); }
        // Свойство, возвращающее количество объектов Line
        public int Count { get { return _Recycle.Count; } }
        // Метод для очистки объекта - удаления всех объектов Line
        public void Clear() { _Recycle.Clear(); }
        // Метод, который отвечает на вопрос - есть ли уже в наборе такой объект Line
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

        // А все что связано с реализацией lEnumerator просто перенаправляем в Lines
        public IEnumerator GetEnumerator() { return _Recycle.GetEnumerator(); }

    }
}
