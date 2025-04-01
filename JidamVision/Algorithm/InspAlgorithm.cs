using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using OpenCvSharp;
using JidamVision.Core;
using JidamVision.Teach;
using System.Security.Policy;

namespace JidamVision.Algorithm
{
    /*
    #ABSTRACT ALGORITHM# - <<<검사 알고리즘 추상화 개발>>> 
     */

    //#MATCH PROP#1 InspAlgorithm 클래스를 추가, 여러 알고리즘을 추상화하기 위함
    //추가 내용은 나중에 개발하고, 현재는 비어 있는 상태로 만들것

    //#MODEL SAVE#7 Xml Serialize를 위해서, 아래 코드 추가
    //XmlSerialize는 추상화된 상태를 알수 없어, 상속된 클래스를 명시적으로 포함해야 함.
    [XmlInclude(typeof(MatchAlgorithm))]
    [XmlInclude(typeof(CrackAlgorithm))]
    [XmlInclude(typeof(ScratchAlgorithm))]
    [XmlInclude(typeof(DentAlgorithm))]
    [XmlInclude(typeof(SootAlgorithm))]
    public abstract class InspAlgorithm
    {
        //#ABSTRACT ALGORITHM#1 검사 알고리즘을 추상화하여, 공통된 값이나, 함수 정의
        
        //알고리즘 타입 정의
        public InspectType InspectType { get; set; } = InspectType.InspNone;

        //알고지즘을 사용할지 여부 결정
        public bool IsUse { get; set; } = true;
        //검사가 완료되었는지를 판단
        public bool IsInspected { get; set; } = false;

        public Rect TeachRect { get; set; }
        public Rect InspRect { get; set; }

        public eImageChannel ImageChannel { get; set; } = eImageChannel.Gray;

        // align한 좌표값을 inv할 때
        protected Mat inversePerspectiveMatrix { get; set; } = null;

        //검사할 원본 이미지
        protected Mat _srcImage = null;

        public List<string> ResultString { get; set; }

        public bool IsDefect { get; set; }

        //검사에 필요한 정보를 설정, 각 알고리즘이 함수를 상속받아서, 필요한 정보를 추가로 입력받는다.
        public virtual void SetInspData(Mat srcImage)
        {
            _srcImage = srcImage;
        }

        //검사 함수로, 상속 받는 클래스는 필수로 구현해야한다.
        public abstract bool DoInspect();

        public virtual void ResetResult()
        {
            IsInspected = false;
            IsDefect = false;
            ResultString.Clear();
        }

        //검사 결과가 Rect정보로 출력이 가능하다면, 이 함수를 상속 받아서, 정보 반환
        public virtual int GetResultRect(out List<Rect> resultArea)
        {
            resultArea  = null;
            return 0;
        }



        protected Point2f perspectiveInverseTransform(Point2f point, Mat inverseMatrix)
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
