using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Graphic.Items;

namespace GraphicEditor
{
    public partial class fmRecycle : Form
    {
        public fmRecycle()
        {
            k = 0.0f;
            kx = 0.0f;
            WidOut = 0.0f;
            ky = 0.0f;
            HeiOut = 0.0f;
            FillState = true;

            InitializeComponent();
        }

        public ArrayList ArOfIndexes = new ArrayList();
        private Recycle RecycleBin = new Recycle();
        private float k;
        private float kx;
        private float ky;
        private float WidOut;
        private float HeiOut;
        private bool FillState;

        public void SetRecycleBin(Recycle RecBin, int WidthOut, int HeightOut)
        {
            RecycleBin = RecBin;
            WidOut = WidthOut;
            kx = (float) pOut.Width / WidOut;
            HeiOut = HeightOut;
            ky = (float)pOut.Height / HeiOut;
            k = kx <= ky ? kx : ky;
        }

        private void fmRecycle_Shown(object sender, EventArgs e)
        {
            clbListOfItems.Items.Clear();
            tsslCount.Text = RecycleBin.Count.ToString();
            ItemOfRecycle ItOfRec;
            for (int i = 0; i < RecycleBin.Count; i++)
            {
                ItOfRec = (ItemOfRecycle) RecycleBin[i];
                clbListOfItems.Items.Add(ItOfRec.Name);
            }
        }

        private void bSelectAll_Click(object sender, EventArgs e)
        {
            for(int i = 0; i < clbListOfItems.Items.Count; i++)
            {
                clbListOfItems.SetItemChecked(i, true);
            }
        }

        private void bUnselectAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < clbListOfItems.Items.Count; i++)
            {
                clbListOfItems.SetItemChecked(i, false);
            }
        }

        private void bDelSelected_Click(object sender, EventArgs e)
        {
            for (int i = clbListOfItems.CheckedIndices.Count - 1; i >=0 ; i--)
            {
                RecycleBin.Remove(clbListOfItems.CheckedIndices[i]);
                clbListOfItems.Items.RemoveAt(clbListOfItems.CheckedIndices[i]);
            }
            pOut.Refresh();
        }

        private void bClearBin_Click(object sender, EventArgs e)
        {
            clbListOfItems.Items.Clear();
            RecycleBin.Clear();
        }

        private void bOk_Click(object sender, EventArgs e)
        {
            if (ArOfIndexes == null) return;
            ArOfIndexes.Clear();
            for (int i = 0; i < clbListOfItems.CheckedIndices.Count; i++)
            {
                ArOfIndexes.Add(clbListOfItems.CheckedIndices[i]);
            }
        }

        private void clbListOfItems_Click(object sender, EventArgs e)
        {
            
        }

        private void ShowSelected(ItemOfRecycle ItOfRec)
        {
            Item It = (Item)ItOfRec.Item;
          //  Graphics OldG = It.G;
            Graphics G = Graphics.FromHwnd(pOut.Handle);
            switch (It.TypeShape)
            {
                case Nows.Line:
                    Line L = (Line)ItOfRec.Item;
                    L.RePain(0, 0, k, G);               //.38
                    break;
                case Nows.Rec:
                    Rec R = (Rec)ItOfRec.Item;
                    R.RePain(0, 0, FillState, k, G);
                    break;
                case Nows.Ellip:
                case Nows.Pie:
                    Pie P = (Pie)ItOfRec.Item;
                    P.RePain(0, 0, FillState, k, G);
                    break;
                case Nows.Group:
                    GroupItem GI = (GroupItem)ItOfRec.Item;
                    GI.RePain(0, 0, FillState, k, G);
                    break;
            }
           // It.G = OldG;
        }

        private void clbListOfItems_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            
            if (e.CurrentValue == CheckState.Unchecked)
            {
                ItemOfRecycle ItOfRec = (ItemOfRecycle)RecycleBin[e.Index];
                ShowSelected(ItOfRec);
            }
            else
            {
                pOut.Refresh();

                for (int i = 0; i <= clbListOfItems.CheckedIndices.Count - 1; i++)
                {
                    if (clbListOfItems.CheckedIndices[i] == e.Index) continue;
                    ItemOfRecycle ItOfRec = (ItemOfRecycle)RecycleBin[clbListOfItems.CheckedIndices[i]];
                    ShowSelected(ItOfRec);
                }
            }
        }

        private void pOut_Resize(object sender, EventArgs e)
        {
            pOut.Refresh();
            kx = pOut.Width / WidOut;
            ky = pOut.Height / HeiOut;
            k = kx <= ky ? kx : ky;
            for (int i = 0; i <= clbListOfItems.CheckedIndices.Count - 1; i++)
            {
                ItemOfRecycle ItOfRec = (ItemOfRecycle)RecycleBin[clbListOfItems.CheckedIndices[i]];
                ShowSelected(ItOfRec);
            }
        }

        private void fmRecycle_Load(object sender, EventArgs e)
        {

        }

        private void cbFill_CheckedChanged(object sender, EventArgs e)
        {
            FillState = !FillState;
            pOut.Refresh();
            for (int i = 0; i <= clbListOfItems.CheckedIndices.Count - 1; i++)
            {
                ItemOfRecycle ItOfRec = (ItemOfRecycle)RecycleBin[clbListOfItems.CheckedIndices[i]];
                ShowSelected(ItOfRec);
            }
        }

        private void bSort_Click(object sender, EventArgs e)
        {
            
        }
    }
}