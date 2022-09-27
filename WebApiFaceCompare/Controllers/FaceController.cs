using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;
using DlibDotNet.Dnn;
using DlibDotNet;
using System.Drawing;
using System.Linq;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using OpenCvSharp.Dnn;
using System.Diagnostics;
using DlibDotNet.Extensions;

namespace WebApiFaceCompare.Controllers
{
    /// <summary>
    /// FaceController
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class FaceController : ControllerBase
    {
        /// <summary>
        /// TestCompare
        /// </summary>
        [HttpGet]
        public void TestCompare()
        {
            try
            {
                var sp = ShapePredictor.Deserialize("Resource/shape_predictor_5_face_landmarks.dat");
                var net = LossMetric.Deserialize("Resource/dlib_face_recognition_resnet_model_v1.dat");
                var detector = Dlib.GetFrontalFaceDetector();
                var imgurl1 = new[] { "https://gitee.com/laolaolulu/public/raw/master/L1.jpeg", "https://gitee.com/laolaolulu/public/raw/master/thumb_A3.jpeg", "https://gitee.com/laolaolulu/public/raw/master/thumb_A7.jpeg", "https://gitee.com/laolaolulu/public/raw/master/thumb_L3.jpeg" };
                var imgurl2 = new[] { "https://gitee.com/laolaolulu/public/raw/master/L2.jpeg", "https://gitee.com/laolaolulu/public/raw/master/thumb_A4.jpeg", "https://gitee.com/laolaolulu/public/raw/master/thumb_A8.jpeg", "https://gitee.com/laolaolulu/public/raw/master/thumb_L4.jpeg" };
                for (int i = 0; i < imgurl1.Length; i++)
                {
                    Matrix<RgbPixel>[] faces = new Matrix<RgbPixel>[2];
                    var t1 = Task.Run(() =>
                    {
                        var req = WebRequest.CreateHttp(imgurl1[i]);
                        using var ss1 = req.GetResponse().GetResponseStream();
                        using var ss2 = (Bitmap)Image.FromStream(ss1);
                        var img2 = ss2.ToMatrix<RgbPixel>();

                        // Dlib.PyramidUp(img2);
                        var s21 = img2.Clone().ToArray();
                            var dets2 = detector.Operator(s21);
                            foreach (var face in dets2)
                            {
                                var shape = sp.Detect(img2, face);
                                var faceChipDetail = Dlib.GetFaceChipDetails(shape, 150, 0.25);
                                faces[0] = Dlib.ExtractImageChip<RgbPixel>(img2, faceChipDetail);
                            }
                        

                    });

                    var t2 = Task.Run(() =>
                    {
                        var req1 = WebRequest.CreateHttp(imgurl2[i]);
                        using var stre = req1.GetResponse().GetResponseStream();
                        using var stre2 = (Bitmap)Image.FromStream(stre);
                        using (var img21 = stre2.ToMatrix<RgbPixel>())
                        {
                            //Dlib.PyramidUp(img21);
                            var dets21 = detector.Operator(img21.Clone());
                            foreach (var face1 in dets21)
                            {
                                var shape1 = sp.Detect(img21, face1);
                                var faceChipDetail1 = Dlib.GetFaceChipDetails(shape1, 150, 0.25);
                                faces[1] = Dlib.ExtractImageChip<RgbPixel>(img21, faceChipDetail1);
                            }
                        }
                    });


                    Task.WaitAll(t1, t2);
                    if (faces[0] != null && faces[1] != null)
                    {
                        var faceDescriptors = net.Operator(faces);
                        var diff = faceDescriptors[0] - faceDescriptors[1];
                        var desnum = Dlib.Length(diff);
                        Debug.WriteLine(string.Format("{0}({1:N2})", (desnum < 0.5).ToString(), desnum));
                    }
                    if (i == 3)
                    {
                        i = 0;
                    }
                }
            }
            catch (System.Exception e)
            {

                throw;
            }


        }

        /// <summary>
        /// Face Compare
        /// </summary>
        /// <param name="image">image file</param>
        /// <param name="imgUrl">web image url</param>
        /// <returns></returns>
        [HttpPost]
        public string Compare(IFormFile image, string imgUrl = "https://gitee.com/laolaolulu/public/raw/master/20220920192624.jpg")
        {
            using var sp = ShapePredictor.Deserialize("Resource/shape_predictor_5_face_landmarks.dat");
            using var net = LossMetric.Deserialize("Resource/dlib_face_recognition_resnet_model_v1.dat");
            using var detector = Dlib.GetFrontalFaceDetector();

            Matrix<RgbPixel>[] faces = new Matrix<RgbPixel>[2];

            var t1 = Task.Run(() =>
            {
                using var img1 = ((Bitmap)Image.FromStream(image.OpenReadStream())).ToMatrix<RgbPixel>();
                var dets1 = detector.Operator(img1);
                foreach (var face in dets1)
                {
                    var shape = sp.Detect(img1, face);
                    var faceChipDetail = Dlib.GetFaceChipDetails(shape, 150, 0.25);
                    faces[0] = Dlib.ExtractImageChip<RgbPixel>(img1, faceChipDetail);
                }
            });

            var t2 = Task.Run(() =>
            {
                var req = WebRequest.CreateHttp(imgUrl);
                using var img2 = ((Bitmap)Image.FromStream(req.GetResponse().GetResponseStream())).ToMatrix<RgbPixel>();
                var dets2 = detector.Operator(img2);
                foreach (var face in dets2)
                {
                    var shape = sp.Detect(img2, face);
                    var faceChipDetail = Dlib.GetFaceChipDetails(shape, 150, 0.25);
                    faces[1] = Dlib.ExtractImageChip<RgbPixel>(img2, faceChipDetail);
                }
            });


            Task.WaitAll(t1, t2);

            if (faces[0] != null && faces[1] != null)
            {
                var faceDescriptors = net.Operator(faces);
                var diff = faceDescriptors[0] - faceDescriptors[1];
                var desnum = Dlib.Length(diff);
                return string.Format("{0}({1:N2})", (desnum < 0.5).ToString(), desnum);
            }

            return "Error";
        }

        /// <summary>
        /// Scan barcode QR code
        /// </summary>
        /// <param name="image">image file</param>
        /// <returns></returns>
        [HttpPost]
        public object Scan(IFormFile image)
        {
            using var img = (System.DrawingCore.Bitmap)System.DrawingCore.Image.FromStream(image.OpenReadStream());
            //// create a barcode reader instance
            var reader = new ZXing.ZKWeb.BarcodeReader();
            var result = reader.DecodeMultiple(img);
            // do something with the result
            if (result != null)
            {
                return result.Select(s => new
                {
                    type = s.BarcodeFormat.ToString(),
                    text = s.Text
                });
            }
            return "Error";
        }

        /// <summary>
        /// create work card
        /// </summary>
        /// <param name="image">photo</param>
        /// <param name="name">name</param>
        /// <param name="code">No</param>
        /// <returns></returns>
        [HttpPost]
        public string CreateCard(IFormFile image, string name = "Sachin", string code = "123456")
        {
            try
            {
                using var photo = Mat.FromStream(image.OpenReadStream(), ImreadModes.Color).Resize(new OpenCvSharp.Size(470, 560));

                var bgimg = Cv2.ImRead("Resource/MedantaIdCardFront.PNG");
                using var roi = new Mat(bgimg, new Rect(new OpenCvSharp.Point(bgimg.Width / 2 - photo.Width / 2, 510), photo.Size()));
                photo.CopyTo(roi);


                var codewrit = new ZXing.BarcodeWriter
                {
                    Format = ZXing.BarcodeFormat.QR_CODE,
                    Options = new ZXing.Common.EncodingOptions()
                    {
                        Margin = 1,
                        Height = 300,
                        Width = 300
                    }
                };

                using var qr = codewrit.Write(new { name, code }.ToString()).ToMat();
                using var qrroi = new Mat(bgimg, new Rect(new OpenCvSharp.Point(880, 1510), qr.Size()));
                qr.CopyTo(qrroi);



                var barwrit = new ZXing.BarcodeWriter
                {
                    Format = ZXing.BarcodeFormat.CODE_128,
                    Options = new ZXing.Common.EncodingOptions()
                    {
                        Margin = 1,
                        Height = 100,
                        Width = 300
                    }
                };

                using var bar = barwrit.Write(code).ToMat();
                using var barroi = new Mat(bgimg, new Rect(new OpenCvSharp.Point(450, 1700), bar.Size()));
                bar.CopyTo(barroi);


                var tsize = Cv2.GetTextSize(name, HersheyFonts.HersheySimplex, 2.7, 1, out int basel);
                bgimg.PutText(name, new OpenCvSharp.Point(bgimg.Width / 2 - tsize.Width / 2, 1230), HersheyFonts.HersheySimplex, 2.7, new Scalar(0, 0, 0), 8);
                bgimg.PutText(code, new OpenCvSharp.Point(450, 1640), HersheyFonts.HersheySimplex, 2, new Scalar(0, 0, 0), 4);

                var imgurl = string.Format("wwwroot/WorkCard/{0}.jpg", code);
                Cv2.ImWrite(imgurl, bgimg);

                //Task.Run(() =>
                //{
                //    while (true)
                //    {
                //        Cv2.ImShow("img32", bgimg.Resize(new OpenCvSharp.Size(bgimg.Width/4, bgimg.Height/4)));
                //        Cv2.WaitKey(100);
                //    }
                //});

                return string.Format("{0}{1}", Request.Host.Value, imgurl.TrimStart("wwwroot".ToArray()));
            }
            catch (System.Exception e)
            {
                return "Error:" + e.Message;
            }
        }
    }
}
