using DlibDotNet;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsBlink
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // define two constants, one for the eye aspect ratio to indicate
            // blink and then a second constant for the number of consecutive
            // frames the eye must be below the threshold
            var EYE_AR_THRESH = 0.2;
            var EYE_AR_CONSEC_FRAMES = 1;
            // initialize the frame counters and the total number of blinks
            var COUNTER = 0;
            var TOTAL = 0;

            VideoCapture cap = new VideoCapture(0);
            using (var detector = Dlib.GetFrontalFaceDetector())
            // 加载人脸68特征点检测模型
            using (var sp = ShapePredictor.Deserialize("Resource/shape_predictor_68_face_landmarks.dat"))
                while (cap.IsOpened())
                {
                    using var frameMat = cap.RetrieveMat();
                    byte[] array = new byte[frameMat.Width * frameMat.Height * frameMat.ElemSize()];
                    Marshal.Copy(frameMat.Data, array, 0, array.Length);

                    using var cimg = Dlib.LoadImageData<RgbPixel>(array, (uint)frameMat.Height, (uint)frameMat.Width, (uint)(frameMat.Width * frameMat.ElemSize()));
                    var dets = detector.Operator(cimg);

                    foreach (var rect in dets)
                    {
                        // 画出检测到的脸的矩形框
                        frameMat.Rectangle(new Rect(rect.Left, rect.Top, (int)rect.Width, (int)rect.Height), new Scalar(255, 0, 0));
                        var shape = sp.Detect(cimg, rect);
                        var eye1 = new Vector2[6];
                        var eye2 = new Vector2[6];
                        for (uint i = 0; i < shape.Parts; i++)
                        {
                            var point = shape.GetPart(i);
                            // 获取第一只眼睛
                            if (i >= 36 && i < 42)
                            {
                                eye1[i - 36] = new Vector2(point.X, point.Y);
                            }
                            // 获取第二只眼睛
                            if (i >= 42 && i < 48)
                            {
                                eye2[i - 42] = new Vector2(point.X, point.Y);
                            }

                            // 画出检测到的脸的68特征点
                            frameMat.Circle(point.X, point.Y, 1, new Scalar(0, 255, 0));
                            //  frameMat.PutText(i.ToString(), new Point(point.X, point.Y), HersheyFonts.HersheySimplex, 0.3, new Scalar(0, 255, 0));
                        }

                        var EAR1 = eyeAspectRatio(eye1);
                        var EAR2 = eyeAspectRatio(eye2);
                        // average the eye aspect ratio together for both eyes
                        var ear = (EAR1 + EAR2) / 2.0;
                        frameMat.PutText(string.Format("EAR: {0:N2}", ear), new OpenCvSharp.Point(300, 30), HersheyFonts.HersheySimplex, 0.7, new Scalar(0, 0, 255), 2);
                        // check to see if the eye aspect ratio is below the blink
                        // threshold, and if so, increment the blink frame counter
                        if (ear < EYE_AR_THRESH)
                        {
                            COUNTER += 1;
                            Debug.WriteLine(ear);
                        }
                        // otherwise, the eye aspect ratio is not below the blink
                        // threshold
                        else
                        {
                            //if the eyes were closed for a sufficient number of
                            // then increment the total number of blinks
                            if (COUNTER >= EYE_AR_CONSEC_FRAMES)
                                TOTAL += 1;
                            // reset the eye frame counter
                            COUNTER = 0;
                        }
                    }

                    frameMat.PutText(string.Format("Blinks: {0}", TOTAL), new OpenCvSharp.Point(10, 30), HersheyFonts.HersheySimplex, 0.7, new Scalar(0, 0, 255), 2);

                    Cv2.ImShow("img", frameMat);
                    // Blink 3 times to save
                    if (TOTAL == 3)
                    {
                        frameMat.ImWrite(string.Format("Resource/{0}.jpg", DateTime.Now.ToString("yyyyMMddHHmmss")));
                        TOTAL = 0;
                    }

                    Cv2.WaitKey(50);
                }

           
        }

        static double eyeAspectRatio(Vector2[] eye)
        {
            // compute the euclidean distances between the two sets of
            // vertical eye landmarks (x, y)-coordinates
            var A = Vector2.Distance(eye[1], eye[5]);
            var B = Vector2.Distance(eye[2], eye[4]);
            // compute the euclidean distance between the horizontal
            // eye landmark (x, y)-coordinates
            var C = Vector2.Distance(eye[0], eye[3]);
            // compute the eye aspect ratio
            var ear = (A + B) / (2.0 * C);
            // return the eye aspect ratio
            return ear;
        }
    }
}
