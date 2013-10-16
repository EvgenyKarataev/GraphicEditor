using System;
using System.Collections.Generic;
using System.Text;

namespace Graphic.Items
{
    public class Buffer
    {
        Object _Item;

        public Buffer()
        {
            _Item = new Object();
        }

        public void CutToBuffer(Object Obj)
        {
            _Item = Obj;

        }
    }
}
