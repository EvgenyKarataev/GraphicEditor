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
        // ����� ��� ���������� ������� Line
        public void Add(OnPage Page)
        {
            Page.Items = new ItemsObj();
            aAllPages.Add(Page);
        }
        // ����� ��� �������� ������� Line
        public void Remove(int IndexToRemove) { aAllPages.RemoveAt(IndexToRemove); }
        // ��������, ������������ ���������� �������� Line
        public int Count { get { return aAllPages.Count; } }
        // ����� ��� ������� ������� - �������� ���� �������� Line
        public void Clear() { aAllPages.Clear(); }
        // �����, ������� �������� �� ������ - ���� �� ��� � ������ ����� ������ Line
        //public bool IsPresent(sAllItems I) { return aAllItems.Contains(I); }

        public OnPage this[int index]
        {
            get { return (OnPage)aAllPages[index]; }
            set { aAllPages[index] = value; }
        }

        // � ��� ��� ������� � ����������� lEnumerator ������ �������������� � Lines
        public IEnumerator GetEnumerator() { return aAllPages.GetEnumerator(); }
    }
}
