namespace GIS_jia1._1
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            menuStrip1 = new MenuStrip();
            打开ToolStripMenuItem = new ToolStripMenuItem();
            清除所有数据ToolStripMenuItem = new ToolStripMenuItem();
            listBox1 = new ListBox();
            contextMenuStrip1 = new ContextMenuStrip(components);
            另存为ToolStripMenuItem = new ToolStripMenuItem();
            geoTIFFToolStripMenuItem = new ToolStripMenuItem();
            jEPGToolStripMenuItem = new ToolStripMenuItem();
            pNGToolStripMenuItem = new ToolStripMenuItem();
            iMGToolStripMenuItem = new ToolStripMenuItem();
            差分编码ToolStripMenuItem = new ToolStripMenuItem();
            差分解码ToolStripMenuItem = new ToolStripMenuItem();
            行首解码ToolStripMenuItem = new ToolStripMenuItem();
            列首解码ToolStripMenuItem = new ToolStripMenuItem();
            removeToolStripMenuItem = new ToolStripMenuItem();
            panel1 = new Doublebuffer();
            menuStrip1.SuspendLayout();
            contextMenuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { 打开ToolStripMenuItem, 清除所有数据ToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(800, 25);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // 打开ToolStripMenuItem
            // 
            打开ToolStripMenuItem.Name = "打开ToolStripMenuItem";
            打开ToolStripMenuItem.Size = new Size(44, 21);
            打开ToolStripMenuItem.Text = "打开";
            打开ToolStripMenuItem.Click += 打开图像ToolStripMenuItem_Click;
            // 
            // 清除所有数据ToolStripMenuItem
            // 
            清除所有数据ToolStripMenuItem.Name = "清除所有数据ToolStripMenuItem";
            清除所有数据ToolStripMenuItem.Size = new Size(92, 21);
            清除所有数据ToolStripMenuItem.Text = "清除所有数据";
            清除所有数据ToolStripMenuItem.Click += 清除所有数据ToolStripMenuItem_Click;
            // 
            // listBox1
            // 
            listBox1.ContextMenuStrip = contextMenuStrip1;
            listBox1.Dock = DockStyle.Left;
            listBox1.FormattingEnabled = true;
            listBox1.ItemHeight = 17;
            listBox1.Location = new Point(0, 25);
            listBox1.Name = "listBox1";
            listBox1.Size = new Size(120, 425);
            listBox1.TabIndex = 1;
            listBox1.SelectedIndexChanged += listBox1_SelectedIndexChanged;
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.Items.AddRange(new ToolStripItem[] { 另存为ToolStripMenuItem, 差分编码ToolStripMenuItem, 差分解码ToolStripMenuItem, removeToolStripMenuItem });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(125, 92);
            // 
            // 另存为ToolStripMenuItem
            // 
            另存为ToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { geoTIFFToolStripMenuItem, jEPGToolStripMenuItem, pNGToolStripMenuItem, iMGToolStripMenuItem });
            另存为ToolStripMenuItem.Name = "另存为ToolStripMenuItem";
            另存为ToolStripMenuItem.Size = new Size(124, 22);
            另存为ToolStripMenuItem.Text = "另存为";
            // 
            // geoTIFFToolStripMenuItem
            // 
            geoTIFFToolStripMenuItem.Name = "geoTIFFToolStripMenuItem";
            geoTIFFToolStripMenuItem.Size = new Size(123, 22);
            geoTIFFToolStripMenuItem.Text = "GeoTIFF";
            geoTIFFToolStripMenuItem.Click += geoTIFFToolStripMenuItem_Click;
            // 
            // jEPGToolStripMenuItem
            // 
            jEPGToolStripMenuItem.Name = "jEPGToolStripMenuItem";
            jEPGToolStripMenuItem.Size = new Size(123, 22);
            jEPGToolStripMenuItem.Text = "JEPG";
            jEPGToolStripMenuItem.Click += jEPGToolStripMenuItem_Click;
            // 
            // pNGToolStripMenuItem
            // 
            pNGToolStripMenuItem.Name = "pNGToolStripMenuItem";
            pNGToolStripMenuItem.Size = new Size(123, 22);
            pNGToolStripMenuItem.Text = "PNG";
            pNGToolStripMenuItem.Click += pNGToolStripMenuItem_Click;
            // 
            // iMGToolStripMenuItem
            // 
            iMGToolStripMenuItem.Name = "iMGToolStripMenuItem";
            iMGToolStripMenuItem.Size = new Size(123, 22);
            iMGToolStripMenuItem.Text = "IMG";
            iMGToolStripMenuItem.Click += iMGToolStripMenuItem_Click;
            // 
            // 差分编码ToolStripMenuItem
            // 
            差分编码ToolStripMenuItem.Name = "差分编码ToolStripMenuItem";
            差分编码ToolStripMenuItem.Size = new Size(124, 22);
            差分编码ToolStripMenuItem.Text = "差分编码";
            差分编码ToolStripMenuItem.Click += 差分编码ToolStripMenuItem_Click;
            // 
            // 差分解码ToolStripMenuItem
            // 
            差分解码ToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { 行首解码ToolStripMenuItem, 列首解码ToolStripMenuItem });
            差分解码ToolStripMenuItem.Name = "差分解码ToolStripMenuItem";
            差分解码ToolStripMenuItem.Size = new Size(124, 22);
            差分解码ToolStripMenuItem.Text = "差分解码";
            // 
            // 行首解码ToolStripMenuItem
            // 
            行首解码ToolStripMenuItem.Name = "行首解码ToolStripMenuItem";
            行首解码ToolStripMenuItem.Size = new Size(180, 22);
            行首解码ToolStripMenuItem.Text = "行首解码";
            行首解码ToolStripMenuItem.Click += 行首解码ToolStripMenuItem_Click;
            // 
            // 列首解码ToolStripMenuItem
            // 
            列首解码ToolStripMenuItem.Name = "列首解码ToolStripMenuItem";
            列首解码ToolStripMenuItem.Size = new Size(180, 22);
            列首解码ToolStripMenuItem.Text = "列首解码";
            列首解码ToolStripMenuItem.Click += 列首解码ToolStripMenuItem_Click;
            // 
            // removeToolStripMenuItem
            // 
            removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            removeToolStripMenuItem.Size = new Size(124, 22);
            removeToolStripMenuItem.Text = "Remove";
            removeToolStripMenuItem.Click += removeToolStripMenuItem_Click;
            // 
            // panel1
            // 
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(120, 25);
            panel1.Name = "panel1";
            panel1.Size = new Size(680, 425);
            panel1.TabIndex = 2;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(panel1);
            Controls.Add(listBox1);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "Form1";
            Text = "Form1";
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            contextMenuStrip1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem 打开ToolStripMenuItem;
        public ListBox listBox1;
        public GIS_jia1._1.Doublebuffer panel1;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem 另存为ToolStripMenuItem;
        private ToolStripMenuItem geoTIFFToolStripMenuItem;
        private ToolStripMenuItem jEPGToolStripMenuItem;
        private ToolStripMenuItem pNGToolStripMenuItem;
        private ToolStripMenuItem iMGToolStripMenuItem;
        private ToolStripMenuItem 差分编码ToolStripMenuItem;
        private ToolStripMenuItem 清除所有数据ToolStripMenuItem;
        private ToolStripMenuItem removeToolStripMenuItem;
        private ToolStripMenuItem 差分解码ToolStripMenuItem;
        private ToolStripMenuItem 行首解码ToolStripMenuItem;
        private ToolStripMenuItem 列首解码ToolStripMenuItem;
    }
}
