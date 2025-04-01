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
    public partial class SootInspProp : UserControl
    {
        public event EventHandler<EventArgs> PropertyChanged;

        SootAlgorithm _sootAlgo = new SootAlgorithm();
        public SootInspProp()
        {
            InitializeComponent();


            txt_ArMin.Leave += OnFilterChanged;
            txt_ArMax.Leave += OnFilterChanged;
            txt_ThMin.Leave += OnFilterChanged;
            txt_ThMax.Leave += OnFilterChanged;
        }



        public void SetAlgorithm(SootAlgorithm sootAlgo)
        {
            _sootAlgo = sootAlgo;
            SetProperty();
        }


        public void SetProperty()
        {
            if (_sootAlgo is null)
                return;

            txt_ArMin.Text = _sootAlgo._areaMin.ToString();
            txt_ArMax.Text = _sootAlgo._areaMax.ToString();
            txt_ThMin.Text = _sootAlgo._binaryMin.ToString();
            txt_ThMax.Text = _sootAlgo._binaryMax.ToString();
        }


        public void GetProperty()
        {
            if (_sootAlgo is null)
                return;



            if (txt_ArMin.Text != "")
            {
                int areaMin = int.Parse(txt_ArMin.Text);
                _sootAlgo._areaMin = areaMin;
            }
            if (txt_ArMax.Text != "")
            {
                int areaMax = int.Parse(txt_ArMax.Text);
                _sootAlgo._areaMax = areaMax;
            }

            if (txt_ThMin.Text != "")
            {
                int binaryMin = int.Parse(txt_ThMin.Text);
                _sootAlgo._binaryMin = binaryMin;
            }
            if (txt_ThMax.Text != "")
            {
                int binaryMax = int.Parse(txt_ThMax.Text);
                _sootAlgo._binaryMax = binaryMax;
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
            if (_sootAlgo == null)
                return;

            if (int.TryParse(txt_ArMin.Text, out int areaMin))
            {
                _sootAlgo._areaMin = areaMin;
                PropertyChanged?.Invoke(this, null);
            }
            else
            {
                MessageBox.Show("숫자만 입력 가능합니다.");
                txt_ArMin.Text = _sootAlgo._areaMin.ToString(); // 기존 값 복원
            }


            if (int.TryParse(txt_ArMax.Text, out int areaMax))
            {
                _sootAlgo._areaMax = areaMax;
                PropertyChanged?.Invoke(this, null);
            }
            else
            {
                MessageBox.Show("숫자만 입력 가능합니다.");
                txt_ArMax.Text = _sootAlgo._areaMax.ToString(); // 기존 값 복원
            }


            if (int.TryParse(txt_ThMin.Text, out int widthMin))
            {
                _sootAlgo._binaryMin = widthMin;
                PropertyChanged?.Invoke(this, null);
            }
            else
            {
                MessageBox.Show("숫자만 입력 가능합니다.");
                txt_ThMin.Text = _sootAlgo._binaryMin.ToString(); // 기존 값 복원
            }


            if (int.TryParse(txt_ThMax.Text, out int widthMax))
            {
                _sootAlgo._binaryMax = widthMax;
                PropertyChanged?.Invoke(this, null);
            }
            else
            {
                MessageBox.Show("숫자만 입력 가능합니다.");
                txt_ThMax.Text = _sootAlgo._binaryMax.ToString(); // 기존 값 복원
            }
        }
    }
}
