using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1
{
   
    public partial class HomeView : UserControl
    {
        public event Action? StartClicked;
        private void Start_Click(object sender, RoutedEventArgs e)
        {
            StartClicked?.Invoke();

        }

       
        public HomeView()
        {
            InitializeComponent();
            var scale = new ScaleTransform(1, 1);
            run_button.RenderTransform = scale;
            run_button.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);
            run_button.PreviewMouseDown += (s, e) =>
            {
                AnimateScale(scale, 0.92);
            };

            run_button.PreviewMouseUp += (s, e) =>
            {
                AnimateScale(scale, 1.0);
            };
        }
        void AnimateScale(ScaleTransform scale, double to)
        {
            var anim = new DoubleAnimation
            {
                To = to,
                Duration = TimeSpan.FromMilliseconds(100)
            };

            scale.BeginAnimation(ScaleTransform.ScaleXProperty, anim);
            scale.BeginAnimation(ScaleTransform.ScaleYProperty, anim);
        }
    }
}
