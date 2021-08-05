using System;
using System.Drawing;

namespace ToyRendererDX11
{
    public abstract class Window
    {
        public string Title { get; protected set; }
        public Size ClientSize { get; protected set; }
        public IntPtr Handle { get; protected set; }

        public Window(string title, int width, int height)
        {
            Title = title;
            ClientSize = new Size(width, height);
        }
    }
}