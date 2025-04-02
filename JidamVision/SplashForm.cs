using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace JidamVision
{
    public partial class SplashForm : Form
    {
        private ProgressBar progressBar;
        private Label messageLabel;

        public SplashForm()
        {
            InitializeComponent();

            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Width = 500;
            this.Height = 300;
            this.BackColor = Color.White;

            // 이미지
            PictureBox logo = new PictureBox
            {
                Image = Image.FromFile("Resources/logo.png"),
                Dock = DockStyle.Top,
                Height = 140,
                SizeMode = PictureBoxSizeMode.Zoom
            };
            Controls.Add(logo);

            // 메시지
            messageLabel = new Label
            {
                Text = "시작 중입니다...",
                Dock = DockStyle.Top,
                Height = 30,
                Font = new Font("맑은 고딕", 10),
                TextAlign = ContentAlignment.MiddleCenter
            };
            Controls.Add(messageLabel);

            // ProgressBar
            progressBar = new ProgressBar
            {
                Minimum = 0,
                Maximum = 100,
                Value = 0,
                Dock = DockStyle.Bottom,
                Height = 20,
                Style = ProgressBarStyle.Continuous
            };
            Controls.Add(progressBar);
        }

        public void SetProgress(int percent, string message = "")
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => SetProgress(percent, message)));
                return;
            }

            progressBar.Value = Math.Min(percent, 100);
            if (!string.IsNullOrEmpty(message))
                messageLabel.Text = message;
            Refresh();
        }
    }
}
