using DlibDotNet;
using DlibDotNet.Dnn;
using OpenCvSharp;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace WinFormsBlink
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        ShapePredictor sp;
        LossMetric net;
        FrontalFaceDetector detector;
        private void Form1_Load(object sender, EventArgs e)
        {
            // 加载人脸68特征点检测模型
            sp = ShapePredictor.Deserialize("Resource/shape_predictor_68_face_landmarks.dat");

            net = LossMetric.Deserialize("Resource/dlib_face_recognition_resnet_model_v1.dat");

            detector = Dlib.GetFrontalFaceDetector();
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


        bool isread = false;
        private void button1_Click(object sender, EventArgs e)
        {
            FullObjectDetection shape = null;
            //  faceDescriptors = null;
            isread = true;
            // define two constants, one for the eye aspect ratio to indicate
            // blink and then a second constant for the number of consecutive
            // frames the eye must be below the threshold
            var EYE_AR_THRESH = 0.2;
            var EYE_AR_CONSEC_FRAMES = 1;
            // initialize the frame counters and the total number of blinks
            var COUNTER = 0;
            var TOTAL = 0;

            using VideoCapture cap = new VideoCapture(0);

            while (isread)
            {
                using var frameMat = cap.RetrieveMat();
                var saveimg = frameMat.Clone();
                byte[] array = new byte[frameMat.Width * frameMat.Height * frameMat.ElemSize()];
                Marshal.Copy(frameMat.Data, array, 0, array.Length);

                using var cimg = Dlib.LoadImageData<RgbPixel>(array, (uint)frameMat.Height, (uint)frameMat.Width, (uint)(frameMat.Width * frameMat.ElemSize()));
                var dets = detector.Operator(cimg);

                foreach (var face in dets)
                {
                    shape = sp.Detect(cimg, face);

                    // 画出检测到的脸的矩形框
                    frameMat.Rectangle(new Rect(face.Left, face.Top, (int)face.Width, (int)face.Height), new Scalar(255, 0, 0));

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
                    saveimg.ImWrite(string.Format("Resource/{0}.jpg", DateTime.Now.ToString("yyyyMMddHHmmss")));
                    TOTAL = 0;
                    img.Image = new Bitmap(saveimg.Cols, saveimg.Rows, (int)saveimg.Step(), System.Drawing.Imaging.PixelFormat.Format24bppRgb, saveimg.Data);
                    isread = false;
                    Cv2.DestroyAllWindows();

                    if (shape != null)
                    {
                        var faceChipDetail = Dlib.GetFaceChipDetails(shape, 150, 0.25);
                        var faceChip = Dlib.ExtractImageChip<RgbPixel>(cimg, faceChipDetail);
                        faces[0] = new Matrix<RgbPixel>(faceChip);
                    }
                }

                Cv2.WaitKey(50);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            isread = false;
            Cv2.DestroyAllWindows();
        }
        Matrix<RgbPixel>[] faces = new Matrix<RgbPixel>[2];
        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            DialogResult result = fileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                pictureBox1.Image = Image.FromFile(fileDialog.FileName);
                var img = Dlib.LoadImageAsMatrix<RgbPixel>(fileDialog.FileName);
                var dets = detector.Operator(img);

                foreach (var face in dets)
                {
                    var shape = sp.Detect(img, face);
                    var faceChipDetail = Dlib.GetFaceChipDetails(shape, 150, 0.25);
                    faces[1] = Dlib.ExtractImageChip<RgbPixel>(img, faceChipDetail);
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (faces[0] != null && faces[1] != null)
            {
                var faceDescriptors = net.Operator(faces);
                var diff = faceDescriptors[0] - faceDescriptors[1];
                var desnum = Dlib.Length(diff);
                //if (Dlib.Length(diff) < 0.6)
                MessageBox.Show(String.Format("{0}({1:N2})", (desnum < 0.6).ToString(), desnum));
            }
        }
    }
}
