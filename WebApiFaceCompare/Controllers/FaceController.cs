using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenCvSharp;

namespace WebApiFaceCompare.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class FaceController : ControllerBase
    {
        [HttpPost]
        public object Compare(string imgUrl, string imgBase64)
        {
            using var cap = new VideoCapture(imgUrl);
            using var img1 = cap.RetrieveMat();
           // using var img2 =Cv2.ImDecode();
            return 0;
        }
    }
}
