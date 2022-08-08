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
//using AForge.Video.DirectShow;
//using AForge.Video;
using Accord.Video;
using Accord.Video.DirectShow;
using NAudio.Wave;
using NAudio.CoreAudioApi;
using System.Numerics;

namespace CaptureDisplay
{
	public partial class MainWindow : Form
	{
		public static Settings settings;
		FilterInfoCollection infoCollection;
		VideoCaptureDevice captureDevice;
		private delegate void SafeCallDelegate();
		DateTimeOffset dateTime;
		double Count = 0;
		bool RACCheck;
		bool FPSCount = true;
		private WasapiCapture wave;
		private WasapiOut waveOut;
		private BufferedWaveProvider provider;
		bool FullscreenBool;
		public static Size size;
		public static Point point;
		public static DockStyle dockStyle;

		bool QuickFix;

		public MainWindow()
		{
			Closed += Form1_Closed;
			settings = Settings.Load(Application.UserAppDataPath);
			this.KeyPreview = true;
			InitializeComponent();

			videoSourcePlayer1.BackColor = Color.Black;
			pictureBox1.BackColor = Color.Black;
			pictureBox2.BackColor = Color.Black;
			videoSourcePlayer1.Dock = DockStyle.Fill;
			pictureBox1.Dock = DockStyle.Fill;
			pictureBox2.Dock = DockStyle.Fill;

			FPSCount = false;
			FPSCounter.Visible = FPSCount;

			DisableSleep.KeepAwake();
		}

		bool Loading;
		void LoadSettings()
		{
			Loading = true;
			DisplaySizeComboBox.SelectedIndex = settings.DisplayModePos;
			RenderModeComboBox.SelectedIndex = settings.RenderMode;
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
				settings.RenderMode = RenderModeComboBox.SelectedIndex;

				if (VideoComboBox.SelectedItem != null)
					settings.VideoName = VideoComboBox.SelectedItem.ToString();

				if (RenderSizeComboBox.SelectedItem != null)
					settings.RenderSizePos = RenderSizeComboBox.SelectedItem.ToString();

				if (AudioComboBox.SelectedItem != null)
					settings.AudioName = AudioComboBox.SelectedItem.ToString();

				settings.Save(Application.UserAppDataPath);
			}
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			infoCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);
			foreach (FilterInfo item in infoCollection)
			{
				VideoComboBox.Items.Add(item.Name);
			}

