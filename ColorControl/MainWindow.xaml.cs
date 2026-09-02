using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ColorControl
{
    public partial class MainWindow : Window
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern int ReleaseDC(
            IntPtr hWnd,
            IntPtr hDC);

        [DllImport("gdi32.dll")]
        private static extern bool SetDeviceGammaRamp(
            IntPtr hdc,
            ushort[] ramp);

        private double currentGamma = 1.0;
        private double currentRed = 1.0;
        private double currentGreen = 1.0;
        private double currentBlue = 1.0;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Slider_PreviewMouseDown(
            object sender,
            MouseButtonEventArgs e)
        {
            if (sender is not Slider slider)
                return;

            Point position = e.GetPosition(slider);

            double ratio =
                position.X / slider.ActualWidth;

            ratio = Math.Clamp(ratio, 0.0, 1.0);

            slider.Value =
                slider.Minimum +
                ratio * (slider.Maximum - slider.Minimum);

            e.Handled = true;

            ApplyColorSettings();
        }

        private void ApplyButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            currentGamma = GammaSlider.Value;

            currentRed = RedSlider.Value / 100.0;
            currentGreen = GreenSlider.Value / 100.0;
            currentBlue = BlueSlider.Value / 100.0;

            ApplyColorSettings();
        }

        private void ApplyColorSettings()
        {
            ushort[] ramp = new ushort[768];

            for (int i = 0; i < 256; i++)
            {
                double input = i / 255.0;

                double gammaValue =
                    Math.Pow(input, 1.0 / currentGamma);

                double red =
                    gammaValue * currentRed;

                double green =
                    gammaValue * currentGreen;

                double blue =
                    gammaValue * currentBlue;

                red = Math.Clamp(red, 0.0, 1.0);
                green = Math.Clamp(green, 0.0, 1.0);
                blue = Math.Clamp(blue, 0.0, 1.0);

                ramp[i] =
                    (ushort)(red * 65535.0);

                ramp[256 + i] =
                    (ushort)(green * 65535.0);

                ramp[512 + i] =
                    (ushort)(blue * 65535.0);
            }

            IntPtr hdc = GetDC(IntPtr.Zero);

            SetDeviceGammaRamp(hdc, ramp);

            ReleaseDC(IntPtr.Zero, hdc);
        }

        private void ResetButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            currentGamma = 1.0;
            currentRed = 1.0;
            currentGreen = 1.0;
            currentBlue = 1.0;

            GammaSlider.Value = 1.0;
            RedSlider.Value = 100;
            GreenSlider.Value = 100;
            BlueSlider.Value = 100;

            ApplyColorSettings();
        }
    }
}
