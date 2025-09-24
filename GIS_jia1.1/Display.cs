using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using OSGeo.GDAL;

namespace GIS_jia1._1
{
    internal class Display
    {
        public static Bitmap TransRasterTobitmap(Dataset dataset)
        {
            //根据波段数量选择处理方式
            int bandCount = dataset.RasterCount;
            int width = dataset.RasterXSize;
            int height = dataset.RasterYSize;
            Bitmap bitmap = bandCount switch
            {
                1 => ReadSingleBand(dataset, width, height),     // 单波段（DEM/灰度）
                2 => ReadTwoBands(dataset, width, height),       // 双波段特殊处理
                >= 3 => ReadRGBBands(dataset, width, height),    // 标准RGB或多波段
                _ => throw new Exception("无效的波段数量")       // 异常情况
            };
            return bitmap;
        }


        static Bitmap ReadRGBBands(Dataset dataset, int width, int height)
        {
            // 1. 确定波段映射
            int bandCount = dataset.RasterCount;
            int[] bandMap = bandCount >= 3
                ? new[] { 1, 2, 3 }  // 标准RGB顺序
                : new[] { 1, 1, 1 }; // 单波段复制到所有通道

            // 2. 创建目标位图（32位ARGB）
            Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);

            // 3. 准备RGB缓冲区（每个像素3字节）
            byte[] rgbBuffer = new byte[width * height * 3];

            // 4. 关键修正：正确设置ReadRaster参数
            dataset.ReadRaster(
               0, 0, width, height, rgbBuffer, width, height, 3, bandMap, 3, width * 3, 1
            );

            // 5. 转换为ARGB格式
            BitmapData bmpData = bitmap.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.WriteOnly,
                PixelFormat.Format32bppArgb);

            byte[] argbBuffer = new byte[width * height * 4];
            for (int i = 0; i < width * height; i++)
            {
                argbBuffer[i * 4] = rgbBuffer[i * 3 + 2]; // B
                argbBuffer[i * 4 + 1] = rgbBuffer[i * 3 + 1]; // G
                argbBuffer[i * 4 + 2] = rgbBuffer[i * 3];     // R
                argbBuffer[i * 4 + 3] = 255;                // A
            }

