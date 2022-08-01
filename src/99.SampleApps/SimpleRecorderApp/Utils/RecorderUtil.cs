using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Management;
using System.Text.RegularExpressions;

namespace SimpleRecorderWpf.Utils
{
    /// <summary title="동영상 문의 RecorderUtil">
    /// 1. Create Date :  2021.07.19
    /// 2. Creator : 홍석원
    /// 3. Description : 동영상 문의 RecorderUtil
    /// 4. Precaution :
    /// 5. History : 
    /// 6. MenuPath :  
    /// 7. OldName : NEW
    /// </summary>
    public class RecorderUtil
    {
        public static string[] GetAllIPv4Address()
        {
            List<string> ipv4s = new List<string>();
            Regex regex = new Regex(@"^(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])$");

            foreach (System.Net.IPAddress ip in System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).AddressList)
            {
                if (regex.IsMatch(ip.ToString()))
                {
                    ipv4s.Add(ip.ToString());
                }
            }

            return ipv4s.ToArray();
        }

        public static string GetWindowsType()
        {
            string query = "SELECT * FROM Win32_OperatingSystem";

            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
            {
                foreach (ManagementObject info in searcher.Get())
                {
                    var os = info.Properties["Caption"].Value.ToString().ToUpper();
                    if (os.Contains("WINDOWS 7")) return "WINDOWS_7";
                    else if (os.Contains("WINDOWS 10")) return "WINDOWS_10";
                    else if (os.Contains("WINDOWS XP")) return "WINDOWS_XP";
                    else if (os.Contains("WINDOWS VISTA")) return "WINDOWS_VISTA";
                    else return "UNKNOWN";
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// 로그 작성 (프로그램 실행 폴더내에 시간별 로그로 기록함)
        /// </summary>
        /// <param name="e"></param>
        public static void WriteLog(Exception e)
        {            
            using(var writer = File.CreateText($"./{DateTime.Now.ToString("yyyyMMddHHmmss")}.log"))
            {
                var setting = new JsonSerializerSettings() { Error = (se, ev) => { ev.ErrorContext.Handled = true; } };
                writer.WriteLine(JsonConvert.SerializeObject(e, Formatting.Indented, setting));
            }
        }
    }
}