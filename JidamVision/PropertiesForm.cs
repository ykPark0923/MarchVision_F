﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using JidamVision.Property;
using JidamVision.Core;
using JidamVision.Teach;
using JidamVision.Algorithm;

namespace JidamVision
{
    /*
    #PANEL TO TAB# - <<<패널 방식을 모든 속성을 한번에 볼 수 있는 탭 방식으로 변경>>> 
    디자인 창에서 [PANEL]을 삭제하고 [TabControl]을 추가할것
     */


    //#PANEL TO TAB#1 enum 타입 이름 변경
    //InspPropType => InspectType
    //Ctrl + R 키를 이용해 변경해보기
    public enum InspectType
    {
        InspNone = -1,
        //InspBinary,
        InspMatch,
        //InspFilter,
        InspCrack,
        InspDent,
        InspScratch,
        InspSoot,
        InspCount
    }

    public partial class PropertiesForm : DockContent
    {
        Dictionary<string, TabPage> _allTabs = new Dictionary<string, TabPage>();

        public PropertiesForm()
        {
            InitializeComponent();
        }

        //#PANEL TO TAB#3 속성탭이 있다면 그것을 반환하고, 없다면 생성
        private void LoadOptionControl(InspectType inspType)
        {
            string tabName = inspType.ToString();

            // 이미 있는 TabPage인지 확인
            foreach (TabPage tabPage in tabPropControl.TabPages)
            {
                if (tabPage.Text == tabName)
                    return;
            }

            // 딕셔너리에 있으면 추가
            if (_allTabs.TryGetValue(tabName, out TabPage page))
            {
                tabPropControl.TabPages.Add(page);
                return;
            }

            // 새로운 UserControl 생성
            UserControl _inspProp = CreateUserControl(inspType);
            if (_inspProp == null) 
                return;

            // 새 탭 추가
            TabPage newTab = new TabPage(tabName)
            {
                Dock = DockStyle.Fill
            };
            _inspProp.Dock = DockStyle.Fill;
            newTab.Controls.Add(_inspProp);
            tabPropControl.TabPages.Add(newTab);
            tabPropControl.SelectedTab = newTab; // 새 탭 선택

            _allTabs[tabName] = newTab;
        }

        //#PANEL TO TAB#2 속성탭 타입에 맞게 UseControl 생성하여 반환
        private UserControl CreateUserControl(InspectType inspPropType)
        {
            UserControl _inspProp = null;
            switch (inspPropType)
            {
                //case InspectType.InspBinary:
                //    BinaryInspProp blobProp = new BinaryInspProp();
                //    blobProp.RangeChanged += RangeSlider_RangeChanged;
                //    blobProp.PropertyChanged += PropertyChanged;
                //    _inspProp = blobProp;
                //    break;
                //case InspectType.InspFilter:
                //    FilterInspProp filterProp = new FilterInspProp();
                //    filterProp.FilterSelected += FilterSelect_FilterChanged;
                //    _inspProp = filterProp;
                //    break;
                case InspectType.InspMatch:
                    MatchInspProp matchProp = new MatchInspProp();
                    matchProp.PropertyChanged += PropertyChanged;
                    _inspProp = matchProp;
                    break;
                case InspectType.InspCrack:
                    CrackInspProp crackProp = new CrackInspProp();
                    crackProp.PropertyChanged += PropertyChanged;
                    _inspProp = crackProp;
                    break;
                case InspectType.InspDent:
                    DentInspProp dentProp = new DentInspProp();
                    dentProp.PropertyChanged += PropertyChanged;
                    _inspProp = dentProp;
                    break;
                case InspectType.InspScratch:
                    ScratchInspProp scratchProp = new ScratchInspProp();
                    scratchProp.PropertyChanged += PropertyChanged;
                    _inspProp = scratchProp;
                    break;
                case InspectType.InspSoot:
                    SootInspProp sootProp = new SootInspProp();
                    sootProp.PropertyChanged += PropertyChanged;
                    _inspProp = sootProp;
                    break;
                default:
                    MessageBox.Show("유효하지 않은 옵션입니다.");
                    return null;
            }
            return _inspProp;
        }
        
