using System;
using System.Collections;

namespace Graphic.Items
{
    public class ItemsObj : IEnumerable
    {
        ArrayList _ItemsObj;
        int _LCount;
        int _RCount;
        int _PCount;
        int _GICount;

        public ItemsObj()
        {
            _PCount = 0;
            _RCount = 0;
            _LCount = 0;
            _GICount = 0;
            _ItemsObj = new ArrayList();
        }

        // Метод для добавления объекта Line
        public void Add(Line L) { _ItemsObj.Add(L); _LCount++; }
        public void Add(Rec R) { _ItemsObj.Add(R); _RCount++; }
        public void Add(Pie P) { _ItemsObj.Add(P); _PCount++;}
        public void Add(GroupItem GI) { _ItemsObj.Add(GI); _GICount++; }
        
        // Метод для удаления объекта Line
        public void RemoveObj(Object Obj) { _ItemsObj.Remove(Obj); }
        public void Remove(int IndexToRemove) { _ItemsObj.RemoveAt(IndexToRemove); }
        // Свойство, возвращающее количество объектов Line
        public int Count { get { return _ItemsObj.Count; } }
        // Метод для очистки объекта - удаления всех объектов Line
        public void Clear() { _ItemsObj.Clear(); }
        // Метод, который отвечает на вопрос - есть ли уже в наборе такой объект Line
        public bool ObjectIsPresent(Object O) { return _ItemsObj.Contains(O); }
        public void Sort()
        {
            IComparer ObjCom = new ObjComparer();

            _ItemsObj.Sort(ObjCom);
        }

        public int LCount
        {
            get {return _LCount;}
            set { _LCount = value; }
        }

        public int RCount
        {
            get {return _RCount;}
            set { _RCount = value; }
        }

        public int PCount
        {
            get {return _PCount;}
            set { _PCount = value; }
        }

        public int GICount
        {
            get { return _GICount; }
            set { _GICount = value; }
        }

        public Nows GetTypeOfObj(int Index)
        {
            Item Obj = (Item)_ItemsObj[Index];
            return Obj.TypeShape;
        }

        public Object this[int index]
        {
            get { return _ItemsObj[index]; }
            set { _ItemsObj[index] = value; }
        }

        // А все что связано с реализацией lEnumerator просто перенаправляем в Lines
        public IEnumerator GetEnumerator() { return _ItemsObj.GetEnumerator(); }
    }
}
