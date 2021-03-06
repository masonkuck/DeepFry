﻿using Microsoft.Win32;
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Media;

namespace DeepFry
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int redValue { get; set; }
        private int greenValue { get; set; }
        private int blueValue { get; set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void UpdatePixel(int red = -1, int green = -1, int blue = -1)
        {
            if (red > -1)
                redValue = red;

            if (green > -1)
                greenValue = green;

            if (blue > -1)
                blueValue = blue;


            pixelRect.Fill = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, (byte)redValue, (byte)greenValue, (byte)blueValue));

            Console.WriteLine("R:" + redValue + " G:" + greenValue + " B:" + blueValue);
        }

        private void redScewSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int i = (int)e.NewValue;

            UpdatePixel(red: i);
        }

        private void greenScewSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int i = (int)e.NewValue;
            UpdatePixel(green: i);
        }

        private void blueScewSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int i = (int)e.NewValue;
            UpdatePixel(blue: i);
        }

        private DeepFryCore.DeepFryUtility util;
        private void convertButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            if (ofd.ShowDialog() == true)
            {
                // not used in the present implementation of this example app. Only used for saving
                //int encodingLevel = (int)encodingSlider.Value;
                int noise = (int)noiseSlider.Value;

                util = new DeepFryCore.DeepFryUtility(ofd.FileName);

                Bitmap preview = util.DeepFry(RedScew: (uint)redValue, GreenScew: (uint)greenValue, BlueScew: (uint)blueValue, NoisePercentage: (uint)noise);


                PreviewImage previewImage = new PreviewImage(preview);

                previewImage.ShowDialog();
            }
        }

        private void distortButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            if (ofd.ShowDialog() == true)
            {
                int circleX = (int)xSlider.Value;
                int circleY = (int)ySlider.Value;
                int radius = (int)radiusSlider.Value;

                util = new DeepFryCore.DeepFryUtility(ofd.FileName);
                Bitmap preview = util.WIPDistort((uint)circleX, (uint)circleY, (uint)radius);

                PreviewImage previewImage = new PreviewImage(preview);

                previewImage.ShowDialog();
            }
        }
    }
}
