using System;
using SharpGen.Runtime;
using SharpGen.Runtime.Diagnostics;
using ToyRendererDX11.Win32;

namespace ToyRendererDX11
{
    static class Program
    {
        private class TestApplication : Win32Application
        {
            public TestApplication(bool headless = false)
                : base(headless)
            {
            }

            protected override void InitializeBeforeRun()
            {
                if (Headless)
                {
                    graphicsDevice = new GraphicsDevice(new System.Drawing.Size(800, 600));
                }
                else
                {
                    graphicsDevice = new GraphicsDevice(MainWindow!);
                }
            }

            public override void OnDraw(int width, int height)
            {
                graphicsDevice!.DeviceContext.Flush();
            }
        }

        static void Main(string[] args)
        {
#if DEBUG
            Configuration.EnableObjectTracking = true;
#endif

            using TestApplication app = new TestApplication(headless: false);
            app.Run();
#if DEBUG
            Console.WriteLine(ObjectTracker.ReportActiveObjects());
#endif
        }
    }
}