			MMDeviceEnumerator enumerator = new MMDeviceEnumerator();
			var captureDevices = enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active).ToArray();
			for (int i = 0; i < captureDevices.Length; i++)
			{
				AudioComboBox.Items.Add(captureDevices[i].FriendlyName);
			}

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

				FPSCount = false;

				if (captureDevice.IsRunning)
				{
					captureDevice.Stop();
					captureDevice.WaitForStop();
				}
			}
			Environment.Exit(0);
			captureDevice = null;
		}

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
				captureDevice.SignalToStop();
				captureDevice.WaitForStop();
				captureDevice.NewFrame -= CaptureDevice_NewFrame;
			}

			if (videoSourcePlayer1.VideoSource != null)
			{
				videoSourcePlayer1.SignalToStop();
				videoSourcePlayer1.WaitForStop();
				videoSourcePlayer1.NewFrame -= VideoSourcePlayer_NewFrame;
			}

			RenderSizeComboBox.Items.Clear();

			if (VideoComboBox.SelectedIndex != -1)
			{
				captureDevice = new VideoCaptureDevice(infoCollection[VideoComboBox.SelectedIndex].MonikerString);

				if (settings.RenderMode == 1 || settings.RenderMode == 2)
				{
					captureDevice.NewFrame += CaptureDevice_NewFrame;

					bool mode = settings.RenderMode == 1 ? true : false;
					// Auto-toggles between the two based on the RenderMode
					pictureBox1.Visible = mode;
					pictureBox2.Visible = !mode;

					videoSourcePlayer1.Visible = false;
				}
				else
				{
					videoSourcePlayer1.NewFrame += VideoSourcePlayer_NewFrame;
					pictureBox1.Visible = false;
					videoSourcePlayer1.Visible = true;
					videoSourcePlayer1.VideoSource = new AsyncVideoSource(captureDevice);
					videoSourcePlayer1.Start();
				}

				captureDevice.Start();
				for (int i = 0; i < captureDevice.VideoCapabilities.Length; i++)
				{
					Size frameSize = captureDevice.VideoCapabilities[i].FrameSize;
					int avgFrameRate = captureDevice.VideoCapabilities[i].AverageFrameRate;
					RenderSizeComboBox.Items.Add($"{frameSize.Width} x{frameSize.Height} @{avgFrameRate}");
					//RenderSizeComboBox.Items.Add(captureDevice.VideoCapabilities[i].FrameSize.Width.ToString() + " x " + captureDevice.VideoCapabilities[i].FrameSize.Height.ToString() + " @" + captureDevice.VideoCapabilities[i].AverageFrameRate);
				}
				if (RenderSizeComboBox.Items.Count > 0)
				{
					if (RenderSizeComboBox.Items.Contains(settings.RenderSizePos))
						RenderSizeComboBox.SelectedIndex = RenderSizeComboBox.Items.IndexOf(settings.RenderSizePos);
					else
						RenderSizeComboBox.SelectedIndex = 0;
				}
			}
		}

		#region Frame Update
		private void CaptureDevice_NewFrame(object sender, NewFrameEventArgs eventArgs)
		{
			DisableSleep.KeepAwake();
			if (dateTime == null)
				dateTime = DateTimeOffset.Now;

			var temp = DateTimeOffset.Now;
			var tempCount = Count;
			tempCount = (temp - dateTime).TotalSeconds;
			Count = tempCount;
			dateTime = temp;

			if (!RACCheck)
			{
				//RACCheck=true;
				try
				{
					if (settings.RenderMode == 1)
					{
						Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone();
						pictureBox1.Image = bitmap;
					}
					else if (settings.RenderMode == 2)
					{
						Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone();
						pictureBox2.Image = bitmap;
					}
				}
				catch
				{

				}
				//RACCheck = false;
			}
			GC.Collect();
		}

		private void VideoSourcePlayer_NewFrame(object sender, ref Bitmap test)
		{
			DisableSleep.KeepAwake();
			if (dateTime == null)
				dateTime = DateTimeOffset.Now;

			var temp = DateTimeOffset.Now;
			var tempCount = Count;
			tempCount = (temp - dateTime).TotalSeconds;
			Count = tempCount;
			dateTime = temp;
		}

		private void WriteLableFPS()
		{
			while (true)
			{
				Thread.Sleep(1000);
				if (FPSCount)
				{
					MethodInvoker inv = delegate 
					{
						this.FPSCounter.Text = $"FPS: {Math.Round((1f / Count), 2)}";
					};

					this.Invoke(inv);
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

		#region Display/Scale Options
		private void RenderSizeComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (RenderSizeComboBox.SelectedIndex != -1)
			{
				if (captureDevice != null)
				{
					var tempFPS = FPSCount;
					FPSCount = false;
					captureDevice.SignalToStop();
					captureDevice.WaitForStop();
					captureDevice.VideoResolution = captureDevice.VideoCapabilities[RenderSizeComboBox.SelectedIndex];
					UpdateDisplayMode();
					captureDevice.Start();
					FPSCount = tempFPS;
					SetSettings();
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
			FPSCount = !FPSCount;
			FPSCounter.Visible = FPSCount;
		}
		void ToggleFullscreen()
		{
			FullscreenBool = !FullscreenBool;
			UpdateDisplayMode();
		}

		void UpdateDisplayMode()
		{
			int displayIndex = DisplaySizeComboBox.SelectedIndex;

			RACCheck = true;
			if (displayIndex != -1 && RenderSizeComboBox.SelectedIndex != -1)
			{
				bool Test = false;
				if (WindowState == FormWindowState.Maximized && !FullscreenBool)
					Test = true;

				switch (displayIndex)
				{
					case 0:
						FormBorderStyle = FormBorderStyle.FixedSingle;
						AutoSize = true;
						if (captureDevice != null)
						{
							videoSourcePlayer1.Dock = DockStyle.None;

							VideoCapabilities device = captureDevice.VideoCapabilities[RenderSizeComboBox.SelectedIndex];

							ClientSize = new Size(device.FrameSize.Width, device.FrameSize.Height);
							videoSourcePlayer1.Size = new Size(device.FrameSize.Width, device.FrameSize.Height);
						}
						break;
					case 1:
						AutoSize = false;
						FormBorderStyle = FormBorderStyle.Sizable;
						videoSourcePlayer1.Dock = DockStyle.Fill;
						break;
					case 2: // Cases 2, 3, and 4 were identical
					case 3:	// So every case will just collapse
					case 4:	// And do the same thing at the end
						AutoSize = false;
						FormBorderStyle = FormBorderStyle.Sizable;
						videoSourcePlayer1.Dock = DockStyle.None;
						break;
				}
			
				RACCheck = false;
				SizeObjectsScale();
				if (FullscreenBool)
				{
					WindowState = FormWindowState.Normal;
					FormBorderStyle = FormBorderStyle.None;
					WindowState = FormWindowState.Maximized;
				}
				else
				{
					if (!Test)
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
			int displayIndex = DisplaySizeComboBox.SelectedIndex;

			int clientWidth = ClientSize.Width;
			int clientHeight = ClientSize.Height;
			int playerWidth = videoSourcePlayer1.ClientSize.Width;
			int playerHeight = videoSourcePlayer1.ClientSize.Height;

			RACCheck = true;

			switch (displayIndex)
			{
				case 0:
					AutoSize = false;
					videoSourcePlayer1.Location = new Point((clientWidth - playerWidth) / 2, (clientHeight - playerHeight) / 2);
					AutoSize = true;
					break;
				case 1:
					break;
				case 2: // Cases 2, 3, and 4 are functionally identical, just with different values. 
				case 3:	// These values are stored in the ratios array that is used
				case 4: // Riiight here ----v
					Vector2 displayRatio = ratios[displayIndex];

					int test = (int)((clientWidth / displayRatio.X) * displayRatio.Y);
					if (test > clientHeight)
						test = (int)((clientHeight / displayRatio.Y) * displayRatio.X);

					videoSourcePlayer1.Size = test > clientHeight ? new Size(test, clientHeight) : new Size(clientWidth, test);
					videoSourcePlayer1.Location = new Point((clientWidth - playerWidth) / 2, (clientHeight - playerHeight) / 2);
					break;
			}

			// This will set PictureBox2's stats to VideoSourcePlayer1's stats, then set PictureBox1's status to PB2's
			pictureBox1.Location = pictureBox2.Location = videoSourcePlayer1.Location;
			pictureBox1.Size = pictureBox2.Size = videoSourcePlayer1.Size;
			pictureBox1.Dock = pictureBox2.Dock = videoSourcePlayer1.Dock;
			RACCheck = false;
		}

		private void MainWindow_ResizeEnd(object sender, EventArgs e) => SizeObjectsScale();

		private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (RenderModeComboBox.SelectedIndex != -1)
			{
				settings.RenderMode = RenderModeComboBox.SelectedIndex;
				SetSettings();
				InitalizeCamera();
			}
		}
		#endregion

		#region HotKeys
		void HotKeyGroup(Keys e)
		{
			if (e == Keys.Escape)
				Close();

			if (e == Keys.F)
				ToggleFullscreen();

			if (e == Keys.C)
				ToggleFPS();
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
				toolStrip2.Visible = !FullscreenBool;
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