            Marshal.Copy(argbBuffer, 0, bmpData.Scan0, argbBuffer.Length);
            bitmap.UnlockBits(bmpData);
            return bitmap;
        }

        private static Bitmap ReadSingleBand(Dataset dataset, int width, int height)
        {
            Band band = dataset.GetRasterBand(1);
            DataType dataType = band.DataType;

            Bitmap bitmap = new(width, height, PixelFormat.Format32bppArgb);
            BitmapData bmpData = bitmap.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.WriteOnly,
                bitmap.PixelFormat);

            // 根据数据类型处理
            if (dataType == DataType.GDT_Byte)
            {
                byte[] grayData = new byte[width * height];
                band.ReadRaster(0, 0, width, height, grayData, width, height, 0, 0);

                // 增强对比度 - 使用2%线性拉伸
                StretchContrast(grayData, 5);
                ConvertGrayToARGB(grayData, bmpData.Scan0);
            }
            else // 默认按Float32处理
            {
                float[] floatData = new float[width * height];
                band.ReadRaster(0, 0, width, height, floatData, width, height, 0, 0);

                // 增强对比度 - 使用2%线性拉伸
                StretchContrast(floatData, 5);
                ConvertFloatToARGB(floatData, bmpData.Scan0);
            }

            bitmap.UnlockBits(bmpData);
            return bitmap;
        }

        // 字节数组对比度拉伸
        private static void StretchContrast(byte[] data, double percent = 2)
        {
            // 计算直方图
            int[] histogram = new int[256];
            foreach (byte value in data)
            {
                histogram[value]++;
            }

            // 计算拉伸范围（排除两端各percent%的值）
            int totalPixels = data.Length;
            int minCut = (int)(totalPixels * percent / 100);
            int maxCut = (int)(totalPixels * (100 - percent) / 100);

            byte min = 0;
            byte max = 255;

            int count = 0;
            for (int i = 0; i < 256; i++)
            {
                count += histogram[i];
                if (count >= minCut)
                {
                    min = (byte)i;
                    break;
                }
            }

            count = 0;
            for (int i = 255; i >= 0; i--)
            {
                count += histogram[i];
                if (count >= maxCut)
                {
                    max = (byte)i;
                    break;
                }
            }

            // 应用线性拉伸
            double scale = 255.0 / (max - min);
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] < min) data[i] = 0;
                else if (data[i] > max) data[i] = 255;
                else data[i] = (byte)((data[i] - min) * scale);
            }
        }

        // 浮点数组对比度拉伸
        private static void StretchContrast(float[] data, double percent = 2)
        {
            // 查找有效值范围
            List<float> validValues = new List<float>();
            foreach (float value in data)
            {
                // 跳过无效值（NaN）
                if (!float.IsNaN(value))
                {
                    validValues.Add(value);
                }
            }

            // 排序以找到百分比截断点
            validValues.Sort();
            int total = validValues.Count;
            int minIndex = (int)(total * percent / 100);
            int maxIndex = (int)(total * (100 - percent) / 100);

            float min = validValues[minIndex];
            float max = validValues[maxIndex];

            // 应用线性拉伸
            double scale = 255.0 / (max - min);
            for (int i = 0; i < data.Length; i++)
            {
                if (float.IsNaN(data[i])) continue;

                if (data[i] < min) data[i] = 0;
                else if (data[i] > max) data[i] = 255;
                else data[i] = (float)((data[i] - min) * scale);
            }
        }

        private static Bitmap ReadTwoBands(Dataset dataset, int width, int height)
        {
            Bitmap bitmap = new(width, height, PixelFormat.Format32bppArgb);
            BitmapData bmpData = bitmap.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.WriteOnly,
                bitmap.PixelFormat);
            byte[] buffer = new byte[width * height * 4];
            dataset.ReadRaster(0, 0, width, height,
                             buffer, width, height,
                             4, new[] { 1, 2 }, // 只读取存在的两个波段
                             0, 0, 4);
            Marshal.Copy(buffer, 0, bmpData.Scan0, buffer.Length);
            return bitmap;
        }
        // 转换方法
        private static void ConvertFloatToARGB(float[] floatData, IntPtr scan0)
        {
            float min = floatData.Min();
            float max = floatData.Max();
            float range = max - min;

            byte[] buffer = new byte[floatData.Length * 4];
            for (int i = 0; i < floatData.Length; i++)
            {
                byte value = (byte)(((floatData[i] - min) / range) * 255);
                buffer[i * 4] = value;     // B
                buffer[i * 4 + 1] = value; // G
                buffer[i * 4 + 2] = value; // R
                buffer[i * 4 + 3] = 255;   // A
            }

            Marshal.Copy(buffer, 0, scan0, buffer.Length);
        }

        private static void ConvertGrayToARGB(byte[] grayData, IntPtr scan0)
        {
            // 1. 创建托管缓冲区（ARGB格式，每个像素4字节）
            byte[] argbBuffer = new byte[grayData.Length * 4];

            // 2. 填充ARGB数据
            for (int i = 0; i < grayData.Length; i++)
            {
                argbBuffer[i * 4] = grayData[i];     // Blue
                argbBuffer[i * 4 + 1] = grayData[i]; // Green
                argbBuffer[i * 4 + 2] = grayData[i]; // Red
                argbBuffer[i * 4 + 3] = 0xFF;        // Alpha (不透明)
            }

            // 3. 将数据复制到非托管内存（Bitmap的Scan0）
            Marshal.Copy(argbBuffer, 0, scan0, argbBuffer.Length);
        }
    }
}
