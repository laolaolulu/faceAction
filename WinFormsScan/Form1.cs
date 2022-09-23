using OpenCvSharp;
using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace WinFormsScan
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        bool isread = false;
        private void button1_Click(object sender, EventArgs e)
        {
            isread = true;
            using VideoCapture cap = new VideoCapture(0);
            while (isread)
            {
                using var frameMat = cap.RetrieveMat();

                var openCVReader = new ZXing.OpenCV.BarcodeReader();
                var openCVResult = openCVReader.DecodeMultiple(frameMat);
                if (openCVResult != null)
                {
                    foreach (var item in openCVResult)
                    {
                        //var ps = item.ResultPoints;
                        //for (int i = 0; i < ps.Length; i++)
                        //{
                        //    frameMat.Line((int)ps[i].X, (int)ps[i].Y, (int)ps[(i + 1) % 4].X, (int)ps[(i + 1) % 4].Y, new Scalar(255, 255, 0), 1, LineTypes.Link8);
                        //};
                        textBox1.AppendText(string.Format("{0}  {1}\r\n", item.BarcodeFormat.ToString(), item.Text));
                    }
                    Cv2.WaitKey(5000);
                }
                Cv2.ImShow("img", frameMat);

                Cv2.WaitKey(50);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            isread = false;
            Cv2.DestroyAllWindows();
        }
        bool save = false;
        private void button3_Click(object sender, EventArgs e)
        {
            isread = true;

            var face_detect = new CascadeClassifier("Resource/lbpcascade_frontalface_improved.xml");
            using VideoCapture cap = new VideoCapture(0);
            using var hog = new HOGDescriptor();
            hog.SetSVMDetector(HOGDescriptor.GetDefaultPeopleDetector());
            while (isread)
            {
                var frameMat = cap.RetrieveMat();
                var image = frameMat.Clone();

                var t1 = Task.Run(() =>
                  {
                      var peoples = hog.DetectMultiScale(image);
                      frameMat.PutText(string.Format("Peoples: {0}", peoples.Length), new OpenCvSharp.Point(10, 30), HersheyFonts.HersheySimplex, 0.7, new Scalar(255, 0, 0), 2);
                      foreach (var item in peoples)
                      {
                          frameMat.Rectangle(item, new Scalar(255, 0, 0));
                      }
                  });

                var t2 = Task.Run(() =>
                   {
                       var faces = face_detect.DetectMultiScale(image);
                       frameMat.PutText(string.Format("Faces: {0}", faces.Length), new OpenCvSharp.Point(300, 30), HersheyFonts.HersheySimplex, 0.7, new Scalar(0, 0, 255), 2);
                       foreach (var item in faces)
                       {
                           frameMat.Rectangle(item, new Scalar(0, 0, 255));
                       }
                       if (save)
                       {
                           if (faces.Length > 0)
                           {
                               var strfilename = DateTime.Now.ToString("yyyyMMddHHmmss");
                               if (!Directory.Exists("Resource/Faces"))
                               {
                                   Directory.CreateDirectory("Resource/Faces");
                               }
                               Cv2.ImWrite(string.Format("Resource/Faces/{0}.jpg", strfilename), image);

                               for (int i = 0; i < faces.Length; i++)
                               {
                                   if (!Directory.Exists(string.Format("Resource/Faces/{0}", strfilename)))
                                   {
                                       Directory.CreateDirectory(string.Format("Resource/Faces/{0}", strfilename));
                                   }
                                   var face = new Mat(image, new Rect(faces[i].X, faces[i].Y, faces[i].Width, faces[i].Y));
                                   Cv2.ImWrite(string.Format("Resource/Faces/{0}/face{1}.jpg", strfilename, i), face);
                               }
                               MessageBox.Show(string.Format("Save Success Path({0})", string.Format("Resource/Faces/{0}", strfilename)));
                           }
                           else
                           {
                               MessageBox.Show("Error:No face detected");
                           }
                           save = false;
                       }
                   });
                Task.WaitAll(t1, t2);
                Cv2.ImShow("img", frameMat);
                Cv2.WaitKey(50);

            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            save = true;
        }
    }
}
