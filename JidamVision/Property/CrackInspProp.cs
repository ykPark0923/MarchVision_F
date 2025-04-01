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


            txt_ArMin.Leave += OnUpdateValue;
            txt_ArMax.Leave += OnUpdateValue;
            txt_ThMin.Leave += OnUpdateValue;
            txt_ThMax.Leave += OnUpdateValue;
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


            int crackAreaMin = _crackAlgo._areaMin;
            int crackAreaMax = _crackAlgo._areaMax;
            int crackBinaryMin = _crackAlgo._binaryMin;
            int crackBinaryMax = _crackAlgo._binaryMax;

            txt_ArMin.Text = crackAreaMin.ToString();
            txt_ArMax.Text = crackAreaMax.ToString();
            txt_ThMin.Text = crackBinaryMin.ToString();
            txt_ThMax.Text = crackBinaryMax.ToString();
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




        private void OnUpdateValue(object sender, EventArgs e)
        {
            if (_crackAlgo == null)
                return;


            int areaMin = _crackAlgo._areaMin;
            if (!int.TryParse(txt_ArMin.Text, out areaMin))
            {
                MessageBox.Show("숫자만 입력 가능합니다.");
                return;
            }

            int areaMax = _crackAlgo._areaMax;
            if (!int.TryParse(txt_ArMin.Text, out areaMax))
            {
                MessageBox.Show("숫자만 입력 가능합니다.");
                return;
            }

            int binaryMin = _crackAlgo._binaryMin;
            if (!int.TryParse(txt_ArMin.Text, out binaryMin))
            {
                MessageBox.Show("숫자만 입력 가능합니다.");
                return;
            }

            int binaryMax = _crackAlgo._binaryMax;
            if (!int.TryParse(txt_ArMin.Text, out binaryMax))
            {
                MessageBox.Show("숫자만 입력 가능합니다.");
                return;
            }

            _crackAlgo._areaMin = areaMin;
            _crackAlgo._areaMax = areaMax;
            _crackAlgo._binaryMin = binaryMin;
            _crackAlgo._binaryMax = binaryMax;

            PropertyChanged?.Invoke(this, null);

        }
    }
}
