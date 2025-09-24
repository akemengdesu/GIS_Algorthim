using System.Drawing;
using OSGeo.GDAL;

namespace GIS_jia1._1
{
    public class RasterInfo
    {
        public Dataset Dataset { get; set; }
        public string Projection { get; set; }
        public double[] GeoTransform { get; set; }
        public string FilePath { get; set; }
        public string EncodingType { get; set; } // 新增：存储编码类型

        public RasterInfo(Dataset dataset, string filePath, string encodingType = null)
        {
            Dataset = dataset;
            FilePath = filePath;
            EncodingType = encodingType;

            // 获取空间参考信息
            Projection = dataset.GetProjection();
            GeoTransform = new double[6];
            dataset.GetGeoTransform(GeoTransform);
        }
    }
    internal class DataCenter
    {
        public List<RasterInfo> RasterInfos = new List<RasterInfo>();
    }
}