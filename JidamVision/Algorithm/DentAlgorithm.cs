using System;
using System.Collections.Generic;
using System.Linq;
using OpenCvSharp;

namespace JidamVision.Algorithm
{
    public class DentAlgorithm : InspAlgorithm
    {
        public int _binaryMin { get; set; } = 30;
        public int _binaryMax { get; set; } = 255;
        public int _areaMin { get; set; } = 26;
        public int _areaMax { get; set; } = 2000;

        public int DentCount { get; set; } = 0;
        public int OutDentCount { get; set; } = 0;

        // align한 좌표값을 inv할 때
        protected Mat inversePerspectiveMatrix { get; set; } = null;

        private List<Rect> _findArea = new List<Rect>();

        public DentAlgorithm()
        {
            InspectType = InspectType.InspDent;
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
            Mat aligned1 = AlignImage(grayImage, _srcImage);
            Mat aligned2 = AlignImage(_srcImage, _srcImage);

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

        private Point2f perspectiveInverseTransform(Point2f point, Mat inverseMatrix)
        {
            // Homogeneous 좌표로 변환 (3x1 크기의 행렬로 설정)
            Mat homogenousPoint = new Mat(3, 1, MatType.CV_32F);
            homogenousPoint.Set<float>(0, 0, point.X);
            homogenousPoint.Set<float>(1, 0, point.Y);
            homogenousPoint.Set<float>(2, 0, 1); // 동차 좌표로 변환

            // inverseMatrix와 homogenousPoint의 데이터 타입을 맞추기 위해 변환 (CV_32F)
            if (inverseMatrix.Type() != MatType.CV_32F)
            {
                inverseMatrix.ConvertTo(inverseMatrix, MatType.CV_32F);
            }

            // 행렬 곱셈: 역변환 행렬을 적용
            Mat transformedPoint = inverseMatrix * homogenousPoint; // 행렬 곱셈

            // 역변환 후 좌표
            float x = transformedPoint.Get<float>(0, 0) / transformedPoint.Get<float>(2, 0);
            float y = transformedPoint.Get<float>(1, 0) / transformedPoint.Get<float>(2, 0);

            return new Point2f(x, y);
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

        // 이미지 align해서 반환하는 메소드
        public Mat AlignImage(Mat _srcImage, Mat _diffSrc)
        {
            if (_diffSrc == null || _diffSrc.Empty())
                throw new Exception("기준 이미지(_diffSrc)가 null이거나 비어 있습니다. SetBaseImage() 또는 BaseImage()를 통해 설정되었는지 확인하세요.");

            Mat src = _srcImage;
            Mat src2 = _diffSrc;
            Mat gray = new Mat();
            Mat binary = new Mat();
            Mat result = new Mat();

            // 1) 그레이스케일 변환
            if (src.Channels() == 3)
                Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY);
            else
                gray = src.Clone();

            #region 좌우상하 원검출을 위해 안쪽영역 지움
            int roiWidth = src.Cols * 70 / 100;
            int roiHeight = src.Rows * 100 / 100;
            int x = (src.Cols - roiWidth) / 2;
            int y = (src.Rows - roiHeight) / 2;
            Rect innerROI = new Rect(x, y, roiWidth, roiHeight);
            Cv2.Rectangle(gray, innerROI, new Scalar(0), -1);

            roiWidth = src.Cols * 100 / 100;
            roiHeight = src.Rows * 65 / 100;
            x = (src.Cols - roiWidth) / 2;
            y = (src.Rows - roiHeight) / 2;
            innerROI = new Rect(x, y, roiWidth, roiHeight);
            Cv2.Rectangle(gray, innerROI, new Scalar(0), -1);
            #endregion

            // 2) 이진화
            Cv2.Threshold(gray, binary, 30, 255, ThresholdTypes.Binary);

            // 3) 윤곽선 추출
            Point[][] contours;
            HierarchyIndex[] hierarchy;
            Cv2.FindContours(binary, out contours, out hierarchy,
                RetrievalModes.Tree, ContourApproximationModes.ApproxSimple);

            var sortedContours = contours
                .OrderByDescending(c => Cv2.ContourArea(c))
                .ToList();

            var targetContours = sortedContours.Skip(4).Take(4);

            List<Point2f> centerPoints = new List<Point2f>();
            foreach (var contour in targetContours)
            {
                Point2f center;
                float radius;
                Cv2.MinEnclosingCircle(contour, out center, out radius);
                centerPoints.Add(center);
            }

            if (centerPoints.Count != 4)
            {
                Console.WriteLine("Error: 4개의 중심을 검출하지 못함. 검출된 중심 개수: " + centerPoints.Count);
                return null;
            }

            Point2f[] sortedCenters = SortCenters(centerPoints);

            int paddingX = 30;
            int paddingY = 30;
            Point2f[] dstPoints =
            {
                new Point2f(paddingX, paddingY),
                new Point2f(src2.Cols - 1 - paddingX, paddingY),
                new Point2f(src2.Cols - 1 - paddingX, src2.Rows - 1 - paddingY),
                new Point2f(paddingX, src2.Rows - 1 - paddingY)
            };

            Mat perspectiveMatrix = Cv2.GetPerspectiveTransform(sortedCenters, dstPoints);
            inversePerspectiveMatrix = perspectiveMatrix.Inv();
            Cv2.WarpPerspective(src, result, perspectiveMatrix, new Size(src2.Cols, src2.Rows));

            return result;
        }

        private Point2f[] SortCenters(List<Point2f> centers)
        {
            if (centers.Count != 4)
                throw new ArgumentException("정렬을 위해서는 정확히 4개의 중심점이 필요합니다.");

            var sortedX = centers.OrderBy(p => p.X).ToArray();
            var left = sortedX.Take(2).OrderBy(p => p.Y).ToArray();
            var right = sortedX.Skip(2).OrderBy(p => p.Y).ToArray();

            return new Point2f[]
            {
                left[0],   // 좌상단
                right[0],  // 우상단
                right[1],  // 우하단
                left[1]    // 좌하단
            };
        }
    }
}
