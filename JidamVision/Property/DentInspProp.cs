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

        DentAlgorithm _dentAlgo = null;
        public DentInspProp()
        {
            InitializeComponent();
            txt_ArMin.Leave += OnUpdateValue;
            txt_ArMax.Leave += OnUpdateValue;
            txt_ThMin.Leave += OnUpdateValue;
            txt_ThMax.Leave += OnUpdateValue;
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

            int dentAreaMin = _dentAlgo._areaMin;
            int dentAreaMax = _dentAlgo._areaMax;
            int dentBinaryMin = _dentAlgo._binaryMin;
            int dentBinaryMax = _dentAlgo._binaryMax;

            txt_ArMin.Text = dentAreaMin.ToString();
            txt_ArMax.Text = dentAreaMax.ToString();
            txt_ThMin.Text = dentBinaryMin.ToString();
            txt_ThMax.Text = dentBinaryMax.ToString();
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






        private void OnUpdateValue(object sender, EventArgs e)
        {
            if (_dentAlgo == null)
                return;

            int areaMin = _dentAlgo._areaMin;
            if (!int.TryParse(txt_ArMin.Text, out areaMin))
            {
                MessageBox.Show("숫자만 입력 가능합니다.");
                return;
            }

            int areaMax = _dentAlgo._areaMax;
            if (!int.TryParse(txt_ArMin.Text, out areaMax))
            {
                MessageBox.Show("숫자만 입력 가능합니다.");
                return;
            }

            int binaryMin = _dentAlgo._binaryMin;
            if (!int.TryParse(txt_ArMin.Text, out binaryMin))
            {
                MessageBox.Show("숫자만 입력 가능합니다.");
                return;
            }

            int binaryMax = _dentAlgo._binaryMax;
            if (!int.TryParse(txt_ArMin.Text, out binaryMax))
            {
                MessageBox.Show("숫자만 입력 가능합니다.");
                return;
            }

            _dentAlgo._areaMin = areaMin;
            _dentAlgo._areaMax = areaMax;
            _dentAlgo._binaryMin = binaryMin;
            _dentAlgo._binaryMax = binaryMax;

            PropertyChanged?.Invoke(this, null);
        }
    }
}
