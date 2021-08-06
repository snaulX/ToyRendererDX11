using System;

namespace ToyRendererDX11
{
    public abstract class Application : IDisposable
    {
        public const string WindowClassName = "ToyRendererWindow";

        protected bool paused;
        protected bool exitRequested;

        protected GraphicsDevice graphicsDevice;

        public Application(bool headless = false)
        {
            Headless = headless;
            Current = this;
        }

        public static Application Current { get; protected set; }

        public bool Headless { get; }

        public Window MainWindow { get; protected set; }

        public virtual void Dispose()
        {
            graphicsDevice?.Dispose();
        }

        protected virtual void InitializeBeforeRun()
        {
        }

        public void Tick()
        {
            if (graphicsDevice != null)
            {
                graphicsDevice.DrawFrame(OnDraw);
            }
            else
            {
                OnDraw(MainWindow!.ClientSize.Width, MainWindow.ClientSize!.Height);
            }
        }

        public abstract void Run();

        public virtual void OnActivated()
        {
        }

        public virtual void OnDeactivated()
        {
        }

        public virtual void OnDraw(int width, int height)
        {
        }

        public virtual void OnKeyboardEvent(KeyboardKey key, bool pressed)
        {

        }
    }
}