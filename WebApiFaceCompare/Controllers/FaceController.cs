using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;
using DlibDotNet.Dnn;
using DlibDotNet;
using DlibDotNet.Extensions;
using System.Drawing;
using System.Linq;

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


    }
}
