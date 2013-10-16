namespace GraphicEditor
{
    partial class fmRecycle
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(fmRecycle));
            this.clbListOfItems = new System.Windows.Forms.CheckedListBox();
            this.pOut = new System.Windows.Forms.Panel();
            this.bOk = new System.Windows.Forms.Button();
            this.bCancel = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslCount = new System.Windows.Forms.ToolStripStatusLabel();
            this.bSelectAll = new System.Windows.Forms.Button();
            this.bUnselectAll = new System.Windows.Forms.Button();
            this.bDelSelected = new System.Windows.Forms.Button();
            this.bClearBin = new System.Windows.Forms.Button();
            this.cbFill = new System.Windows.Forms.CheckBox();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // clbListOfItems
            // 
            this.clbListOfItems.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.clbListOfItems.CheckOnClick = true;
            this.clbListOfItems.FormattingEnabled = true;
            this.clbListOfItems.Location = new System.Drawing.Point(12, 5);
            this.clbListOfItems.Name = "clbListOfItems";
            this.clbListOfItems.Size = new System.Drawing.Size(212, 274);
            this.clbListOfItems.TabIndex = 0;
            this.clbListOfItems.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.clbListOfItems_ItemCheck);
            this.clbListOfItems.Click += new System.EventHandler(this.clbListOfItems_Click);
            // 
            // pOut
            // 
            this.pOut.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pOut.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pOut.Location = new System.Drawing.Point(230, 5);
            this.pOut.Name = "pOut";
            this.pOut.Size = new System.Drawing.Size(507, 252);
            this.pOut.TabIndex = 1;
            this.pOut.Resize += new System.EventHandler(this.pOut_Resize);
            // 
            // bOk
            // 
            this.bOk.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.bOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bOk.Location = new System.Drawing.Point(384, 304);
            this.bOk.Name = "bOk";
            this.bOk.Size = new System.Drawing.Size(97, 23);
            this.bOk.TabIndex = 2;
            this.bOk.Text = "Восстановить";
            this.bOk.UseVisualStyleBackColor = true;
            this.bOk.Click += new System.EventHandler(this.bOk_Click);
            // 
            // bCancel
            // 
            this.bCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bCancel.Location = new System.Drawing.Point(532, 304);
            this.bCancel.Name = "bCancel";
            this.bCancel.Size = new System.Drawing.Size(75, 23);
            this.bCancel.TabIndex = 3;
            this.bCancel.Text = "Выход";
            this.bCancel.UseVisualStyleBackColor = true;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.tsslCount});
            this.statusStrip1.Location = new System.Drawing.Point(0, 342);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(749, 22);
            this.statusStrip1.TabIndex = 4;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(189, 17);
            this.toolStripStatusLabel1.Text = "Количество объектов в корзине: ";
            // 
            // tsslCount
            // 
            this.tsslCount.Name = "tsslCount";
            this.tsslCount.Size = new System.Drawing.Size(118, 17);
            this.tsslCount.Text = "toolStripStatusLabel2";
            // 
            // bSelectAll
            // 
            this.bSelectAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bSelectAll.Location = new System.Drawing.Point(12, 285);
            this.bSelectAll.Name = "bSelectAll";
            this.bSelectAll.Size = new System.Drawing.Size(90, 23);
            this.bSelectAll.TabIndex = 5;
            this.bSelectAll.Text = "Отметить все";
            this.bSelectAll.UseVisualStyleBackColor = true;
            this.bSelectAll.Click += new System.EventHandler(this.bSelectAll_Click);
            // 
            // bUnselectAll
            // 
            this.bUnselectAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bUnselectAll.Location = new System.Drawing.Point(108, 285);
            this.bUnselectAll.Name = "bUnselectAll";
            this.bUnselectAll.Size = new System.Drawing.Size(116, 23);
            this.bUnselectAll.TabIndex = 6;
            this.bUnselectAll.Text = "Убрать выделение";
            this.bUnselectAll.UseVisualStyleBackColor = true;
            this.bUnselectAll.Click += new System.EventHandler(this.bUnselectAll_Click);
            // 
            // bDelSelected
            // 
            this.bDelSelected.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bDelSelected.Location = new System.Drawing.Point(12, 314);
            this.bDelSelected.Name = "bDelSelected";
            this.bDelSelected.Size = new System.Drawing.Size(90, 23);
            this.bDelSelected.TabIndex = 7;
            this.bDelSelected.Text = "Удалить";
            this.bDelSelected.UseVisualStyleBackColor = true;
            this.bDelSelected.Click += new System.EventHandler(this.bDelSelected_Click);
            // 
            // bClearBin
            // 
            this.bClearBin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bClearBin.Location = new System.Drawing.Point(108, 314);
            this.bClearBin.Name = "bClearBin";
            this.bClearBin.Size = new System.Drawing.Size(116, 23);
            this.bClearBin.TabIndex = 8;
            this.bClearBin.Text = "Отчистить корзину";
            this.bClearBin.UseVisualStyleBackColor = true;
            this.bClearBin.Click += new System.EventHandler(this.bClearBin_Click);
            // 
            // cbFill
            // 
            this.cbFill.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbFill.AutoSize = true;
            this.cbFill.Checked = true;
            this.cbFill.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbFill.Location = new System.Drawing.Point(230, 263);
            this.cbFill.Name = "cbFill";
            this.cbFill.Size = new System.Drawing.Size(135, 17);
            this.cbFill.TabIndex = 9;
            this.cbFill.Text = "Закрашивать фигуры";
            this.cbFill.UseVisualStyleBackColor = true;
            this.cbFill.CheckedChanged += new System.EventHandler(this.cbFill_CheckedChanged);
            // 
            // fmRecycle
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(749, 364);
            this.Controls.Add(this.cbFill);
            this.Controls.Add(this.bClearBin);
            this.Controls.Add(this.bDelSelected);
            this.Controls.Add(this.bUnselectAll);
            this.Controls.Add(this.bSelectAll);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.bCancel);
            this.Controls.Add(this.bOk);
            this.Controls.Add(this.pOut);
            this.Controls.Add(this.clbListOfItems);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(500, 0);
            this.Name = "fmRecycle";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Корзина";
            this.Load += new System.EventHandler(this.fmRecycle_Load);
            this.Shown += new System.EventHandler(this.fmRecycle_Shown);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckedListBox clbListOfItems;
        private System.Windows.Forms.Panel pOut;
        private System.Windows.Forms.Button bOk;
        private System.Windows.Forms.Button bCancel;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel tsslCount;
        private System.Windows.Forms.Button bSelectAll;
        private System.Windows.Forms.Button bUnselectAll;
        private System.Windows.Forms.Button bDelSelected;
        private System.Windows.Forms.Button bClearBin;
        private System.Windows.Forms.CheckBox cbFill;
    }
}