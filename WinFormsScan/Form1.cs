using OpenCvSharp;
using System;
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
    }
}
