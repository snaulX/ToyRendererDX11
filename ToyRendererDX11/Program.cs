#if DEBUG
using SharpGen.Runtime;
using SharpGen.Runtime.Diagnostics;
#endif

namespace ToyRendererDX11
{
    static class Program
    {
        private static void Main(string[] args)
        {
#if DEBUG
            Configuration.EnableObjectTracking = true;
#endif

            using TestApplication app = new TestApplication(headless: false);
            app.Run();
#if DEBUG
            System.Console.WriteLine(ObjectTracker.ReportActiveObjects());
#endif
        }
    }
}
