﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;
using Emgu.CV;
using Emgu.Util;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System.IO;

namespace FaceDetectionCamera
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        FilterInfoCollection filter;
        VideoCaptureDevice device;
        private void Form1_Load(object sender, EventArgs e)
        {
            filter = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo device in filter)
                cboDevice.Items.Add(device.Name);
            cboDevice.SelectedIndex = 0;
            device = new VideoCaptureDevice();
        }

        private void btnDetect_Click(object sender, EventArgs e)
        {
            device = new VideoCaptureDevice(filter[cboDevice.SelectedIndex].MonikerString);
            device.NewFrame += Device_NewFrame;
            device.Start();

        }
        static readonly CascadeClassifier cascadeClassifier = new CascadeClassifier("haarcascade_frontalface_alt_tree.xml");
        static readonly CascadeClassifier smileClassifier = new CascadeClassifier("haarcascade_smile.xml");
        private void Device_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone();
            Image<Bgr, byte> grayImage = new Image<Bgr, byte>(bitmap);
            Rectangle[] rectangles =cascadeClassifier.DetectMultiScale(grayImage, 1.2, 1);
            foreach (Rectangle rectangle in rectangles)
            {
                using(Graphics graphics=Graphics.FromImage(bitmap))
                {
                    using(Pen pen=new Pen(Color.Red,1))
                    {
                        graphics.DrawRectangle(pen, rectangle);
                    }
                }
               
            }

            Rectangle[] rect = smileClassifier.DetectMultiScale(grayImage, 1.8, 20);
                foreach (Rectangle r in rect)
                {
                   using (Graphics graphics = Graphics.FromImage(bitmap))
                   {
                     using (Pen pen = new Pen(Color.Green, 1))
                     {
                        graphics.DrawRectangle(pen, r);
                     }

                   }
                }
            pic.Image = bitmap; 
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (device.IsRunning)
                device.Stop();
        }
    }
}
