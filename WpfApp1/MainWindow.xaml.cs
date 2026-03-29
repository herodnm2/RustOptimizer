using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Animation;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            SetupButton(Homee_button);
            SetupButton(win_button);
            SetupButton(rust_button);

            Sidebar.Visibility = Visibility.Collapsed;

            var home = new HomeView();
            home.StartClicked += OnStartClicked;

            SidebarColumn.Width = new GridLength(0);
            MainContent.Content = home;

            Loaded += (s, e) => EnableShadow();
        }

        // 🔥 SIDEBAR START
        private void OnStartClicked()
        {
            Sidebar.Visibility = Visibility.Visible;
            SidebarColumn.Width = new GridLength(200);

            HomeButton.Visibility = Visibility.Collapsed;

            SwitchView(new SettingsView());
            SelectButton(Homee_button);
        }

        // 🔥 АНИМАЦИЯ ПЕРЕКЛЮЧЕНИЯ
        void SwitchView(UserControl view)
        {
            var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(150));

            fadeOut.Completed += (s, e) =>
            {
                MainContent.Content = view;

                var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(150));
                MainContent.BeginAnimation(OpacityProperty, fadeIn);
            };

            MainContent.BeginAnimation(OpacityProperty, fadeOut);
        }

        // 🔥 ВЫБОР КНОПКИ (ГЛАВНЫЙ ФИКС)
        void SelectButton(Button btn)
        {
            Homee_button.Tag = null;
            win_button.Tag = null;
            rust_button.Tag = null;

            btn.Tag = "Selected";
        }

        
        private void Home_Click(object sender, RoutedEventArgs e)
        {
            SwitchView(new HomeView());
            SelectButton((Button)sender);
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            SwitchView(new SettingsView());
            SelectButton((Button)sender);
        }
        private void Ruster_Click(object sender, RoutedEventArgs e)
        {
            SwitchView(new WinView());
            SelectButton((Button)sender);
        }
        private void Rust_Click(object sender, RoutedEventArgs e)
        {
            SwitchView(new RustView());
            SelectButton((Button)sender);
        }

        
        private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

       
        private void Close_button(object sender, RoutedEventArgs e)
        {
            Close();
        }

       
        void SetupButton(Button btn)
        {
            var scale = new System.Windows.Media.ScaleTransform(1, 1);

            btn.RenderTransform = scale;
            btn.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);

            btn.PreviewMouseDown += (s, e) =>
            {
                AnimateScale(scale, 0.92);
            };

            btn.PreviewMouseUp += (s, e) =>
            {
                AnimateScale(scale, 1.0);
            };
        }

        void AnimateScale(System.Windows.Media.ScaleTransform scale, double to)
        {
            var anim = new DoubleAnimation
            {
                To = to,
                Duration = TimeSpan.FromMilliseconds(100)
            };

            scale.BeginAnimation(System.Windows.Media.ScaleTransform.ScaleXProperty, anim);
            scale.BeginAnimation(System.Windows.Media.ScaleTransform.ScaleYProperty, anim);
        }

       
        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        private void Resize(IntPtr direction)
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            SendMessage(hwnd, 0x112, direction, IntPtr.Zero);
        }

        private void Resize_Left(object sender, MouseButtonEventArgs e) => Resize((IntPtr)0xF001);
        private void Resize_Right(object sender, MouseButtonEventArgs e) => Resize((IntPtr)0xF002);
        private void Resize_Top(object sender, MouseButtonEventArgs e) => Resize((IntPtr)0xF003);
        private void Resize_Bottom(object sender, MouseButtonEventArgs e) => Resize((IntPtr)0xF006);

        private void Resize_TopLeft(object sender, MouseButtonEventArgs e) => Resize((IntPtr)0xF004);
        private void Resize_TopRight(object sender, MouseButtonEventArgs e) => Resize((IntPtr)0xF005);
        private void Resize_BottomLeft(object sender, MouseButtonEventArgs e) => Resize((IntPtr)0xF007);
        private void Resize_BottomRight(object sender, MouseButtonEventArgs e) => Resize((IntPtr)0xF008);

       
        private void EnableShadow()
        {
            var hwnd = new WindowInteropHelper(this).Handle;

            var margins = new MARGINS
            {
                cxLeftWidth = 0,
                cxRightWidth = 0,
                cyTopHeight = 0,
                cyBottomHeight = 0
            };

            DwmExtendFrameIntoClientArea(hwnd, ref margins);
        }

        [DllImport("dwmapi.dll")]
        public static extern int DwmExtendFrameIntoClientArea(IntPtr hwnd, ref MARGINS margins);

        public struct MARGINS
        {
            public int cxLeftWidth;
            public int cxRightWidth;
            public int cyTopHeight;
            public int cyBottomHeight;
        }
    }
}