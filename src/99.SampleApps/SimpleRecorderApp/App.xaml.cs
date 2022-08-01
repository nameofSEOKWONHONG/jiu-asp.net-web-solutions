using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using SimpleRecorderWpf.Utils;

namespace SimpleRecorderWpf
{
    /// <summary title="동영상 문의 App">
    /// 1. Create Date :  2021.07.19
    /// 2. Creator : 홍석원
    /// 3. Description : 동영상 문의 App
    /// 4. Precaution : 
    /// 5. History : 
    /// 6. MenuPath :  
    /// 7. OldName : NEW
    /// </summary> 
    public partial class App : Application
    {
        /// <summary>
        /// 중복실행방지 Mutex
        /// </summary>
        private System.Threading.Mutex _mutex;

        protected override void OnStartup(StartupEventArgs e)
        {
            string mutexName = "SimpleRecorderWpf";
            bool isCreatedNew = false;
            try
            {
                _mutex = new System.Threading.Mutex(true, mutexName, out isCreatedNew);

                LanguageChange(CultureInfo.CurrentCulture);
       
                if (!isCreatedNew)
                {
                    MessageBox.Show(System.Windows.Application.Current.FindResource("AlreadyExecute").ToString(), System.Windows.Application.Current.FindResource("AlreadyExecuteCaption").ToString(), MessageBoxButton.OK, MessageBoxImage.Information);
                    Environment.Exit(0);
                }
                else
                {
                    //Application Exception Event Handler
                    this.Dispatcher.UnhandledException += (o, ex) =>
                    {
                        MessageBox.Show(ex.Exception.Message, Application.Current.FindResource("Caption").ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                        RecorderUtil.WriteLog(ex.Exception);
                        Environment.Exit(0);
                    };
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.Current.FindResource("Caption").ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                RecorderUtil.WriteLog(ex);
                Environment.Exit(-1);
            }

            base.OnStartup(e);
        }

        #region [application 언어 설정]

        /// <summary>
        /// 언어 설정
        /// </summary>
        /// <param name="cultureInfo"></param>
        private static void LanguageChange(CultureInfo cultureInfo)
        {
            var exists = (new[] { "ko-KR", "en-US", "es", "id-ID", "ja-JP", "th-TH", "vi-VN", "zh-CN", "zh-TW" }).FirstOrDefault(m => m == (cultureInfo.Name));
            if(exists == null)
            {
                //영어권으로 강제 설정
                cultureInfo = new CultureInfo("en-US");
            }
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary()
            {
                Source = new Uri(string.Format("..\\Language\\StringResource.{0}.xaml", cultureInfo.Name), UriKind.Relative)
            });
        }

        #endregion [application 언어 설정]
    }
}