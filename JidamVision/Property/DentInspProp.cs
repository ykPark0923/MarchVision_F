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

namespace JidamVision.Property
{
    public partial class DentInspProp : UserControl
    {
        public event EventHandler<EventArgs> PropertyChanged;

        DentAlgorithm _dentAlgo = new DentAlgorithm();
        public DentInspProp()
        {
            InitializeComponent();
            txt_ArMin.Leave += OnFilterChanged;
            txt_ArMax.Leave += OnFilterChanged;
            txt_ThMin.Leave += OnFilterChanged;
            txt_ThMax.Leave += OnFilterChanged;
        }

        public void SetAlgorithm(DentAlgorithm dentAlgo)
        {
            _dentAlgo = dentAlgo;
            SetProperty();
        }

        public void SetProperty()
        {
            if (_dentAlgo is null)
                return;

            txt_ArMin.Text = _dentAlgo._areaMin.ToString();
            txt_ArMax.Text = _dentAlgo._areaMax.ToString();
            txt_ThMin.Text = _dentAlgo._binaryMin.ToString();
            txt_ThMax.Text = _dentAlgo._binaryMax.ToString();
        }


        public void GetProperty()
        {
            if (_dentAlgo is null)
                return;



            if (txt_ArMin.Text != "")
            {
                int areaMin = int.Parse(txt_ArMin.Text);
                _dentAlgo._areaMin = areaMin;
            }
            if (txt_ArMax.Text != "")
            {
                int areaMax = int.Parse(txt_ArMax.Text);
                _dentAlgo._areaMax = areaMax;
            }

            if (txt_ThMin.Text != "")
            {
                int binaryMin = int.Parse(txt_ThMin.Text);
                _dentAlgo._binaryMin = binaryMin;
            }
            if (txt_ThMax.Text != "")
            {
                int binaryMax = int.Parse(txt_ThMax.Text);
                _dentAlgo._binaryMax = binaryMax;
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
            if (_dentAlgo == null)
                return;

            if (int.TryParse(txt_ArMin.Text, out int areaMin))
            {
                _dentAlgo._areaMin = areaMin;
                PropertyChanged?.Invoke(this, null);
            }
            else
            {
                MessageBox.Show("숫자만 입력 가능합니다.");
                txt_ArMin.Text = _dentAlgo._areaMin.ToString(); // 기존 값 복원
            }


            if (int.TryParse(txt_ArMax.Text, out int areaMax))
            {
                _dentAlgo._areaMax = areaMax;
                PropertyChanged?.Invoke(this, null);
            }
            else
            {
                MessageBox.Show("숫자만 입력 가능합니다.");
                txt_ArMax.Text = _dentAlgo._areaMax.ToString(); // 기존 값 복원
            }


            if (int.TryParse(txt_ThMin.Text, out int widthMin))
            {
                _dentAlgo._binaryMin = widthMin;
                PropertyChanged?.Invoke(this, null);
            }
            else
            {
                MessageBox.Show("숫자만 입력 가능합니다.");
                txt_ThMin.Text = _dentAlgo._binaryMin.ToString(); // 기존 값 복원
            }


            if (int.TryParse(txt_ThMax.Text, out int widthMax))
            {
                _dentAlgo._binaryMax = widthMax;
                PropertyChanged?.Invoke(this, null);
            }
            else
            {
                MessageBox.Show("숫자만 입력 가능합니다.");
                txt_ThMax.Text = _dentAlgo._binaryMax.ToString(); // 기존 값 복원
            }
        }
    }
}
