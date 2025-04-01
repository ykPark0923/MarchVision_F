using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using OpenCvSharp;

namespace JidamVision.Algorithm
{
    public class ScratchAlgorithm : InspAlgorithm
    {
        [XmlIgnore]
        private Mat _templateImage = null;
        private Mat baseImage = null;
        public int _binaryMin { get; set; } = 200;
        public int _binaryMax { get; set; } = 255;
        public int _areaMin { get; set; } = 10;
        public int _areaMax { get; set; } = 600;
        public int _ratioMin { get; set; } = 2;
        public int _ratioMax { get; set; } = 25;

        public int ScratchCount { get; set; } = 0;
        public int OutScratchCount { get; set; } = 0;

        private List<Rect> _findArea = new List<Rect>();

        public ScratchAlgorithm()
        {
            InspectType = InspectType.InspScratch;
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


            #region Scratch는 외곽에 안생김, 외곽지움
            // 외곽 영역 제거 (중심부만 남김)
            int roiWidth = diff.Cols * 80 / 100;   // 중심 너비 (86%)
            int roiHeight = diff.Rows * 80 / 100;  // 중심 높이 (79%)
            int x = (diff.Cols - roiWidth) / 2;
            int y = (diff.Rows - roiHeight) / 2;
            Rect innerROI = new Rect(x, y, roiWidth, roiHeight);

            // 해당 영역만큼 자르기
            Mat roiImage = new Mat(diff, innerROI);
            #endregion

            Cv2.Threshold(roiImage, diff, _binaryMin, _binaryMax, ThresholdTypes.Binary);

            // 커널을 사용한 형태학적 연산 (닫기 연산)
            Mat kernel = new Mat(5, 5, MatType.CV_8U, Scalar.All(30));
            Mat closedDiff = new Mat();
            Cv2.MorphologyEx(diff, closedDiff, MorphTypes.Close, kernel);

            Point[][] contours;
            HierarchyIndex[] hierarchy;
            Cv2.FindContours(closedDiff, out contours, out hierarchy, RetrievalModes.External, ContourApproximationModes.ApproxSimple);


            if (_findArea is null)
                _findArea = new List<Rect>();

            _findArea.Clear();

            int findScratchCount = 0;


            foreach (var contour in contours)
            {
                // 윤곽선의 면적 계산
                double area = Cv2.ContourArea(contour);
                if (area >= _areaMin && area <= _areaMax)  // 면적 조건 확인
                {
                    // 윤곽선의 최소 면적 직사각형 찾기
                    RotatedRect box = Cv2.MinAreaRect(contour);
                    float aspectRatio = Math.Max(box.Size.Width, box.Size.Height) / Math.Min(box.Size.Width, box.Size.Height);

                    if (aspectRatio >= _ratioMin && aspectRatio <= _ratioMax)  // 비율 조건 확인
                    {
                        // 해당 윤곽선의 바운딩 박스를 그리기
                        Rect boundingBox = Cv2.BoundingRect(contour);
                        boundingBox.X += x;
                        boundingBox.Y += y;

                        // boundingBox의 좌상단 좌표 (시작 좌표)
                        Point2f topLeft = new Point2f(boundingBox.X, boundingBox.Y);

                        // 역변환하여 원본 좌표를 계산
                        Point2f originalTopLeft = perspectiveInverseTransform(topLeft, inversePerspectiveMatrix);


                        findScratchCount++;
                        // boundingBox의 좌상단 좌표를 계산된 원본 좌표로 보정
                        Rect boundingBoxWithOffset = new Rect(
                            (int)(boundingBox.X + (originalTopLeft.X - topLeft.X)),
                            (int)(boundingBox.Y + (originalTopLeft.Y - topLeft.Y)),
                            boundingBox.Width,
                            boundingBox.Height
                        );

                        string scratchInfo;
                        scratchInfo = $"Crack X:{boundingBoxWithOffset.X}, Y:{boundingBoxWithOffset.Y}, Size({boundingBoxWithOffset.Width},{boundingBoxWithOffset.Height})";
                        ResultString.Add(scratchInfo);

                        _findArea.Add(boundingBoxWithOffset);
                        IsInspected = true;
                    }

                    OutScratchCount = findScratchCount;

                    IsDefect = false;
                    if (ScratchCount > 0)
                    {
                        string result = "OK";
                        if (findScratchCount != ScratchCount)
                        {
                            result = "NG";
                            IsDefect = true;
                        }
                        string resultInfo = "";
                        resultInfo = $"[{result}] scratch count [in : {ScratchCount},out : {findScratchCount}]";
                        ResultString.Add(resultInfo);
                    }
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
