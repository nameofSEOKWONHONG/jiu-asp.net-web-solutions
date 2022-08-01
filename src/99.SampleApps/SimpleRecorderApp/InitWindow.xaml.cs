using SimpleRecorderWpf.Codec;
using SimpleRecorderWpf.Utils;
using MahApps.Metro.Controls;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace SimpleRecorderWpf
{
    /// <summary title="동영상 문의 InitWindow">
    /// 1. Create Date :  2021.07.19
    /// 2. Creator : 홍석원
    /// 3. Description : 동영상 문의 InitWindow
    /// 4. Precaution : 
    /// 5. History : [2021.08.06](홍석원) - 업로드시에 오류 발생하는 이슈가 있어 파일명이 같은 케이스로 예상되어 GUID로 진행하도록 함.
    ///                                  - SessionId로 할 수도 있겠지만 난수로 하는게 맞을 것으로 예상됨.
    /// 6. MenuPath :  
    /// 7. OldName : NEW
    /// </summary> 
    public partial class InitWindow : MetroWindow
    {
        /// <summary>
        /// 초기화시 설정한 파일명 (yyyyMMddHHmmss_[guid(5charactor)])
        /// </summary>
        public string FileName { get; private set; }
        private const string USER_APPLICATION_FOLDER_NAME = "OwnerRecorder";

        /// <summary>
        /// 초기화 설정 윈도우
        /// </summary>
        public InitWindow()
        {
            InitializeComponent();

            this.Loaded += async (s, e) => {

                //if (SimpleRecorderWpf.Properties.Settings.Default.InitCodec == "N")
                //{
                //    SimpleRecorderWpf.Properties.Settings.Default.InitCodec = "Y";
                //    SimpleRecorderWpf.Properties.Settings.Default.Save();
                //}
                this.InitFile();                      
                await Task.Delay(500);
                this.InitCodec();
                await Task.Delay(1500);

                this.DialogResult = true;
            };
        }

        /// <summary>
        /// 프로그램 시작시에만 삭제 처리함.
        /// 종료 시에 삭제할 수도 있지만 사용자가 수동으로 업로드할 수도 있는 상황에 대비하기 위하여 시작시에만 삭제하는 것으로 함.
        /// </summary>
        private void DeleteMp4Files()
        {
            var exists = System.IO.Path.Combine(Environment.GetFolderPath(System.Environment.SpecialFolder
                .ApplicationData), USER_APPLICATION_FOLDER_NAME);

            if (!Directory.Exists(exists))
            {
                Directory.CreateDirectory(exists);
            }

            var mp4Files = Directory.GetFiles(exists, "*.mp4");
            if (mp4Files!= null)
            {
                foreach (var file in mp4Files)
                {
                    File.Delete(file);
                }
            }
        }

        /// <summary>
        /// 코덱 설치 및 설정
        /// </summary>
        private void InitCodec()
        {
            //코덱 설치 유무 확인
            if (!CodecHandler.IsExistsX264Codec())
            {
                //코덱 설치
                CodecHandler.SetupX264();

                //코덱 설치 유무 재확인
                if (!CodecHandler.IsExistsX264Codec())
                {
                    //실패시 강제 종료
                    MessageBox.Show(System.Windows.Application.Current.FindResource("CodecErrorAfterInstall").ToString(), System.Windows.Application.Current.FindResource("ErrorCaption").ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                    Environment.Exit(0);
                }
            }
            
            //설치된 코덱 설정
            CodecHandler.x264CodecSetting();
        }

        /// <summary>
        /// 녹화 파일명 생성
        /// </summary>
        private void InitFile()
        {
            var exists = System.IO.Path.Combine(Environment.GetFolderPath(System.Environment.SpecialFolder
                .ApplicationData), USER_APPLICATION_FOLDER_NAME);

            if (!Directory.Exists(exists))
            {
                Directory.CreateDirectory(exists);
            }

            this.FileName = $"{exists}\\SimpleRecorder_{DateTime.Now.ToString("yyyyMMddHHmmss")}_{Guid.NewGuid().ToString("N").Substring(0, 5)}.mp4";
        }

    }
}
