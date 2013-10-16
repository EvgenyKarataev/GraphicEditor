using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.IO;
using Graphic.Items;

namespace GraphicEditor
{

    public partial class fmMain : Form
    {
        public fmMain()
        {
            HeigFm = 0;
            WidFm = 0;
            OldPageIndex = 0;

            Refreshed = false;
            down = false;

            ELast = new PointF();
            PprLast = new PointF();
            PnOut = new PointF();
            Pn = new PointF();
            CurrentPage = Album[0];

            Saved.Modificated = false;
            Saved.Name = "";
            InitializeComponent();
        }

        readonly fmRecycle FmRec = new fmRecycle();
        fmAbout FmAbout = new fmAbout();
        public Recycle RecycleBin = new Recycle();
        // ReSharper disable FieldCanBeMadeReadOnly
        AllPages Album = new AllPages();

        // ReSharper restore FieldCanBeMadeReadOnly
        OnPage CurrentPage;
        SavedResult Saved;
        DataBuffer Buffer = new DataBuffer();

        PointF Pn;//содержит начальную точкв рисования при Down мыши
        PointF PnOut;//содержит начальную точку рисования при Up мыши
        PointF PprLast;//содержит начальную точку для перерисовки фигуры в фоновый цвет
        PointF ELast; // содержит конечную предыдущую точку для линий
        bool down;//если начали рисовать
        bool Refreshed;
        private bool CtrlPressed = false;

        Color ItCol;//цвет фигуры и цвет контура фигуры
        Color BorderCol;//цвет фигуры и цвет контура фигуры

        // private Graphics g;

        private float startAng = 0.0f;
        private float sweepAng = -270.0f;

        private int Dlg = 0;
        private int Dlv = 0;
        int OldPageIndex;
        int CountForNames = 1;//номер страницы при создании новой
        int WidFm;//для скролов в Табах
        int HeigFm;//для скролов в Табах

        private Nows Now;//что чейчас рисовать
        Corners Corner = Corners.None;
        float LinesWidth = 1.0f;//толщина линий (границ фигур)

        private void tsmExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        void Select(float X, float Y, bool CtrlPressed)
        {
            Graphics g = Graphics.FromHwnd(tcTabs.TabPages[tcTabs.SelectedIndex].Handle);

            if ((Corner = CurrentPage.Selected.Corners.Contains(X, Y)) != Corners.None)
                return;

            Object Shape = CurrentPage.FindShape(X, Y);
            if (CurrentPage.Selected.Count > 0)
                CurrentPage.Selected.Corners.Hide(Dlg, Dlv, g, tcTabs.TabPages[tcTabs.SelectedIndex].BackColor);
            if (Shape == null)
            {
                if ((Corner = CurrentPage.Selected.Corners.Contains(X, Y)) == Corners.None)
                {

                    CurrentPage.Selected.Clear();
                    tcTabs.TabPages[tcTabs.SelectedIndex].Cursor = Now == Nows.None ? Cursors.Default : Cursors.Cross;
                    CurrentPage.DoPos = false;
                    tscbAlls.SelectedIndex = -1;
                }
                return;
            }
            //если фигура есть среди выделенных    
            if (CurrentPage.Selected.ObjectIsPresent(Shape))
            {
                if (CtrlPressed) //если нажат Ctrl, то надо эту фигуру удалить из выделенных
                    CurrentPage.Selected.RemoveObj(Shape);
                //else //если Ctrl не нажат
            }
            else if (CtrlPressed) //если фигуры нет среди выделенных и если нажат Ctrl то надо добавить эту фигуру
            {
                CurrentPage.Selected.Add(Shape);
                tscbAlls.SelectedIndex = -1;
            }
            else //если Ctrl не нажат, то надо выделить эту фигуру и еще проверить может просто меняется позиция фигур.
            {
                CurrentPage.Selected.Clear();
                CurrentPage.Selected.Add(Shape);
                if (CurrentPage.DoPos)//когда меняем позицию относительно других фигур
                    DoPositionAt(tcTabs.TabPages[tcTabs.SelectedIndex].Cursor);
                else
                    tscbAlls.SelectedIndex = CurrentPage.Selected.ZOrder;
            }
            CurrentPage.Selected.ReSelect();
            tcTabs.TabPages[tcTabs.SelectedIndex].Cursor = Cursors.SizeAll;
            Corner = Corners.None;
            if (CurrentPage.Selected.Count == 1)
                tscbAlls.SelectedIndex = CurrentPage.Selected.ZOrder;
        }

        private void pOut_MouseDown(object sender, MouseEventArgs e)
        {
            down = true;
            Pn.X = e.X; Pn.Y = e.Y;
            PprLast.X = e.X; PprLast.Y = e.Y;
            ELast.X = e.X; ELast.Y = e.Y;

            Refreshed = false;

            if (Now != Nows.Pie)
                PieButtons(false);
            if (Now == Nows.None || CurrentPage.DoPos)
            {
                Select(e.X + Dlg, e.Y + Dlv, CtrlPressed);
                CheckEnables();
            }

        }

        private void RePainAllItems(bool Fill)
        {
            Graphics g = Graphics.FromHwnd(tcTabs.TabPages[tcTabs.SelectedIndex].Handle);

            Line L;
            Rec R;
            Pie P;
            GroupItem GI;
            for (int i = 0; i < CurrentPage.Items.Count; i++)
            {
                switch (CurrentPage.Items.GetTypeOfObj(i))
                {
                    case Nows.Line:
                        L = (Line)CurrentPage.Items[i];
                        L.RePain(Dlg, Dlv, 1, g);
                        break;
                    case Nows.Rec:
                        R = (Rec)CurrentPage.Items[i];
                        R.RePain(Dlg, Dlv, Fill, 1, g);
                        break;
                    case Nows.Ellip:
                    case Nows.Pie:
                        P = (Pie)CurrentPage.Items[i];
                        P.RePain(Dlg, Dlv, Fill, 1, g);
                        break;
                    case Nows.Group:
                        GI = (GroupItem)CurrentPage.Items[i];
                        GI.RePain(Dlg, Dlv, Fill, 1, g);
                        break;
                }
            }
            if (CurrentPage.Selected.Count != 0 && Fill) CurrentPage.Selected.Corners.Show(Dlg, Dlv, g);
        }

        void DoDown(PointF e)
        {
            Saved.Modificated = true;

            Graphics g = Graphics.FromHwnd(tcTabs.TabPages[tcTabs.SelectedIndex].Handle);
            // ReSharper disable RedundantAssignment
            PointF PnHere = new PointF();
            // ReSharper restore RedundantAssignment
            PnHere = Pn;//начальная точка рисования или где была кликнута мышка в DOWN
            PointF Tek = new PointF();
            Tek.X = e.X;//где  мышка
            Tek.Y = e.Y;//            сейчас              

            float XnToRePaint = PnHere.X; // с какой точки начинать перерисовывать
            float YnToRePaint = PnHere.Y; // с какой точки начинать перерисовывать
            if (PnHere.X > Tek.X)
            {
                float buf = PnHere.X;
                PnHere.X = Tek.X;
                Tek.X = buf;
            }
            if (PprLast.X < Pn.X)
                XnToRePaint = PprLast.X;
            if (PnHere.Y > Tek.Y)
            {
                float buf = PnHere.Y;
                PnHere.Y = Tek.Y;
                Tek.Y = buf;
            }
            if (PprLast.Y < Pn.Y)
                YnToRePaint = PprLast.Y;

            switch (Now)
            {
                case Nows.None:
                    if (CurrentPage.Selected.Count == 0)
                    {
                        //перекрашиваем предыдущею линию
                        Pen MyP = new Pen(BorderCol);
                        MyP.DashStyle = DashStyle.Dot;
                        g.DrawRectangle(new Pen(tcTabs.TabPages[tcTabs.SelectedIndex].BackColor), XnToRePaint, YnToRePaint, Math.Abs(PprLast.X - Pn.X), Math.Abs(PprLast.Y - Pn.Y));
                        //рисуем новую
                        RePainAllItems(true);
                        g.DrawRectangle(MyP, PnHere.X, PnHere.Y, Math.Abs(e.X - Pn.X), Math.Abs(e.Y - Pn.Y));
                        //  break;
                    }
                    else
                        if (Corner != Corners.None)
                            CurrentPage.Selected.Transform(e.X - PprLast.X, e.Y - PprLast.Y, Dlg, Dlv, tcTabs.TabPages[tcTabs.SelectedIndex].BackColor, Corner, g);
                        else
                            CurrentPage.Selected.Move(e.X - PprLast.X, e.Y - PprLast.Y, Dlg, Dlv, tcTabs.TabPages[tcTabs.SelectedIndex].BackColor, g);
                    break;
                case Nows.Line:
                    {
                        g.DrawLine(new Pen(tcTabs.TabPages[tcTabs.SelectedIndex].BackColor), Pn.X, Pn.Y, ELast.X, ELast.Y);//перекрашиваем предыдущею линию
                        g.DrawLine(new Pen(BorderCol), Pn.X, Pn.Y, e.X, e.Y);//рисуем новую
                        break;
                    }
                case Nows.Rec:
                    {
                        //перекрашиваем предыдущею линию
                        g.DrawRectangle(new Pen(tcTabs.TabPages[tcTabs.SelectedIndex].BackColor), XnToRePaint, YnToRePaint, Math.Abs(PprLast.X - Pn.X), Math.Abs(PprLast.Y - Pn.Y));
                        //рисуем новую
                        g.DrawRectangle(new Pen(BorderCol), PnHere.X, PnHere.Y, Math.Abs(e.X - Pn.X), Math.Abs(e.Y - Pn.Y));
                        break;
                    }
                case Nows.Ellip:
                    {
                        //перекрашиваем предыдущею линию
                        g.DrawEllipse(new Pen(tcTabs.TabPages[tcTabs.SelectedIndex].BackColor), XnToRePaint, YnToRePaint, Math.Abs(PprLast.X - Pn.X), Math.Abs(PprLast.Y - Pn.Y));
                        //рисуем новую
                        g.DrawEllipse(new Pen(BorderCol), PnHere.X, PnHere.Y, Math.Abs(e.X - Pn.X), Math.Abs(e.Y - Pn.Y));
                        break;
                    }
                case Nows.Pie:
                    {
                        //перекрашиваем предыдущею линию
                        if (PprLast.X != Pn.X && PprLast.Y != Pn.Y)
                            g.DrawPie(new Pen(tcTabs.TabPages[tcTabs.SelectedIndex].BackColor), XnToRePaint, YnToRePaint, Math.Abs(PprLast.X - Pn.X), Math.Abs(PprLast.Y - Pn.Y), startAng, sweepAng);
                        //рисуем новую
                        if (e.X != Pn.X && e.Y != Pn.Y)
                            g.DrawPie(new Pen(BorderCol), PnHere.X, PnHere.Y, Math.Abs(e.X - Pn.X), Math.Abs(e.Y - Pn.Y), startAng, sweepAng);
                        break;
                    }
            }
            ELast.X = PprLast.X = e.X;
            ELast.Y = PprLast.Y = e.Y; //сохраняем координаты новой линии как предыдущие
            PnOut = PnHere;
        }

