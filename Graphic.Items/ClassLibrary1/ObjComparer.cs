using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

namespace Graphic.Items
{
    public class ObjComparer : IComparer
    {
        // Calls CaseInsensitiveComparer.Compare with the parameters reversed.
        int IComparer.Compare(Object x, Object y)
        {
            Item It1 = (Item) x;
            Item It2 = (Item) y;
            return ((new CaseInsensitiveComparer()).Compare(It1.ZOrder, It2.ZOrder));
        }
    }
}
