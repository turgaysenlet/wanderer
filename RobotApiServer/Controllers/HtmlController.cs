using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Xml.Linq;

namespace ApiTest2.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HtmlController : ControllerBase
    {
        private readonly ILogger<HtmlController> _logger;

        public HtmlController(ILogger<HtmlController> logger)
        {
            _logger = logger;
        }

       
        [HttpGet(Name = "Html")]
        public ContentResult GetHtml()
        {
            return new ContentResult
            {
                ContentType = "text/html",
                StatusCode = (int)HttpStatusCode.OK,
                Content = CreateContent()
            };
        }

        private string CreateContent()
        {
            var htmlStr =
            @"<table width=""100%"" align=""center"" cellpadding=""2"" cellspacing=""2"" border=""0"" bgcolor=""#EAEAEA"" >
            <tr align=""""left"""" style=""""background-color:#004080;color:White;"""" >
            <td> ID </td>                        
            <td> Name </td>            
            <td>Pass</td>                        
         </tr>";

            htmlStr += "<tr><td>" + "1" + "</td><td>" + "a" + "</td><td>" + "x" + "</td></tr>";
            htmlStr += "<tr><td>" + "2" + "</td><td>" + "b" + "</td><td>" + "y" + "</td></tr>";

            htmlStr += "</table>";

            return "<html><body>" + htmlStr + "</body></html>";
        }
    }
}