        private void pOut_MouseMove(object sender, MouseEventArgs e)
        {
            if (down)
            {
                if (Now != Nows.None || CurrentPage.Selected.Count != 0)
                    if (!Refreshed)
                    {
                        tcTabs.TabPages[tcTabs.SelectedIndex].Refresh();
                        RePainAllItems(false);
                        Refreshed = true;
                    }
                    else
                        RePainAllItems(false);
                DoDown(new PointF(e.X, e.Y));
            }
            else
            {
                if (Now == Nows.None && CurrentPage.Selected.Count != 0)
                {
                    if (!CurrentPage.DoPos)
                        switch (CurrentPage.Selected.Corners.Contains(e.X + Dlg, e.Y + Dlv))
                        {
                            case Corners.TopLeft:
                            case Corners.BottomRight:
                                tcTabs.TabPages[tcTabs.SelectedIndex].Cursor = Cursors.SizeNWSE;
                                break;
                            case Corners.TopRight:
                            case Corners.BottomLeft:
                                tcTabs.TabPages[tcTabs.SelectedIndex].Cursor = Cursors.SizeNESW;
                                break;
                            case Corners.Top:
                            case Corners.Bottom:
                                tcTabs.TabPages[tcTabs.SelectedIndex].Cursor = Cursors.SizeNS;
                                break;
                            case Corners.Left:
                            case Corners.Right:
                                tcTabs.TabPages[tcTabs.SelectedIndex].Cursor = Cursors.SizeWE;
                                break;
                            case Corners.None:
                                if (CurrentPage.Selected.ContainsPoint(e.X + Dlg, e.Y + Dlv, false))
                                    tcTabs.TabPages[tcTabs.SelectedIndex].Cursor = Cursors.SizeAll;
                                else
                                    tcTabs.TabPages[tcTabs.SelectedIndex].Cursor = Now == Nows.None ? Cursors.Default : Cursors.Cross;

                                break;
                        }

                }
                else if (!CurrentPage.DoPos && Now == Nows.None)
                    tcTabs.TabPages[tcTabs.SelectedIndex].Cursor = Cursors.Default;
            }
        }

        private void pOut_MouseUp(object sender, MouseEventArgs e)
        {
            if (down)
            {
                down = false;

                Graphics g = Graphics.FromHwnd(tcTabs.TabPages[tcTabs.SelectedIndex].Handle);
                switch (Now)
                {
                    case Nows.None:
                        if (CurrentPage.Selected.Count != 0)
                            CurrentPage.Selected.ReSelect();
                        else
                        {
                            g.DrawRectangle(new Pen(tcTabs.TabPages[tcTabs.SelectedIndex].BackColor), PnOut.X, PnOut.Y,
                                            Math.Abs(e.X - Pn.X), Math.Abs(e.Y - Pn.Y));
                            CurrentPage.Selected.Clear();

                            for (int i = 0; i < CurrentPage.Items.Count; i++)
                            {
                                Item It = (Item)CurrentPage.Items[i];
                                if (It.X1 >= PnOut.X + Dlg && It.X2 <= Math.Max(e.X, Pn.X) &&
                                    It.Y1 >= PnOut.Y + Dlv && It.Y2 <= Math.Max(e.Y, Pn.Y))
                                    CurrentPage.Selected.Add(CurrentPage.Items[i]);
                            }
                            if (CurrentPage.Selected.Count == 1)
                                tscbAlls.SelectedIndex = CurrentPage.Selected.ZOrder;
                            else
                                tscbAlls.SelectedIndex = -1;
                            if (CurrentPage.Selected.Count == 1)
                            {
                                Item It = (Item)CurrentPage.Selected[0];
                                if (It.TypeShape == Nows.Pie)
                                {
                                    PieButtons(true);
                                    Pie P = (Pie)CurrentPage.Selected[0];
                                    tstbAlfa.Text = P.startAngle.ToString();
                                    tstbBeta.Text = P.sweepAngle.ToString();
                                }
                            }
                        }
                        break;
                    case Nows.Line:
                        {
                            if (Pn.X != e.X && Pn.Y != e.Y)
                            {
                                Line L = new Line(Pn.X + Dlg, Pn.Y + Dlv, e.X + Dlg, e.Y + Dlv, BorderCol, ItCol,
                                    CurrentPage.ZOrdCurrent++, LinesWidth, string.Format("Линия {0}", CurrentPage.LC++));
                                CurrentPage.Items.Add(L);
                                CurrentPage.Selected.Clear();
                                CurrentPage.Selected.Add(L);
                                // CurrentPage.Selected.ZOrder = L.ZOrder;
                                L.RePain(Dlg, Dlv, 1, g);

                                tscbAlls.Items.Add(L.Name);
                                tscbAlls.SelectedIndex = tscbAlls.Items.Count - 1;

                            }
                            break;
                        }
                    case Nows.Rec:
                        {
                            if (Pn.X != e.X && Pn.Y != e.Y)
                            {
                                Rec R = new Rec(PnOut.X + Dlg, PnOut.Y + Dlv, Math.Abs(e.X - Pn.X), 
                                    Math.Abs(e.Y - Pn.Y), BorderCol, ItCol, CurrentPage.ZOrdCurrent++, 
                                    LinesWidth, string.Format("Прямоугольник {0}", CurrentPage.RC++));
                                CurrentPage.Items.Add(R);
                                CurrentPage.Selected.Clear();
                                CurrentPage.Selected.Add(R);
                                // CurrentPage.Selected.ZOrder = R.ZOrder;
                                R.RePain(Dlg, Dlv, true, 1, g);

                                tscbAlls.Items.Add(R.Name);
                                tscbAlls.SelectedIndex = tscbAlls.Items.Count - 1;

                            }
                            break;
                        }
                    case Nows.Ellip:
                        {
                            if (Pn.X != e.X && Pn.Y != e.Y)
                            {
                                Pie E = new Pie(PnOut.X + Dlg, PnOut.Y + Dlv, Math.Abs(e.X - Pn.X), 
                                    Math.Abs(e.Y - Pn.Y), BorderCol, ItCol, CurrentPage.ZOrdCurrent++, LinesWidth, 
                                    Nows.Ellip, 0.0f, 360.0f, string.Format("Другая фигура {0}", CurrentPage.PC++));
                                CurrentPage.Items.Add(E);
                                CurrentPage.Selected.Clear();
                                CurrentPage.Selected.Add(E);
                                // CurrentPage.Selected.ZOrder = E.ZOrder;
                                E.RePain(Dlg, Dlv, true, 1, g);

                                tscbAlls.Items.Add(E.Name);
                                tscbAlls.SelectedIndex = tscbAlls.Items.Count - 1;

                            }
                            break;
                        }
                    case Nows.Pie:
                        {
                            if (Pn.X != e.X && Pn.Y != e.Y)
                            {
                                Pie E = new Pie(PnOut.X + Dlg, PnOut.Y + Dlv, Math.Abs(e.X - Pn.X), Math.Abs(e.Y - Pn.Y), 
                                    BorderCol, ItCol, CurrentPage.ZOrdCurrent++, LinesWidth, Nows.Pie, startAng,
                                    sweepAng, string.Format("Другая фигура {0}", CurrentPage.PC++));
                                CurrentPage.Items.Add(E);
                                CurrentPage.Selected.Clear();
                                CurrentPage.Selected.Add(E);
                                E.RePain(Dlg, Dlv, true, 1, g);
                                tscbAlls.Items.Add(E.Name);
                                tscbAlls.SelectedIndex = tscbAlls.Items.Count - 1;
                            }
                            break;
                        }
                }
                RePainAllItems(true);
                WriteCounts();
            }
            CheckEnables();
        }

        void WriteCounts()
        {
            tslLnOut.Text = CurrentPage.Items.LCount.ToString();
            tslRecOut.Text = CurrentPage.Items.RCount.ToString();
            tslOthOut.Text = CurrentPage.Items.PCount.ToString();
            tslAllOut.Text = CurrentPage.Items.Count.ToString();
        }

        void CheckEnables()
        {
            if (CurrentPage.Selected.Count == 0)
            {
                tsbCopy.Enabled = false;
                tsbCut.Enabled = false;
                tsmiCopy.Enabled = false;
                tsmiCut.Enabled = false;
                tsmi2Copy.Enabled = false;
                tsmi2Cut.Enabled = false;
                stmi2Delete.Enabled = false;
                tsmiDelete.Enabled = false;
                tsmiShape.Enabled = false;
                tsmiShapeColor.Enabled = false;
                tsmiBorderColor.Enabled = false;
                tsmiBorderWid.Enabled = false;
                tsmiPosition.Enabled = false;
                tsmi2Position.Enabled = false;
                tsAboutShape.Enabled = false;
                tscbAlls.SelectedIndex = -1;
                tsmiDoGroup.Enabled = false;
                tsmi2DoGroup.Enabled = false;
                tsmiUnGroup.Enabled = false;
                tsmi2UnGroup.Enabled = false;
            }
            else
            {
                
                if (CurrentPage.Selected.Count > 1)
                {
                    tsmiDoGroup.Enabled = true;
                    tsmi2DoGroup.Enabled = true;
                }
                else
                {
                    tsmiDoGroup.Enabled = false;
                    tsmi2DoGroup.Enabled = false;
                }
                if (CurrentPage.Selected.Count == 1)
                {
                    if (CurrentPage.Selected.Count == 1)
                    {
                        Item It = (Item)CurrentPage.Selected[0];
                        if (It.TypeShape == Nows.Pie)
                        {
                            PieButtons(true);
                            Pie P = (Pie)CurrentPage.Selected[0];
                            tstbAlfa.Text = P.startAngle.ToString();
                            tstbBeta.Text = P.sweepAngle.ToString();
                        }
                        else if (It.TypeShape == Nows.Group)
                        {
                            tsmiUnGroup.Enabled = true;
                            tsmi2UnGroup.Enabled = true;
                        }
                    }
                }

                
                tsbCopy.Enabled = true;
                tsbCut.Enabled = true;
                tsmiCopy.Enabled = true;
                tsmiCut.Enabled = true;
                tsmi2Copy.Enabled = true;
                tsmi2Cut.Enabled = true;
                stmi2Delete.Enabled = true;
                tsmiDelete.Enabled = true;
                tsmiShape.Enabled = true;
                tsmiShapeColor.Enabled = true;
                tsmiBorderColor.Enabled = true;
                tsmiBorderWid.Enabled = true;
                tsmiPosition.Enabled = true;
                tsmi2Position.Enabled = true;
                tsAboutShape.Enabled = true;
                // Item It = (Item)CurrentPage.Selected.Item;
                tstbX.Text = CurrentPage.Selected.X1.ToString();
                tstbY.Text = CurrentPage.Selected.Y1.ToString();
                tstbWidth.Text = Convert.ToString(CurrentPage.Selected.X2 - CurrentPage.Selected.X1);
                tstbHeight.Text = Convert.ToString(CurrentPage.Selected.Y2 - CurrentPage.Selected.Y1);
            }
        }

