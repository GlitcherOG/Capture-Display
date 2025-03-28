﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAudio.Wave;
using NAudio.CoreAudioApi;
using System.Numerics;
using GitHub.secile.Video;

namespace CaptureDisplay
{
    public partial class DisplayLabel : Form
    {
        public static Settings settings;
        UsbCamera captureDevice;

        string[] infoCollection;
        private delegate void SafeCallDelegate();

        private WasapiCapture wave;
        private WasapiOut waveOut;
        private BufferedWaveProvider provider;

        bool showFPS = true;
        bool isFullscreen;

        public double CurrentFPS; /*{ get; private set; }*/
        public DateTimeOffset dateTime;
        public double Count = 0;

        public static Size size;
        public static Point point;
        public static DockStyle dockStyle;

        bool RACCheck; // Resize and Change check, to disable framedrawing when modes or sizes are being set
        bool QuickFix; // Patch bool to fix a thing, best left untouched

        bool isDrawing;

        public DisplayLabel()
        {
            Closed += Form1_Closed;
            settings = Settings.Load(Application.UserAppDataPath);
            this.KeyPreview = true;
            InitializeComponent();

            DisplaySizeComboBox.SelectedIndex = 0;
            pictureBox1.BackColor = Color.Black;
            pictureBox1.Dock = DockStyle.Fill;

            showFPS = false;
            FPSCounter.Visible = showFPS;

            DisableSleep.KeepAwake();
        }

        #region Settings
        bool Loading;
        void LoadSettings()
        {
            Loading = true;
            DisplaySizeComboBox.SelectedIndex = settings.DisplayModePos;
            if (VideoComboBox.Items.Contains(settings.VideoName))
            {
                VideoComboBox.SelectedIndex = VideoComboBox.Items.IndexOf(settings.VideoName);
                UpdateDisplayMode();
            }
            if (AudioComboBox.Items.Contains(settings.AudioName))
            {
                AudioComboBox.SelectedIndex = AudioComboBox.Items.IndexOf(settings.AudioName);
                AudioCapture();
            }
            Loading = false;
        }
        void SetSettings()
        {
            if (!Loading)
            {
                settings.DisplayModePos = DisplaySizeComboBox.SelectedIndex;

                if (VideoComboBox.SelectedItem != null)
                    settings.VideoName = VideoComboBox.SelectedItem.ToString();

                if (RenderSizeComboBox.SelectedItem != null)
                    settings.RenderSizePos = RenderSizeComboBox.SelectedItem.ToString();

                if (AudioComboBox.SelectedItem != null)
                    settings.AudioName = AudioComboBox.SelectedItem.ToString();

                settings.Save(Application.UserAppDataPath);
            }
        }
        #endregion

        #region Form Opening and Closing
        private void Form1_Load(object sender, EventArgs e)
        {
            GetDevicesLists();

            LoadSettings();

            UpdateDisplayMode();

            Thread thread1 = new Thread(WriteLableFPS);
            thread1.Start();
        }
        private void Form1_Closed(object sender, EventArgs e)
        {
            if (captureDevice != null)
            {
                if (wave != null)
                {
                    wave.StopRecording();
                    waveOut.Stop();
                }

                showFPS = false;

                if (captureDevice != null)
                {
                    captureDevice.Stop();
                }
            }
            captureDevice = null;
            wave = null;
            Environment.Exit(1);
        }
        #endregion

        void GetDevicesLists(bool SetLast = false)
        {
            string VideoDevice = VideoComboBox.Text;
            string AudioDevice = AudioComboBox.Text;

            VideoComboBox.Items.Clear();
            AudioComboBox.Items.Clear();

            infoCollection = UsbCamera.FindDevices();
            foreach (string item in infoCollection)
                VideoComboBox.Items.Add(item);

            MMDeviceEnumerator enumerator = new MMDeviceEnumerator();
            var captureDevices = enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active).ToArray();
            for (int i = 0; i < captureDevices.Length; i++)
                AudioComboBox.Items.Add(captureDevices[i].FriendlyName);

            if (SetLast)
            {
                if (VideoComboBox.Items.Contains(VideoDevice))
                {
                    VideoComboBox.SelectedIndex = VideoComboBox.Items.IndexOf(VideoDevice);
                }
                if (AudioComboBox.Items.Contains(AudioDevice))
                {
                    AudioComboBox.SelectedIndex = AudioComboBox.Items.IndexOf(AudioDevice);
                }
            }
        }

