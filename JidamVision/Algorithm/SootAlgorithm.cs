using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using OpenCvSharp;

namespace JidamVision.Algorithm
{
    public class SootAlgorithm : InspAlgorithm
    {
        [XmlIgnore]
        private Mat _templateImage = null;
        private Mat baseImage = null;
        public int _binaryMin { get; set; } = 20;
        public int _binaryMax { get; set; } = 255;
        public int _areaMin { get; set; } = 20;
        public int _areaMax { get; set; } = 1000;

        public int SootCount { get; set; } = 0;
        public int OutSootCount { get; set; } = 0;

        private List<Rect> _findArea = new List<Rect>();

        private bool sootDetected;

        public SootAlgorithm()
        {
            InspectType = InspectType.InspSoot;
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



            // 1. 히스토그램 균등화: 이미지를 균등화하여 대비를 개선
            Cv2.EqualizeHist(aligned1, aligned1);
            Cv2.EqualizeHist(aligned2, aligned2);

            // 2. 블러 적용하여 노이즈 제거
            Cv2.GaussianBlur(aligned1, aligned1, new OpenCvSharp.Size(5, 5), 0);
            Cv2.GaussianBlur(aligned2, aligned2, new OpenCvSharp.Size(5, 5), 0);


            Mat diffImage = new Mat();
            Cv2.Subtract(aligned2, aligned1, diffImage); // 밝아진 부분 강조

            // 4. 연한 Soot도 잡을 수 있게 임계값을 낮추어 적용
            Mat sootMask = new Mat();

            Cv2.Threshold(diffImage, sootMask, _binaryMin, _binaryMax, ThresholdTypes.Binary);

            // Soot와 배경의 경계를 분명히 하기 위해 Canny 엣지 검출 적용
            Mat edges = new Mat();
            Cv2.Canny(diffImage, edges, 100, 200); // 엣지 검출

            //파라미터화****************************************************************************
            Mat bgMask = new Mat();
            Cv2.Threshold(diffImage, bgMask, 37, 255, ThresholdTypes.BinaryInv); // 어두운 부분은 배경으로 설정

            #region dent는 외곽에 안생김, 외곽지움
            // 외곽 영역 제거 (중심부만 남김)
            int roiWidth = diffImage.Cols * 80 / 100;   // 중심 너비 (86%)
            int roiHeight = diffImage.Rows * 79 / 100;  // 중심 높이 (79%)
            int x = (diffImage.Cols - roiWidth) / 2;
            int y = (diffImage.Rows - roiHeight) / 2;
            Rect innerROI = new Rect(x, y, roiWidth, roiHeight);

            // 외곽을 제거하기 위한 마스크 생성
            Mat mask = Mat.Zeros(sootMask.Size(), MatType.CV_8UC1); // sootMask 크기의 제로 행렬 생성
            Cv2.Rectangle(mask, innerROI, new Scalar(255), -1); // 중심 영역만 흰색으로 유지

            // 8. 중심 부분만 남기고 외곽은 제거
            Cv2.BitwiseAnd(sootMask, mask, sootMask); // 외곽을 제거하여 sootMask를 업데이트
            #endregion

            // 최종 Soot 검출 결과
            Mat finalMask = new Mat();
            Cv2.BitwiseAnd(sootMask, bgMask, finalMask); // 배경 제거된 최종 sootMask


            Point[][] contours;
            HierarchyIndex[] hierarchy;
            Cv2.FindContours(finalMask, out contours, out hierarchy, RetrievalModes.External, ContourApproximationModes.ApproxSimple);


            if (_findArea is null)
                _findArea = new List<Rect>();

            _findArea.Clear();

            int findSootCount = 0;
            List<Rect> boundingBoxes = new List<Rect>();

            foreach (var contour in contours)
            {
                double area = Cv2.ContourArea(contour);
                if (area >= _areaMin && area <= _areaMax) // 작은 노이즈는 무시
                {
                    boundingBoxes.Add(Cv2.BoundingRect(contour)); // 윤곽선을 감싸는 바운딩 박스를 추가
                }
            }

            // 가까운 영역 병합
            List<Rect> mergedBoxes = MergeBoundingBoxes(boundingBoxes, 200);


            if (mergedBoxes.Count > 0) // 리스트에 요소가 있는지 확인
            {
                Rect firstBox = mergedBoxes[0]; // 첫 번째 요소 선택
                Point2f topLeft = new Point2f(firstBox.X, firstBox.Y);

                // 역변환하여 원본 좌표를 계산
                Point2f originalTopLeft = perspectiveInverseTransform(topLeft, inversePerspectiveMatrix);


                findSootCount++;
                // boundingBox의 좌상단 좌표를 계산된 원본 좌표로 보정
                Rect boundingBoxWithOffset = new Rect(
                    (int)(firstBox.X + (originalTopLeft.X - topLeft.X) + InspRect.X),
                    (int)(firstBox.Y + (originalTopLeft.Y - topLeft.Y) + InspRect.Y),
                    firstBox.Width,
                    firstBox.Height
                );

                string sootInfo;
                sootInfo = $"Crack X:{boundingBoxWithOffset.X}, Y:{boundingBoxWithOffset.Y}, Size({boundingBoxWithOffset.Width},{boundingBoxWithOffset.Height})";
                ResultString.Add(sootInfo);

                _findArea.Add(boundingBoxWithOffset);
                IsInspected = true;
            }

            OutSootCount = findSootCount;

            IsDefect = false;
            if (SootCount > 0)
            {
                string result = "OK";
                if (findSootCount != SootCount)
                {
                    result = "NG";
                    IsDefect = true;
                }
                string resultInfo = "";
                resultInfo = $"[{result}] soot count [in : {SootCount},out : {findSootCount}]";
                ResultString.Add(resultInfo);
            }

            return true;
        }