        private void tsmiRefresh_Click(object sender, EventArgs e)
        {
            if (tcTabs.SelectedIndex >= 0)
            {
                tcTabs.TabPages[tcTabs.SelectedIndex].Refresh();
                //   RePainAllItems(true);
            }
        }

        private void tsniLine_Click(object sender, EventArgs e)
        {
            ChangeIcons();

            //PieButtons(false);
            Now = Nows.Line;
            tsbLine.Image = ilList.Images[2 * (int)Now + 1];
            tsbLine.ToolTipText = tsniLine.ToolTipText;
            tsniLine.Image = ilList.Images[2 * (int)Now + 1];
            for (int i = 0; i < tcTabs.TabCount; i++)
                tcTabs.TabPages[i].Cursor = Cursors.Cross;
        }

        private void tsmiRectangle_Click(object sender, EventArgs e)
        {
            ChangeIcons();

            // PieButtons(false);
            Now = Nows.Rec;
            tsbRectangle.Image = ilList.Images[2 * (int)Now + 1];
            tsbRectangle.ToolTipText = tsmiRectangle.ToolTipText;
            tsmiRectangle.Image = ilList.Images[2 * (int)Now + 1];
            for (int i = 0; i < tcTabs.TabCount; i++)
                tcTabs.TabPages[i].Cursor = Cursors.Cross;
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            ChangeIcons();

            // PieButtons(false);
            Now = Nows.None;
            tsbMouse.Image = ilList.Images[2 * (int)Now + 1];
            for (int i = 0; i < tcTabs.TabCount; i++)
                tcTabs.TabPages[i].Cursor = Cursors.Default;
        }

        private void ChangeIcons()
        {
            switch (Now)
            {
                case Nows.None:
                    tsbMouse.Image = ilList.Images[2 * (int)Now];
                    break;
                case Nows.Line:
                    tsbLine.Image = ilList.Images[2 * (int)Now];
                    tsniLine.Image = ilList.Images[2 * (int)Now];
                    break;
                case Nows.Rec:
                    tsbRectangle.Image = ilList.Images[2 * (int)Now];
                    tsmiRectangle.Image = ilList.Images[2 * (int)Now];
                    break;
                case Nows.Ellip:
                    tssbPie.Image = ilList.Images[2 * (int)Now];
                    tsmiEllipse.Image = ilList.Images[2 * (int)Now];
                    break;
                case Nows.Pie:
                    tssbPie.Image = ilList.Images[2 * (int)Now];
                    tsmiPie.Image = ilList.Images[2 * (int)Now];
                    break;
            }
        }

        private void PieButtons(bool Vis)
        {
            tslAlfa.Visible = Vis;
            tslBeta.Visible = Vis;
            tstbAlfa.Visible = Vis;
            tstbBeta.Visible = Vis;
            tsbAddAlfa.Visible = Vis;
            tsbAddBeta.Visible = Vis;
            tsbLessAlfa.Visible = Vis;
            tsbLessBeta.Visible = Vis;
            tssPieSeparator.Visible = Vis;
        }

        private void tsmiPie_Click(object sender, EventArgs e)
        {
            ChangeIcons();

            PieButtons(true);
            Now = Nows.Pie;
            tssbPie.ButtonClick += tsmiPie_Click;
            tssbPie.Image = ilList.Images[2 * (int)Now + 1];
            tssbPie.ToolTipText = tsmiPie.ToolTipText;
            tsmiPie.Image = ilList.Images[2 * (int)Now + 1];

            tstbAlfa.Text = startAng.ToString();
            tstbBeta.Text = sweepAng.ToString();

            for (int i = 0; i < tcTabs.TabCount; i++)
                tcTabs.TabPages[i].Cursor = Cursors.Cross;
        }

        private void tsmiEllipse_Click(object sender, EventArgs e)
        {
            ChangeIcons();

            //PieButtons(false);
            Now = Nows.Ellip;
            tssbPie.ButtonClick += tsmiEllipse_Click;
            tssbPie.Image = ilList.Images[2 * (int)Now + 1];
            tssbPie.ToolTipText = tsmiEllipse.ToolTipText;
            tsmiEllipse.Image = ilList.Images[2 * (int)Now + 1];
            for (int i = 0; i < tcTabs.TabCount; i++)
                tcTabs.TabPages[i].Cursor = Cursors.Cross;
        }

        private void toolStripContainer1_LeftToolStripPanel_Click(object sender, EventArgs e)
        {

        }

        private void tsbRenameTab_Click(object sender, EventArgs e)
        {
            if (!tstbNewTabsName.Enabled)
            {
                tstbNewTabsName.Enabled = true;
                tstbNewTabsName.Focus();
                tstbNewTabsName.Text = tcTabs.TabPages[tcTabs.SelectedIndex].Text;
            }
            else
            {
                tstbNewTabsName.Enabled = false;
                tscbAllTabs.Items[tcTabs.SelectedIndex] = tstbNewTabsName.Text;
                tstbNewTabsName.Text = "";
            }

        }

        private void tstbNewTabsName_TextChanged(object sender, EventArgs e)
        {
            if (tstbNewTabsName.Enabled)
                tcTabs.TabPages[tcTabs.SelectedIndex].Text = tstbNewTabsName.Text;
        }

        private void tsbAddTab_Click(object sender, EventArgs e)
        {
            CountForNames++;
            TabPage TP = new TabPage(string.Format("Страница {0}", CountForNames));
            Size Sz = new Size(WidFm, HeigFm);

            TP.AutoScrollMinSize = Sz;

            tcTabs.TabPages.Add(TP);
            tscbAllTabs.Items.Add(string.Format("Страница {0}", CountForNames));
            tcTabs.TabPages[tcTabs.TabCount - 1].BackColor = Color.White;
            tcTabs.TabPages[tcTabs.TabCount - 1].MouseDown += pOut_MouseDown;
            tcTabs.TabPages[tcTabs.TabCount - 1].MouseMove += pOut_MouseMove;
            tcTabs.TabPages[tcTabs.TabCount - 1].MouseUp += pOut_MouseUp;
            tcTabs.TabPages[tcTabs.TabCount - 1].Scroll += tpMain_Scroll;
            tcTabs.TabPages[tcTabs.TabCount - 1].Paint += tpMain_Paint;
            tcTabs.TabPages[tcTabs.TabCount - 1].ContextMenuStrip = cmsItemCon;


            Album.Add(new OnPage());
           // if (OldPageIndex > 0)
           //     Album[OldPageIndex] = CurrentPage;
            CurrentPage = Album[tcTabs.TabCount - 1];

            //OldPageIndex = tcTabs.TabCount - 1;
            tcTabs.SelectedIndex = tcTabs.TabCount - 1;
            CurrentPage.G = Graphics.FromHwnd(tcTabs.TabPages[tcTabs.SelectedIndex].Handle);


            //tsmiRefresh_Click(sender, e);
        }

        private void tsbDelTab_Click(object sender, EventArgs e)
        {
            // if (tcTabs.TabCount > 1)
            //{
            Album[OldPageIndex] = CurrentPage;
            OldPageIndex = -1;
            CurrentPage.Items = null;
            Album.Remove(tcTabs.SelectedIndex);
            tscbAllTabs.Items.RemoveAt(tcTabs.SelectedIndex);

            tcTabs.TabPages.RemoveAt(tcTabs.SelectedIndex);
            int In = tcTabs.SelectedIndex;
            if (In > 0)
                tcTabs.SelectedIndex = --In;
            // }
        }

        private void tscbAllTabs_SelectedIndexChanged(object sender, EventArgs e)
        {
            tcTabs.SelectedIndex = tscbAllTabs.SelectedIndex;
        }

        private void tcTabs_SelectedIndexChanged(object sender, EventArgs e)
        {
            tscbAllTabs.SelectedIndex = tcTabs.SelectedIndex;
            if (tcTabs.SelectedIndex >= 0)
            {
              //  if (OldPageIndex >= 0)
                //    Album[OldPageIndex] = CurrentPage;
                CurrentPage = Album[tcTabs.SelectedIndex];
             //   OldPageIndex = tcTabs.SelectedIndex;
                tsmiRefresh_Click(sender, e);
                CurrentPage.ZOrdCurrent = CurrentPage.Items.Count;
                WriteCounts();
                WriteItemsToList();
            }

        }

        private void WriteItemsToList()
        {
            tscbAlls.Items.Clear();
            tscbAlls.Text = "";
            Item It;
            for (int i = 0; i < CurrentPage.Items.Count; i++)
            {
                It = (Item) CurrentPage.Items[i];
                tscbAlls.Items.Add(It.Name);
            }
            if (CurrentPage.Selected.Count == 1)
            {
                It = (Item) CurrentPage.Selected[0];
                tscbAlls.SelectedIndex = It.ZOrder;
            }
        }

        private void tsbPrevTab_Click(object sender, EventArgs e)
        {
            if (tcTabs.SelectedIndex - 1 >= 0) tcTabs.SelectedIndex--;
        }

        private void tsbNextTab_Click(object sender, EventArgs e)
        {
            if (tcTabs.SelectedIndex + 1 < tcTabs.TabCount) tcTabs.SelectedIndex++;
        }

        private void tsbFirstTab_Click(object sender, EventArgs e)
        {
            tcTabs.SelectedIndex = 0;
        }

        private void tsbLastTab_Click(object sender, EventArgs e)
        {
            tcTabs.SelectedIndex = tcTabs.TabCount - 1;
        }

        private void main_Paint(object sender, PaintEventArgs e)
        {
            tsmiRefresh_Click(sender, e);
        }

