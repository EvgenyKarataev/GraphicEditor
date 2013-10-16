using System.Collections;

namespace Graphic.Items
{
    public class AllPages : IEnumerable
    {
        readonly ArrayList aAllPages;

        public AllPages()
        {
            aAllPages = new ArrayList();
            Add(new OnPage());
        }
        // Метод для добавления объекта Line
        public void Add(OnPage Page)
        {
            Page.Items = new ItemsObj();
            aAllPages.Add(Page);
        }
        // Метод для удаления объекта Line
        public void Remove(int IndexToRemove) { aAllPages.RemoveAt(IndexToRemove); }
        // Свойство, возвращающее количество объектов Line
        public int Count { get { return aAllPages.Count; } }
        // Метод для очистки объекта - удаления всех объектов Line
        public void Clear() { aAllPages.Clear(); }
        // Метод, который отвечает на вопрос - есть ли уже в наборе такой объект Line
        //public bool IsPresent(sAllItems I) { return aAllItems.Contains(I); }

        public OnPage this[int index]
        {
            get { return (OnPage)aAllPages[index]; }
            set { aAllPages[index] = value; }
        }

        // А все что связано с реализацией lEnumerator просто перенаправляем в Lines
        public IEnumerator GetEnumerator() { return aAllPages.GetEnumerator(); }
    }
}
