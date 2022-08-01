using Gma.System.MouseKeyHook;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;

namespace SimpleRecorderWpf
{
    /// <summary title="동영상 문의 PointerWindow">
    /// 1. Create Date :  2021.07.19
    /// 2. Creator : 홍석원
    /// 3. Description : 동영상 문의 PointerWindow
    /// 4. Precaution : 
    /// 5. History : [2021.08.31](홍석원) - 마우스 위치에 기본적으로 남색 서클이 표시되도록 변경함.
    /// 6. MenuPath :  
    /// 7. OldName : NEW
    /// </summary> 
    public partial class PointerWindow : Window
    {
        #region [win32]

        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_NOACTIVATE = 0x08000000;

        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        #endregion [win32]

        #region [private]

        /// <summary>
        /// 마우스 hook 인터페이스
        /// Window 마우스 이벤트를 활용할 수도 있겠으나 일단 후킹으로 처리하도록 함.
        /// </summary>
        private IKeyboardMouseEvents hook;
        /// <summary>
        /// 보정값 width
        /// </summary>
        private const int ARRANGE_WIDTH = 20;
        /// <summary>
        /// 보정값 height
        /// </summary>
        private const int ARRANGE_HEIGHT = 20;
        /// <summary>
        /// 노란색 포인터 추가 보정값 width
        /// </summary>
        private const int YELLOW_POINTER_ARRAGE_WIDTH = 10;
        /// <summary>
        /// 노란색 포인터 추가 보정값 height
        /// </summary>
        private const int YELLOW_POINTER_ARRAGE_HEIGHT = 10;


        #endregion [private]

        #region [ctor]

        public PointerWindow()
        {
            InitializeComponent();

            #region [mousehook 이벤트 등록]

            this.Loaded += (s, e) =>
            {
                #region [pointer circle color설정 및 위치 초기화]
                var strokeBrush = new SolidColorBrush(Color.FromRgb(255, 204, 60));
                this.pointer.Stroke = strokeBrush;

                var point = System.Windows.Forms.Cursor.Position;
                Canvas.SetLeft(this.pointer, (point.X - ARRANGE_WIDTH) - YELLOW_POINTER_ARRAGE_WIDTH);
                Canvas.SetTop(this.pointer, (point.Y - ARRANGE_HEIGHT) - YELLOW_POINTER_ARRAGE_HEIGHT);
                #endregion

                #region [mouse hooking 초기화]
                hook = Hook.GlobalEvents();

                hook.MouseDown += Hook_MouseDown;
                hook.MouseMove += Hook_MouseMove;
                #endregion

                this.Topmost = false;
                this.Topmost = true;
            };

            this.Closing += (s, e) =>
            {
                if (hook != null)
                {
                    hook.MouseDown -= Hook_MouseDown;
                    hook.MouseMove -= Hook_MouseMove;
                    hook.Dispose();
                    hook = null;
                }
            };

            #endregion [mousehook 이벤트 등록]
        }

        #endregion [ctor]

        private void Hook_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Canvas.SetLeft(this.pointer, (e.X - ARRANGE_WIDTH) - YELLOW_POINTER_ARRAGE_WIDTH);
            Canvas.SetTop(this.pointer, (e.Y - ARRANGE_HEIGHT) - YELLOW_POINTER_ARRAGE_HEIGHT);
        }

        /// <summary>
        /// 마우스 다운 클릭시 포인터 활성화 및 비활성화
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hook_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Canvas.SetLeft(this.highlight, e.X - ARRANGE_WIDTH);
            Canvas.SetTop(this.highlight, e.Y - ARRANGE_HEIGHT);

            Task.Factory.StartNew(() =>
            {
                SetTimeout(() =>
                {
                    this.highlight.Visibility = Visibility.Visible;
                    SetTimeout(() =>
                    {
                        this.highlight.Visibility = Visibility.Hidden;
                    }, 150);
                }, 10);
            });
        }

        /// <summary>
        /// javascript의 setTimeout과 역활이 같다.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="timeout"></param>
        private void SetTimeout(Action action, int timeout)
        {
            Thread t = new Thread(() =>
            {
                Thread.Sleep(timeout);
                this.Dispatcher.Invoke(() =>
                {
                    if (action != null)
                    {
                        action();
                    }
                });
            });
            t.Start();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            //윈도우 활성화를 막는다. 해당 창으로 포커스가 오지 않도록 함.
            //var helper = new WindowInteropHelper(this);
            //SetWindowLong(helper.Handle, GWL_EXSTYLE,
            //    GetWindowLong(helper.Handle, GWL_EXSTYLE) | WS_EX_NOACTIVATE);
        }
    }
}