        private void main_Resize(object sender, EventArgs e)
        {
            Dlv = tcTabs.TabPages[tcTabs.SelectedIndex].VerticalScroll.Value;
            Dlg = tcTabs.TabPages[tcTabs.SelectedIndex].HorizontalScroll.Value;
            tsmiRefresh_Click(sender, e);
        }

        private void tpMain_Scroll(object sender, ScrollEventArgs e)
        {
            Dlv = tcTabs.TabPages[tcTabs.SelectedIndex].VerticalScroll.Value;
            Dlg = tcTabs.TabPages[tcTabs.SelectedIndex].HorizontalScroll.Value;
            tsmiRefresh_Click(sender, e);
        }

        private void main_Shown(object sender, EventArgs e)
        {
            CurrentPage.G = Graphics.FromHwnd(tcTabs.TabPages[0].Handle);
        }

        private void main_Load(object sender, EventArgs e)
        {
            WidFm = tpMain.Width;
            HeigFm = tpMain.Height;
            for (int i = 5; i < 58; i++)
                tsColors.Items[i].MouseUp += tsbWhite_MouseUp;

            ItCol = tsbColor.BackColor;
            BorderCol = tsbBorderColor.BackColor;
            Size Sz = new Size(WidFm, HeigFm);
            tcTabs.TabPages[0].AutoScrollMinSize = Sz;

            Now = Nows.None;
            tsbMouse.Image = ilList.Images[2 * (int)Now + 1];
            //g = Graphics.FromHwnd(tcTabs.TabPages[tcTabs.SelectedIndex].Handle);
        }

        private void toolStripButton58_Click(object sender, EventArgs e)
        {
            // cdColors.;
        }

        private void tsbWhite_MouseUp(object sender, MouseEventArgs e)
        {
            ToolStripButton But = (ToolStripButton)sender;
            if (e.Button == MouseButtons.Left)
            {
                if (But.DisplayStyle == ToolStripItemDisplayStyle.Image)
                {
                    tsbColor.Image = But.Image;
                    tsbColor.BackColor = tsColors.BackColor;
                    tsbColor.DisplayStyle = ToolStripItemDisplayStyle.Image;
                    ItCol = Color.Empty;
                }
                else
                {
                    tsbColor.BackColor = But.BackColor;
                    ItCol = But.BackColor;
                    tsbColor.DisplayStyle = ToolStripItemDisplayStyle.None;
                }

            }
            else if (e.Button == MouseButtons.Right)
            {
                tsbBorderColor.BackColor = But.BackColor;
                BorderCol = But.BackColor;
            }
        }


        private void WriteLToFile(BinaryWriter dataOut, Line L)
        {
            dataOut.Write((int)L.TypeShape);
            dataOut.Write(L.BorderCol.ToArgb());
            dataOut.Write(L.Col.ToArgb());
            dataOut.Write(L.ZOrder);
            dataOut.Write(L.BorderWidth);
            dataOut.Write(L.Xn);
            dataOut.Write(L.Yn);
            dataOut.Write(L.Xk);
            dataOut.Write(L.Yk);
        }

        private void WriteRToFile(BinaryWriter dataOut, Rec R)
        {
            dataOut.Write((int)R.TypeShape);
            dataOut.Write(R.BorderCol.ToArgb());
            dataOut.Write(R.Col.ToArgb());
            dataOut.Write(R.ZOrder);
            dataOut.Write(R.BorderWidth);
            dataOut.Write(R.Xn);
            dataOut.Write(R.Yn);
            dataOut.Write(R.Width);
            dataOut.Write(R.Height);
        }

        private void WritePToFile(BinaryWriter dataOut, Pie P)
        {
            dataOut.Write((int)P.TypeShape);
            dataOut.Write(P.BorderCol.ToArgb());
            dataOut.Write(P.Col.ToArgb());
            dataOut.Write(P.ZOrder);
            dataOut.Write(P.BorderWidth);
            dataOut.Write(P.Xn);
            dataOut.Write(P.Yn);
            dataOut.Write(P.Width);
            dataOut.Write(P.Height);
            dataOut.Write(P.startAngle);
            dataOut.Write(P.sweepAngle);
        }

        private void WriteGIToFile(BinaryWriter dataOut, GroupItem GI)
        {
            Item It;
            dataOut.Write((int)GI.TypeShape);
            dataOut.Write(GI.Count);
            for (int i = 0; i < GI.Count; i++)
            {
                It = (Item) GI[i];
                switch (It.TypeShape)
                {
                    case Nows.Line:
                        Line L = (Line)GI[i];
                        WriteLToFile(dataOut, L);
                        break;
                    case Nows.Rec:
                        Rec R = (Rec)GI[i];
                        WriteRToFile(dataOut, R);
                        break;
                    case Nows.Ellip:
                    case Nows.Pie:
                        Pie P = (Pie)GI[i];
                        WritePToFile(dataOut, P);
                        break;
                    case Nows.Group:
                        GroupItem GR = (GroupItem)GI[i];
                        WriteGIToFile(dataOut, GR);
                        break;
                }
            }
        }

        private void Save(string FileName, SaveMode Mode)
        {
            BinaryWriter dataOut;
            try
            {
                dataOut = new BinaryWriter(new FileStream(FileName, FileMode.Create));
            }
            catch (IOException exc)
            {
                MessageBox.Show(string.Format("{0}\nНевозможно создать файл.", exc.Message), "Graphic Editor Error");
                return;
            }

            try
            {

                int j;
                int first = 0;
                int last = 0;

                switch (Mode)
                {
                    case SaveMode.sSavePage:
                        first = last = tcTabs.SelectedIndex;
                        break;
                    case SaveMode.sSaveAlbum:
                        first = 0;
                        last = tcTabs.TabPages.Count - 1;
                        break;
                }

                dataOut.Write(tcTabs.TabPages.Count);
                for (j = first; j <= last; j++)
                {
                    dataOut.Write(tcTabs.TabPages[j].Text);
                    dataOut.Write(Album[j].Items.Count);
                    for (int i = 0; i < Album[j].Items.Count; i++)
                    {
                        switch (Album[j].Items.GetTypeOfObj(i))
                        {
                            case Nows.Line:
                                Line L = (Line)Album[j].Items[i];
                                WriteLToFile(dataOut, L);
                                break;
                            case Nows.Rec:
                                Rec R = (Rec)Album[j].Items[i];
                                WriteRToFile(dataOut, R);
                                break;
                            case Nows.Ellip:
                            case Nows.Pie:
                                Pie P = (Pie)Album[j].Items[i];
                                WritePToFile(dataOut, P);
                                break;
                            case Nows.Group:
                                GroupItem GI = (GroupItem)Album[j].Items[i];
                                WriteGIToFile(dataOut, GI);
                                break;
                        }
                    }
                }
            }
            catch (IOException exc) { MessageBox.Show(string.Format("{0}\nОшибка записи.", exc.Message), "Graphic Editor Error"); }
            dataOut.Close();

        }

        private void tsmiSave_Click(object sender, EventArgs e)
        {
            sfdSave.Title = "Сохранить страницу";
            sfdSave.Filter = "GraphicEditor Page (*.pkep)|*.pkep|All files (*.*)|*.*";
            if (sfdSave.ShowDialog() == DialogResult.OK)
            {
                Save(sfdSave.FileName, SaveMode.sSavePage);
            }

        }

        private void tsmiSaveAlbum_Click(object sender, EventArgs e)
        {
            sfdSave.Title = "Сохранить альбом";
            sfdSave.Filter = "GraphicEditor Album (*.akep)|*.akep|All files (*.*)|*.*";
            if (Saved.Name == "")
                if (sfdSave.ShowDialog() == DialogResult.OK)
                {
                    Save(sfdSave.FileName, SaveMode.sSaveAlbum);
                    Saved.Name = sfdSave.FileName;
                    Saved.Modificated = false;
                    // ReSharper disable RedundantThisQualifier
                    this.Text = Application.ProductName + " - " + Saved.Name;
                    // ReSharper restore RedundantThisQualifier
                }
                else
                {
                }
            else if (Saved.Modificated) Save(Saved.Name, SaveMode.sSaveAlbum);
        }

        private void tsmiSaveAlbumAs_Click(object sender, EventArgs e)
        {
            sfdSave.Title = "Сохранить альбом как";
            sfdSave.Filter = "GraphicEditor Album (*.akep)|*.akep|All files (*.*)|*.*";
            if (sfdSave.ShowDialog() == DialogResult.OK)
            {
                Save(sfdSave.FileName, SaveMode.sSaveAlbum);
                Saved.Name = sfdSave.FileName;
                Saved.Modificated = false;
                // ReSharper disable RedundantThisQualifier
                this.Text = "GraphicEditor" + " - " + Saved.Name;
                // ReSharper restore RedundantThisQualifier
            }
        }

        private Line ReadLFromFile(BinaryReader dataIn)
        {
            int ClBack;
            int ClBor;
            int ZOr;
            float BorWid, Xn, Yn, Xk, Yk;

            ClBor = dataIn.ReadInt32();
            ClBack = dataIn.ReadInt32();
            ZOr = dataIn.ReadInt32();
            BorWid = dataIn.ReadSingle();
            Xn = dataIn.ReadSingle();
            Yn = dataIn.ReadSingle();
            Xk = dataIn.ReadSingle();
            Yk = dataIn.ReadSingle();
            return new Line(Xn, Yn, Xk, Yk, Color.FromArgb(ClBor), Color.FromArgb(ClBack), ZOr, BorWid,
                string.Format("Линия {0}", CurrentPage.LC++));
        }

        private Rec ReadRFromFile(BinaryReader dataIn)
        {
            int ClBack;
            int ClBor;
            int ZOr;
            float BorWid, Xn, Yn, Wid, Hei;

            ClBor = dataIn.ReadInt32();
            ClBack = dataIn.ReadInt32();
            ZOr = dataIn.ReadInt32();
            BorWid = dataIn.ReadSingle();
            Xn = dataIn.ReadSingle();
            Yn = dataIn.ReadSingle();
            Wid = dataIn.ReadSingle();
            Hei = dataIn.ReadSingle();
            return new Rec(Xn, Yn, Wid, Hei, Color.FromArgb(ClBor), Color.FromArgb(ClBack), ZOr, BorWid,
                string.Format("Прямоугольник {0}", CurrentPage.RC++));
        }

