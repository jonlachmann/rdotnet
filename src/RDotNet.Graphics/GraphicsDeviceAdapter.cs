using RDotNet.Graphics.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace RDotNet.Graphics
{
    internal class GraphicsDeviceAdapter : IDisposable
    {
        private readonly IGraphicsDevice device;
        private readonly List<GCHandle> delegateHandles;
        private DeviceDescription description;
        private REngine engine;
        private IntPtr gdd;

        public GraphicsDeviceAdapter(IGraphicsDevice device)
        {
            if (device == null)
            {
                throw new ArgumentNullException("device");
            }
            this.device = device;
            delegateHandles = new List<GCHandle>();
            gdd = IntPtr.Zero;
        }

        public REngine Engine
        {
            get { return engine; }
        }

        protected TDelegate GetFunction<TDelegate>() where TDelegate : class
        {
            return Engine.GetFunction<TDelegate>();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            description.Dispose();
            if (disposing && gdd != IntPtr.Zero)
            {
                Kill();
            }
        }

        public void Kill()
        {
            GetFunction<GEkillDevice>()(description.DangerousGetHandle());
            gdd = IntPtr.Zero;
        }

        public void SetEngine(REngine engine)
        {
            if (gdd != IntPtr.Zero)
            {
                throw new InvalidOperationException("engine is already set");
            }
            if (engine == null)
            {
                throw new ArgumentNullException("engine");
            }
            if (this.engine != null)
            {
                throw new InvalidOperationException();
            }
            if (!engine.IsRunning)
            {
                throw new ArgumentException(null, "engine");
            }
            this.engine = engine;

            //this.GetFunction<R_GE_checkVersionOrDie>()(this.device.Version);
            GetFunction<R_CheckDeviceAvailable>()();
            var oldSuspended = GetInterruptsSuspended(engine);
            SetInterruptsSuspended(engine, true);

            description = new DeviceDescription();
            SetMethod();
            gdd = GetFunction<GEcreateDevDesc>()(description.DangerousGetHandle());
            GetFunction<GEaddDevice2>()(gdd, device.Name);

            SetInterruptsSuspended(engine, oldSuspended);
            if (GetInterruptsPending(engine) && !GetInterruptsSuspended(engine))
            {
                GetFunction<Rf_onintr>()();
            }
        }

        private static void SetInterruptsSuspended(REngine engine, bool value)
        {
            var pointer = engine.DangerousGetHandle("R_interrupts_suspended");
            Marshal.WriteInt32(pointer, Convert.ToInt32(value));
        }

        private static bool GetInterruptsSuspended(REngine engine)
        {
            var pointer = engine.DangerousGetHandle("R_interrupts_suspended");
            return Convert.ToBoolean(Marshal.ReadInt32(pointer));
        }

        private static bool GetInterruptsPending(REngine engine)
        {
            var pointer = engine.DangerousGetHandle("R_interrupts_pending");
            return Convert.ToBoolean(Marshal.ReadInt32(pointer));
        }

        private void SetMethod()
        {
            var activate = (_DevDesc_activate)Activate;
            Alloc(activate);
            description.SetMethod("activate", activate);
            var cap = (_DevDesc_cap)Capture;
            Alloc(cap);
            description.SetMethod("cap", cap);
            var circle = (_DevDesc_circle)DrawCircle;
            Alloc(circle);
            description.SetMethod("circle", circle);
            var clip = (_DevDesc_clip)Clip;
            Alloc(clip);
            description.SetMethod("clip", clip);
            var close = (_DevDesc_close)Close;
            Alloc(close);
            description.SetMethod("close", close);
            var deactivate = (_DevDesc_deactivate)Deactivate;
            Alloc(deactivate);
            description.SetMethod("deactivate", deactivate);
            var line = (_DevDesc_line)DrawLine;
            Alloc(line);
            description.SetMethod("line", line);
            var locator = (_DevDesc_locator)GetLocation;
            Alloc(locator);
            description.SetMethod("locator", locator);
            var metricInfo = (_DevDesc_metricInfo)GetMetricInfo;
            Alloc(metricInfo);
            description.SetMethod("metricInfo", metricInfo);
            var mode = (_DevDesc_mode)ChangeMode;
            Alloc(mode);
            description.SetMethod("mode", mode);
            var newPage = (_DevDesc_newPage)NewPage;
            Alloc(newPage);
            description.SetMethod("newPage", newPage);
            var path = (_DevDesc_path)DrawPath;
            Alloc(path);
            description.SetMethod("path", path);
            var polygon = (_DevDesc_polygon)DrawPolygon;
            Alloc(polygon);
            description.SetMethod("polygon", polygon);
            var polyline = (_DevDesc_Polyline)DrawPolyline;
            Alloc(polyline);
            description.SetMethod("polyline", polyline);
            var raster = (_DevDesc_raster)DrawRaster;
            Alloc(raster);
            description.SetMethod("raster", raster);
            var rect = (_DevDesc_rect)DrawRectangle;
            Alloc(rect);
            description.SetMethod("rect", rect);
            var size = (_DevDesc_size)Resize;
            Alloc(size);
            description.SetMethod("size", size);
            var strWidth = (_DevDesc_strWidth)MeasureWidth;
            Alloc(strWidth);
            description.SetMethod("strWidth", strWidth);
            var text = (_DevDesc_text)DrawText;
            Alloc(text);
            description.SetMethod("text", text);
            var strWidthUTF8 = (_DevDesc_strWidth)MeasureWidth;
            Alloc(strWidthUTF8);
            description.SetMethod("strWidthUTF8", strWidthUTF8);
            var textUTF8 = (_DevDesc_text)DrawText;
            Alloc(textUTF8);
            description.SetMethod("textUTF8", textUTF8);
            var newFrameConfirm = (_DevDesc_newFrameConfirm)ConfirmNewFrame;
            Alloc(newFrameConfirm);
            description.SetMethod("newFrameConfirm", newFrameConfirm);
            var getEvent = (_DevDesc_getEvent)GetEvent;
            Alloc(getEvent);
            description.SetMethod("getEvent", getEvent);
            var eventHelper = (_DevDesc_eventHelper)EventHelper;
            Alloc(eventHelper);
            description.SetMethod("eventHelper", eventHelper);
        }

        private void Alloc(Delegate d)
        {
            var handle = GCHandle.Alloc(d);
            delegateHandles.Add(handle);
        }

        private void FreeAll()
        {
            delegateHandles.ForEach(handle => handle.Free());
            delegateHandles.Clear();
        }

        private void Activate(IntPtr pDevDesc)
        {
            device.OnActivated(description);
        }

        private void Deactivate(IntPtr pDevDesc)
        {
            device.OnDeactivated(description);
        }

        private void NewPage(IntPtr gc, IntPtr dd)
        {
            var context = new GraphicsContext(gc);
            device.OnNewPageRequested(context, description);
        }

        private void Resize(out double left, out double right, out double bottom, out double top, IntPtr dd)
        {
            var rectangle = device.OnResized(description);
            left = rectangle.Left;
            right = rectangle.Right;
            bottom = rectangle.Bottom;
            top = rectangle.Top;
        }

        private void Close(IntPtr dd)
        {
            device.OnClosed(description);
            ClearDevDesc();
        }

        private void ClearDevDesc()
        {
            var geDevDesc = (GEDevDesc)Marshal.PtrToStructure(gdd, typeof(GEDevDesc));
            geDevDesc.dev = IntPtr.Zero;
            Marshal.StructureToPtr(geDevDesc, gdd, false);
        }

        private bool ConfirmNewFrame(IntPtr dd)
        {
            return device.ConfirmNewFrame(description);
        }

        private void ChangeMode(int mode, IntPtr dd)
        {
            if (mode == 0)
            {
                device.OnDrawStarted(description);
            }
            else if (mode == 1)
            {
                device.OnDrawStopped(description);
            }
        }

        private void DrawCircle(double x, double y, double r, IntPtr gc, IntPtr dd)
        {
            var context = new GraphicsContext(gc);
            var center = new Point(x, y);
            device.DrawCircle(center, r, context, description);
        }

        private void Clip(double x0, double x1, double y0, double y1, IntPtr dd)
        {
            var rectangle = new Rectangle(Math.Min(x0, x1), Math.Min(y0, y1), Math.Abs(x0 - x1), Math.Abs(y0 - y1));
            device.Clip(rectangle, description);
        }

        private bool GetLocation(out double x, out double y, IntPtr dd)
        {
            var location = device.GetLocation(description);
            if (!location.HasValue)
            {
                x = default(double);
                y = default(double);
                return false;
            }

            var p = location.Value;
            x = p.X;
            y = p.Y;
            return true;
        }

        private void DrawLine(double x1, double y1, double x2, double y2, IntPtr gc, IntPtr dd)
        {
            var context = new GraphicsContext(gc);
            var source = new Point(x1, y1);
            var destination = new Point(x2, y2);
            device.DrawLine(source, destination, context, description);
        }

        private void GetMetricInfo(int c, IntPtr gc, out double ascent, out double descent, out double width, IntPtr dd)
        {
            var context = new GraphicsContext(gc);
            var metric = device.GetMetricInfo(c, context, description);
            ascent = metric.Ascent;
            descent = metric.Descent;
            width = metric.Width;
        }

        private void DrawPolygon(int n, IntPtr x, IntPtr y, IntPtr gc, IntPtr dd)
        {
            var context = new GraphicsContext(gc);
            var points = GetPoints(n, x, y);
            device.DrawPolygon(points, context, description);
        }

        private void DrawPolyline(int n, IntPtr x, IntPtr y, IntPtr gc, IntPtr dd)
        {
            var context = new GraphicsContext(gc);
            var points = GetPoints(n, x, y);
            device.DrawPolyline(points, context, description);
        }

        private void DrawRectangle(double x0, double y0, double x1, double y1, IntPtr gc, IntPtr dd)
        {
            var context = new GraphicsContext(gc);
            var rectangle = new Rectangle(Math.Min(x0, x1), Math.Min(y0, y1), Math.Abs(x0 - x1), Math.Abs(y0 - y1));
            device.DrawRectangle(rectangle, context, description);
        }

        private void DrawPath(IntPtr x, IntPtr y, int npoly, IntPtr nper, bool winding, IntPtr gc, IntPtr dd)
        {
            var context = new GraphicsContext(gc);
            var points = GetPoints(x, y, npoly, nper);
            device.DrawPath(points, winding, context, description);
        }

        private void DrawRaster(IntPtr raster, int w, int h, double x, double y, double width, double height, double rot, bool interpolate, IntPtr gc, IntPtr dd)
        {
            var context = new GraphicsContext(gc);
            var output = new Raster(w, h);
            unchecked
            {
                for (var i = 0; i < w; i++)
                {
                    for (var j = 0; j < h; j++)
                    {
                        output[i, j] = Color.FromUInt32((uint)Marshal.ReadInt32(raster));
                        raster = IntPtr.Add(raster, sizeof(int));
                    }
                }
            }
            device.DrawRaster(output, new Rectangle(x, y, width, height), rot, interpolate, context, description);
        }

        private IntPtr Capture(IntPtr dd)
        {
            var raster = device.Capture(description);
            return Engine.CreateIntegerMatrix(raster).DangerousGetHandle();
        }

        private double MeasureWidth(string str, IntPtr gc, IntPtr dd)
        {
            var context = new GraphicsContext(gc);
            return device.MeasureWidth(str, context, description);
        }

        private void DrawText(double x, double y, string str, double rot, double hadj, IntPtr gc, IntPtr dd)
        {
            var context = new GraphicsContext(gc);
            device.DrawText(str, new Point(x, y), rot, hadj, context, description);
        }

        private IntPtr GetEvent(IntPtr sexp, string s)
        {
            return IntPtr.Zero;
        }

        private void EventHelper(IntPtr dd, int code)
        { }

        private IEnumerable<Point> GetPoints(int n, IntPtr x, IntPtr y)
        {
            return Enumerable.Range(0, n).Select(
               index => {
                   var offset = sizeof(double) * index;
                   var px = Utility.ReadDouble(x, offset);
                   var py = Utility.ReadDouble(y, offset);
                   return new Point(px, py);
               }
               );
        }

        private IEnumerable<IEnumerable<Point>> GetPoints(IntPtr x, IntPtr y, int npoly, IntPtr nper)
        {
            if (!Engine.IsRunning)
            {
                throw new InvalidOperationException();
            }
            for (var index = 0; index < npoly; index++)
            {
                var offset = sizeof(int) * index;
                var n = Marshal.ReadInt32(nper, offset);
                yield return GetPoints(n, x, y);
                var pointOffset = sizeof(double) * n;
                x = IntPtr.Add(x, pointOffset);
                y = IntPtr.Add(y, pointOffset);
            }
        }
    }
}