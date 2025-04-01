using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using JidamVision.Algorithm;
using JidamVision.Core;
using JidamVision.Teach;
using static System.Windows.Forms.MonthCalendar;

namespace JidamVision.Property
{
    public partial class CrackInspProp : UserControl
    {
        public event EventHandler<EventArgs> PropertyChanged;

        CrackAlgorithm _crackAlgo = new CrackAlgorithm();
        public CrackInspProp()
        {
            InitializeComponent();


            txt_ArMin.Leave += OnFilterChanged;
            txt_ArMax.Leave += OnFilterChanged;
            txt_ThMin.Leave += OnFilterChanged;
            txt_ThMax.Leave += OnFilterChanged;
        }

        public void SetAlgorithm(CrackAlgorithm crackAlgo)
        {
            _crackAlgo = crackAlgo;
            SetProperty();
        }


        public void SetProperty()
        {
            if (_crackAlgo is null)
                return;

            txt_ArMin.Text = _crackAlgo._areaMin.ToString();
            txt_ArMax.Text = _crackAlgo._areaMax.ToString();
            txt_ThMin.Text = _crackAlgo._binaryMin.ToString();
            txt_ThMax.Text = _crackAlgo._binaryMax.ToString();
        }


        public void GetProperty()
        {
            if (_crackAlgo is null)
                return;



            if (txt_ArMin.Text != "")
            {
                int areaMin = int.Parse(txt_ArMin.Text);
                _crackAlgo._areaMin = areaMin;
            }
            if (txt_ArMax.Text != "")
            {
                int areaMax = int.Parse(txt_ArMax.Text);
                _crackAlgo._areaMax = areaMax;
            }

            if (txt_ThMin.Text != "")
            {
                int binaryMin = int.Parse(txt_ThMin.Text);
                _crackAlgo._binaryMin = binaryMin;
            }
            if (txt_ThMax.Text != "")
            {
                int binaryMax = int.Parse(txt_ThMax.Text);
                _crackAlgo._binaryMax = binaryMax;
            }
        }

        private void UpdateBinary()
        {
            GetProperty();

        }

        private void OnValueChanged(object sender, EventArgs e)
        {
            UpdateBinary();
        }




        private void OnFilterChanged(object sender, EventArgs e)
        {
            if (_crackAlgo == null)
                return;

            if (int.TryParse(txt_ArMin.Text, out int areaMin))
            {
                _crackAlgo._areaMin = areaMin;
                PropertyChanged?.Invoke(this, null);
            }
            else
            {
                MessageBox.Show("숫자만 입력 가능합니다.");
                txt_ArMin.Text = _crackAlgo._areaMin.ToString(); // 기존 값 복원
            }


            if (int.TryParse(txt_ArMax.Text, out int areaMax))
            {
                _crackAlgo._areaMax = areaMax;
                PropertyChanged?.Invoke(this, null);
            }
            else
            {
                MessageBox.Show("숫자만 입력 가능합니다.");
                txt_ArMax.Text = _crackAlgo._areaMax.ToString(); // 기존 값 복원
            }


            if (int.TryParse(txt_ThMin.Text, out int widthMin))
            {
                _crackAlgo._binaryMin = widthMin;
                PropertyChanged?.Invoke(this, null);
            }
            else
            {
                MessageBox.Show("숫자만 입력 가능합니다.");
                txt_ThMin.Text = _crackAlgo._binaryMin.ToString(); // 기존 값 복원
            }


            if (int.TryParse(txt_ThMax.Text, out int widthMax))
            {
                _crackAlgo._binaryMax = widthMax;
                PropertyChanged?.Invoke(this, null);
            }
            else
            {
                MessageBox.Show("숫자만 입력 가능합니다.");
                txt_ThMax.Text = _crackAlgo._binaryMax.ToString(); // 기존 값 복원
            }
        }
    }
}
