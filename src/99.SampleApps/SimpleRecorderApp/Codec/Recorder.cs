using SharpAvi.Output;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleRecorderWpf.Codec
{
    /// <summary title="동영상 문의 Recorder">
    /// 1. Create Date :  2021.07.19
    /// 2. Creator : 홍석원
    /// 3. Description : Recorder
    /// 4. Precaution :
    /// 5. History : 
    /// 6. MenuPath :  
    /// 7. OldName : NEW
    /// </summary>
    public class Recorder : IDisposable
    {
        #region [private]

        private AviWriter writer;
        private RecorderParams Params;
        private IAviVideoStream videoStream;
        private Thread screenThread;
        private ManualResetEvent stopThread = new ManualResetEvent(false);
        private const Int32 CURSOR_SHOWING = 0x00000001;

        #endregion [private]

        #region [win32]

        [StructLayout(LayoutKind.Sequential)]
        private struct CURSORINFO
        {
            public Int32 cbSize;
            public Int32 flags;
            public IntPtr hCursor;
            public POINTAPI ptScreenPos;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct POINTAPI
        {
            public int x;
            public int y;
        }

        [DllImport("user32.dll")]
        private static extern bool GetCursorInfo(out CURSORINFO pci);

        [DllImport("user32.dll")]
        private static extern bool DrawIcon(IntPtr hDC, int X, int Y, IntPtr hIcon);

        #endregion [win32]

        #region [ctor]

        /// <summary>
        /// 레코더
        /// </summary>
        /// <param name="Params"></param>
        public Recorder(RecorderParams Params)
        {
            this.Params = Params;

            // Create AVI writer and specify FPS
            writer = Params.CreateAviWriter();

            // Create video stream
            videoStream = Params.CreateVideoStream(writer);
            // Set only name. Other properties were when creating stream,
            // either explicitly by arguments or implicitly by the encoder used
            videoStream.Name = "Captura";

            screenThread = new Thread(RecordScreen)
            {
                Name = typeof(Recorder).Name + ".RecordScreen",
                IsBackground = true
            };

            screenThread.Start();
        }

        #endregion [ctor]

        private void RecordScreen()
        {
            var frameInterval = TimeSpan.FromSeconds(1 / (double)writer.FramesPerSecond);
            var buffer = new byte[Params.Width * Params.Height * 4];
            Task videoWriteTask = null;
            var timeTillNextFrame = TimeSpan.Zero;

            while (!stopThread.WaitOne(timeTillNextFrame))
            {
                var timestamp = DateTime.Now;

                Screenshot(buffer);

                // Wait for the previous frame is written
                videoWriteTask?.Wait();

                // Start asynchronous (encoding and) writing of the new frame
                videoWriteTask = videoStream.WriteFrameAsync(true, buffer, 0, buffer.Length);

                timeTillNextFrame = timestamp + frameInterval - DateTime.Now;
                if (timeTillNextFrame < TimeSpan.Zero)
                    timeTillNextFrame = TimeSpan.Zero;
            }

            // Wait for the last frame is written
            videoWriteTask?.Wait();
        }

        public void Screenshot(byte[] Buffer)
        {
            using (var BMP = new Bitmap(Params.Width, Params.Height))
            {
                using (var g = Graphics.FromImage(BMP))
                {
                    g.CopyFromScreen(System.Drawing.Point.Empty, System.Drawing.Point.Empty,
                        new System.Drawing.Size(Params.Width, Params.Height), CopyPixelOperation.SourceCopy);

                    CURSORINFO pci;
                    pci.cbSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(CURSORINFO));

                    if (GetCursorInfo(out pci))
                    {
                        if (pci.flags == CURSOR_SHOWING)
                        {
                            DrawIcon(g.GetHdc(), pci.ptScreenPos.x, pci.ptScreenPos.y, pci.hCursor);
                            g.ReleaseHdc();
                        }
                    }

                    g.Flush();

                    var bits = BMP.LockBits(new Rectangle(0, 0, Params.Width, Params.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);
                    Marshal.Copy(bits.Scan0, Buffer, 0, Buffer.Length);
                    BMP.UnlockBits(bits);
                }
            }
        }

        public void Dispose()
        {
            stopThread.Set();
            screenThread.Join();

            // Close writer: the remaining data is written to a file and file is closed
            writer.Close();

            stopThread.Dispose();
        }
    }
}