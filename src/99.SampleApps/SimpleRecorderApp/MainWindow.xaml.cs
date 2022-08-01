using ControlzEx;
using SimpleRecorderWpf.Codec;
using SimpleRecorderWpf.Utils;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace SimpleRecorderWpf
{
    /// <summary title="동영상 문의 MainWindow">
    /// 1. Create Date :  2021.07.19
    /// 2. Creator : 홍석원
    /// 3. Description : 동영상 문의 MainWindow
    /// 4. Precaution : 
    /// 5. History : 
    /// 6. MenuPath :  
    /// 7. OldName : NEW
    /// </summary> 
    public partial class MainWindow : MetroWindow
    {
        #region [private]

        /// <summary>
        /// 녹화 시간
        /// </summary>
        private DateTime _workTime = new DateTime(2021, 01, 01, 0, 0, 0);

        /// <summary>
        /// 녹화 시간 타이머
        /// </summary>
        private readonly Timer _workTimer = new Timer();

        /// <summary>
        /// mp4 파일명
        /// </summary>
        private string _fileName = string.Empty;

        /// <summary>
        /// 녹화 구현 클래스
        /// </summary>
        private Recorder _recorder = null;

        /// <summary>
        /// 화면 포인터 구현 Window 클래스
        /// </summary>
        private PointerWindow _pointerWindow = new PointerWindow();

        private int _limitCnt = 0;
        /// <summary>
        /// 1:30초 녹화 후 멈춘 경우에 사용함.
        /// </summary>
        private static object _syncLock = new object();

        private PropertyChangeNotifier _topMostChangeNotifier;

        private const int SPANISH_LANGUAGE_WIDTH = 260;
        #endregion [private]

        #region [ctor]

        public MainWindow()
        {
            InitializeComponent();
            this.SetPrimaryMoniorInfo();

            this.Loaded += (s, e) =>
            {
                #region [초기화 작업]
                _topMostChangeNotifier = new PropertyChangeNotifier(this, Window.TopmostProperty);
                _topMostChangeNotifier.ValueChanged += (ss, ee) =>
                {
                    this.Topmost = true;
                };


                _pointerWindow.Show();

                InitWindow initWindow = new InitWindow();
                initWindow.ShowDialog();
                this._fileName = initWindow.FileName;
                #endregion [초기화 작업]

                #region [녹화 설정 및 시작]

                _recorder = new Recorder(new RecorderParams(
                    _fileName,
                    15,
                    SharpAvi.KnownFourCCs.Codecs.X264,
                    70));

                #endregion [녹화 설정 및 시작]

                #region [이벤트 등록]

                this.SetTimerEvent();
                this.SetWindowEvent();
                this.SetCloseEvent();

                #endregion [이벤트 등록]
            };
        }
        #endregion [ctor]

        #region [초기화 작업]

        /// <summary>
        /// 메인 모니터 확인
        /// </summary>
        private void SetPrimaryMoniorInfo()
        {
            var primaryPoint = Screen.AllScreens.Where(s => s.Primary == true).FirstOrDefault();
            if (primaryPoint != null)
            {
                var workingArea = primaryPoint.WorkingArea;
                var startLocation = new System.Windows.Point(workingArea.Size.Width - this.Width, workingArea.Size.Height - this.Height);
                this.WindowStartupLocation = WindowStartupLocation.Manual;
                this.Left = startLocation.X - 5;
                this.Top = startLocation.Y - 5;
            }
        }

        #endregion [초기화 작업]

        #region [이벤트]

        private void SetTimerEvent()
        {
            const int MAX_LIMIT_CNT = 9999999;

            this._workTimer.Enabled = true;
            this._workTimer.Interval = 1000 * 1; //1초
            this._workTimer.Tick += (sender, args) =>
            {
                RecordingTime.Content = _workTime.ToString("mm:ss");
                _workTime = _workTime.AddSeconds(1);

                if(this._limitCnt > MAX_LIMIT_CNT)
                {
                    this._workTimer.Stop();
                    lock(_syncLock)
                    {
                        System.Windows.Application.Current.Dispatcher.Invoke(new Action(() => { this.StopClick(null, null); }));
                    }
                }
                this._limitCnt += 1;
            };
            this._workTimer.Start();
        }

        private void SetWindowEvent()
        {
            this.MouseDown += (s, e) =>
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    DragMove();
                }
            };

            this.btnStop.Click += StopClick;
        }

        private void StopClick(object sender, EventArgs args)
        {
            if (_recorder != null)
            {
                _recorder.Dispose();
                _pointerWindow.Close();
                System.Threading.Thread.Sleep(1000);

                //ProcessStartInfo startInfo = new ProcessStartInfo
                //{
                //    Arguments = @"C:\Users\19066Seokwon\AppData\Roaming\OwnerRecorder",
                //    FileName = "explorer.exe"
                //};

                //Process.Start(startInfo);
            }

            this.Hide();
            Environment.Exit(0);
        }

        private void SetCloseEvent()
        {
            this.Closing += (sender, args) =>
            {
                if (this._workTimer != null)
                {
                    this._workTimer.Stop();
                    this._workTimer.Dispose();
                }
            };
        }

        #endregion [이벤트]
    }
}