        #region Video
        private void VideoComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (VideoComboBox.SelectedIndex != -1)
            {
                RenderSizeComboBox.SelectedIndex = -1;
                SetSettings();
                InitalizeCamera();
            }
        }

        public void InitalizeCamera()
        {
            if (captureDevice != null)
            {
                captureDevice.Stop();
                this.FormClosing -= (s, ev) => captureDevice.Release();
                //captureDevice = null;
            }

            RenderSizeComboBox.Items.Clear();

            if (VideoComboBox.SelectedIndex != -1)
            {
                try
                {
                    var formats = UsbCamera.GetVideoFormat(VideoComboBox.SelectedIndex);
                    captureDevice = new UsbCamera(VideoComboBox.SelectedIndex, formats[0]);
                    this.FormClosing += (s, ev) => captureDevice.Release(); // release when close.
                    pictureBox1.Visible = true;

                    Size originalSize = formats[0].Size;

                    try
                    {
                        // Set preview control but handle potential exceptions
                        captureDevice.SetPreviewControl(pictureBox1.Handle, originalSize);
                        pictureBox1.Resize += (s, ev) =>
                        {
                            try
                            {
                                captureDevice.SetPreviewSize(originalSize);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Error in resize: " + ex.Message);
                            }
                        };
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error setting preview: " + ex.Message);
                    }

                    captureDevice.Start();

                    for (int i = 0; i < formats.Length; i++)
                    {
                        Size frameSize = formats[i].Size;
                        int avgFrameRate = formats[i].Fps;
                        RenderSizeComboBox.Items.Add($"{frameSize.Width} x{frameSize.Height} @{avgFrameRate}");
                    }
                    if (RenderSizeComboBox.Items.Count > 0)
                    {
                        if (RenderSizeComboBox.Items.Contains(settings.RenderSizePos))
                            RenderSizeComboBox.SelectedIndex = RenderSizeComboBox.Items.IndexOf(settings.RenderSizePos);
                        else
                            RenderSizeComboBox.SelectedIndex = 0;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error initializing camera: " + ex.Message);
                }
            }
        }
        #endregion

        #region Frame Update
        private void WriteLableFPS()
        {
            while (true)
            {
                Thread.Sleep(200);
                if (showFPS)
                {
                    MethodInvoker inv = delegate
                    {
                        this.FPSCounter.Text = $"FPS: {CurrentFPS}";
                    };

                    this.Invoke(inv);
                }
            }
        }
        #endregion

        #region Display/Scale Options
        private void RenderSizeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (RenderSizeComboBox.SelectedIndex != -1)
            {
                if (captureDevice != null)
                {
                    try
                    {
                        var tempFPS = showFPS;
                        showFPS = false;

                        // Properly stop the camera before releasing
                        captureDevice.Stop();
                        this.FormClosing -= (s, ev) => captureDevice.Release();
                        captureDevice.Release();

                        // Create new camera with selected format
                        var VideoFormat = UsbCamera.GetVideoFormat(VideoComboBox.SelectedIndex)[RenderSizeComboBox.SelectedIndex];
                        captureDevice = new UsbCamera(VideoComboBox.SelectedIndex, VideoFormat);
                        this.FormClosing += (s, ev) => captureDevice.Release();

                        // Get the original size for preview
                        Size originalSize = VideoFormat.Size;

                        // Wrap the preview control setup in try/catch to avoid crashes
                        try
                        {
                            // Set the preview control with proper error handling
                            //captureDevice.PreviewCaptured += (bmp) =>
                            //{
                            //    lock (pictureBox1)
                            //    {
                            //        try
                            //        {
                            //            pictureBox1.Image = bmp;
                            //            if (dateTime == null)
                            //                dateTime = DateTimeOffset.Now;

                            //            var temp = DateTimeOffset.Now;
                            //            var tempCount = Count;
                            //            tempCount = (temp - dateTime).TotalSeconds;
                            //            Count = tempCount;
                            //            dateTime = temp;
                            //            CurrentFPS = Math.Round((1f / Count), 2);
                            //        }
                            //        catch
                            //        {
                            //            Console.WriteLine("Error Writing Frame");
                            //        }
                            //    }
                            //};
                            captureDevice.SetPreviewControl(pictureBox1.Handle, originalSize);

                            // Remove existing event handlers to avoid multiple subscriptions
                            pictureBox1.Resize -= new EventHandler(PictureBox_Resize);
                            pictureBox1.Resize += new EventHandler(PictureBox_Resize);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error setting preview control: " + ex.Message);
                        }

                        UpdateDisplayMode();
                        captureDevice.Start();
                        showFPS = tempFPS;
                        SetSettings();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error changing resolution: " + ex.Message);
                    }
                }

                SizeObjectsScale();
            }
        }

        // Separate event handler method for resize to better handle exceptions
        private void PictureBox_Resize(object sender, EventArgs e)
        {
            if (captureDevice != null)
            {
                try
                {
                    // Get the current format
                    var format = UsbCamera.GetVideoFormat(VideoComboBox.SelectedIndex)[RenderSizeComboBox.SelectedIndex];
                    captureDevice.SetPreviewSize(format.Size);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error in PictureBox_Resize: " + ex.Message);
                }
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e) => ToggleFPS();
        private void DisplaySizeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetSettings();
            UpdateDisplayMode();
        }

        void ToggleFPS()
        {
            showFPS = !showFPS;
            FPSCounter.Visible = showFPS;
        }
        void ToggleFullscreen()
        {
            isFullscreen = !isFullscreen;
            UpdateDisplayMode();
        }

        void UpdateDisplayMode()
        {
            isDrawing = false; // Attempt to avoid clashing with drawcheck in rare race condition

            int displayIndex = DisplaySizeComboBox.SelectedIndex;

            RACCheck = true;
            if (displayIndex != -1 && RenderSizeComboBox.SelectedIndex != -1)
            {
                switch (displayIndex)
                {
                    case 0:
                        // Match Size

                        FormBorderStyle = FormBorderStyle.FixedSingle;
                        AutoSize = true;
                        if (captureDevice != null)
                        {
                            try
                            {
                                var device = UsbCamera.GetVideoFormat(VideoComboBox.SelectedIndex)[RenderSizeComboBox.SelectedIndex];

                                // Use a try-catch block to handle any casting issues
                                try
                                {
                                    int width = device.Size.Width;
                                    int height = device.Size.Height;

                                    // Set the client size
                                    ClientSize = new Size(width, height);

                                    // Instead of trying to resize the form immediately,
                                    // let the picturebox handle it through Dock property
                                    pictureBox1.Dock = DockStyle.Fill;
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("Error setting size: " + ex.Message);
                                    // Fall back to letting the picturebox handle sizing
                                    pictureBox1.Dock = DockStyle.Fill;
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Error getting device format: " + ex.Message);
                            }
                        }
                        break;
                    case 1:
                        // Stretch Size
                        AutoSize = false;
                        FormBorderStyle = FormBorderStyle.Sizable;

                        // When in stretch mode, we need to adjust how the preview is handled
                        pictureBox1.Dock = DockStyle.Fill;

                        if (captureDevice != null)
                        {
                            try
                            {
                                // Make sure preview adjusts to pictureBox size
                                captureDevice.SetPreviewSize(pictureBox1.ClientSize);

                                // Ensure the pictureBox will properly handle the stretch
                                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Error setting stretch size: " + ex.Message);
                            }
                        }
                        break;
                    case 2: // Cases 2, 3, and 4 were identical
                    case 3: // So every case will just collapse
                    case 4: // And do the same thing at the end
                            // 3: 16:9
                            // 4: 16:10
                            // 5: 4:3

                        AutoSize = false;
                        FormBorderStyle = FormBorderStyle.Sizable;
                        break;
                }

                RACCheck = false;
                SizeObjectsScale();
                if (isFullscreen)
                {
                    WindowState = FormWindowState.Normal;
                    FormBorderStyle = FormBorderStyle.None;
                    WindowState = FormWindowState.Maximized;
                }
                else
                {
                    WindowState = FormWindowState.Normal;
                }
            }
        }

        private void Fullscreen_Click(object sender, EventArgs e) => ToggleFullscreen();

        private void MainWindow_SizeChanged(object sender, EventArgs e) => SizeObjectsScale();

        Vector2[] ratios = new Vector2[] {
            new Vector2(-1, -1),
            new Vector2(-1, -1),
            new Vector2(16, 9),
            new Vector2(16, 10),
            new Vector2(4, 3)
        };

        void SizeObjectsScale()
        {
            int index = DisplaySizeComboBox.SelectedIndex;

            RACCheck = true;

            switch (index)
            {
                case 0:

                    AutoSize = false;
                    pictureBox1.Location = new Point((ClientSize.Width - pictureBox1.ClientSize.Width) / 2,
                                                            (ClientSize.Height - pictureBox1.ClientSize.Height) / 2);
                    AutoSize = true;
                    break;
                case 1:
                    // Stretch Mode - ensure picturebox fills the form
                    pictureBox1.Dock = DockStyle.Fill;
                    pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;

                    // Force a resize of the preview to match current size
                    if (captureDevice != null)
                    {
                        try
                        {
                            captureDevice.SetPreviewSize(pictureBox1.ClientSize);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error in stretch mode sizing: " + ex.Message);
                        }
                    }
                    break;
                case 2: // Cases 2, 3, and 4 are functionally identical, just with different values. 
                case 3: // These values are stored in the ratios array that is used
                case 4: // Riiight here ----v
                    Vector2 displayRatio = ratios[index];

                    int test = (int)((ClientSize.Width / displayRatio.X) * displayRatio.Y);
                    if (test > ClientSize.Height)
                        test = (int)((ClientSize.Height / displayRatio.Y) * displayRatio.X);

                    pictureBox1.Size = test > ClientSize.Height ? new Size(test, ClientSize.Height) :
                                                                        new Size(ClientSize.Width, test);

                    pictureBox1.Location = new Point((ClientSize.Width - pictureBox1.ClientSize.Width) / 2,
                                                            (ClientSize.Height - pictureBox1.ClientSize.Height) / 2);
                    break;
                default:
                    break;
            }

            RACCheck = false;
        }

        private void MainWindow_ResizeEnd(object sender, EventArgs e) => SizeObjectsScale();

        // Add this method to handle form resize to properly update stretch mode
        private void MainWindow_Resize(object sender, EventArgs e)
        {
            if (DisplaySizeComboBox.SelectedIndex == 1 && captureDevice != null)
            {
                try
                {
                    captureDevice.SetPreviewSize(pictureBox1.ClientSize);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error in MainWindow_Resize: " + ex.Message);
                }
            }
        }

        #endregion

        #region Audio
        void AudioCapture()
        {
            if (wave != null)
            {
                wave.StopRecording();
                waveOut.Stop();
            }
            MMDeviceEnumerator enumerator = new MMDeviceEnumerator();
            var temp = enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active);

            var Temp2 = temp[AudioComboBox.SelectedIndex];
            wave = new WasapiCapture(Temp2, true);
            wave.ShareMode = AudioClientShareMode.Shared;
            //wave.WaveFormat = new WaveFormat(40000, 16, 1);

            waveOut = new WasapiOut();

            provider = new BufferedWaveProvider(wave.WaveFormat);

            waveOut.Init(provider);
            waveOut.Play();

            wave.DataAvailable += Wave_DataAvailable;
            wave.StartRecording();
            SetSettings();
        }
        private void AudioComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (AudioComboBox.SelectedIndex != -1)
            {
                if (wave != null)
                {
                    wave.StopRecording();
                    waveOut.Stop();
                }
                AudioCapture();
            }
        }

        private void Wave_DataAvailable(object sender, WaveInEventArgs e)
        {
            provider.AddSamples(e.Buffer, 0, e.BytesRecorded);
        }
        #endregion

        #region HotKeys
        void HotKeyGroup(Keys e)
        {
            switch (e)
            {
                case Keys.Escape:
                    Close();
                    break;
                case Keys.F:
                    ToggleFullscreen();
                    InvokeDisplayLabel("Toggled Fullscreen...");
                    break;
                case Keys.C:
                    ToggleFPS();
                    break;
                case Keys.R:
                    InitalizeCamera();
                    AudioCapture();
                    InvokeDisplayLabel("Reinitialised Inputs...");
                    break;
                case Keys.Q:
                    GetDevicesLists(true);
                    InvokeDisplayLabel("Refreshed Input List...");
                    break;
            }
        }

        void InvokeDisplayLabel(string Input)
        {
            DisplayLabelString = Input;
            Thread thread1 = new Thread(UpdateDisplayLabel);
            thread1.Start();
        }

        string DisplayLabelString;
        void UpdateDisplayLabel()
        {
            string Input = DisplayLabelString;
            MethodInvoker inv = delegate
            {
                HotKeyDisplay.Visible = true;
                HotKeyDisplay.Text = Input;
            };

            this.Invoke(inv);
            Thread.Sleep(3000);
            if (DisplayLabelString == Input)
            {
                MethodInvoker inv1 = delegate
                {
                    HotKeyDisplay.Visible = false;
                };
                DisplayLabelString = "";
                this.Invoke(inv1);
            }
        }

        // These two now pass their keycodes to the above function, so all hotKEYs are now centralised.
        private void videoSourcePlayer1_KeyDown(object sender, KeyEventArgs e) => HotKeyGroup(e.KeyCode);
        private void pictureBox1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e) => HotKeyGroup(e.KeyCode);

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                toolStrip2.Visible = !toolStrip2.Visible;
        }

        private void pictureBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ToggleFullscreen();
                toolStrip2.Visible = !isFullscreen;
            }
        }
        #endregion

        private void MainWindow_MouseEnter(object sender, EventArgs e)
        {
            if (!QuickFix)
            {
                QuickFix = true;
                UpdateDisplayMode();
            }
        }
    }
}