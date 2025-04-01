namespace JidamVision.Property
{
    partial class CrackInspProp
    {
        /// <summary> 
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                if (txt_ArMin != null)
                    txt_ArMin.Leave -= OnFilterChanged;
                if (txt_ArMax != null)
                    txt_ArMax.Leave -= OnFilterChanged;
                if (txt_ThMin != null)
                    txt_ThMin.Leave -= OnFilterChanged;
                if (txt_ThMax != null)
                    txt_ThMax.Leave -= OnFilterChanged;

                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 구성 요소 디자이너에서 생성한 코드

        /// <summary> 
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txt_ThMax = new System.Windows.Forms.TextBox();
            this.txt_ThMin = new System.Windows.Forms.TextBox();
            this.lbl_Threshold = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txt_ArMax = new System.Windows.Forms.TextBox();
            this.txt_ArMin = new System.Windows.Forms.TextBox();
            this.lbl_Area = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txt_ThMax);
            this.groupBox1.Controls.Add(this.txt_ThMin);
            this.groupBox1.Controls.Add(this.lbl_Threshold);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txt_ArMax);
            this.groupBox1.Controls.Add(this.txt_ArMin);
            this.groupBox1.Controls.Add(this.lbl_Area);
            this.groupBox1.Location = new System.Drawing.Point(2, 2);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox1.Size = new System.Drawing.Size(472, 420);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Crack(깨짐)";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(150, 53);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(14, 12);
            this.label3.TabIndex = 23;
            this.label3.Text = "~";
            // 
            // txt_ThMax
            // 
            this.txt_ThMax.Location = new System.Drawing.Point(169, 46);
            this.txt_ThMax.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txt_ThMax.Name = "txt_ThMax";
            this.txt_ThMax.Size = new System.Drawing.Size(80, 21);
            this.txt_ThMax.TabIndex = 22;
            this.txt_ThMax.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txt_ThMin
            // 
            this.txt_ThMin.Location = new System.Drawing.Point(67, 46);
            this.txt_ThMin.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txt_ThMin.Name = "txt_ThMin";
            this.txt_ThMin.Size = new System.Drawing.Size(80, 21);
            this.txt_ThMin.TabIndex = 21;
            this.txt_ThMin.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lbl_Threshold
            // 
            this.lbl_Threshold.AutoSize = true;
            this.lbl_Threshold.Location = new System.Drawing.Point(4, 53);
            this.lbl_Threshold.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbl_Threshold.Name = "lbl_Threshold";
            this.lbl_Threshold.Size = new System.Drawing.Size(62, 12);
            this.lbl_Threshold.TabIndex = 20;
            this.lbl_Threshold.Text = "Threshold";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(150, 26);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(14, 12);
            this.label2.TabIndex = 19;
            this.label2.Text = "~";
            // 
            // txt_ArMax
            // 
            this.txt_ArMax.Location = new System.Drawing.Point(169, 19);
            this.txt_ArMax.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txt_ArMax.Name = "txt_ArMax";
            this.txt_ArMax.Size = new System.Drawing.Size(80, 21);
            this.txt_ArMax.TabIndex = 18;
            this.txt_ArMax.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txt_ArMin
            // 
            this.txt_ArMin.Location = new System.Drawing.Point(67, 19);
            this.txt_ArMin.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txt_ArMin.Name = "txt_ArMin";
            this.txt_ArMin.Size = new System.Drawing.Size(80, 21);
            this.txt_ArMin.TabIndex = 17;
            this.txt_ArMin.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lbl_Area
            // 
            this.lbl_Area.AutoSize = true;
            this.lbl_Area.Location = new System.Drawing.Point(4, 26);
            this.lbl_Area.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbl_Area.Name = "lbl_Area";
            this.lbl_Area.Size = new System.Drawing.Size(31, 12);
            this.lbl_Area.TabIndex = 16;
            this.lbl_Area.Text = "Area";
            // 
            // CrackInspProp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "CrackInspProp";
            this.Size = new System.Drawing.Size(476, 424);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txt_ThMax;
        private System.Windows.Forms.TextBox txt_ThMin;
        private System.Windows.Forms.Label lbl_Threshold;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txt_ArMax;
        private System.Windows.Forms.TextBox txt_ArMin;
        private System.Windows.Forms.Label lbl_Area;
    }
}
