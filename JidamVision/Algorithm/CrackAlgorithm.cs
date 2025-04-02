using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using JidamVision.Teach;
using OpenCvSharp;

namespace JidamVision.Algorithm
{
    public class CrackAlgorithm : InspAlgorithm
    {
        [XmlIgnore]
        private Mat _templateImage = null;
        private Mat baseImage = null;
        public int _binaryMin { get; set; } = 50;
        public int _binaryMax { get; set; } = 255;
        public int _areaMin { get; set; } = 70;
        public int _areaMax { get; set; } = 1000;

        public int CrackCount { get; set; } = 0;
        public int OutCrackCount { get; set; } = 0;


        private List<Rect> _findArea = new List<Rect>();


        public CrackAlgorithm()
        {
            InspectType = InspectType.InspCrack;
        }


        public void SetTemplateImage(Mat templateImage)
        {
            _templateImage = templateImage;
            if (_templateImage.Type() == MatType.CV_8UC3)
                Cv2.CvtColor(_templateImage, baseImage, ColorConversionCodes.BGR2GRAY);
            else
                baseImage = _templateImage;
        }

        public override bool DoInspect()
        {

            ResetResult();

            if (_srcImage == null)
                return false;


            Mat targetImage = _srcImage[InspRect];
            Mat grayImage = new Mat();
            if (targetImage.Type() == MatType.CV_8UC3)
                Cv2.CvtColor(targetImage, grayImage, ColorConversionCodes.BGR2GRAY);
            else
                grayImage = targetImage;



            //비교할 srcImage는 xml과 같은 경로에 있는 images/폴더 아래에 있는 Image.bmp
            Mat aligned1 = AlignImage(grayImage, baseImage);
            Mat aligned2 = AlignImage(baseImage, baseImage);

            if (aligned1 == null || aligned2 == null)
                return false;


            Mat diff = new Mat();
            Cv2.Absdiff(aligned1, aligned2, diff);

            #region crack은 외곽에만 생김, 내부영역 지워버림
            // 이미지 크기 계산
            int roiWidth = diff.Cols * 90 / 100;   // 전체 너비의 90%
            int roiHeight = diff.Rows * 90 / 100;  // 전체 높이의 90%

            // 중심을 기준으로 ROI 좌표 설정
            int x = (diff.Cols - roiWidth) / 2;  // 중앙 정렬 X 좌표
            int y = (diff.Rows - roiHeight) / 2; // 중앙 정렬 Y 좌표

            // ROI 생성
            Rect innerROI = new Rect(x, y, roiWidth, roiHeight);

            // 안쪽 영역을 검정색으로 채우기
            Cv2.Rectangle(diff, innerROI, new Scalar(0), -1);
            #endregion

            Cv2.Threshold(diff, diff, _binaryMin, _binaryMax, ThresholdTypes.Binary);

            Point[][] contours;
            HierarchyIndex[] hierarchy;
            Cv2.FindContours(diff, out contours, out hierarchy, RetrievalModes.External, ContourApproximationModes.ApproxSimple);


            if (_findArea is null)
                _findArea = new List<Rect>();

            _findArea.Clear();

            int findCrackCount = 0;

            foreach (var contour in contours)
            {
                double area = Cv2.ContourArea(contour);
                if (area >= _areaMin && area <= _areaMax)  // 일정 크기 이상만 감지
                {
                    Rect boundingBox = Cv2.BoundingRect(contour);

                    // boundingBox의 좌상단 좌표 (시작 좌표)
                    Point2f topLeft = new Point2f(boundingBox.X, boundingBox.Y);

                    // 역변환하여 원본 좌표를 계산
                    Point2f originalTopLeft = perspectiveInverseTransform(topLeft, inversePerspectiveMatrix);

                    findCrackCount++;                    
                    // boundingBox의 좌상단 좌표를 계산된 원본 좌표로 보정
                    Rect boundingBoxWithOffset = new Rect(
                        (int)(boundingBox.X + (originalTopLeft.X - topLeft.X)+ InspRect.X),
                        (int)(boundingBox.Y + (originalTopLeft.Y - topLeft.Y) + InspRect.Y),
                        boundingBox.Width,
                        boundingBox.Height
                    );

                    string crackInfo;
                    crackInfo = $"Crack X:{boundingBoxWithOffset.X}, Y:{boundingBoxWithOffset.Y}, Size({boundingBoxWithOffset.Width},{boundingBoxWithOffset.Height})";
                    ResultString.Add(crackInfo);

                    _findArea.Add(boundingBoxWithOffset);
                    IsInspected = true;
                }

                ResultForm resultForm = MainForm.GetDockForm<ResultForm>();

                int crackCountFromResult = this.CrackCount;

                if (resultForm != null)
                {
                    var existingWindows = (resultForm.TreeListView.Objects as IEnumerable<InspWindow>)?.ToList();
                    if (existingWindows != null)
                    {
                        var matchedWindow = existingWindows.FirstOrDefault(w => w.UID == this.InspRect.ToString());
                        if (matchedWindow != null)
                        {
                            var matchedResult = matchedWindow.InspResultList
                                .FirstOrDefault(res => res.InspType == this.InspectType);

                            if (matchedResult != null)
                            {
                                var match = System.Text.RegularExpressions.Regex.Match(
                                    matchedResult.ResultInfos, @"in : (\d+)");

                                if (match.Success)
                                {
                                    crackCountFromResult = int.Parse(match.Groups[1].Value);
                                }
                            }
                        }
                    }
                }

                OutCrackCount = findCrackCount;

                IsDefect = false;
                if (CrackCount > 0)
                {
                    string result = "OK";
                    if (findCrackCount != CrackCount)
                    {
                        result = "NG";
                        IsDefect = true;
                    }
                    string resultInfo = "";
                    resultInfo = $"[{result}] crack count [in : {CrackCount},out : {findCrackCount}]";
                    ResultString.Add(resultInfo);
                }

            }
            return true;
        }

        public override int GetResultRect(out List<Rect> resultArea)
        {
            resultArea = null;

            //#ABSTRACT ALGORITHM#7 검사가 완료되지 않았다면, 리턴
            if (!IsInspected)
                return -1;

            if (_findArea is null || _findArea.Count <= 0)
                return -1;

            resultArea = _findArea;
            return resultArea.Count;
        }
    }
}
