using Emgu.CV;
using Emgu.CV.Structure;
using System.Collections.Generic;
using System.Drawing;
using static Triangulation.Program;

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
            this.ROIOffset = Point.Empty;
            this.edges = new List<Point>();
            this.nodes = new List<PointF>();
            this.trianglesData = new List<TriangleData>();
            this.components = new System.ComponentModel.Container();
            this.ImgBox = new Emgu.CV.UI.ImageBox();
            this.MenuStrip = new System.Windows.Forms.MenuStrip();
            this.FileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.OpenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.ColorsMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.Color0 = new System.Windows.Forms.ToolStripMenuItem();
            this.Color1 = new System.Windows.Forms.ToolStripMenuItem();
            this.Color2 = new System.Windows.Forms.ToolStripMenuItem();
            this.Color3 = new System.Windows.Forms.ToolStripMenuItem();
            this.Color4 = new System.Windows.Forms.ToolStripMenuItem();
            this.Color5 = new System.Windows.Forms.ToolStripMenuItem();
            this.Color6 = new System.Windows.Forms.ToolStripMenuItem();
            this.Color7 = new System.Windows.Forms.ToolStripMenuItem();
            this.Color8 = new System.Windows.Forms.ToolStripMenuItem();
            this.Color9 = new System.Windows.Forms.ToolStripMenuItem();
            this.Color10 = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.ImgBox)).BeginInit();
            this.MenuStrip.SuspendLayout();
            this.Footer.SuspendLayout();
            this.ColorsMenu.SuspendLayout();
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
            this.OpenToolStripMenuItem,
            this.SaveAsToolStripMenuItem});
            this.FileToolStripMenuItem.Name = "FileToolStripMenuItem";
            this.FileToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.FileToolStripMenuItem.Text = "Файл";
            // 
            // OpenToolStripMenuItem
            // 
            this.OpenToolStripMenuItem.Name = "OpenToolStripMenuItem";
            this.OpenToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.OpenToolStripMenuItem.Text = "Открыть";
            // 
            // SaveAsToolStripMenuItem
            // 
            this.SaveAsToolStripMenuItem.Name = "SaveAsToolStripMenuItem";
            this.SaveAsToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.SaveAsToolStripMenuItem.Text = "Сохранить как...";
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
            this.Footer.Location = new System.Drawing.Point(0, 517);
            this.Footer.Name = "Footer";
            this.Footer.Size = new System.Drawing.Size(834, 45);
            this.Footer.TabIndex = 4;
            // 
            // EdgesCount
            // 
            this.EdgesCount.AutoSize = true;
            this.EdgesCount.Location = new System.Drawing.Point(462, 17);
            this.EdgesCount.Name = "EdgesCount";
            this.EdgesCount.Size = new System.Drawing.Size(0, 13);
            this.EdgesCount.TabIndex = 10;
            // 
            // NodesCount
            // 
            this.NodesCount.AutoSize = true;
            this.NodesCount.Location = new System.Drawing.Point(341, 17);
            this.NodesCount.Name = "NodesCount";
            this.NodesCount.Size = new System.Drawing.Size(0, 13);
            this.NodesCount.TabIndex = 9;
            // 
            // TrianglesCount
            // 
            this.TrianglesCount.AutoSize = true;
            this.TrianglesCount.Location = new System.Drawing.Point(223, 17);
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
            this.EdgesCountLabel.Location = new System.Drawing.Point(385, 17);
            this.EdgesCountLabel.Name = "EdgesCountLabel";
            this.EdgesCountLabel.Size = new System.Drawing.Size(77, 13);
            this.EdgesCountLabel.TabIndex = 4;
            this.EdgesCountLabel.Text = "Кол-во ребер:";
            // 
            // NodesCountLabel
            // 
            this.NodesCountLabel.AutoSize = true;
            this.NodesCountLabel.Location = new System.Drawing.Point(265, 17);
            this.NodesCountLabel.Name = "NodesCountLabel";
            this.NodesCountLabel.Size = new System.Drawing.Size(76, 13);
            this.NodesCountLabel.TabIndex = 3;
            this.NodesCountLabel.Text = "Кол-во узлов:";
            // 
            // TrianglesCountLabel
            // 
            this.TrianglesCountLabel.AutoSize = true;
            this.TrianglesCountLabel.Location = new System.Drawing.Point(100, 17);
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
            // ColorsMenu
            // 
            this.ColorsMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Color0,
            this.Color1,
            this.Color2,
            this.Color3,
            this.Color4,
            this.Color5,
            this.Color6,
            this.Color7,
            this.Color8,
            this.Color9,
            this.Color10});
            this.ColorsMenu.Name = "ColorsMenu";
            this.ColorsMenu.ShowImageMargin = false;
            this.ColorsMenu.Size = new System.Drawing.Size(62, 246);
            // 
            // Color0
            // 
            this.Color0.BackColor = System.Drawing.Color.Gray;
            this.Color0.ForeColor = System.Drawing.Color.Gray;
            this.Color0.ImageTransparentColor = System.Drawing.Color.Gray;
            this.Color0.Name = "Color0";
            this.Color0.Size = new System.Drawing.Size(61, 22);
            this.Color0.Text = "0";
            // 
            // Color1
            // 
            this.Color1.BackColor = System.Drawing.Color.Orange;
            this.Color1.ForeColor = System.Drawing.Color.Orange;
            this.Color1.Name = "Color1";
            this.Color1.Size = new System.Drawing.Size(61, 22);
            this.Color1.Text = "1";
            // 
            // Color2
            // 
            this.Color2.BackColor = System.Drawing.Color.Blue;
            this.Color2.ForeColor = System.Drawing.Color.Blue;
            this.Color2.Name = "Color2";
            this.Color2.Size = new System.Drawing.Size(61, 22);
            this.Color2.Text = "2";
            // 
            // Color3
            // 
            this.Color3.BackColor = System.Drawing.Color.Yellow;
            this.Color3.ForeColor = System.Drawing.Color.Yellow;
            this.Color3.Name = "Color3";
            this.Color3.Size = new System.Drawing.Size(61, 22);
            this.Color3.Text = "3";
            // 
            // Color4
            // 
            this.Color4.BackColor = System.Drawing.Color.Red;
            this.Color4.ForeColor = System.Drawing.Color.Red;
            this.Color4.Name = "Color4";
            this.Color4.Size = new System.Drawing.Size(61, 22);
            this.Color4.Text = "4";
            // 
            // Color5
            // 
            this.Color5.BackColor = System.Drawing.Color.RoyalBlue;
            this.Color5.ForeColor = System.Drawing.Color.RoyalBlue;
            this.Color5.Name = "Color5";
            this.Color5.Size = new System.Drawing.Size(61, 22);
            this.Color5.Text = "5";
            // 
            // Color6
            // 
            this.Color6.BackColor = System.Drawing.Color.LawnGreen;
            this.Color6.ForeColor = System.Drawing.Color.LawnGreen;
            this.Color6.Name = "Color6";
            this.Color6.Size = new System.Drawing.Size(61, 22);
            this.Color6.Text = "6";
            // 
            // Color7
            // 
            this.Color7.BackColor = System.Drawing.Color.ForestGreen;
            this.Color7.ForeColor = System.Drawing.Color.ForestGreen;
            this.Color7.Name = "Color7";
            this.Color7.Size = new System.Drawing.Size(61, 22);
            this.Color7.Text = "7";
            // 
            // Color8
            // 
            this.Color8.BackColor = System.Drawing.Color.DarkGreen;
            this.Color8.ForeColor = System.Drawing.Color.DarkGreen;
            this.Color8.Name = "Color8";
            this.Color8.Size = new System.Drawing.Size(61, 22);
            this.Color8.Text = "8";
            // 
            // Color9
            // 
            this.Color9.BackColor = System.Drawing.Color.DarkBlue;
            this.Color9.ForeColor = System.Drawing.Color.DarkBlue;
            this.Color9.Name = "Color9";
            this.Color9.Size = new System.Drawing.Size(61, 22);
            this.Color9.Text = "9";
            // 
            // Color10
            // 
            this.Color10.BackColor = System.Drawing.Color.Cyan;
            this.Color10.ForeColor = System.Drawing.Color.Cyan;
            this.Color10.Name = "Color10";
            this.Color10.Size = new System.Drawing.Size(61, 22);
            this.Color10.Text = "10";
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
            this.ColorsMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public Emgu.CV.UI.ImageBox ImgBox;
        public System.Windows.Forms.MenuStrip MenuStrip;
        public System.Windows.Forms.ToolStripMenuItem FileToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem OpenToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem SaveAsToolStripMenuItem;
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
        public System.Windows.Forms.Panel Footer;
        public System.Windows.Forms.ContextMenuStrip ColorsMenu;
        public System.Windows.Forms.ToolStripMenuItem Color0;
        public System.Windows.Forms.ToolStripMenuItem Color1;
        public System.Windows.Forms.ToolStripMenuItem Color2;
        public System.Windows.Forms.ToolStripMenuItem Color3;
        public System.Windows.Forms.ToolStripMenuItem Color4;
        public System.Windows.Forms.ToolStripMenuItem Color5;
        public System.Windows.Forms.ToolStripMenuItem Color6;
        public System.Windows.Forms.ToolStripMenuItem Color7;
        public System.Windows.Forms.ToolStripMenuItem Color8;
        public System.Windows.Forms.ToolStripMenuItem Color9;
        public System.Windows.Forms.ToolStripMenuItem Color10;

        public List<Point> edges;
        public List<PointF> nodes;
        public List<TriangleData> trianglesData;
        public int scale = 200;
        public Image<Bgr, byte> img;
        public Point mouseDownLocation;
        public int selectedColorId;
        public Point ROIOffset;

        //private static Rectangle roi;
    }
}