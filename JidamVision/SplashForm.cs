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
            this.Width = 600;
            this.Height = 400;
            this.BackColor = Color.Black;

            // 이미지
            string imagePath = Path.Combine(Application.StartupPath, "Resources", "logo.png");
            if (File.Exists(imagePath))
            {
                PictureBox logo = new PictureBox
                {
                    Image = Image.FromFile(imagePath),
                    SizeMode = PictureBoxSizeMode.StretchImage,  // ✅ 꽉 채우기
                    Dock = DockStyle.Fill
                };
                Controls.Add(logo);
            }

            // 메시지
            messageLabel = new Label
            {
                Text = "시작 중입니다...",
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Dock = DockStyle.Bottom,
                Height = 30,
                Font = new Font("맑은 고딕", 10, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };
            Controls.Add(messageLabel);
            messageLabel.BringToFront();

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
            progressBar.BringToFront();
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
