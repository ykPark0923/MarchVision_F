using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using OpenCvSharp;

namespace JidamVision.Algorithm
{
    public class DentAlgorithm : InspAlgorithm
    {
        [XmlIgnore]
        private Mat _templateImage = null;
        private Mat baseImage = null;
        public int _binaryMin { get; set; } = 30;
        public int _binaryMax { get; set; } = 255;
        public int _areaMin { get; set; } = 26;
        public int _areaMax { get; set; } = 2000;

        public int DentCount { get; set; } = 0;
        public int OutDentCount { get; set; } = 0;

        private List<Rect> _findArea = new List<Rect>();

        public DentAlgorithm()
        {
            InspectType = InspectType.InspDent;
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



            IsInspected = false;
            ResultString = new List<string>();
            _findArea.Clear();

            //비교할 srcImage는 xml과 같은 경로에 있는 images/폴더 아래에 있는 Image.bmp
            Mat aligned1 = AlignImage(grayImage, baseImage);
            Mat aligned2 = AlignImage(baseImage, baseImage);

            if (aligned1 == null || aligned2 == null)
                return false;

            Mat diff = new Mat();
            Cv2.Absdiff(aligned1, aligned2, diff);
            Cv2.Threshold(diff, diff, _binaryMin, _binaryMax, ThresholdTypes.Binary);

            #region dent는 외곽에 안생김, 외곽지움
            // 외곽 영역 제거 (중심부만 남김)
            int roiWidth = diff.Cols * 86 / 100;   // 중심 너비 (86%)
            int roiHeight = diff.Rows * 79 / 100;  // 중심 높이 (79%)
            int x = (diff.Cols - roiWidth) / 2;
            int y = (diff.Rows - roiHeight) / 2;
            Rect innerROI = new Rect(x, y, roiWidth, roiHeight);

            // 외곽을 검정색으로 덮기 (중심만 남김)
            Mat mask = Mat.Zeros(diff.Size(), MatType.CV_8UC1);
            Cv2.Rectangle(mask, innerROI, new Scalar(255), -1); // 중심을 흰색으로 유지
            Cv2.BitwiseAnd(diff, mask, diff); // 중심 부분만 유지
            #endregion

                        
            Point[][] contours;
            HierarchyIndex[] hierarchy;
            Cv2.FindContours(diff, out contours, out hierarchy, RetrievalModes.External, ContourApproximationModes.ApproxSimple);


            if (_findArea is null)
                _findArea = new List<Rect>();

            _findArea.Clear();

            int findDentCount = 0;

            foreach (var contour in contours)
            {
                double area = Cv2.ContourArea(contour);
                if (area >= _areaMin && area <= _areaMax)  // 일정 크기 이상의 Dent만 감지
                {
                    Rect boundingBox = Cv2.BoundingRect(contour);

                    // boundingBox의 좌상단 좌표 (시작 좌표)
                    Point2f topLeft = new Point2f(boundingBox.X, boundingBox.Y);

                    // 역변환하여 원본 좌표를 계산
                    Point2f originalTopLeft = perspectiveInverseTransform(topLeft, inversePerspectiveMatrix);


                    findDentCount++;
                    // boundingBox의 좌상단 좌표를 계산된 원본 좌표로 보정
                    Rect boundingBoxWithOffset = new Rect(
                        (int)(boundingBox.X + (originalTopLeft.X - topLeft.X)),
                        (int)(boundingBox.Y + (originalTopLeft.Y - topLeft.Y)),
                        boundingBox.Width,
                        boundingBox.Height
                    );

                    string dentInfo;
                    dentInfo = $"Dent X:{boundingBoxWithOffset.X}, Y:{boundingBoxWithOffset.Y}, Size({boundingBoxWithOffset.Width},{boundingBoxWithOffset.Height})";
                    ResultString.Add(dentInfo);

                    _findArea.Add(boundingBoxWithOffset);
                }

                OutDentCount = findDentCount;

                IsDefect = false;
                if (DentCount > 0)
                {
                    string result = "OK";
                    if (findDentCount != DentCount)
                    {
                        result = "NG";
                        IsDefect = true;
                    }
                    string resultInfo = "";
                    resultInfo = $"[{result}] dent count [in : {DentCount},out : {findDentCount}]";
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
