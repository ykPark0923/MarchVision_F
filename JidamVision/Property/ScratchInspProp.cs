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

        ScratchAlgorithm _scratchAlgo = null;
        public ScratchInspProp()
        {
            InitializeComponent();



            txt_ArMin.Leave += OnUpdateValue;
            txt_ArMax.Leave += OnUpdateValue;
            txt_ThMin.Leave += OnUpdateValue;
            txt_ThMax.Leave += OnUpdateValue;
            txt_RtMin.Leave += OnUpdateValue;
            txt_RtMax.Leave += OnUpdateValue;
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


            int scratchAreaMin = _scratchAlgo._areaMin;
            int scratcAreaMax = _scratchAlgo._areaMax;
            int scratcBinaryMin = _scratchAlgo._binaryMin;
            int scratcBinaryMax = _scratchAlgo._binaryMax;
            int scratcRatioMin = _scratchAlgo._ratioMin;
            int scratcRatioMax = _scratchAlgo._ratioMax;

            txt_ArMin.Text = scratchAreaMin.ToString();
            txt_ArMax.Text = scratcAreaMax.ToString();
            txt_ThMin.Text = scratcBinaryMin.ToString();
            txt_ThMax.Text = scratcBinaryMax.ToString();
            txt_RtMin.Text = scratcRatioMin.ToString();
            txt_RtMax.Text = scratcRatioMax.ToString();
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






        private void OnUpdateValue(object sender, EventArgs e)
        {
            if (_scratchAlgo == null)
                return;


            int areaMin = _scratchAlgo._areaMin;
            if (!int.TryParse(txt_ArMin.Text, out areaMin))
            {
                MessageBox.Show("숫자만 입력 가능합니다.");
                return;
            }

            int areaMax = _scratchAlgo._areaMax;
            if (!int.TryParse(txt_ArMin.Text, out areaMax))
            {
                MessageBox.Show("숫자만 입력 가능합니다.");
                return;
            }

            int binaryMin = _scratchAlgo._binaryMin;
            if (!int.TryParse(txt_ArMin.Text, out binaryMin))
            {
                MessageBox.Show("숫자만 입력 가능합니다.");
                return;
            }

            int binaryMax = _scratchAlgo._binaryMax;
            if (!int.TryParse(txt_ArMin.Text, out binaryMax))
            {
                MessageBox.Show("숫자만 입력 가능합니다.");
                return;
            }

            int ratioMin = _scratchAlgo._ratioMin;
            if (!int.TryParse(txt_ArMin.Text, out ratioMin))
            {
                MessageBox.Show("숫자만 입력 가능합니다.");
                return;
            }

            int ratioMax = _scratchAlgo._ratioMax;
            if (!int.TryParse(txt_ArMin.Text, out ratioMax))
            {
                MessageBox.Show("숫자만 입력 가능합니다.");
                return;
            }


            _scratchAlgo._areaMin = areaMin;
            _scratchAlgo._areaMax = areaMax;
            _scratchAlgo._binaryMin = binaryMin;
            _scratchAlgo._binaryMax = binaryMax;
            _scratchAlgo._ratioMin = ratioMin;
            _scratchAlgo._ratioMax = ratioMax;

            PropertyChanged?.Invoke(this, null);
        }
    }
}
