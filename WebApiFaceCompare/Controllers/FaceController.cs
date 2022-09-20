using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenCvSharp;

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
        /// <param name="imgUrl">web image url</param>
        /// <param name="imgBase64">base64 image</param>
        /// <returns></returns>
        [HttpPost]
        public object Compare(string imgUrl="2", string imgBase64="3")
        {
            using var cap = new VideoCapture(imgUrl);
            using var img1 = cap.RetrieveMat();
           // using var img2 =Cv2.ImDecode();
            return 0;
        }
    }
}
