using OSGeo.GDAL;
using OSGeo.OGR;
using System.Runtime.InteropServices;
using System.Text;

namespace GIS_jia1._1
{
    internal static class Program
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetDllDirectory(string lpPathName);

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            InitializeGdal();

            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }

        private static void InitializeGdal()
        {
            string appPath = AppDomain.CurrentDomain.BaseDirectory;
            string gdalRootPath = Path.Combine(appPath, "gdal");
            string gdalBinPath = Path.Combine(gdalRootPath, "x64");
            string gdalDataPath = Path.Combine(gdalRootPath, "data");
            string projLibPath = Path.Combine(gdalRootPath, "share");

            // 设置 DLL 目录
            SetDllDirectory(gdalBinPath);

            // 设置环境变量
            Environment.SetEnvironmentVariable("GDAL_DATA", gdalDataPath);
            Environment.SetEnvironmentVariable("PROJ_LIB", projLibPath);
            Environment.SetEnvironmentVariable("PROJ_DATA", projLibPath);

            // 同时使用 GDAL 配置选项
            Gdal.SetConfigOption("GDAL_DATA", gdalDataPath);
            Gdal.SetConfigOption("PROJ_LIB", projLibPath);
            Gdal.SetConfigOption("PROJ_DATA", projLibPath);

            // 注册编码提供程序
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            // 初始化GDAL和OGR
            Gdal.AllRegister();
            Ogr.RegisterAll();
        }

    }
}