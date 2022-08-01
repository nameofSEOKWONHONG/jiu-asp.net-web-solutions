using Microsoft.Win32;
using SharpAvi.Codecs;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace SimpleRecorderWpf.Codec
{
    /// <summary title="동영상 문의 CodecHandler">
    /// 1. Create Date :  2021.07.19
    /// 2. Creator : 홍석원
    /// 3. Description : CodecHandler
    /// 4. Precaution :
    /// 5. History : 
    /// 6. MenuPath :  
    /// 7. OldName : NEW
    /// </summary>
    public class CodecHandler
    {
        /// <summary>
        /// X264 코덱 설치 여부
        /// </summary>
        /// <returns></returns>
        public static bool IsExistsX264Codec()
        {
            var codecs = Mpeg4VideoEncoderVcm.GetAvailableCodecs();
            if (codecs == null || codecs.Length <= 0) return false;
            return codecs.FirstOrDefault(m => m.Name == "x264vfw - H.264/MPEG-4 AVC codec") != null;
        }

        /// <summary>
        /// X264 코덱 설정
        /// </summary>
        public static void x264CodecSetting()
        {
            RegistryHelper.x264LogLevelDisable();
            RegistryHelper.x264Zerolatency();
        }

        #region [install codec]

        /// <summary>
        /// X264 코덱 설치
        /// </summary>
        public static void SetupX264()
        {
            var fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "x264vfw_full_44_2851bm_44825.exe");
            var fileBuffer = SimpleRecorderWpf.Properties.Resources.x264vfw_full_44_2851bm_44825;
            using (var stream = new FileStream(fileName, FileMode.OpenOrCreate))
            {
                stream.Write(fileBuffer, 0, fileBuffer.Length);
                stream.Flush();
                stream.Close();
            }

            Process p = new Process();
            p.StartInfo = new ProcessStartInfo(fileName, "/S");
            p.Start();

            p.WaitForExit();
        }

        #endregion [install codec]
    }

    /// <summary title="동영상 문의 RegistryHelper">
    /// 1. Create Date :  2021.07.19
    /// 2. Creator : 홍석원
    /// 3. Description : RegistryHelper
    /// 4. Precaution :
    /// 5. History : 
    /// 6. MenuPath :  
    /// 7. OldName : NEW
    /// </summary>
    internal class RegistryHelper
    {
        /// <summary>
        /// x264로그 표시 비활성화
        /// </summary>
        public static void x264LogLevelDisable()
        {
            #region[64bit]
            var win64LogRegistry = Registry.CurrentUser.OpenSubKey("Software\\GNU\\x264", true);
            if (win64LogRegistry == null)
            {
                Registry.CurrentUser.CreateSubKey("Software\\GNU\\x264");
                win64LogRegistry = Registry.CurrentUser.OpenSubKey("Software\\GNU\\x264", true);
            }

            var win64LogValue = win64LogRegistry.GetValue("log_level");
            if (win64LogValue == null)
            {
                win64LogRegistry.SetValue("log_level", 0);
            }
            else
            {
                if (win64LogValue.ToString() != "0")
                {
                    win64LogRegistry.SetValue("log_level", 0);
                }
            }
            #endregion

            #region [32bit]
            var win32LogRegistry = Registry.CurrentUser.OpenSubKey("Software\\GNU\\x264vfw64", true);
            if (win32LogRegistry == null)
            {
                Registry.CurrentUser.CreateSubKey("Software\\GNU\\x264vfw64");
                win32LogRegistry = Registry.CurrentUser.OpenSubKey("Software\\GNU\\x264vfw64", true);
            }

            var win32LogValue = win32LogRegistry.GetValue("log_level");
            if (win32LogValue == null)
            {
                win32LogRegistry.SetValue("log_level", 0);
            }
            else
            {
                if (win32LogValue.ToString() != "0")
                {
                    win64LogRegistry.SetValue("log_level", 0);
                }
            }
            #endregion
        }

        /// <summary>
        /// x264 코덱 즉시 실행 설정
        /// </summary>
        public static void x264Zerolatency()
        {
            #region [64bit]
            var win64Registry = Registry.CurrentUser.OpenSubKey("Software\\GNU\\x264", true);
            if (win64Registry == null)
            {
                Registry.CurrentUser.CreateSubKey("Software\\GNU\\x264");
                win64Registry = Registry.CurrentUser.OpenSubKey("Software\\GNU\\x264", true);
            }

            var win64Value = win64Registry.GetValue("zerolatency");
            if (win64Value == null)
            {
                win64Registry.SetValue("zerolatency", 1);
            }
            else
            {
                if (win64Value.ToString() != "0")
                {
                    win64Registry.SetValue("zerolatency", 1);
                }
            }
            #endregion

            #region [32bit]
            var win32Registry = Registry.CurrentUser.OpenSubKey("Software\\GNU\\x264vfw64", true);
            if (win32Registry == null)
            {
                Registry.CurrentUser.CreateSubKey("Software\\GNU\\x264vfw64");
                win32Registry = Registry.CurrentUser.OpenSubKey("Software\\GNU\\x264vfw64", true);
            }

            var win32Value = win32Registry.GetValue("zerolatency");
            if (win32Value == null)
            {
                win32Registry.SetValue("zerolatency", 1);
            }
            else
            {
                if (win32Value.ToString() == "0")
                {
                    win32Registry.SetValue("zerolatency", 1);
                }
            }
            #endregion
        }
    }
}