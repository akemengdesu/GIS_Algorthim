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

        public RasterInfo(Dataset dataset, string filePath)
        {
            Dataset = dataset;
            FilePath = filePath;

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