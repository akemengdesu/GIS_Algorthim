using OSGeo.GDAL;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;

namespace GIS_jia1._1
{
    internal class MainController
    {
        private DataCenter _dataCenter;
        private Form1 _form;
        private Bitmap _currentImage;
        private float _zoom = 1.0f;
        private Point _offset = Point.Empty;
        private Point _dragStart;
        private bool _dragging;
        private Label _statusLabel;

        public MainController(Form1 form, DataCenter dataCenter)
        {
            _form = form;
            _dataCenter = dataCenter;
            InitializeUI();
        }

        private void InitializeUI()
        {
            _statusLabel = new Label
            {
                Dock = DockStyle.Bottom,
                Height = 20,
                BackColor = Color.FromArgb(40, 40, 40),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleLeft
            };
            _form.Controls.Add(_statusLabel);

            var resetButton = new Button
            {
                Text = "重置",
                Dock = DockStyle.Bottom
            };
            resetButton.Click += (s, e) => ResetView();
            _form.Controls.Add(resetButton);
        }

        public void HandlePaint(PaintEventArgs e)
        {
            if (_currentImage == null) return;

            using (Bitmap buffer = new Bitmap(_form.panel1.Width, _form.panel1.Height))
            using (Graphics g = Graphics.FromImage(buffer))
            {
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.PixelOffsetMode = PixelOffsetMode.Half;
                g.SmoothingMode = SmoothingMode.None;
                g.Clear(_form.panel1.BackColor);

                Rectangle destRect = new Rectangle(
                    _offset.X,
                    _offset.Y,
                    (int)(_currentImage.Width * _zoom),
                    (int)(_currentImage.Height * _zoom));

                g.DrawImage(_currentImage, destRect, 0, 0, _currentImage.Width, _currentImage.Height, GraphicsUnit.Pixel);
                e.Graphics.DrawImage(buffer, 0, 0);
            }

            UpdateStatus();
        }

        public void HandleMouseWheel(MouseEventArgs e)
        {
            float zoomCenterX = (e.X - _offset.X) / _zoom;
            float zoomCenterY = (e.Y - _offset.Y) / _zoom;

            float oldZoom = _zoom;
            _zoom *= e.Delta > 0 ? 1.2f : 0.8f;
            _zoom = Math.Max(0.1f, Math.Min(20.0f, _zoom));

            _offset.X = (int)(e.X - zoomCenterX * _zoom);
            _offset.Y = (int)(e.Y - zoomCenterY * _zoom);

            _form.panel1.Invalidate();
        }

