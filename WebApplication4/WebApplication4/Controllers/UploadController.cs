using IBM.Cloud.SDK.Core.Authentication.Iam;
using IBM.Watson.SpeechToText.v1;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;

namespace WebApplication4.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class UploadController : Controller
    {
        private IHostingEnvironment _hostingEnvironment;

        public UploadController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpPost, DisableRequestSizeLimit]
        public ActionResult UploadFile()
        {
            try
            {
                string sd = string.Empty;
                //System.IO.File.ReadAllBytes("audio-file2.flac");
                var file = Request.Form.Files[0];
                string folderName = "Upload";
                //file.
                byte[] fileBytes;
                string webRootPath = _hostingEnvironment.WebRootPath;
                string newPath = Path.Combine(webRootPath, folderName);
                if (!Directory.Exists(newPath))
                {
                    Directory.CreateDirectory(newPath);
                }
                if (file.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        file.CopyTo(ms);
                        fileBytes = ms.ToArray();
                       // string s = Convert.ToBase64String(fileBytes);
                        // act on the Base64 data
                    }

                    IamAuthenticator authenticator = new IamAuthenticator(
                 apikey: "Z0HoP-EE0_mV1d93s1BMlq2bhxcn4qsggaPHYpHbtkJo"
                        );


                    SpeechToTextService speechToText = new SpeechToTextService(authenticator);
                    speechToText.SetServiceUrl("https://api.eu-gb.speech-to-text.watson.cloud.ibm.com/instances/280a18df-3c6b-4b2a-a8ca-890982952740");


                    speechToText.DisableSslVerification(true);

                    var result = speechToText.Recognize(
                    audio: fileBytes, //System.IO.File.ReadAllBytes("audio-file2.flac"),
                    //contentType: "audio/flac",
                    wordAlternativesThreshold: 0.9f,
        keywords: new List<string>()
        {
        "colorado",
        "tornado",
        "tornadoes"
        },
        keywordsThreshold: 0.5f
        );



                    var obj = JObject.Parse(result.Response);
                     sd = (string)obj["results"][0]["alternatives"][0]["transcript"];
                    //string fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    //string fullPath = Path.Combine(newPath, fileName);
                    //using (var stream = new FileStream(fullPath, FileMode.Create))
                    //{
                    //    file.CopyTo(stream);
                    //}
                    var obj2 = new Sample();
                    obj2.input_type = "text";
                    obj2.input_data = "My name is Kartheek. I like to hear people calling me Kartheek." +
                        " some people call me Kartheek. some call as Satya, some call as Durga, some call as Jagadeeswar.";
                    obj2.summary_type = "general_summary";
                    obj2.N = 2;
                    var client = new RestClient("https://unfound-text-summarization-v1.p.rapidapi.com/summarization");
                    var request = new RestRequest(Method.POST);
                    request.AddHeader("x-rapidapi-host", "unfound-text-summarization-v1.p.rapidapi.com");
                    request.AddHeader("x-rapidapi-key", "b55ec18f6bmsh03c9b4753ede959p1aa268jsn41bfb689d2ed");
                    request.AddHeader("content-type", "application/json");
                    request.AddHeader("accept", "application/json");
                    request.AddParameter("application/json", 
                        obj2,
                        ParameterType.RequestBody);
                    IRestResponse response = client.Execute(request);

                    // "{\"input_data\": \"My name is Kartheek. I like to hear people calling me Kartheek. some people call me Kartheek. some call as Satya, some call as Durga, some call as Jagadeeswar.\",\"input_type\": \"text\",\"summary_type\": \"general_summary\",\"N\": 2}", 


                }
                return Json(sd);
            }
            catch (System.Exception ex)
            {
                return Json("Upload Failed: " + ex.Message);
            }
        }
    }

    public class Sample
    {
        public string input_data { get; set; }
        public string input_type { get; set; }
        public string summary_type { get; set; }
        public int N { get; set; }


        //input_data
        //    "My name is Kartheek. I like to hear people calling me Kartheek. some people call me Kartheek. some call as Satya, some call as Durga, some call as Jagadeeswar."
        //  input_type
        //    "text"
        //    summary_type
        //    "general_summary"
        //    N
        //    2, ParameterType.RequestBody);
                    
    }
}