        private Pie ReadPFromFile(BinaryReader dataIn, Nows TS)
        {
            int ClBack;
            int ClBor;
            int ZOr;
            float BorWid, Xn, Yn, Wid, Hei, StAng, SweAng;

            ClBor = dataIn.ReadInt32();
            ClBack = dataIn.ReadInt32();
            ZOr = dataIn.ReadInt32();
            BorWid = dataIn.ReadSingle();
            Xn = dataIn.ReadSingle();
            Yn = dataIn.ReadSingle();
            Wid = dataIn.ReadSingle();
            Hei = dataIn.ReadSingle();
            StAng = dataIn.ReadSingle();
            SweAng = dataIn.ReadSingle();
            return new Pie(Xn, Yn, Wid, Hei, Color.FromArgb(ClBor), Color.FromArgb(ClBack), ZOr, BorWid, TS, StAng,
                SweAng, string.Format("Другая фигура {0}", CurrentPage.PC++));
                                
        }

        private GroupItem ReadGIFromFile(BinaryReader dataIn)
        {
            int Count = dataIn.ReadInt32();
            Nows TS;
            GroupItem GI = new GroupItem();
            for (int i = 0; i < Count; i++)
            {
                TS = (Nows)dataIn.ReadInt32();
                switch (TS)
                {
                    case Nows.Line:
                        GI.Add(ReadLFromFile(dataIn));
                        break;
                    case Nows.Rec:
                        GI.Add(ReadRFromFile(dataIn));
                        break;
                    case Nows.Ellip:
                    case Nows.Pie:
                        GI.Add(ReadPFromFile(dataIn, TS));
                        break;
                    case Nows.Group:
                        GI.Add(ReadGIFromFile(dataIn));
                        break;
                }
            }
            GI.Name = string.Format("Группа {0}", CurrentPage.GC++);
            return GI;
        }

        private void Open(string FileName)
        {
            BinaryReader dataIn;
            try
            {
                dataIn = new BinaryReader(new FileStream(FileName, FileMode.Open));
            }
            catch (IOException exc)
            {
                MessageBox.Show(string.Format("{0}\nНевозможно создать файл.", exc.Message), "Graphic Editor Error");
                return;
            }

            try
            {
                tspbProgress.Visible = true;
                tspbProgress.Value = 0;
                //Album.Clear();
                int Count = tcTabs.TabCount;
                for (int i = Count - 1; i >= 0; i--)
                {
                    tsbDelTab_Click(null, null);
                }
                Nows TS;
                int AmountPages = dataIn.ReadInt32();
                int AmountItems = 0;
                Graphics g;
                for (int j = 0; j < AmountPages; j++)
                {
                    tsbAddTab_Click(null, null);
                    tcTabs.TabPages[j].Text = dataIn.ReadString();
                    g = Graphics.FromHwnd(tcTabs.TabPages[j].Handle);
                    AmountItems = dataIn.ReadInt32();
                    //  Album.Add(new sOnPage());
                    tspbProgress.Step = tspbProgress.Maximum / (AmountPages + AmountItems);

                    for (int i = 0; i < AmountItems; i++)
                    {
                        if (tspbProgress.Value + tspbProgress.Step < tspbProgress.Maximum)
                            tspbProgress.Value += tspbProgress.Step;
                        else
                            tspbProgress.Value = tspbProgress.Maximum;
                        TS = (Nows)dataIn.ReadInt32();
                        switch (TS)
                        {
                            case Nows.Line:
                                Album[j].Items.Add(ReadLFromFile(dataIn));
                                break;
                            case Nows.Rec:
                                Album[j].Items.Add(ReadRFromFile(dataIn));
                                break;
                            case Nows.Ellip:
                            case Nows.Pie:
                                Album[j].Items.Add(ReadPFromFile(dataIn, TS));
                                break;
                            case Nows.Group:
                                Album[j].Items.Add(ReadGIFromFile(dataIn));
                                break;
                        }
                    }
                }
                tspbProgress.Visible = false;
            }
            catch (IOException exc) { MessageBox.Show(string.Format("{0}\nОшибка записи.", exc.Message), "Graphic Editor Error"); }

            dataIn.Close();

        }

        private void tsmiOpen_Click(object sender, EventArgs e)
        {
            if (Saved.Modificated)
            {
                switch (MessageBox.Show("Альбом был изменен. Хотите сохранить изменения?", "Внимание!!!",
                    MessageBoxButtons.YesNoCancel))
                {
                    case DialogResult.Yes:
                        tsmiSaveAlbum_Click(sender, e);
                        break;
                    case DialogResult.No: break;
                    case DialogResult.Cancel: return;
                }

            }

            ofdOpen.Title = "Открыть страницу или альбом";
            ofdOpen.Filter = "GraphicEditor Files (*.akep, *.pkep)|*.akep, *pkep|All files (*.*)|*.*";
            if (ofdOpen.ShowDialog() == DialogResult.OK)
            {
                Open(ofdOpen.FileName);
                tsmiRefresh_Click(sender, e);
                Saved.Name = ofdOpen.FileName;
                Saved.Modificated = false;
                Sort();
                // ReSharper disable RedundantThisQualifier
                this.Text = "GraphicEditor" + " - " + Saved.Name;
                // ReSharper restore RedundantThisQualifier
            }
        }

        private void tpMain_Paint(object sender, PaintEventArgs e)
        {
            if (down) return;
            RePainAllItems(true);
        }

        private void tsmiChangeCol_Click(object sender, EventArgs e)
        {
            if (cdColors.ShowDialog() == DialogResult.Cancel) return;

            //  Graphics g = Graphics.FromHwnd(tcTabs.TabPages[tcTabs.SelectedIndex].Handle);
            if (CurrentPage.Selected.Count != 0)
            {
                for (int i = 0; i < CurrentPage.Selected.Count; i++)
                {
                    Item It = (Item)CurrentPage.Selected[i];
                    switch (It.TypeShape)
                    {
                        case Nows.Rec:
                            Rec R = (Rec)CurrentPage.Selected[i];
                            R.Col = cdColors.Color;
                            // R.RePain(Dlg, Dlv, true, 1, g);
                            break;
                        case Nows.Ellip:
                        case Nows.Pie:
                            Pie P = (Pie)CurrentPage.Selected[i];
                            P.Col = cdColors.Color;
                            // P.RePain(Dlg, Dlv, true, 1, g);
                            break;
                    }
                }
                RePainAllItems(true);
            }
            else
            {
                ItCol = cdColors.Color;
                tsbColor.BackColor = ItCol;
            }
        }

        private void tsmiChBorCol_Click(object sender, EventArgs e)
        {
            if (cdColors.ShowDialog() != DialogResult.Cancel)
            {
                if (CurrentPage.Selected.Count != 0)
                {
                    //    Graphics g = Graphics.FromHwnd(tcTabs.TabPages[tcTabs.SelectedIndex].Handle);
                    for (int i = 0; i < CurrentPage.Selected.Count; i++)
                    {
                        Item It = (Item)CurrentPage.Selected[i];
                        switch (It.TypeShape)
                        {
                            case Nows.Line:
                                Line L = (Line)CurrentPage.Selected[i];
                                L.BorderCol = cdColors.Color;
                                //  L.RePain(Dlg, Dlv, 1, g);
                                break;
                            case Nows.Rec:
                                Rec R = (Rec)CurrentPage.Selected[i];
                                R.BorderCol = cdColors.Color;
                                //   R.RePain(Dlg, Dlv, false, 1, g);
                                break;
                            case Nows.Ellip:
                            case Nows.Pie:
                                Pie P = (Pie)CurrentPage.Selected[i];
                                P.BorderCol = cdColors.Color;
                                // P.RePain(Dlg, Dlv, false, 1, g);
                                break;
                        }
                    }
                    RePainAllItems(true);
                }
                else
                {
                    BorderCol = cdColors.Color;
                    tsbBorderColor.BackColor = BorderCol;
                }
            }
        }

        private void tsmiAbout_Click(object sender, EventArgs e)
        {


            FmAbout.ShowDialog();

        }

        private void tsmiNewAlbum_Click(object sender, EventArgs e)
        {
            if (Saved.Modificated)
            {
                switch (MessageBox.Show("Альбом был изменен. Хотите сохранить изменения?", "Внимание!!!",
                    MessageBoxButtons.YesNoCancel))
                {
                    case DialogResult.Yes:
                        tsmiSaveAlbum_Click(sender, e);
                        break;
                    case DialogResult.No: break;
                    case DialogResult.Cancel: return;
                }

            }
            // ReSharper disable RedundantThisQualifier
            this.Text = "GraphicEditor";
            // ReSharper restore RedundantThisQualifier
            int Count = tcTabs.TabCount;
            for (int i = Count - 1; i >= 0; i--)
            {
                tsbDelTab_Click(null, null);
            }
            CountForNames = 0;
            CurrentPage.ZOrdCurrent = 0;
            tsbAddTab_Click(null, null);
            toolStripButton1_Click(null, null);
            CtrlPressed = false;
            WriteCounts();
            CheckEnables();
        }

        private void cmsItemCon_Opening(object sender, CancelEventArgs e)
        {
            if (CurrentPage.Selected.Count == 1)
            {
                Item It = (Item)CurrentPage.Selected[0];
                tscbLinesWidth.Text = It.BorderWidth.ToString();
            }
            else tscbLinesWidth.Text = LinesWidth.ToString();
        }

        /* 
         * когда меняем толщину линий на панели,т.е. толщину линий для выделенной фигуры и для всех фигур тоже
        */
        private void tscbLlinesWidth_SelectedIndexChanged(object sender, EventArgs e)
        {
            LinesWidth = (float)Convert.ToDouble(tscbLlinesWidth.Items[tscbLlinesWidth.SelectedIndex].ToString());
            //в контекстном меню меням толщину линий тоже
            tscbLinesWidth.Text = tscbLlinesWidth.Items[tscbLlinesWidth.SelectedIndex].ToString();
        }

        /*когда меняем толщину из контекстного меню*/
        private void tscbLinesWidth_SelectedIndexChanged(object sender, EventArgs e)
        {
            float NewLineWD = (float)Convert.ToDouble(tscbLinesWidth.Items[tscbLinesWidth.SelectedIndex].ToString());
            bool NeedRefresh = false;//если толщина уменьшилась, то нужно перерисовать все фигуры заново
            if (CurrentPage.Selected.Count != 0)
            {
                for (int i = 0; i < CurrentPage.Selected.Count; i++)
                {
                    Item It = (Item)CurrentPage.Selected[i];
                    switch (It.TypeShape)
                    {
                        case Nows.Line:
                            Line L = (Line)CurrentPage.Selected[i];
                            if (L.BorderWidth != NewLineWD)
                            {
                                if (NewLineWD < L.BorderWidth)
                                    NeedRefresh = true;
                                L.BorderWidth = NewLineWD;

                            }
                            break;
                        case Nows.Rec:
                            Rec R = (Rec)CurrentPage.Selected[i];
                            if (R.BorderWidth != NewLineWD)
                            {
                                if (NewLineWD < R.BorderWidth)
                                    NeedRefresh = true;
                                R.BorderWidth = NewLineWD;
                            }
                            break;
                        case Nows.Ellip:
                        case Nows.Pie:
                            Pie P = (Pie)CurrentPage.Selected[i];
                            if (P.BorderWidth != NewLineWD)
                            {
                                if (NewLineWD < P.BorderWidth)
                                    NeedRefresh = true;
                                P.BorderWidth = NewLineWD;
                            }
                            break;

                    }
                }
                if (NeedRefresh)
                    tcTabs.TabPages[tcTabs.SelectedIndex].Refresh();
                RePainAllItems(true);
            }
            else
            {
                LinesWidth = NewLineWD;
                tscbLlinesWidth.Text = LinesWidth.ToString();
            }
            cmsItemCon.Close();//выход из контекстного меню
        }

