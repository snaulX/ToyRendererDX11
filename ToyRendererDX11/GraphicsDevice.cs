using System;
using System.Drawing;
using System.Runtime.CompilerServices;
using SharpGen.Runtime;
using Vortice;
using Vortice.Direct3D;
using Vortice.Direct3D11;
using Vortice.DXGI;
using Vortice.DXGI.Debug;
using Vortice.Mathematics;
using static Vortice.Direct3D11.D3D11;
using static Vortice.DXGI.DXGI;

namespace ToyRendererDX11
{
    public class GraphicsDevice : IDisposable
    {
        private static readonly FeatureLevel[] s_featureLevels = new[]
        {
            FeatureLevel.Level_11_1,
            FeatureLevel.Level_11_0,
            FeatureLevel.Level_10_1,
            FeatureLevel.Level_10_0
        };

        private const int FrameCount = 2;

        public readonly Size Size;
        public readonly IDXGIFactory2 Factory;
        public readonly ID3D11Device1 Device;
        public readonly FeatureLevel FeatureLevel;
        public readonly ID3D11DeviceContext1 DeviceContext;
        public readonly IDXGISwapChain1 SwapChain;
        public readonly ID3D11Texture2D BackBufferTexture;
        public readonly ID3D11Texture2D OffscreenTexture;
        public readonly ID3D11RenderTargetView RenderTargetView;

        public GraphicsDevice()
        {
            if (CreateDXGIFactory1(out Factory).Failure)
                throw new InvalidOperationException("Cannot create IDXGIFactory1");

            using (IDXGIAdapter1 adapter = GetHardwareAdapter())
            {
                DeviceCreationFlags creationFlags = DeviceCreationFlags.BgraSupport;
#if DEBUG
                if (SdkLayersAvailable())
                {
                    creationFlags |= DeviceCreationFlags.Debug;
                }
#endif

                if (D3D11CreateDevice(
                    adapter!,
                    DriverType.Unknown,
                    creationFlags,
                    s_featureLevels,
                    out ID3D11Device tempDevice, out FeatureLevel, out ID3D11DeviceContext tempContext).Failure)
                {
                    // If the initialization fails, fall back to the WARP device.
                    // For more information on WARP, see:
                    // http://go.microsoft.com/fwlink/?LinkId=286690
                    D3D11CreateDevice(
                        null,
                        DriverType.Warp,
                        creationFlags,
                        s_featureLevels,
                        out tempDevice, out FeatureLevel, out tempContext).CheckError();
                }

                Device = tempDevice.QueryInterface<ID3D11Device1>();
                DeviceContext = tempContext.QueryInterface<ID3D11DeviceContext1>();
                tempContext.Dispose();
                tempDevice.Dispose();
            }
        }

        private IDXGIAdapter1 GetHardwareAdapter()
        {
            IDXGIAdapter1 adapter = null;
            IDXGIFactory6 factory6 = Factory.QueryInterfaceOrNull<IDXGIFactory6>();
            if (factory6 != null)
            {
                for (int adapterIndex = 0;
                    factory6.EnumAdapterByGpuPreference(adapterIndex, GpuPreference.HighPerformance, out adapter).Success;
                    adapterIndex++)
                {
                    if (adapter == null)
                        continue;

                    AdapterDescription1 desc = adapter.Description1;

                    if ((desc.Flags & AdapterFlags.Software) != AdapterFlags.None)
                    {
                        // Don't select the Basic Render Driver adapter.
                        adapter.Dispose();
                        continue;
                    }

                    return adapter;
                }


                factory6.Dispose();
            }

            if (adapter == null)
            {
                for (int adapterIndex = 0;
                    Factory.EnumAdapters1(adapterIndex, out adapter).Success;
                    adapterIndex++)
                {
                    AdapterDescription1 desc = adapter.Description1;

                    if ((desc.Flags & AdapterFlags.Software) != AdapterFlags.None)
                    {
                        // Don't select the Basic Render Driver adapter.
                        adapter.Dispose();
                        continue;
                    }

                    return adapter;
                }
            }

            return adapter;
        }

        public void Dispose()
        {
            BackBufferTexture?.Dispose();
            OffscreenTexture?.Dispose();
            RenderTargetView.Dispose();
            DeviceContext.ClearState();
            DeviceContext.Flush();
            DeviceContext.Dispose();
            Device.Dispose();
            SwapChain.Dispose();
            Factory.Dispose();

            if (DXGIGetDebugInterface1(out IDXGIDebug1 dxgiDebug).Success)
            {
                dxgiDebug!.ReportLiveObjects(DebugAll, ReportLiveObjectFlags.Summary | ReportLiveObjectFlags.IgnoreInternal);
                dxgiDebug!.Dispose();
            }
        }
    }
}
