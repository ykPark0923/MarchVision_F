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
    public partial class ScratchInspProp : UserControl
    {
        public event EventHandler<EventArgs> PropertyChanged;

        ScratchAlgorithm _scratchAlgo = new ScratchAlgorithm();
        public ScratchInspProp()
        {
            InitializeComponent();



            txt_ArMin.Leave += OnFilterChanged;
            txt_ArMax.Leave += OnFilterChanged;
            txt_ThMin.Leave += OnFilterChanged;
            txt_ThMax.Leave += OnFilterChanged;
            txt_RtMin.Leave += OnFilterChanged;
            txt_RtMax.Leave += OnFilterChanged;
        }

        public void SetAlgorithm(ScratchAlgorithm scratchAlgo)
        {
            _scratchAlgo = scratchAlgo;
            SetProperty();
        }


        public void SetProperty()
        {
            if (_scratchAlgo is null)
                return;

            txt_ArMin.Text = _scratchAlgo._areaMin.ToString();
            txt_ArMax.Text = _scratchAlgo._areaMax.ToString();
            txt_ThMin.Text = _scratchAlgo._binaryMin.ToString();
            txt_ThMax.Text = _scratchAlgo._binaryMax.ToString();
            txt_RtMin.Text = _scratchAlgo._ratioMin.ToString();
            txt_RtMax.Text = _scratchAlgo._ratioMax.ToString();
        }



        public void GetProperty()
        {
            if (_scratchAlgo is null)
                return;



            if (txt_ArMin.Text != "")
            {
                int areaMin = int.Parse(txt_ArMin.Text);
                _scratchAlgo._areaMin = areaMin;
            }
            if (txt_ArMax.Text != "")
            {
                int areaMax = int.Parse(txt_ArMax.Text);
                _scratchAlgo._areaMax = areaMax;
            }

            if (txt_ThMin.Text != "")
            {
                int binaryMin = int.Parse(txt_ThMin.Text);
                _scratchAlgo._binaryMin = binaryMin;
            }
            if (txt_ThMax.Text != "")
            {
                int binaryMax = int.Parse(txt_ThMax.Text);
                _scratchAlgo._binaryMax = binaryMax;
            }

            if (txt_RtMin.Text != "")
            {
                int ratioMin = int.Parse(txt_RtMin.Text);
                _scratchAlgo._ratioMin = ratioMin;
            }
            if (txt_RtMax.Text != "")
            {
                int ratioMax = int.Parse(txt_RtMax.Text);
                _scratchAlgo._ratioMax = ratioMax;
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
            if (_scratchAlgo == null)
                return;

            if (int.TryParse(txt_ArMin.Text, out int areaMin))
            {
                _scratchAlgo._areaMin = areaMin;
                PropertyChanged?.Invoke(this, null);
            }
            else
            {
                MessageBox.Show("숫자만 입력 가능합니다.");
                txt_ArMin.Text = _scratchAlgo._areaMin.ToString(); // 기존 값 복원
            }


            if (int.TryParse(txt_ArMax.Text, out int areaMax))
            {
                _scratchAlgo._areaMax = areaMax;
                PropertyChanged?.Invoke(this, null);
            }
            else
            {
                MessageBox.Show("숫자만 입력 가능합니다.");
                txt_ArMax.Text = _scratchAlgo._areaMax.ToString(); // 기존 값 복원
            }


            if (int.TryParse(txt_ThMin.Text, out int binaryMin))
            {
                _scratchAlgo._binaryMin = binaryMin;
                PropertyChanged?.Invoke(this, null);
            }
            else
            {
                MessageBox.Show("숫자만 입력 가능합니다.");
                txt_ThMin.Text = _scratchAlgo._binaryMin.ToString(); // 기존 값 복원
            }


            if (int.TryParse(txt_ThMax.Text, out int binaryMax))
            {
                _scratchAlgo._binaryMax = binaryMax;
                PropertyChanged?.Invoke(this, null);
            }
            else
            {
                MessageBox.Show("숫자만 입력 가능합니다.");
                txt_ThMax.Text = _scratchAlgo._binaryMax.ToString(); // 기존 값 복원
            }


            if (int.TryParse(txt_RtMin.Text, out int ratioMin))
            {
                _scratchAlgo._ratioMin = ratioMin;
                PropertyChanged?.Invoke(this, null);
            }
            else
            {
                MessageBox.Show("숫자만 입력 가능합니다.");
                txt_ThMin.Text = _scratchAlgo._binaryMin.ToString(); // 기존 값 복원
            }


            if (int.TryParse(txt_ThMax.Text, out int ratioMax))
            {
                _scratchAlgo._ratioMin = ratioMin;
                PropertyChanged?.Invoke(this, null);
            }
            else
            {
                MessageBox.Show("숫자만 입력 가능합니다.");
                txt_ThMax.Text = _scratchAlgo._binaryMax.ToString(); // 기존 값 복원
            }
        }
    }
}
