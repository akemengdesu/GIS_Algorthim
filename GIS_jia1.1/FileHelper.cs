using OSGeo.GDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GIS_jia1._1
{
    internal class FileHelper
    {
        public static RasterInfo OpenRaster()
        {
            OpenFileDialog file = new()
            {
                Filter = "栅格文件|*.tif;*.jpg;*.png;*.img",
                Title = "加载栅格数据"
            };

            if (file.ShowDialog() != DialogResult.OK)
                return null;

            // 强制注册所有驱动（确保支持所有格式）
            Gdal.AllRegister();

            // 打开数据集
            Dataset dataset = Gdal.Open(file.FileName, Access.GA_ReadOnly);
            if (dataset is null)
            {
                throw new Exception("无法打开文件，可能格式不支持或文件损坏");
            }

            // 检查波段数量
            if (dataset.RasterCount < 1)
            {
                dataset.Dispose(); // 释放资源
                throw new Exception("文件不包含任何波段数据");
            }

            return new RasterInfo(dataset, file.FileName);
        }

        /// <summary>
        /// 保存 Dataset 为指定格式
        /// </summary>
        /// <param name="dataset">要保存的 Dataset</param>
        /// <param name="format">目标格式（如 "GTiff", "JPEG", "PNG"）</param>
        /// <returns>是否保存成功</returns>
        public static bool SaveDataset(Dataset dataset, string format)
        {
            if (dataset == null)
            {
                MessageBox.Show("没有可保存的数据集", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            SaveFileDialog saveDialog = new SaveFileDialog();

            // 根据格式设置文件过滤器
            switch (format.ToUpper())
            {
                case "GTIFF":
                    saveDialog.Filter = "GeoTIFF 文件|*.tif";
                    saveDialog.DefaultExt = "tif";
                    break;
                case "JPEG":
                    saveDialog.Filter = "JPEG 文件|*.jpg";
                    saveDialog.DefaultExt = "jpg";
                    break;
                case "PNG":
                    saveDialog.Filter = "PNG 文件|*.png";
                    saveDialog.DefaultExt = "png";
                    break;
                case "IMG":
                    saveDialog.Filter = "ERDAS IMG 文件|*.img";
                    saveDialog.DefaultExt = "img";
                    break;
                default:
                    saveDialog.Filter = "所有文件|*.*";
                    break;
            }

            saveDialog.Title = "保存栅格数据";

            if (saveDialog.ShowDialog() != DialogResult.OK)
                return false;

            try
            {
                // 获取驱动
                Driver driver = Gdal.GetDriverByName(format);
                if (driver == null)
                {
                    MessageBox.Show($"不支持格式: {format}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                // 创建输出数据集
                string outputFile = saveDialog.FileName;
                Dataset outputDataset = driver.CreateCopy(outputFile, dataset, 0, null, null, null);

                if (outputDataset != null)
                {
                    outputDataset.Dispose();
                    MessageBox.Show("保存成功!", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return true;
                }
                else
                {
                    MessageBox.Show("保存失败", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存过程中发生错误: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// 获取支持的格式列表
        /// </summary>
        public static string[] GetSupportedFormats()
        {
            List<string> formats = new List<string>();
            int count = Gdal.GetDriverCount();

            for (int i = 0; i < count; i++)
            {
                Driver driver = Gdal.GetDriver(i);
                string driverName = driver.ShortName;

                // 只列出支持创建的驱动
                if (driver.GetMetadataItem("DCAP_CREATE", null) != null ||
                    driver.GetMetadataItem("DCAP_CREATECOPY", null) != null)
                {
                    formats.Add(driverName);
                }
            }

            return formats.ToArray();
        }
    }
}