        private void tsbCopy_Click(object sender, EventArgs e)
        {
            Buffer.Clear();
            for (int i = 0; i < CurrentPage.Selected.Count; i++)
            {
                Item It = (Item)CurrentPage.Selected[i];
                switch (It.TypeShape)
                {
                    case Nows.Line:
                        Line L = (Line)CurrentPage.Selected[i];
                        Buffer.Add(new Line(L));
                        break;
                    case Nows.Rec:
                        Rec R = (Rec)CurrentPage.Selected[i];
                        Buffer.Add(new Rec(R));
                        break;
                    case Nows.Ellip:
                    case Nows.Pie:
                        Pie P = (Pie)CurrentPage.Selected[i];
                        Buffer.Add(new Pie(P));
                        break;
                }
            }
            if (Buffer.Count != 0)
            {
                tsbPaste.Enabled = true;
                tsmiPaste.Enabled = true;
                tsmi2Paste.Enabled = true;
            }
            else
            {
                tsbPaste.Enabled = false;
                tsmiPaste.Enabled = false;
                tsmi2Paste.Enabled = false;
            }
        }

        private void tsbCut_Click(object sender, EventArgs e)
        {
            tsbCopy_Click(sender, e);
            tsmiDelete_Click(sender, e);
            RecycleBin.Clear();
        }