        public void ShowProperty(InspWindow window)
        {
            foreach (InspAlgorithm algo in window.AlgorithmList)
            {
                LoadOptionControl(algo.InspectType);
            }

            tabPropControl.SelectedIndex = 0;
        }

        public void ResetProperty()
        {
            tabPropControl.TabPages.Clear();
        }

        public void UpdateProperty(InspWindow window)
        {
            if (window is null)
                return;

            foreach (TabPage tabPage in tabPropControl.TabPages)
            {
                if (tabPage.Controls.Count > 0)
                {
                    UserControl uc = tabPage.Controls[0] as UserControl;

                    if (uc is MatchInspProp matchProp)
                    {
                        MatchAlgorithm matchAlgo = (MatchAlgorithm)window.FindInspAlgorithm(InspectType.InspMatch);
                        if (matchAlgo is null)
                            continue;

                        matchProp.SetAlgorithm(matchAlgo);
                    }
                    //else if (uc is BinaryInspProp binaryProp)
                    //{
                    //    BlobAlgorithm blobAlgo = (BlobAlgorithm)window.FindInspAlgorithm(InspectType.InspBinary);
                    //    if (blobAlgo is null)
                    //        continue;

                    //    binaryProp.SetAlgorithm(blobAlgo);
                    //}
                    if (uc is CrackInspProp crackProp)
                    {
                        CrackAlgorithm crackAlgo = (CrackAlgorithm)window.FindInspAlgorithm(InspectType.InspCrack);
                        if (crackAlgo is null)
                            continue;

                        crackProp.SetAlgorithm(crackAlgo);
                    }
                    else if (uc is DentInspProp dentProp)
                    {
                        DentAlgorithm dentAlgo = (DentAlgorithm)window.FindInspAlgorithm(InspectType.InspDent);
                        if (dentAlgo is null)
                            continue;

                        dentProp.SetAlgorithm(dentAlgo);
                    }
                    else if (uc is ScratchInspProp scratchProp)
                    {
                        ScratchAlgorithm scratchAlgo = (ScratchAlgorithm)window.FindInspAlgorithm(InspectType.InspScratch);
                        if (scratchAlgo is null)
                            continue;

                        scratchProp.SetAlgorithm(scratchAlgo);
                    }
                    else if (uc is SootInspProp sootProp)
                    {
                        SootAlgorithm sootAlgo = (SootAlgorithm)window.FindInspAlgorithm(InspectType.InspSoot);
                        if (sootAlgo is null)
                            continue;

                        sootProp.SetAlgorithm(sootAlgo);
                    }
                }
            }
        }

        //#BINARY FILTER#16 이진화 속성 변경시 발생하는 이벤트 수정
        private void RangeSlider_RangeChanged(object sender, RangeChangedEventArgs e)
        {
            // 속성값을 이용하여 이진화 임계값 설정
            int lowerValue = e.LowerValue;
            int upperValue = e.UpperValue;
            bool invert = e.Invert;
            ShowBinaryMode showBinMode = e.ShowBinMode;
            Global.Inst.InspStage.PreView?.SetBinary(lowerValue, upperValue, invert, showBinMode);
        }

        private void FilterSelect_FilterChanged(object sender, FilterSelectedEventArgs e)
        {
            //선택된 필터값 PrieviewImage의 ApplyFilter로 보냄
            string filter1 = e.FilterSelected1;
            int filter2 = e.FilterSelected2;
            Global.Inst.InspStage.PreView?.ApplyFilter(filter1, filter2);

        }

        private void PropertyChanged(object sender, EventArgs e)
        {
            Global.Inst.InspStage.RedrawMainView();
        }
    }
}
