using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace WpfApp1
{
    public partial class WinView : UserControl
    {
        private const string UltimateGuid = "e9a42b02-d5df-448d-aa00-03f14749eb61";

        public WinView()
        {
            InitializeComponent();
            DetectPowerPlan();
            NotifyBox.RenderTransform = new TranslateTransform(0, 20);
        }
        private void ApplyUltimate()
        {
            RunCmd(
                "for /f \"tokens=3\" %%a in ('powercfg -duplicatescheme e9a42b02-d5df-448d-aa00-03f14749eb61') do powercfg /setactive %%a"
            );
        }
        private void DetectPowerPlan()
        {
            string output = RunCmdRead("powercfg /getactivescheme").ToLower();

            bool isHigh = output.Contains("8c5e7fda");

            PowerToggle.IsChecked = isHigh;

            if (isHigh)
            {
                PowerText.Text = "Power Plan: MAX PERFORMANCE";
                PowerText.Foreground = Brushes.LimeGreen;
            }
            else
            {
                PowerText.Text = "Power Plan: DEFAULT";
                PowerText.Foreground = Brushes.Red;
            }
        }
        private string RunCmdAdminRead(string command)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = "/c " + command,
                Verb = "runas",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var process = Process.Start(psi)!;
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            return output;
        }
        private void PowerToggle_Changed(object sender, RoutedEventArgs e)
        {
            if (PowerToggle.IsChecked == true)
            {
                RunCmd("powercfg /setactive SCHEME_MIN");
                ShowNotify("Power Plan applied!");
            }
            else
            {
                RunCmd("powercfg /setactive SCHEME_BALANCED");
            }

            System.Threading.Thread.Sleep(300);
            DetectPowerPlan();
        }
        void ShowNotify(string text)
        {
            NotifyText.Text = text;
            NotifyBox.Opacity = 1;

            var transform = NotifyBox.RenderTransform as TranslateTransform;
            if (transform == null)
            {
                transform = new TranslateTransform(0, 20);
                NotifyBox.RenderTransform = transform;
            }

            var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(200));
            var moveUp = new DoubleAnimation(20, 0, TimeSpan.FromMilliseconds(200));

            NotifyBox.BeginAnimation(OpacityProperty, fadeIn);
            transform.BeginAnimation(TranslateTransform.YProperty, moveUp);

            var timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(2);
            timer.Tick += (s, e) =>
            {
                timer.Stop();

                var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(200));
                NotifyBox.BeginAnimation(OpacityProperty, fadeOut);
            };
            timer.Start();
        }
        private void PowerHint_Show(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(200));
            HintBox.BeginAnimation(OpacityProperty, fadeIn);
        }

        private void PowerHint_Hide(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(200));
            HintBox.BeginAnimation(OpacityProperty, fadeOut);
        }
        private string RunCmdRead(string command)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = "/c " + command,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var process = Process.Start(psi)!;
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            return output;
        }
        private void RunCmd(string command)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = "/c " + command,
                    Verb = "runas",
                    UseShellExecute = true,
                    CreateNoWindow = true
                };

                var process = Process.Start(psi);
                process?.WaitForExit(); 
            }
            catch
            {
            }
        }
    }
}