        private void tsbPaste_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < Buffer.Count; i++)
            {
                Item It = (Item)Buffer[i];
                It.ZOrder = CurrentPage.ZOrdCurrent++;
                Graphics g = Graphics.FromHwnd(tcTabs.TabPages[tcTabs.SelectedIndex].Handle);
                CurrentPage.Selected.Corners.Hide(Dlg, Dlv, g, tcTabs.TabPages[tcTabs.SelectedIndex].BackColor);
                switch (It.TypeShape)
                {
                    case Nows.Line:
                        Line L = (Line)Buffer[i];
                        CurrentPage.Items.Add(new Line(L));
                        CurrentPage.Selected.Add(L);
                        tscbAlls.Items.Add(string.Format("Линия {0}", ++CurrentPage.LC));
                        break;
                    case Nows.Rec:
                        Rec R = (Rec)Buffer[i];
                        CurrentPage.Items.Add(new Rec(R));
                        CurrentPage.Selected.Add(R);
                        tscbAlls.Items.Add(string.Format("Прямоугольник {0}", ++CurrentPage.RC));
                        break;
                    case Nows.Ellip:
                    case Nows.Pie:
                        Pie P = (Pie)Buffer[i];
                        CurrentPage.Items.Add(new Pie(P));
                        CurrentPage.Selected.Add(P);
                        tscbAlls.Items.Add(string.Format("Другая фигура {0}", ++CurrentPage.PC));
                        break;
                }
            }
            tscbAlls.SelectedIndex = tscbAlls.Items.Count - 1;
            RePainAllItems(true);
            CheckEnables();
            WriteCounts();
        }

        private void tsmiDelete_Click(object sender, EventArgs e)
        {
            if (CurrentPage.Selected.Count == 0) return;
            ItemOfRecycle ItOfRec;
            ItOfRec.Item = null;
            for (int i = CurrentPage.Selected.Count - 1; i >= 0; i--)
            {
                Item It = (Item)CurrentPage.Selected[i];
                int ZOrder = It.ZOrder;
                switch (It.TypeShape)
                {
                    case Nows.Line:
                        CurrentPage.Items.LCount -= 1;
                        Line L = (Line)CurrentPage.Selected[i];
                        ItOfRec.Item = new Line(L);
                        break;
                    case Nows.Rec:
                        CurrentPage.Items.RCount -= 1;
                        Rec R = (Rec)CurrentPage.Selected[i];
                        ItOfRec.Item = new Rec(R);
                        break;
                    case Nows.Ellip:
                    case Nows.Pie:
                        CurrentPage.Items.PCount -= 1;
                        Pie P = (Pie)CurrentPage.Selected[i];
                        ItOfRec.Item = new Pie(P);
                        break;
                    case Nows.Group:
                        CurrentPage.Items.PCount -= 1;
                        GroupItem GI = (GroupItem)CurrentPage.Selected[i];
                        ItOfRec.Item = new GroupItem(GI);
                        break;
                }

                ItOfRec.Name = tscbAlls.Items[ZOrder].ToString();
                RecycleBin.Add(ItOfRec);
                CurrentPage.Items.Remove(ZOrder); //удаляем саму фигуру
                tscbAlls.Items.RemoveAt(ZOrder); // удаляем ее из списка всех фигур
                tscbAlls.Text = "";//в списке не выделена фигура
                WriteCounts();//записывает кол-во всех фигур
                for (int j = ZOrder; j <= CurrentPage.Items.Count - 1; j++)
                {
                    It = (Item)CurrentPage.Items[j];
                    It.ZOrder--;
                }
                CurrentPage.ZOrdCurrent--;
            }

            CurrentPage.Selected.Clear();
            tcTabs.TabPages[tcTabs.SelectedIndex].Refresh();
            CheckEnables();//проверяет состояние кнопок
            PieButtons(false);
        }

        private void fmMain_KeyPress(object sender, KeyPressEventArgs e)
        {

            if ((Keys)e.KeyChar == Keys.Delete)
                MessageBox.Show("Del");
        }

        private void tstbX_TextChanged(object sender, EventArgs e)
        {
            if (!tstbX.Focused) return;
            float NewX = 0;
            try
            {
                NewX = tstbX.Text != "" ? Convert.ToSingle(tstbX.Text) : 0;
            }
            catch
            {
                MessageBox.Show("Error, Invalid value!!!", "GraphicEditor");
            }

            if (CurrentPage.Selected.Count != 0)
            {
                Graphics g = Graphics.FromHwnd(tcTabs.TabPages[tcTabs.SelectedIndex].Handle);
                for (int i = 0; i < CurrentPage.Selected.Count; i++)
                {
                    Item It = (Item)CurrentPage.Selected[i];
                    switch (It.TypeShape)
                    {
                        case Nows.Line:
                            Line L = (Line)CurrentPage.Selected[i];
                            L.Move(NewX - CurrentPage.Selected.X1, 0, Dlg, Dlv, tcTabs.TabPages[tcTabs.SelectedIndex].BackColor, g);

                            break;
                        case Nows.Rec:
                            Rec R = (Rec)CurrentPage.Selected[i];
                            R.Move(NewX - CurrentPage.Selected.X1, 0, Dlg, Dlv, tcTabs.TabPages[tcTabs.SelectedIndex].BackColor, g);
                            break;
                        case Nows.Ellip:
                        case Nows.Pie:
                            Pie P = (Pie)CurrentPage.Selected[i];
                            P.Move(NewX - CurrentPage.Selected.X1, 0, Dlg, Dlv, tcTabs.TabPages[tcTabs.SelectedIndex].BackColor, g);
                            break;

                    }
                }
                CurrentPage.Selected.ReSelect();
                CurrentPage.Selected.Corners.Show(Dlg, Dlv, g);
                tcTabs.TabPages[tcTabs.SelectedIndex].Refresh();
            }
        }

        private void tstbY_TextChanged(object sender, EventArgs e)
        {
            if (!tstbY.Focused) return;
            float NewY = 0;
            try
            {
                if (tstbY != null)
                    NewY = tstbY.Text != "" ? Convert.ToSingle(tstbY.Text) : 0;
            }
            catch
            {
                MessageBox.Show("Error, Invalid value!!!", "GraphicEditor");
            }

            if (CurrentPage.Selected.Count != 0)
            {
                Graphics g = Graphics.FromHwnd(tcTabs.TabPages[tcTabs.SelectedIndex].Handle);
                for (int i = 0; i < CurrentPage.Selected.Count; i++)
                {
                    Item It = (Item)CurrentPage.Selected[i];
                    switch (It.TypeShape)
                    {
                        case Nows.Line:
                            Line L = (Line)CurrentPage.Selected[i];
                            L.Move(0, NewY - CurrentPage.Selected.Y1, Dlg, Dlv, tcTabs.TabPages[tcTabs.SelectedIndex].BackColor, g);

                            break;
                        case Nows.Rec:
                            Rec R = (Rec)CurrentPage.Selected[i];
                            R.Move(0, NewY - CurrentPage.Selected.Y1, Dlg, Dlv, tcTabs.TabPages[tcTabs.SelectedIndex].BackColor, g);
                            break;
                        case Nows.Ellip:
                        case Nows.Pie:
                            Pie P = (Pie)CurrentPage.Selected[i];
                            P.Move(0, NewY - CurrentPage.Selected.Y1, Dlg, Dlv, tcTabs.TabPages[tcTabs.SelectedIndex].BackColor, g);
                            break;
                    }
                }
                CurrentPage.Selected.ReSelect();
                CurrentPage.Selected.Corners.Show(Dlg, Dlv, g);
                tcTabs.TabPages[tcTabs.SelectedIndex].Refresh();
            }
        }

        private void tstbWidth_TextChanged(object sender, EventArgs e)
        {
            if (!tstbWidth.Focused) return;
            float NewX = 0;
            try
            {
                NewX = tstbWidth.Text != "" ? Convert.ToSingle(tstbWidth.Text) : 0;
            }
            catch
            {
                MessageBox.Show("Error, Invalid value!!!", "GraphicEditor");
            }

            if (CurrentPage.Selected.Count != 0)
            {
                Graphics g = Graphics.FromHwnd(tcTabs.TabPages[tcTabs.SelectedIndex].Handle);
                for (int i = 0; i < CurrentPage.Selected.Count; i++)
                {
                    Item It = (Item)CurrentPage.Selected[i];
                    switch (It.TypeShape)
                    {
                        case Nows.Line:
                            Line L = (Line)CurrentPage.Selected[i];
                            L.MoveX2(NewX, Dlg, Dlv, tcTabs.TabPages[tcTabs.SelectedIndex].BackColor, g);
                            break;
                        case Nows.Rec:
                            Rec R = (Rec)CurrentPage.Selected[i];
                            R.MoveX2(NewX, Dlg, Dlv, tcTabs.TabPages[tcTabs.SelectedIndex].BackColor, g);
                            break;
                        case Nows.Ellip:
                        case Nows.Pie:
                            Pie P = (Pie)CurrentPage.Selected[i];
                            P.MoveX2(NewX, Dlg, Dlv, tcTabs.TabPages[tcTabs.SelectedIndex].BackColor, g);
                            break;
                    }
                }
                CurrentPage.Selected.ReSelect();
                CurrentPage.Selected.Corners.Show(Dlg, Dlv, g);
                tcTabs.TabPages[tcTabs.SelectedIndex].Refresh();
            }
        }

        private void tstbHeight_TextChanged(object sender, EventArgs e)
        {
            if (!tstbHeight.Focused) return;
            float NewY = 0;
            try
            {
                NewY = tstbHeight.Text != "" ? Convert.ToSingle(tstbHeight.Text) : 0;
            }
            catch
            {
                MessageBox.Show("Error, Invalid value!!!", "GraphicEditor");
            }

            if (CurrentPage.Selected.Count != 0)
            {
                Graphics g = Graphics.FromHwnd(tcTabs.TabPages[tcTabs.SelectedIndex].Handle);
                for (int i = 0; i < CurrentPage.Selected.Count; i++)
                {
                    Item It = (Item)CurrentPage.Selected[i];
                    switch (It.TypeShape)
                    {
                        case Nows.Line:
                            Line L = (Line)CurrentPage.Selected[i];
                            L.MoveY2(NewY, Dlg, Dlv, tcTabs.TabPages[tcTabs.SelectedIndex].BackColor, g);
                            break;
                        case Nows.Rec:
                            Rec R = (Rec)CurrentPage.Selected[i];
                            R.MoveY2(NewY, Dlg, Dlv, tcTabs.TabPages[tcTabs.SelectedIndex].BackColor, g);
                            break;
                        case Nows.Ellip:
                        case Nows.Pie:
                            Pie P = (Pie)CurrentPage.Selected[i];
                            P.MoveY2(NewY, Dlg, Dlv, tcTabs.TabPages[tcTabs.SelectedIndex].BackColor, g);
                            break;
                    }
                }
                CurrentPage.Selected.ReSelect();
                CurrentPage.Selected.Corners.Show(Dlg, Dlv, g);
                tcTabs.TabPages[tcTabs.SelectedIndex].Refresh();
            }
        }

        private void tscbAlls_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (CurrentPage.Selected.Count != 0)
                CurrentPage.Selected.Corners.Hide(Dlg, Dlv, CurrentPage.G, tcTabs.TabPages[tcTabs.SelectedIndex].BackColor);
            if (tscbAlls.SelectedIndex >= 0 && tscbAlls.SelectedIndex < CurrentPage.Items.Count)
            {
                CurrentPage.Selected.Clear();
                CurrentPage.Selected.Add(CurrentPage.Items[tscbAlls.SelectedIndex]);
            }
            RePainAllItems(true);
            CheckEnables();
        }

        private void tsmiFirst_Click(object sender, EventArgs e)
        {
            if (CurrentPage.Selected.Count == 0) return;
            Item It = (Item)CurrentPage.Selected[0];
            int from = It.ZOrder;
            It.ZOrder = CurrentPage.Items.Count - 1;
            for (int i = from + 1; i < CurrentPage.Items.Count; i++)
            {
                It = (Item)CurrentPage.Items[i];
                It.ZOrder--;
            }
            CurrentPage.Items.Sort();
            string NameOfShape = tscbAlls.Items[from].ToString();
            tscbAlls.Items.RemoveAt(from);
            tscbAlls.Items.Add(NameOfShape);
            tscbAlls.SelectedIndex = tscbAlls.Items.Count - 1;
        }

        private void tsmiLast_Click(object sender, EventArgs e)
        {
            if (CurrentPage.Selected.Count == 0) return;
            Item It = (Item)CurrentPage.Selected[0];
            int upto = It.ZOrder;
            It.ZOrder = 0;
            for (int i = 0; i < upto; i++)
            {
                It = (Item)CurrentPage.Items[i];
                It.ZOrder++;
            }
            CurrentPage.Items.Sort();
            string NameOfShape = tscbAlls.Items[upto].ToString();
            tscbAlls.Items.RemoveAt(upto);
            tscbAlls.Items.Insert(0, NameOfShape);
            tscbAlls.SelectedIndex = 0;
        }

        private void tsmiUp_Click(object sender, EventArgs e)
        {
            if (CurrentPage.Selected.Count == 0) return;
            Item It = (Item)CurrentPage.Selected[0];
            int from = It.ZOrder;
            if (from >= CurrentPage.Items.Count - 1) return;
            It.ZOrder++;
            It = (Item)CurrentPage.Items[from + 1];
            It.ZOrder--;
            CurrentPage.Items.Sort();
            string NameOfShape = tscbAlls.Items[from].ToString();
            tscbAlls.Items.RemoveAt(from);
            tscbAlls.Items.Insert(from + 1, NameOfShape);
            tscbAlls.SelectedIndex = from + 1;
        }

        private void tsmi2Backward_Click(object sender, EventArgs e)
        {
            if (CurrentPage.Selected.Count == 0) return;
            Item It = (Item)CurrentPage.Selected[0];
            int from = It.ZOrder;
            if (from <= 0) return;
            It.ZOrder--;
            It = (Item)CurrentPage.Items[from - 1];
            It.ZOrder++;
            CurrentPage.Items.Sort();
            string NameOfShape = tscbAlls.Items[from].ToString();
            tscbAlls.Items.RemoveAt(from);
            tscbAlls.Items.Insert(from - 1, NameOfShape);
            tscbAlls.SelectedIndex = from - 1;
        }

        private void tsmiAt_Click(object sender, EventArgs e)
        {
            if (CurrentPage.Selected.Count == 0) return;
            if (CurrentPage.Selected.Count == 0) return;
            tcTabs.TabPages[tcTabs.SelectedIndex].Cursor = Cursors.PanEast;
            CurrentPage.DoPos = true;
            CurrentPage.ZOrderBeforeDoPos = CurrentPage.Selected.ZOrder;
        }

        private void tsmiBehind_Click(object sender, EventArgs e)
        {
            if (CurrentPage.Selected.Count == 0) return;
            if (CurrentPage.Selected.Count == 0) return;
            tcTabs.TabPages[tcTabs.SelectedIndex].Cursor = Cursors.PanWest;
            CurrentPage.DoPos = true;
            CurrentPage.ZOrderBeforeDoPos = CurrentPage.Selected.ZOrder;
        }

        private void DoPositionAt(Cursor Cur)
        {
            CurrentPage.DoPos = false;
            tcTabs.TabPages[tcTabs.SelectedIndex].Cursor = Now == Nows.None ? Cursors.SizeAll : Cursors.Cross;
            int NewPos = CurrentPage.Selected.ZOrder;
            Item It = (Item)CurrentPage.Items[CurrentPage.ZOrderBeforeDoPos];
            int from = It.ZOrder;
            It.ZOrder += NewPos - CurrentPage.ZOrderBeforeDoPos;
            if (Cur == Cursors.PanEast)
            {
                if (NewPos <= CurrentPage.ZOrderBeforeDoPos) return;
                if (from >= CurrentPage.Items.Count - 1) return;
                for (int i = from + 1; i <= NewPos; i++)
                {
                    It = (Item)CurrentPage.Items[i];
                    It.ZOrder--;
                }

            }
            else
            {
                if (NewPos >= CurrentPage.ZOrderBeforeDoPos) return;
                if (from <= 0) return;
                for (int i = NewPos; i < CurrentPage.ZOrderBeforeDoPos; i++)
                {
                    It = (Item)CurrentPage.Items[i];
                    It.ZOrder++;
                }
            }
            CurrentPage.Items.Sort();
            string NameOfShape = tscbAlls.Items[from].ToString();
            tscbAlls.Items.RemoveAt(from);
            tscbAlls.Items.Insert(NewPos, NameOfShape);
            tscbAlls.SelectedIndex = NewPos;
        }

        private void tsbRecycle_Click_1(object sender, EventArgs e)
        {
            CtrlPressed = false;
            if (RecycleBin != null)
                FmRec.SetRecycleBin(RecycleBin, WidFm, HeigFm);

            FmRec.ShowDialog();
            if (FmRec.DialogResult == DialogResult.OK)
            {
                ItemOfRecycle ItOfRec;
                Item It;
                for (int i = 0; i < FmRec.ArOfIndexes.Count; i++)
                {
                    if (RecycleBin == null) return;
                    ItOfRec = (ItemOfRecycle)RecycleBin[(int)FmRec.ArOfIndexes[i]];
                    It = (Item)ItOfRec.Item;
                    It.ZOrder = CurrentPage.ZOrdCurrent++;
                    switch (It.TypeShape)
                    {
                        case Nows.Line:
                            Line L = (Line)ItOfRec.Item;
                            CurrentPage.Items.Add(new Line(L));
                            tscbAlls.Items.Add(string.Format(ItOfRec.Name));
                            break;
                        case Nows.Rec:
                            Rec R = (Rec)ItOfRec.Item;
                            CurrentPage.Items.Add(new Rec(R));
                            tscbAlls.Items.Add(string.Format(ItOfRec.Name));
                            break;
                        case Nows.Ellip:
                        case Nows.Pie:
                            Pie P = (Pie)ItOfRec.Item;
                            CurrentPage.Items.Add(new Pie(P));
                            tscbAlls.Items.Add(string.Format(ItOfRec.Name));
                            break;
                        case Nows.Group:
                            GroupItem GI = (GroupItem)ItOfRec.Item;
                            CurrentPage.Items.Add(new GroupItem(GI));
                            tscbAlls.Items.Add(string.Format(ItOfRec.Name));
                            break;
                    }
                }
                for (int i = FmRec.ArOfIndexes.Count - 1; i >= 0; i--)
                {
                    if (RecycleBin == null) return;
                    RecycleBin.Remove((int)FmRec.ArOfIndexes[i]);
                }
                tscbAlls.SelectedIndex = tscbAlls.Items.Count - 1;
                WriteCounts();
                if (CurrentPage.Selected.Count == 1)
                {
                    It = (Item)CurrentPage.Selected[0];
                    if (It.TypeShape == Nows.Pie)
                    {
                        PieButtons(true);
                        Pie P = (Pie)CurrentPage.Selected[0];
                        tstbAlfa.Text = P.startAngle.ToString();
                        tstbBeta.Text = P.sweepAngle.ToString();
                    }
                }
            }
        }

        private void fmMain_KeyUp(object sender, KeyEventArgs e)
        {
            CtrlPressed = e.Control;
        }


        private void fmMain_KeyDown(object sender, KeyEventArgs e)
        {
            CtrlPressed = e.Control;
        }

        private void tsmiHideSelection_Click(object sender, EventArgs e)
        {
            Graphics g = Graphics.FromHwnd(tcTabs.TabPages[tcTabs.SelectedIndex].Handle);
            CurrentPage.Selected.Corners.Hide(Dlg, Dlv, g, tcTabs.TabPages[tcTabs.SelectedIndex].BackColor);
            CurrentPage.Selected.Clear();
            RePainAllItems(true);
        }

        private void tsmiSelectAll_Click(object sender, EventArgs e)
        {
            Graphics g = Graphics.FromHwnd(tcTabs.TabPages[tcTabs.SelectedIndex].Handle);
            CurrentPage.Selected.Corners.Hide(Dlg, Dlv, g, tcTabs.TabPages[tcTabs.SelectedIndex].BackColor);
            CurrentPage.Selected.Clear();
            for (int i = 0; i < CurrentPage.Items.Count; i++)
                CurrentPage.Selected.Add(CurrentPage.Items[i]);
            RePainAllItems(true);
            if (CurrentPage.Selected.Count > 0)
                CurrentPage.Selected.Corners.Show(Dlg, Dlv, g);
        }

        private void tsmiInvertSelection_Click(object sender, EventArgs e)
        {
            ArrayList ArOfIndex = new ArrayList();
            for (int i = 0; i < CurrentPage.Selected.Count; i++)
            {
                Item It = (Item)CurrentPage.Selected[i];
                ArOfIndex.Add(It.ZOrder);
            }

            Graphics g = Graphics.FromHwnd(tcTabs.TabPages[tcTabs.SelectedIndex].Handle);
            CurrentPage.Selected.Corners.Hide(Dlg, Dlv, g, tcTabs.TabPages[tcTabs.SelectedIndex].BackColor);
            CurrentPage.Selected.Clear();
            for (int i = 0; i < CurrentPage.Items.Count; i++)
            {
                Item It = (Item)CurrentPage.Items[i];
                if (!ArOfIndex.Contains(It.ZOrder))
                    CurrentPage.Selected.Add(CurrentPage.Items[i]);
            }
            RePainAllItems(true);
            if (CurrentPage.Selected.Count > 0)
                CurrentPage.Selected.Corners.Show(Dlg, Dlv, g);
        }

        private void tsbAddAlfa_MouseDown(object sender, MouseEventArgs e)
        {
            if (CurrentPage.Selected.Count != 1) return;
            Item It = (Item)CurrentPage.Selected[0];
            if (It.TypeShape != Nows.Pie) return;
            Pie P = (Pie)CurrentPage.Selected[0];
            tstbAlfa.Text = (P.startAngle + 5).ToString();
        }

        private void tsbLessAlfa_Click(object sender, EventArgs e)
        {
            if (CurrentPage.Selected.Count != 1) return;
            Item It = (Item)CurrentPage.Selected[0];
            if (It.TypeShape != Nows.Pie) return;
            Pie P = (Pie)CurrentPage.Selected[0];
            tstbAlfa.Text = (P.startAngle - 5).ToString();
        }

        private void tsbAddBeta_Click(object sender, EventArgs e)
        {
            if (CurrentPage.Selected.Count != 1) return;
            Item It = (Item)CurrentPage.Selected[0];
            if (It.TypeShape != Nows.Pie) return;
            Pie P = (Pie)CurrentPage.Selected[0];
            tstbBeta.Text = (P.sweepAngle + 5).ToString();
        }

        private void tsbLessBeta_Click(object sender, EventArgs e)
        {
            if (CurrentPage.Selected.Count != 1) return;
            Item It = (Item)CurrentPage.Selected[0];
            if (It.TypeShape != Nows.Pie) return;
            Pie P = (Pie)CurrentPage.Selected[0];
            tstbBeta.Text = (P.sweepAngle - 5).ToString();
        }

        private void tstbAlfa_TextChanged(object sender, EventArgs e)
        {
            Graphics g = Graphics.FromHwnd(tcTabs.TabPages[tcTabs.SelectedIndex].Handle);

            if (CurrentPage.Selected.Count != 1) return;
            Item It = (Item)CurrentPage.Selected[0];
            if (It.TypeShape != Nows.Pie) return;
            Pie P = (Pie)CurrentPage.Selected[0];
            float delta;
            try
            {
                if (tstbAlfa.Text == "")
                    return;
                delta = (float)Convert.ToDouble(tstbAlfa.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("Угол должен быть числом!!!");
                tstbAlfa.Text = P.startAngle.ToString();
                return;
            }
            P.RePain(Dlg, Dlv, true, tcTabs.TabPages[tcTabs.SelectedIndex].BackColor, 1, g);
            P.startAngle = delta;
            RePainAllItems(true);
        }

        private void tstbBeta_TextChanged(object sender, EventArgs e)
        {
            Graphics g = Graphics.FromHwnd(tcTabs.TabPages[tcTabs.SelectedIndex].Handle);
            if (CurrentPage.Selected.Count != 1) return;
            Item It = (Item)CurrentPage.Selected[0];
            if (It.TypeShape != Nows.Pie) return;
            Pie P = (Pie)CurrentPage.Selected[0];
            float delta;
            try
            {
                if (tstbBeta.Text == "")
                    return;
                delta = (float)Convert.ToDouble(tstbBeta.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("Угол должен быть числом!!!");
                tstbBeta.Text = P.sweepAngle.ToString();
                return;
            }
            P.RePain(Dlg, Dlv, true, tcTabs.TabPages[tcTabs.SelectedIndex].BackColor, 1, g);
            P.sweepAngle = delta;
            RePainAllItems(true);
        }

        private void tsmiGroup_Click(object sender, EventArgs e)
        {
            if (CurrentPage.Selected.Count <= 1) return;
            GroupItem GI = new GroupItem();
            for (int i = CurrentPage.Selected.Count - 1; i >= 0; i--)
            {
                Item It = (Item)CurrentPage.Selected[i];
                switch (It.TypeShape)
                {
                    case Nows.Line:
                        Line L = (Line)CurrentPage.Selected[i];
                        GI.Add(new Line(L));
                        CurrentPage.Items.RemoveObj(L);
                        CurrentPage.ZOrdCurrent--;
                        break;
                    case Nows.Rec:
                        Rec R = (Rec)CurrentPage.Selected[i];
                        GI.Add(new Rec(R));
                        CurrentPage.Items.RemoveObj(R);
                        CurrentPage.ZOrdCurrent--;
                        break;
                    case Nows.Ellip:
                    case Nows.Pie:
                        Pie P = (Pie)CurrentPage.Selected[i];
                        GI.Add(new Pie(P));
                        CurrentPage.Items.RemoveObj(P);
                        CurrentPage.ZOrdCurrent--;
                        break;
                    case Nows.Group:
                        GroupItem Gr = (GroupItem)CurrentPage.Selected[i];
                        GI.Add(new GroupItem(Gr));
                        CurrentPage.Items.RemoveObj(Gr);
                        CurrentPage.ZOrdCurrent--;
                        break;
                }

            }
            GI.Name = string.Format("Группа {0}", CurrentPage.GC++);
            CurrentPage.Items.Add(GI);
            CurrentPage.ZOrdCurrent++;
            Sort();
            CurrentPage.Selected.Clear();
            CurrentPage.Selected.Add(GI);
            tscbAlls.SelectedIndex = GI.ZOrder;
            CheckEnables();
        }

        private void Sort()
        {
            CurrentPage.Items.Sort();
            tscbAlls.Items.Clear();
            Item It;
            for (int i = 0; i < CurrentPage.Items.Count; i++)
            {
                It = (Item) CurrentPage.Items[i];
                It.ZOrder = i;
                tscbAlls.Items.Add(It.Name);
            }
        }

        private void tsmiUnGroup_Click(object sender, EventArgs e)
        {
            if (CurrentPage.Selected.Count != 1) return;
            Item It = (Item) CurrentPage.Selected[0];
            if (It.TypeShape != Nows.Group) return;
            Graphics g = Graphics.FromHwnd(tcTabs.TabPages[tcTabs.SelectedIndex].Handle);
            CurrentPage.Selected.Corners.Hide(Dlg, Dlv, g, tcTabs.TabPages[tcTabs.SelectedIndex].BackColor);
            GroupItem GR = (GroupItem) CurrentPage.Selected[0];
            for (int i = GR.ZOrder + 1; i < CurrentPage.Items.Count; i++)
            {
                It = (Item) CurrentPage.Items[i];
                It.ZOrder += GR.Count - 1;
            }
            CurrentPage.ZOrdCurrent += GR.Count - 1;
            int ZFromGroup = GR.ZOrder;
            for (int i = 0; i < GR.Count; i++)
            {
                It = (Item)GR[i];
                switch (It.TypeShape)
                {
                    case Nows.Line:
                        Line L = (Line) GR[i];
                        L.ZOrder = ZFromGroup++;
                        CurrentPage.Items.Add(new Line(L));
                        CurrentPage.Items.LCount--;
                        break;
                    case Nows.Rec:
                        Rec R = (Rec)GR[i];
                        R.ZOrder = ZFromGroup++;
                        CurrentPage.Items.Add(new Rec(R));
                        CurrentPage.Items.RCount--;
                        break;
                    case Nows.Ellip:
                    case Nows.Pie:
                        Pie P = (Pie)GR[i];
                        P.ZOrder = ZFromGroup++;
                        CurrentPage.Items.Add(new Pie(P));
                        CurrentPage.Items.PCount--;
                        break;
                    case Nows.Group:
                        GroupItem GI = (GroupItem)GR[i];
                        GI.ZOrder = ZFromGroup++;
                        CurrentPage.Items.Add(new GroupItem(GI));
                        CurrentPage.Items.GICount--;
                        break;
                }
            }
            CurrentPage.Items.RemoveObj(GR);
            CurrentPage.Selected.Clear();
            Sort();
            tscbAlls.SelectedIndex = -1;
            tscbAlls.Text = "";
            RePainAllItems(true);
        }
    }
}