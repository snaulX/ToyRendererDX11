using System;
using System.Diagnostics;
using System.Drawing;
using Vortice;
using static ToyRendererDX11.Win32.User32;

namespace ToyRendererDX11.Win32
{
    public class Win32Window : Window
    {
        private const int CW_USEDEFAULT = unchecked((int)0x80000000);

        public unsafe Win32Window(string title, int width, int height) : base(title, width, height)
        {
            int x = 0;
            int y = 0;
            WindowStyles style = 0;
            WindowExStyles styleEx = 0;
            const bool resizable = true;

            // Setup the screen settings depending on whether it is running in full screen or in windowed mode.
            //if (fullscreen)
            //{
            //style = User32.WindowStyles.WS_POPUP | User32.WindowStyles.WS_VISIBLE;
            //styleEx = User32.WindowStyles.WS_EX_APPWINDOW;

            //width = screenWidth;
            //height = screenHeight;
            //}
            //else
            {
                if (ClientSize.Width > 0 && ClientSize.Height > 0)
                {
                    int screenWidth = GetSystemMetrics(SystemMetrics.SM_CXSCREEN);
                    int screenHeight = GetSystemMetrics(SystemMetrics.SM_CYSCREEN);

                    // Place the window in the middle of the screen.WS_EX_APPWINDOW
                    x = (screenWidth - ClientSize.Width) / 2;
                    y = (screenHeight - ClientSize.Height) / 2;
                }

                style = resizable ? WindowStyles.WS_OVERLAPPEDWINDOW :
                    WindowStyles.WS_POPUP | WindowStyles.WS_BORDER | WindowStyles.WS_CAPTION | WindowStyles.WS_SYSMENU;

                styleEx = WindowExStyles.WS_EX_APPWINDOW | WindowExStyles.WS_EX_WINDOWEDGE;
            }
            style |= WindowStyles.WS_CLIPCHILDREN | WindowStyles.WS_CLIPSIBLINGS;

            int windowWidth;
            int windowHeight;

            if (ClientSize.Width > 0 && ClientSize.Height > 0)
            {
                var rect = new RawRect(0, 0, ClientSize.Width, ClientSize.Height);

                // Adjust according to window styles
                AdjustWindowRectEx(&rect, (uint)style, 0, (uint)styleEx);

                windowWidth = rect.Right - rect.Left;
                windowHeight = rect.Bottom - rect.Top;
            }
            else
            {
                x = y = windowWidth = windowHeight = CW_USEDEFAULT;
            }

            fixed (char* lpszClassName = Application.WindowClassName)
            {
                fixed (char* lpWindowName = Title)
                {
                    Handle = CreateWindowExW(
                    (uint)styleEx,
                    (ushort*)lpszClassName,
                    (ushort*)lpWindowName,
                    (uint)style,
                    x,
                    y,
                    windowWidth,
                    windowHeight,
                    IntPtr.Zero,
                    IntPtr.Zero,
                    IntPtr.Zero,
                    null
                    );

                    if (Handle == IntPtr.Zero)
                    {
                        return;
                    }
                }
            }

            ShowWindow(Handle, (int)ShowWindowCommand.Normal);
            ClientSize = new Size(windowWidth, windowHeight);
        }

        public void Destroy()
        {
            IntPtr hwnd = Handle;
            if (hwnd != IntPtr.Zero)
            {
                IntPtr destroyHandle = hwnd;
                Handle = IntPtr.Zero;

                Debug.WriteLine($"[WIN32] - Destroying window: {destroyHandle}");
                DestroyWindow(destroyHandle);
            }
        }
    }
}