        public void HandleMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _dragging = true;
                _dragStart = e.Location;
                _form.panel1.Cursor = Cursors.Hand;
            }
        }

        public void HandleMouseMove(MouseEventArgs e)
        {
            if (_dragging)
            {
                _offset.X += e.X - _dragStart.X;
                _offset.Y += e.Y - _dragStart.Y;
                _dragStart = e.Location;

                Rectangle invalidRect = new Rectangle(
                    Math.Min(e.X, _dragStart.X) - 10,
                    Math.Min(e.Y, _dragStart.Y) - 10,
                    Math.Abs(e.X - _dragStart.X) + 20,
                    Math.Abs(e.Y - _dragStart.Y) + 20);

                _form.panel1.Invalidate(invalidRect);
            }
        }

        public void HandleMouseUp(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _dragging = false;
                _form.panel1.Cursor = Cursors.Default;
            }
        }

        public void HandleResize()
        {
            _form.panel1.Invalidate();
        }

        private void UpdateStatus()
        {
            if (_currentImage == null)
            {
                _statusLabel.Text = "无图像加载";
                return;
            }

            string status = $"缩放: {_zoom * 100:0}% | 偏移: ({_offset.X}, {_offset.Y}) | " +
                           $"图像尺寸: {_currentImage.Width}x{_currentImage.Height} | " +
                           $"像素格式: {_currentImage.PixelFormat}";

            _statusLabel.Text = status;
        }

        private void ResetView()
        {
            _zoom = 1.0f;
            _offset = Point.Empty;
            _form.panel1.Invalidate();
        }

        public void LoadRasterImage()
        {
            RasterInfo rasterInfo = FileHelper.OpenRaster();
            if (rasterInfo != null)
            {
                _dataCenter.RasterInfos.Add(rasterInfo);

                // 在列表框中显示文件名和坐标系信息
                string displayText = Path.GetFileName(rasterInfo.FilePath);
                if (!string.IsNullOrEmpty(rasterInfo.Projection))
                {
                    // 提取坐标系名称（简化显示）
                    string coordSystem = ExtractCoordSystemName(rasterInfo.Projection);
                    displayText += $" [{coordSystem}]";
                }

                _form.listBox1.Items.Add(displayText);
                _currentImage = Display.TransRasterTobitmap(rasterInfo.Dataset);
                _form.panel1.Invalidate();

                // 在状态栏显示详细信息
                ShowSpatialReferenceInfo(rasterInfo);
            }
            else
            {
                MessageBox.Show("未能成功加载数据", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string ExtractCoordSystemName(string projectionWkt)
        {
            // 简单提取坐标系名称
            if (projectionWkt.Contains("GEOGCS"))
                return "地理坐标系";
            else if (projectionWkt.Contains("PROJCS"))
                return "投影坐标系";
            else
                return "未知坐标系";
        }

        private void ShowSpatialReferenceInfo(RasterInfo rasterInfo)
        {
            StringBuilder sb = new StringBuilder();

            if (!string.IsNullOrEmpty(rasterInfo.Projection))
            {
                sb.Append(" | 有空间参考");

                // 可以进一步解析WKT字符串获取更多信息
                if (rasterInfo.Projection.Contains("WGS") || rasterInfo.Projection.Contains("4326"))
                    sb.Append(" (WGS84)");
                else if (rasterInfo.Projection.Contains("CGCS") || rasterInfo.Projection.Contains("4490"))
                    sb.Append(" (CGCS2000)");
            }
            else
            {
                sb.Append(" | 无空间参考");
            }

            _statusLabel.Text += sb.ToString();
        }

        public void ShowDetailedSpatialReferenceInfo(int index)
        {
            if (index >= 0 && index < _dataCenter.RasterInfos.Count)
            {
                RasterInfo rasterInfo = _dataCenter.RasterInfos[index];

                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"文件: {Path.GetFileName(rasterInfo.FilePath)}");
                sb.AppendLine();

                if (!string.IsNullOrEmpty(rasterInfo.Projection))
                {
                    sb.AppendLine("空间参考系统 (WKT格式):");
                    sb.AppendLine(rasterInfo.Projection);
                    sb.AppendLine();

                    sb.AppendLine("地理变换参数:");
                    for (int i = 0; i < 6; i++)
                    {
                        sb.AppendLine($"  [{i}]: {rasterInfo.GeoTransform[i]}");
                    }
                }
                else
                {
                    sb.AppendLine("该文件不包含空间参考信息");
                }

                MessageBox.Show(sb.ToString(), "空间参考信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public void HandleListBoxSelection()
        {
            if (_form.listBox1.SelectedIndex >= 0 && _form.listBox1.SelectedIndex < _dataCenter.RasterInfos.Count)
            {
                // 从 RasterInfo 中获取 Dataset
                RasterInfo selectedRaster = _dataCenter.RasterInfos[_form.listBox1.SelectedIndex];
                Dataset selectedDataset = selectedRaster.Dataset;

                // 转换并显示图像
                _currentImage = Display.TransRasterTobitmap(selectedDataset);
                _form.panel1.Invalidate();
            }
        }

        public void ClearAllDatasets()
        {
            _dataCenter.RasterInfos.Clear();
            _form.listBox1.Items.Clear();
            _currentImage?.Dispose();
            _currentImage = null;
            ResetView();
        }

        public void CloseCurrentDataset()
        {
            int selectedIndex = _form.listBox1.SelectedIndex;
            if (selectedIndex >= 0 && selectedIndex < _dataCenter.RasterInfos.Count)
            {
                _dataCenter.RasterInfos.RemoveAt(selectedIndex);
                _form.listBox1.Items.RemoveAt(selectedIndex);
                _currentImage?.Dispose();
                _currentImage = null;
                if (_dataCenter.RasterInfos.Count > 0)
                {
                    int newIndex = Math.Min(selectedIndex, _dataCenter.RasterInfos.Count - 1);
                    _form.listBox1.SelectedIndex = newIndex;
                    HandleListBoxSelection();
                }
                else
                {
                    ResetView();
                }
            }
        }

        public void DifferEncoding()
        {
            RasterInfo rasterInfo = GetCurrentRasterInfo();
            if (rasterInfo?.Dataset == null)
            {
                MessageBox.Show("请先选择一个数据集", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                // 调用比较函数，选择最优的差分编码方法
                var result = Algorthim.CompareDifferEncodingMethods(rasterInfo.Dataset);
                Dataset encodedDataset = result.encodedDataset;
                double compressionRatio = result.compressionRatio;
                string methodName = result.methodName;

                // 创建新的RasterInfo对象
                RasterInfo encodedRasterInfo = new RasterInfo(
                    encodedDataset,
                    $"{methodName}_{Path.GetFileName(rasterInfo.FilePath)}"
                );

                _dataCenter.RasterInfos.Add(encodedRasterInfo);
                _form.listBox1.Items.Add($"{methodName}: {Path.GetFileName(rasterInfo.FilePath)} (压缩比: {compressionRatio:F2})");
                _currentImage = Display.TransRasterTobitmap(encodedDataset);
                _form.panel1.Invalidate();

                // 显示压缩效率信息
                MessageBox.Show($"已应用{methodName}编码\n压缩效率: {compressionRatio:F2}",
                                "编码结果",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);

            }
            catch (Exception ex)
            {
                MessageBox.Show($"差分编码失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 在MainController类中添加以下两个函数

        /// <summary>
        /// 执行行首差分解码
        /// </summary>
        public void ExecuteRowFirstDecoding()
        {
            RasterInfo rasterInfo = GetCurrentRasterInfo();
            if (rasterInfo?.Dataset == null)
            {
                MessageBox.Show("请先选择一个数据集", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                // 调用行首解码函数
                Dataset decodedDataset = Algorthim.RowFirstDifferDecoding(rasterInfo.Dataset);
                string methodName = "行首差分解码";

                // 获取原始文件名
                string originalFileName = Path.GetFileName(rasterInfo.FilePath);

                // 创建新的RasterInfo对象
                RasterInfo decodedRasterInfo = new RasterInfo(
                    decodedDataset,
                    $"{methodName}_{originalFileName}"
                );

                // 添加到数据中心和列表
                _dataCenter.RasterInfos.Add(decodedRasterInfo);
                _form.listBox1.Items.Add($"{methodName}: {originalFileName}");

                // 更新当前显示图像
                _currentImage = Display.TransRasterTobitmap(decodedDataset);
                _form.panel1.Invalidate();

                MessageBox.Show($"已成功应用{methodName}",
                                "解码结果",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"行首差分解码失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 执行列首差分解码
        /// </summary>
        public void ExecuteColumnFirstDecoding()
        {
            RasterInfo rasterInfo = GetCurrentRasterInfo();
            if (rasterInfo?.Dataset == null)
            {
                MessageBox.Show("请先选择一个数据集", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                // 调用列首解码函数
                Dataset decodedDataset = Algorthim.ColumnFirstDifferDecoding(rasterInfo.Dataset);
                string methodName = "列首差分解码";

                // 获取原始文件名
                string originalFileName = Path.GetFileName(rasterInfo.FilePath);

                // 创建新的RasterInfo对象
                RasterInfo decodedRasterInfo = new RasterInfo(
                    decodedDataset,
                    $"{methodName}_{originalFileName}"
                );

                // 添加到数据中心和列表
                _dataCenter.RasterInfos.Add(decodedRasterInfo);
                _form.listBox1.Items.Add($"{methodName}: {originalFileName}");

                // 更新当前显示图像
                _currentImage = Display.TransRasterTobitmap(decodedDataset);
                _form.panel1.Invalidate();

                MessageBox.Show($"已成功应用{methodName}",
                                "解码结果",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"列首差分解码失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 保存当前选中的 Dataset
        /// </summary>
        /// <param name="format">目标格式</param>
        public void SaveCurrentDataset(string format)
        {
            if (_form.listBox1.SelectedIndex < 0 || _form.listBox1.SelectedIndex >= _dataCenter.RasterInfos.Count)
            {
                MessageBox.Show("请先选择一个数据集", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            RasterInfo selectedRaster = _dataCenter.RasterInfos[_form.listBox1.SelectedIndex];
            FileHelper.SaveDataset(selectedRaster.Dataset, format);
        }

        /// <summary>
        /// 获取当前选中的 Dataset
        /// </summary>
        public RasterInfo GetCurrentRasterInfo()
        {
            if (_form.listBox1.SelectedIndex >= 0 && _form.listBox1.SelectedIndex < _dataCenter.RasterInfos.Count)
            {
                return _dataCenter.RasterInfos[_form.listBox1.SelectedIndex];
            }
            return null;
        }
    }
}
