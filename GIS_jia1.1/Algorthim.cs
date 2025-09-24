using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OSGeo.GDAL;

namespace GIS_jia1._1
{
    internal class Algorthim
    {
        public static Dataset RowFirstDifferEncoding(Dataset indataset)
        {
            if (indataset == null)
            {
                throw new ArgumentNullException(nameof(indataset), "Input dataset cannot be null.");
            }

            int width = indataset.RasterXSize;
            int height = indataset.RasterYSize;
            int bandCount = indataset.RasterCount;

            Driver driver = Gdal.GetDriverByName("MEM");
            Dataset encodedDataset = driver.Create("row_first_encoded", width, height, bandCount, DataType.GDT_Float64, null);

            for (int b = 1; b <= bandCount; b++)
            {
                Band inBand = indataset.GetRasterBand(b);
                Band outBand = encodedDataset.GetRasterBand(b);
                double[] inBuffer = new double[width * height];
                double[] outBuffer = new double[width * height];

                // 读取原始数据
                inBand.ReadRaster(0, 0, width, height, inBuffer, width, height, 0, 0);

                // 按行处理
                for (int row = 0; row < height; row++)
                {
                    int rowStart = row * width;

                    // 行首像素保持不变
                    outBuffer[rowStart] = inBuffer[rowStart];

                    // 行内其他像素与行首像素做差分
                    for (int col = 1; col < width; col++)
                    {
                        int pixelIndex = rowStart + col;
                        outBuffer[pixelIndex] = inBuffer[pixelIndex] - inBuffer[rowStart];
                    }
                }

                // 写入编码后的数据
                outBand.WriteRaster(0, 0, width, height, outBuffer, width, height, 0, 0);
            }

            return encodedDataset;
        }

        public static Dataset ColumnFirstDifferEncoding(Dataset indataset)
        {
            if (indataset == null)
            {
                throw new ArgumentNullException(nameof(indataset), "Input dataset cannot be null.");
            }

            int width = indataset.RasterXSize;
            int height = indataset.RasterYSize;
            int bandCount = indataset.RasterCount;

            Driver driver = Gdal.GetDriverByName("MEM");
            Dataset encodedDataset = driver.Create("column_first_encoded", width, height, bandCount, DataType.GDT_Float64, null);

            for (int b = 1; b <= bandCount; b++)
            {
                Band inBand = indataset.GetRasterBand(b);
                Band outBand = encodedDataset.GetRasterBand(b);
                double[] inBuffer = new double[width * height];
                double[] outBuffer = new double[width * height];

                // 读取原始数据
                inBand.ReadRaster(0, 0, width, height, inBuffer, width, height, 0, 0);

                // 按列处理
                for (int col = 0; col < width; col++)
                {
                    // 列首像素保持不变
                    outBuffer[col] = inBuffer[col];

                    // 列内其他像素与列首像素做差分
                    for (int row = 1; row < height; row++)
                    {
                        int pixelIndex = row * width + col;
                        outBuffer[pixelIndex] = inBuffer[pixelIndex] - inBuffer[col];
                    }
                }

                // 写入编码后的数据
                outBand.WriteRaster(0, 0, width, height, outBuffer, width, height, 0, 0);
            }

            return encodedDataset;
        }

        public static (Dataset encodedDataset, double compressionRatio, string methodName) CompareDifferEncodingMethods(Dataset indataset)
        {
            if (indataset == null)
            {
                throw new ArgumentNullException(nameof(indataset), "Input dataset cannot be null.");
            }

            // 获取原始数据集信息
            int width = indataset.RasterXSize;
            int height = indataset.RasterYSize;
            int bandCount = indataset.RasterCount;

            // 计算原始数据集的理论大小（字节）
            // 假设原始数据类型为GDT_Float64（8字节）
            long originalSize = width * height * bandCount * 8;

            // 执行行首差分编码
            Dataset rowFirstEncoded = RowFirstDifferEncoding(indataset);
            // 计算行首差分编码后的理论大小
            long rowFirstSize = width * height * bandCount * 8; // 同样使用GDT_Float64
            double rowFirstRatio = (double)originalSize / rowFirstSize;

            // 执行列首差分编码
            Dataset columnFirstEncoded = ColumnFirstDifferEncoding(indataset);
            // 计算列首差分编码后的理论大小
            long columnFirstSize = width * height * bandCount * 8; // 同样使用GDT_Float64
            double columnFirstRatio = (double)originalSize / columnFirstSize;

            // 比较压缩效率
            Dataset bestDataset;
            double bestRatio;
            string methodName;

            if (rowFirstRatio >= columnFirstRatio)
            {
                bestDataset = rowFirstEncoded;
                bestRatio = rowFirstRatio;
                methodName = "行首差分";
                columnFirstEncoded.Dispose(); // 释放较差的编码结果
            }
            else
            {
                bestDataset = columnFirstEncoded;
                bestRatio = columnFirstRatio;
                methodName = "列首差分";
                rowFirstEncoded.Dispose(); // 释放较差的编码结果
            }

            // 返回最优编码结果和压缩效率
            return (bestDataset, bestRatio, methodName);
        }

    }
}
