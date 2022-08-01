using SharpAvi;
using SharpAvi.Codecs;
using SharpAvi.Output;
using System.Windows.Forms;

namespace SimpleRecorderWpf.Codec
{
    /// <summary title="동영상 문의 RecorderParams">
    /// 1. Create Date :  2021.07.19
    /// 2. Creator : 홍석원
    /// 3. Description : RecorderParams
    /// 4. Precaution :
    /// 5. History : 
    /// 6. MenuPath :  
    /// 7. OldName : NEW
    /// </summary>
    public class RecorderParams
    {
        #region [property]

        public string FileName { get; private set; }
        public int FramesPerSecond { get; private set; }
        public int Quality { get; private set; }
        public FourCC Codec { get; private set; }
        public int Left { get; private set; }
        public int Top { get; private set; }
        public int Height { get; private set; }
        public int Width { get; private set; }

        #endregion [property]

        #region [ctor]

        /// <summary>
        /// 레코더 파라메터
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="FrameRate"></param>
        /// <param name="Encoder"></param>
        /// <param name="Quality"></param>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public RecorderParams(string filename, int FrameRate, FourCC Encoder, int Quality, int left = 0, int top = 0, int width = 0, int height = 0)
        {
            FileName = filename;
            FramesPerSecond = FrameRate;
            Codec = Encoder;
            this.Quality = Quality;

            if (width <= 0) Width = Screen.PrimaryScreen.Bounds.Width;
            else Width = width;
            if (height <= 0) Height = Screen.PrimaryScreen.Bounds.Height;
            else Height = height;

            Left = left;
            Top = top;
        }

        #endregion [ctor]

        public AviWriter CreateAviWriter()
        {
            return new AviWriter(FileName)
            {
                FramesPerSecond = FramesPerSecond,
                EmitIndex1 = true,
            };
        }

        public IAviVideoStream CreateVideoStream(AviWriter writer)
        {
            // Select encoder type based on FOURCC of codec
            if (Codec == KnownFourCCs.Codecs.Uncompressed)
                return writer.AddUncompressedVideoStream(Width, Height);
            else if (Codec == KnownFourCCs.Codecs.MotionJpeg)
                return writer.AddMotionJpegVideoStream(Width, Height, Quality);
            else
            {
                return writer.AddMpeg4VideoStream(Width, Height, (double)writer.FramesPerSecond,
                    // It seems that all tested MPEG-4 VfW codecs ignore the quality affecting parameters passed through VfW API
                    // They only respect the settings from their own configuration dialogs, and Mpeg4VideoEncoder currently has no support for this
                    quality: Quality,
                    codec: Codec,
                    // Most of VfW codecs expect single-threaded use, so we wrap this encoder to special wrapper
                    // Thus all calls to the encoder (including its instantiation) will be invoked on a single thread although encoding (and writing) is performed asynchronously
                    forceSingleThreadedAccess: true);
            }
        }
    }
}