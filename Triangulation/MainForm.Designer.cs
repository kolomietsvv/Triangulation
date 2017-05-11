namespace Triangulation
{
    public partial class MainForm
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
            this.components = new System.ComponentModel.Container();
            this.ImgBox = new Emgu.CV.UI.ImageBox();
            this.MenuStrip = new System.Windows.Forms.MenuStrip();
            this.FileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.OpenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Footer = new System.Windows.Forms.Panel();
            this.EdgesCount = new System.Windows.Forms.Label();
            this.NodesCount = new System.Windows.Forms.Label();
            this.TrianglesCount = new System.Windows.Forms.Label();
            this.MaxTriangleCount = new System.Windows.Forms.Label();
            this.MinTriangleCount = new System.Windows.Forms.Label();
            this.CoordinateY = new System.Windows.Forms.Label();
            this.CoordinateX = new System.Windows.Forms.Label();
            this.MaxTriangleSizeLabel = new System.Windows.Forms.Label();
            this.MinTriangleSizeLabel = new System.Windows.Forms.Label();
            this.EdgesCountLabel = new System.Windows.Forms.Label();
            this.NodesCountLabel = new System.Windows.Forms.Label();
            this.TrianglesCountLabel = new System.Windows.Forms.Label();
            this.CoordinateYLabel = new System.Windows.Forms.Label();
            this.CoordinateXLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.ImgBox)).BeginInit();
            this.MenuStrip.SuspendLayout();
            this.Footer.SuspendLayout();
            this.SuspendLayout();
            // 
            // ImgBox
            // 
            this.ImgBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ImgBox.Location = new System.Drawing.Point(0, 24);
            this.ImgBox.Margin = new System.Windows.Forms.Padding(10);
            this.ImgBox.Name = "ImgBox";
            this.ImgBox.Size = new System.Drawing.Size(834, 538);
            this.ImgBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.ImgBox.TabIndex = 2;
            this.ImgBox.TabStop = false;
            // 
            // MenuStrip
            // 
            this.MenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FileToolStripMenuItem});
            this.MenuStrip.Location = new System.Drawing.Point(0, 0);
            this.MenuStrip.Name = "MenuStrip";
            this.MenuStrip.Size = new System.Drawing.Size(834, 24);
            this.MenuStrip.TabIndex = 3;
            this.MenuStrip.Text = "menuStrip1";
            // 
            // FileToolStripMenuItem
            // 
            this.FileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.OpenToolStripMenuItem});
            this.FileToolStripMenuItem.Name = "FileToolStripMenuItem";
            this.FileToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.FileToolStripMenuItem.Text = "Файл";
            // 
            // OpenToolStripMenuItem
            // 
            this.OpenToolStripMenuItem.Name = "OpenToolStripMenuItem";
            this.OpenToolStripMenuItem.Size = new System.Drawing.Size(121, 22);
            this.OpenToolStripMenuItem.Text = "Открыть";
            // 
            // Footer
            // 
            this.Footer.Controls.Add(this.EdgesCount);
            this.Footer.Controls.Add(this.NodesCount);
            this.Footer.Controls.Add(this.TrianglesCount);
            this.Footer.Controls.Add(this.MaxTriangleCount);
            this.Footer.Controls.Add(this.MinTriangleCount);
            this.Footer.Controls.Add(this.CoordinateY);
            this.Footer.Controls.Add(this.CoordinateX);
            this.Footer.Controls.Add(this.MaxTriangleSizeLabel);
            this.Footer.Controls.Add(this.MinTriangleSizeLabel);
            this.Footer.Controls.Add(this.EdgesCountLabel);
            this.Footer.Controls.Add(this.NodesCountLabel);
            this.Footer.Controls.Add(this.TrianglesCountLabel);
            this.Footer.Controls.Add(this.CoordinateYLabel);
            this.Footer.Controls.Add(this.CoordinateXLabel);
            this.Footer.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.Footer.Location = new System.Drawing.Point(0, 512);
            this.Footer.Name = "Footer";
            this.Footer.Size = new System.Drawing.Size(834, 50);
            this.Footer.TabIndex = 4;
            // 
            // EdgesCount
            // 
            this.EdgesCount.AutoSize = true;
            this.EdgesCount.Location = new System.Drawing.Point(442, 17);
            this.EdgesCount.Name = "EdgesCount";
            this.EdgesCount.Size = new System.Drawing.Size(0, 13);
            this.EdgesCount.TabIndex = 10;
            // 
            // NodesCount
            // 
            this.NodesCount.AutoSize = true;
            this.NodesCount.Location = new System.Drawing.Point(331, 17);
            this.NodesCount.Name = "NodesCount";
            this.NodesCount.Size = new System.Drawing.Size(0, 13);
            this.NodesCount.TabIndex = 9;
            // 
            // TrianglesCount
            // 
            this.TrianglesCount.AutoSize = true;
            this.TrianglesCount.Location = new System.Drawing.Point(213, 17);
            this.TrianglesCount.Name = "TrianglesCount";
            this.TrianglesCount.Size = new System.Drawing.Size(0, 13);
            this.TrianglesCount.TabIndex = 5;
            // 
            // MaxTriangleCount
            // 
            this.MaxTriangleCount.AutoSize = true;
            this.MaxTriangleCount.Location = new System.Drawing.Point(735, 28);
            this.MaxTriangleCount.Name = "MaxTriangleCount";
            this.MaxTriangleCount.Size = new System.Drawing.Size(0, 13);
            this.MaxTriangleCount.TabIndex = 8;
            // 
            // MinTriangleCount
            // 
            this.MinTriangleCount.AutoSize = true;
            this.MinTriangleCount.Location = new System.Drawing.Point(729, 6);
            this.MinTriangleCount.Name = "MinTriangleCount";
            this.MinTriangleCount.Size = new System.Drawing.Size(0, 13);
            this.MinTriangleCount.TabIndex = 8;
            // 
            // CoordinateY
            // 
            this.CoordinateY.AutoSize = true;
            this.CoordinateY.Location = new System.Drawing.Point(35, 28);
            this.CoordinateY.Name = "CoordinateY";
            this.CoordinateY.Size = new System.Drawing.Size(0, 13);
            this.CoordinateY.TabIndex = 7;
            // 
            // CoordinateX
            // 
            this.CoordinateX.AutoSize = true;
            this.CoordinateX.Location = new System.Drawing.Point(35, 6);
            this.CoordinateX.Name = "CoordinateX";
            this.CoordinateX.Size = new System.Drawing.Size(0, 13);
            this.CoordinateX.TabIndex = 5;
            // 
            // MaxTriangleSizeLabel
            // 
            this.MaxTriangleSizeLabel.AutoSize = true;
            this.MaxTriangleSizeLabel.Location = new System.Drawing.Point(533, 28);
            this.MaxTriangleSizeLabel.Name = "MaxTriangleSizeLabel";
            this.MaxTriangleSizeLabel.Size = new System.Drawing.Size(202, 13);
            this.MaxTriangleSizeLabel.TabIndex = 6;
            this.MaxTriangleSizeLabel.Text = "Максимальный размер треугольника:";
            // 
            // MinTriangleSizeLabel
            // 
            this.MinTriangleSizeLabel.AutoSize = true;
            this.MinTriangleSizeLabel.Location = new System.Drawing.Point(533, 6);
            this.MinTriangleSizeLabel.Name = "MinTriangleSizeLabel";
            this.MinTriangleSizeLabel.Size = new System.Drawing.Size(196, 13);
            this.MinTriangleSizeLabel.TabIndex = 5;
            this.MinTriangleSizeLabel.Text = "Минимальный размер треугольника:";
            // 
            // EdgesCountLabel
            // 
            this.EdgesCountLabel.AutoSize = true;
            this.EdgesCountLabel.Location = new System.Drawing.Point(365, 17);
            this.EdgesCountLabel.Name = "EdgesCountLabel";
            this.EdgesCountLabel.Size = new System.Drawing.Size(77, 13);
            this.EdgesCountLabel.TabIndex = 4;
            this.EdgesCountLabel.Text = "Кол-во ребер:";
            // 
            // NodesCountLabel
            // 
            this.NodesCountLabel.AutoSize = true;
            this.NodesCountLabel.Location = new System.Drawing.Point(255, 17);
            this.NodesCountLabel.Name = "NodesCountLabel";
            this.NodesCountLabel.Size = new System.Drawing.Size(76, 13);
            this.NodesCountLabel.TabIndex = 3;
            this.NodesCountLabel.Text = "Кол-во узлов:";
            // 
            // TrianglesCountLabel
            // 
            this.TrianglesCountLabel.AutoSize = true;
            this.TrianglesCountLabel.Location = new System.Drawing.Point(91, 17);
            this.TrianglesCountLabel.Name = "TrianglesCountLabel";
            this.TrianglesCountLabel.Size = new System.Drawing.Size(122, 13);
            this.TrianglesCountLabel.TabIndex = 2;
            this.TrianglesCountLabel.Text = "Кол-во треугольников:";
            // 
            // CoordinateYLabel
            // 
            this.CoordinateYLabel.AutoSize = true;
            this.CoordinateYLabel.Location = new System.Drawing.Point(12, 28);
            this.CoordinateYLabel.Name = "CoordinateYLabel";
            this.CoordinateYLabel.Size = new System.Drawing.Size(17, 13);
            this.CoordinateYLabel.TabIndex = 1;
            this.CoordinateYLabel.Text = "Y:";
            // 
            // CoordinateXLabel
            // 
            this.CoordinateXLabel.AutoSize = true;
            this.CoordinateXLabel.Location = new System.Drawing.Point(12, 6);
            this.CoordinateXLabel.Name = "CoordinateXLabel";
            this.CoordinateXLabel.Size = new System.Drawing.Size(17, 13);
            this.CoordinateXLabel.TabIndex = 0;
            this.CoordinateXLabel.Text = "X:";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(834, 562);
            this.Controls.Add(this.Footer);
            this.Controls.Add(this.ImgBox);
            this.Controls.Add(this.MenuStrip);
            this.MainMenuStrip = this.MenuStrip;
            this.MinimumSize = new System.Drawing.Size(850, 600);
            this.Name = "MainForm";
            this.Text = "MainForm";
            ((System.ComponentModel.ISupportInitialize)(this.ImgBox)).EndInit();
            this.MenuStrip.ResumeLayout(false);
            this.MenuStrip.PerformLayout();
            this.Footer.ResumeLayout(false);
            this.Footer.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public Emgu.CV.UI.ImageBox ImgBox;
        public System.Windows.Forms.MenuStrip MenuStrip;
        public System.Windows.Forms.ToolStripMenuItem FileToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem OpenToolStripMenuItem;
        private System.Windows.Forms.Panel Footer;
        private System.Windows.Forms.Label MaxTriangleSizeLabel;
        private System.Windows.Forms.Label MinTriangleSizeLabel;
        private System.Windows.Forms.Label EdgesCountLabel;
        private System.Windows.Forms.Label NodesCountLabel;
        private System.Windows.Forms.Label TrianglesCountLabel;
        private System.Windows.Forms.Label CoordinateYLabel;
        private System.Windows.Forms.Label CoordinateXLabel;
        public System.Windows.Forms.Label TrianglesCount;
        public System.Windows.Forms.Label CoordinateX;
        public System.Windows.Forms.Label CoordinateY;
        public System.Windows.Forms.Label MaxTriangleCount;
        public System.Windows.Forms.Label MinTriangleCount;
        public System.Windows.Forms.Label EdgesCount;
        public System.Windows.Forms.Label NodesCount;
    }
}