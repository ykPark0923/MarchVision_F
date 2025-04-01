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


            txt_ArMin.Leave += OnUpdateValue;
            txt_ArMax.Leave += OnUpdateValue;
            txt_ThMin.Leave += OnUpdateValue;
            txt_ThMax.Leave += OnUpdateValue;
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




            int sootAreaMin = _sootAlgo._areaMin;
            int sootAreaMax = _sootAlgo._areaMax;
            int sootBinaryMin = _sootAlgo._binaryMin;
            int sootBinaryMax = _sootAlgo._binaryMax;

            txt_ArMin.Text = sootAreaMin.ToString();
            txt_ArMax.Text = sootAreaMax.ToString();
            txt_ThMin.Text = sootBinaryMin.ToString();
            txt_ThMax.Text = sootBinaryMax.ToString();
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



        private void OnUpdateValue(object sender, EventArgs e)
        {
            if (_sootAlgo == null)
                return;


            int areaMin = _sootAlgo._areaMin;
            if (!int.TryParse(txt_ArMin.Text, out areaMin))
            {
                MessageBox.Show("숫자만 입력 가능합니다.");
                return;
            }

            int areaMax = _sootAlgo._areaMax;
            if (!int.TryParse(txt_ArMin.Text, out areaMax))
            {
                MessageBox.Show("숫자만 입력 가능합니다.");
                return;
            }

            int binaryMin = _sootAlgo._binaryMin;
            if (!int.TryParse(txt_ArMin.Text, out binaryMin))
            {
                MessageBox.Show("숫자만 입력 가능합니다.");
                return;
            }

            int binaryMax = _sootAlgo._binaryMax;
            if (!int.TryParse(txt_ArMin.Text, out binaryMax))
            {
                MessageBox.Show("숫자만 입력 가능합니다.");
                return;
            }

            _sootAlgo._areaMin = areaMin;
            _sootAlgo._areaMax = areaMax;
            _sootAlgo._binaryMin = binaryMin;
            _sootAlgo._binaryMax = binaryMax;

            PropertyChanged?.Invoke(this, null);
        }
    }
}
