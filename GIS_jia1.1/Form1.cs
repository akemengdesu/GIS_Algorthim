using OSGeo.GDAL;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;

namespace GIS_jia1._1
{
    public partial class Form1 : Form
    {
        private DataCenter _dataCenter = new DataCenter();
        private MainController _controller;

        public Form1()
        {
            InitializeComponent();

            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                 ControlStyles.UserPaint |
                 ControlStyles.OptimizedDoubleBuffer, true);

            _controller = new MainController(this, _dataCenter);

            // 订阅事件
            panel1.Paint += (s, e) => _controller.HandlePaint(e);
            panel1.MouseDown += (s, e) => _controller.HandleMouseDown(e);
            panel1.MouseMove += (s, e) => _controller.HandleMouseMove(e);
            panel1.MouseUp += (s, e) => _controller.HandleMouseUp(e);
            panel1.MouseWheel += (s, e) => _controller.HandleMouseWheel(e);
            panel1.Resize += (s, e) => _controller.HandleResize();
        }

        #region Windows 窗体设计器生成的代码
        private void 打开图像ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _controller.LoadRasterImage();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            _controller.HandleListBoxSelection();
        }

        private void geoTIFFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _controller.SaveCurrentDataset("GTiff");
        }

        private void jEPGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _controller.SaveCurrentDataset("JPEG");
        }

        private void pNGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _controller.SaveCurrentDataset("PNG");
        }

        private void iMGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _controller.SaveCurrentDataset("HFA");
        }

        #endregion

        private void 差分编码ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _controller.DifferEncoding();
        }

        private void 清除所有数据ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _controller.ClearAllDatasets();
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _controller.CloseCurrentDataset();
        }
    }
}