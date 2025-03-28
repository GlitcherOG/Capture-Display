﻿namespace CaptureDisplay
{
    partial class DisplayLabel
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DisplayLabel));
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.VideoComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.AudioComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel3 = new System.Windows.Forms.ToolStripLabel();
            this.RenderSizeComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel4 = new System.Windows.Forms.ToolStripLabel();
            this.DisplaySizeComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.Fullscreen = new System.Windows.Forms.ToolStripButton();
            this.FPSCounter = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.HotKeyDisplay = new System.Windows.Forms.Label();
            this.toolStrip2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip2
            // 
            this.toolStrip2.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.VideoComboBox,
            this.toolStripSeparator1,
            this.toolStripLabel2,
            this.AudioComboBox,
            this.toolStripSeparator2,
            this.toolStripLabel3,
            this.RenderSizeComboBox,
            this.toolStripSeparator3,
            this.toolStripLabel4,
            this.DisplaySizeComboBox,
            this.toolStripButton1,
            this.toolStripSeparator4,
            this.Fullscreen});
            this.toolStrip2.Location = new System.Drawing.Point(0, 0);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(1264, 25);
            this.toolStrip2.TabIndex = 4;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(46, 22);
            this.toolStripLabel1.Text = " Video :";
            // 
            // VideoComboBox
            // 
            this.VideoComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.VideoComboBox.Name = "VideoComboBox";
            this.VideoComboBox.Size = new System.Drawing.Size(121, 25);
            this.VideoComboBox.SelectedIndexChanged += new System.EventHandler(this.VideoComboBox_SelectedIndexChanged);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(48, 22);
            this.toolStripLabel2.Text = " Audio :";
            // 
            // AudioComboBox
            // 
            this.AudioComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.AudioComboBox.DropDownWidth = 200;
            this.AudioComboBox.MaxDropDownItems = 20;
            this.AudioComboBox.Name = "AudioComboBox";
            this.AudioComboBox.Size = new System.Drawing.Size(121, 25);
            this.AudioComboBox.SelectedIndexChanged += new System.EventHandler(this.AudioComboBox_SelectedIndexChanged);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripLabel3
            // 
            this.toolStripLabel3.Name = "toolStripLabel3";
            this.toolStripLabel3.Size = new System.Drawing.Size(76, 22);
            this.toolStripLabel3.Text = " Render Size :";
            // 
            // RenderSizeComboBox
            // 
            this.RenderSizeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.RenderSizeComboBox.Name = "RenderSizeComboBox";
            this.RenderSizeComboBox.Size = new System.Drawing.Size(121, 25);
            this.RenderSizeComboBox.SelectedIndexChanged += new System.EventHandler(this.RenderSizeComboBox_SelectedIndexChanged);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripLabel4
            // 
            this.toolStripLabel4.Name = "toolStripLabel4";
            this.toolStripLabel4.Size = new System.Drawing.Size(88, 22);
            this.toolStripLabel4.Text = " Display Mode :";
            // 
            // DisplaySizeComboBox
            // 
            this.DisplaySizeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.DisplaySizeComboBox.Items.AddRange(new object[] {
            "Match Size",
            "Stretch Size",
            "Aspect Ratio (16:9)",
            "Aspect Ratio (16:10)",
            "Aspect Ratio (4:3)"});
            this.DisplaySizeComboBox.Name = "DisplaySizeComboBox";
            this.DisplaySizeComboBox.Size = new System.Drawing.Size(121, 25);
            this.DisplaySizeComboBox.SelectedIndexChanged += new System.EventHandler(this.DisplaySizeComboBox_SelectedIndexChanged);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.toolStripButton1.Size = new System.Drawing.Size(76, 22);
            this.toolStripButton1.Text = "FPS Counter";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
            // 
            // Fullscreen
            // 
            this.Fullscreen.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.Fullscreen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.Fullscreen.Image = ((System.Drawing.Image)(resources.GetObject("Fullscreen.Image")));
            this.Fullscreen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Fullscreen.Name = "Fullscreen";
            this.Fullscreen.Size = new System.Drawing.Size(64, 22);
            this.Fullscreen.Text = "Fullscreen";
            this.Fullscreen.Click += new System.EventHandler(this.Fullscreen_Click);
            // 
            // FPSCounter
            // 
            this.FPSCounter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.FPSCounter.AutoSize = true;
            this.FPSCounter.BackColor = System.Drawing.Color.Transparent;
            this.FPSCounter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.FPSCounter.Font = new System.Drawing.Font("Microsoft Sans Serif", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FPSCounter.ForeColor = System.Drawing.Color.White;
            this.FPSCounter.Location = new System.Drawing.Point(12, 633);
            this.FPSCounter.Name = "FPSCounter";
            this.FPSCounter.Size = new System.Drawing.Size(124, 41);
            this.FPSCounter.TabIndex = 5;
            this.FPSCounter.Text = "FPS: 0";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.pictureBox1.Location = new System.Drawing.Point(302, 257);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(148, 90);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 7;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseDoubleClick);
            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseDown);
            this.pictureBox1.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.pictureBox1_PreviewKeyDown);
            // 
            // HotKeyDisplay
            // 
            this.HotKeyDisplay.AutoSize = true;
            this.HotKeyDisplay.BackColor = System.Drawing.Color.Black;
            this.HotKeyDisplay.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.HotKeyDisplay.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HotKeyDisplay.ForeColor = System.Drawing.Color.White;
            this.HotKeyDisplay.Location = new System.Drawing.Point(12, 25);
            this.HotKeyDisplay.Name = "HotKeyDisplay";
            this.HotKeyDisplay.Size = new System.Drawing.Size(65, 35);
            this.HotKeyDisplay.TabIndex = 10;
            this.HotKeyDisplay.Text = "???";
            this.HotKeyDisplay.Visible = false;
            // 
            // DisplayLabel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(1264, 681);
            this.Controls.Add(this.HotKeyDisplay);
            this.Controls.Add(this.FPSCounter);
            this.Controls.Add(this.toolStrip2);
            this.Controls.Add(this.pictureBox1);
            this.ForeColor = System.Drawing.Color.Black;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "DisplayLabel";
            this.Text = "Capture Display";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResizeEnd += new System.EventHandler(this.MainWindow_ResizeEnd);
            this.SizeChanged += new System.EventHandler(this.MainWindow_SizeChanged);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.videoSourcePlayer1_KeyDown);
            this.MouseEnter += new System.EventHandler(this.MainWindow_MouseEnter);
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripComboBox VideoComboBox;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.ToolStripComboBox AudioComboBox;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripLabel toolStripLabel3;
        private System.Windows.Forms.ToolStripComboBox RenderSizeComboBox;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripLabel toolStripLabel4;
        private System.Windows.Forms.ToolStripComboBox DisplaySizeComboBox;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.Label FPSCounter;
        private System.Windows.Forms.ToolStripButton Fullscreen;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label HotKeyDisplay;
    }
}

