using SaigeVision.Net.V2;
using SaigeVision.Net.V2.Detection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using JidamVision.Core;
using OpenCvSharp;
using System.Windows.Markup;

namespace JidamVision.Inspect
{
    public class InspSaige
    {

        public InspectType InspectType { get; set; } = InspectType.InspNone;
        private DetectionEngine engine { get; set; } = null;
        private string _file { get; set; } = null;
        private Mat srcImage { get; set; } = null;
        private SrImage srImage { get; set; } = null;

        private bool IsInspected = false;

        private List<Rect> _findArea = new List<Rect>();


        public InspSaige()
        {
            InspectType = InspectType.InspSaige;
        }

        //mainform load될때 실행
        public void InitializeSaige()
        {
            string modelPath = @"D:\\MarchVision_F\\ExternalLib\\PCBDetectModel.saigedet";
            engine = new DetectionEngine(modelPath, 0);
        }


        // ImageSpace로부터 직접 이미지 받아와서 설정
        public void SetSrcImage(ImageSpace imageSpace, int index = 0)
        {
            int width = imageSpace.Width;
            int height = imageSpace.Height;
            int channelSize = imageSpace.PixelBpp / 8; // bpp → channelSize (예: 24bpp → 3)

            if (channelSize != 1 && channelSize != 3)
                throw new NotSupportedException("Only 8bpp (Gray) or 24bpp (Color) supported");

            IntPtr dataPtr = imageSpace.GetnspectionBufferPtr(index);

            srImage = new SrImage(width, height, channelSize, dataPtr);
            //Console.WriteLine("set saige srcimage");

        }


        public void doInspect(ImageSpace imageSpace, int index = 0)
        {
            int width = imageSpace.Width;
            int height = imageSpace.Height;
            int channelSize = imageSpace.PixelBpp / 8; // bpp → channelSize (예: 24bpp → 3)

            if (channelSize != 1 && channelSize != 3)
                throw new NotSupportedException("Only 8bpp (Gray) or 24bpp (Color) supported");

            IntPtr dataPtr = imageSpace.GetnspectionBufferPtr(index);

            srImage = new SrImage(width, height, channelSize, dataPtr);

            if (srImage == null) return;

            if (_findArea is null)
                _findArea = new List<Rect>();

            _findArea.Clear();

            //inspection
            engine.Inspection(srImage);

            ModelInfo modelInfo = engine.GetModelInfo();

            Dictionary<string, OpenCvSharp.Scalar> classColors = new Dictionary<string, OpenCvSharp.Scalar>
            {
                { "Dent", new OpenCvSharp.Scalar(85, 253, 255) },       // 노란색
                { "Scratch", new OpenCvSharp.Scalar(245, 35, 0) },    // 파란색
                { "Crack", new OpenCvSharp.Scalar(255, 255, 255) },      // 흰색
                { "Soot", new OpenCvSharp.Scalar(245, 43, 115) }     // 보라색 (기본값)
            };

            DetectionResult result = engine.GetResult();
            int detectedcount = result.DetectedObjects.Length;

            string DetectResult = "Detected Num. of Objects : " + detectedcount;
            Console.WriteLine(DetectResult);

            for (int i = 0; i < detectedcount; i++)
            {

                IsInspected = true;
                DetectedObject detObj = result.DetectedObjects[i];

                DetectResult += "\n" + "X: " + detObj.BoundingBox.X + "\n" + "Y: " + detObj.BoundingBox.Y + "\n";
                DetectResult += "Width: " + detObj.BoundingBox.Width + "\n" + "Height: " + detObj.BoundingBox.Height + "\n";
                DetectResult += "Class: " + detObj.ClassInfo.Name + "\n" + "Score :" + detObj.Score + "\n" + "Area :" + detObj.Area + "\n";


                // 해당 클래스의 색상 찾기 (없으면 기본값 사용)
                OpenCvSharp.Scalar roiColor = classColors.ContainsKey(detObj.ClassInfo.Name)
                    ? classColors[detObj.ClassInfo.Name]
                    : classColors["Other"];

                // ROI 박스 그리기
                OpenCvSharp.Rect roiRect = new OpenCvSharp.Rect(
                    Convert.ToInt32(detObj.BoundingBox.X),
                    Convert.ToInt32(detObj.BoundingBox.Y),
                    Convert.ToInt32(detObj.BoundingBox.Width),
                    Convert.ToInt32(detObj.BoundingBox.Height)
                );

                _findArea.Add(roiRect);
                //mat.Rectangle(roiRect, roiColor, 4);
            }

            srImage.Dispose();
        }

        public int GetResultRect(out List<Rect> resultArea)
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