        // 병합할 바운딩 박스가 가까운지 판단하는 함수 (중심점 거리 기준)
        public List<Rect> MergeBoundingBoxes(List<Rect> boxes, int threshold, int minArea = 50)
        {
            if (boxes.Count == 0)
                return new List<Rect>();

            List<Rect> mergedBoxes = new List<Rect>(boxes);
            bool merged;

            do
            {
                merged = false;
                List<Rect> newBoxes = new List<Rect>();

                while (mergedBoxes.Count > 0)
                {
                    Rect current = mergedBoxes[0];
                    mergedBoxes.RemoveAt(0);

                    for (int i = 0; i < mergedBoxes.Count; i++)
                    {
                        if (IsClose(current, mergedBoxes[i], threshold)) // 두 박스가 가까우면
                        {
                            // 두 박스를 병합
                            current = UnionRect(current, mergedBoxes[i]);
                            mergedBoxes.RemoveAt(i);
                            merged = true;
                            i--; // 리스트 크기 감소에 따른 인덱스 수정
                        }
                    }

                    // 최소 크기 기준을 만족하는 박스만 추가
                    if (current.Width * current.Height >= minArea)
                    {
                        newBoxes.Add(current);
                    }
                }

                mergedBoxes = newBoxes;
            } while (merged); // 더 이상 병합이 발생하지 않을 때까지 반복

            return mergedBoxes;
        }
        private bool IsClose(Rect box1, Rect box2, int threshold)
        {
            int centerX1 = box1.X + box1.Width / 2;
            int centerY1 = box1.Y + box1.Height / 2;
            int centerX2 = box2.X + box2.Width / 2;
            int centerY2 = box2.Y + box2.Height / 2;

            double distance = Math.Sqrt(Math.Pow(centerX1 - centerX2, 2) + Math.Pow(centerY1 - centerY2, 2));

            return distance < threshold; // 두 박스의 중심점 거리가 threshold 이하이면 가까운 것으로 판단
        }

        // 두 박스를 병합하는 함수
        private Rect UnionRect(Rect box1, Rect box2)
        {
            int x = Math.Min(box1.X, box2.X);
            int y = Math.Min(box1.Y, box2.Y);
            int width = Math.Max(box1.X + box1.Width, box2.X + box2.Width) - x;
            int height = Math.Max(box1.Y + box1.Height, box2.Y + box2.Height) - y;

            return new Rect(x, y, width, height); // 병합된 박스를 반환
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
