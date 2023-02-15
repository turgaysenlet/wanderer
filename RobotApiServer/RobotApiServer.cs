using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Drawing;
using System.Net;
using System.Net.Mime;
using System.Text;
using Wanderer.Software.ImageProcessing;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using Image = SixLabors.ImageSharp.Image;
using System.Drawing.Imaging;
using Wanderer.Hardware;
using System.Net.Sockets;
using Wanderer.Software.Mapping;
using static Emgu.CV.Fuzzy.FuzzyInvoke;

namespace Wanderer.Software.Api
{
    public class RobotApiServer : ApiServerCls
    {
        public int Port { get; set; } = 5000;
        public string Address
        {
            get; set;
        }
        public RobotApiServer()
        {
            State = ModuleStateEnu.Created;
        }
        public IPAddress GetIpAddress()
        {
            string hostName = Dns.GetHostName();
            Console.WriteLine(hostName);
            // Get the IP from GetHostByName method of dns class.
            var ipAddress = Dns.GetHostByName(hostName).AddressList.Where(t => t.AddressFamily == AddressFamily.InterNetwork).First();
            Console.WriteLine("IP Address is : " + ipAddress);
            return ipAddress;
        }
        public void StartWithController()
        {
            string[] args = null;
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseAuthorization();
            app.MapControllers();
            State = ModuleStateEnu.Started;
            app.Run($"http://localhost:{Port}");
        }
       
        public void Start()
        {
            string[] args = null;
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthorization();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            WebApplication app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();

            RobotApiEndpoint(app);
            MapApiEndpoint(app);
            KaanApiEndpoint(app);
            EntitiesApiEndpoint(app);
            EntitiesWithIdApiEndpoint(app);
            DevicesApiEndpoint(app);
            DevicesWithIdApiEndpoint(app);
            ModulesApiEndpoint(app);
            ModulesWithIdApiEndpoint(app);
            CameraViewEndpoint(app);
            CameraDistanceEndpoint(app);
            MapViewEndpoint(app);
            SpeechApiEndpoint(app);
            Address = $"http://{GetIpAddress().ToString()}:{Port}";
            this.Name = $"{GetType().Name} on {Address}";
            Console.WriteLine(this.Name);
            State = ModuleStateEnu.Started;
            app.Run(Address);
        }

        private void MapViewEndpoint(WebApplication app)
        {
            app.MapGet("/maps/{id}", (HttpContext httpContext, string id) =>
            {
                MapCls map = ((MapCls)Wanderer.Software.EntityCls.Entities.Where(module => module.GetType() == typeof(MapCls)).FirstOrDefault());
                T264 t264 = ((T264)Wanderer.Hardware.Device.Devices.Where(device => device.GetType() == typeof(T264)).FirstOrDefault());
                if (t264 != null) {
                    map.Poses = t264.Poses;
                    var pose = t264.PosePositionOrientationDegrees();
                    map.LocationX = (float)pose[0];
                    map.LocationY = (float)pose[1];
                }
                D435 d435 = ((D435)Wanderer.Hardware.Device.Devices.Where(device => device.GetType() == typeof(D435)).FirstOrDefault());
                if (d435 != null)
                {
                    var color = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("CalculateOccupancyMap");
                    Console.ForegroundColor = color;
                    map.CalculateOccupancyMap(d435.Vertices);
                }
                Bitmap image = map.GenerateBitmap(1000, 1000);
                httpContext.Response.Headers.CacheControl = $"public,max-age={TimeSpan.FromSeconds(1).TotalSeconds}";
                MemoryStream stream = new MemoryStream();
                image.Save(stream, ImageFormat.Jpeg);
                stream.Position = 0;
                return Results.Stream(stream, "image/jpeg");
            });
        }

        private void SpeechApiEndpoint(WebApplication app)
        {
            app.MapGet("/speak/{text}", (HttpContext httpContext, string text) =>
            {
                Wanderer.Software.Speech.SpeechSynthesisServerCls.Instance.Speak(text);
                return text;// Results.Text(text);
            });
        }
        private void CameraDistanceEndpoint(WebApplication app)
        {
            app.MapGet("/distance", (HttpContext httpContext) =>
            {
                D435 d435 = ((D435)Wanderer.Hardware.Device.Devices.Where(device => device.GetType() == typeof(D435)).FirstOrDefault());
                var distance = d435.Distance();
                return distance;
            });
            //app.MapGet("/process-image/{strImage}", (string strImage, HttpContext http, CancellationToken token) =>
            //{
            //    http.Response.Headers.CacheControl = $"public,max-age={TimeSpan.FromHours(24).TotalSeconds}";
            //    return Results.Stream(stream => ResizeImageAsync(strImage, stream, token), "image/jpeg");
            //});
        }
        private void CameraViewEndpoint(WebApplication app)
        {
            app.MapGet("/cameras/{id}", (HttpContext httpContext, string id) =>
            {
                D435 d435 = ((D435)Wanderer.Hardware.Device.Devices.Where(device => device.GetType() == typeof(D435)).FirstOrDefault());
                Bitmap image = id == "0" ? d435.ColorBitmap : d435.DepthColorBitmap;
                httpContext.Response.Headers.CacheControl = $"public,max-age={TimeSpan.FromSeconds(1).TotalSeconds}";
                MemoryStream stream = new MemoryStream();
                image.Save(stream, ImageFormat.Jpeg);
                stream.Position = 0;
                return Results.Stream(stream, "image/jpeg");
            });
            //app.MapGet("/process-image/{strImage}", (string strImage, HttpContext http, CancellationToken token) =>
            //{
            //    http.Response.Headers.CacheControl = $"public,max-age={TimeSpan.FromHours(24).TotalSeconds}";
            //    return Results.Stream(stream => ResizeImageAsync(strImage, stream, token), "image/jpeg");
            //});
        }
        private async Task ResizeImageAsync(string strImage, Stream stream, CancellationToken token)
        {
            var strPath = $"wwwroot/img/{strImage}";
            using var image = await Image.LoadAsync(strPath, token);
            int width = image.Width / 2;
            int height = image.Height / 2;
            image.Mutate(x => x.Resize(width, height));
            await image.SaveAsync(stream, JpegFormat.Instance, cancellationToken: token);
        }
        private Service NewService(WebApplication app, string pattern, Func<HttpContext, string, object[]> func)
        {
            app.MapGet(pattern, func);
            return AddService(new Service(pattern));
        }
        private Service NewService(WebApplication app, string pattern, Func<HttpContext, object[]> func)
        {
            app.MapGet(pattern, func);
            return AddService(new Service(pattern));
        }

        private Service NewService(WebApplication app, string pattern, Func<IResult> func)
        {
            app.MapGet(pattern, func);
            return AddService(new Service(pattern));
        }

        private Service ModulesWithIdApiEndpoint(WebApplication app)
        {
            return NewService(app, "/modules/{id}", (HttpContext httpContext, string id) =>
            {
                return Wanderer.Software.ModuleCls.Modules.Where(module => module.ModuleNo.ToString() == id).ToArray();
            });
        }

        private Service DevicesWithIdApiEndpoint(WebApplication app)
        {
            return NewService(app, "/devices/{id}", (HttpContext httpContext, string id) =>
            {
                return Wanderer.Hardware.Device.Devices.Where(device => device.DeviceNo.ToString() == id).ToArray();
            });
        }

        private Service EntitiesWithIdApiEndpoint(WebApplication app)
        {
            return NewService(app, "/entities/{id}", (HttpContext httpContext, string id) =>
            {
                return Wanderer.Software.EntityCls.Entities.Where(entity => entity.EntityNo.ToString() == id).ToArray();
            });
        }
        private Service ModulesApiEndpoint(WebApplication app)
        {
            return NewService(app, "/modules", (HttpContext httpContext) =>
            {
                return Wanderer.Software.ModuleCls.Modules.Select(module => module).ToArray();
            });
        }

        private Service DevicesApiEndpoint(WebApplication app)
        {
            return NewService(app, "/devices", (HttpContext httpContext) =>
            {
                return Wanderer.Hardware.Device.Devices.Select(device => device).ToArray();
            });
        }

        private Service EntitiesApiEndpoint(WebApplication app)
        {
            return NewService(app, "/entities", (HttpContext httpContext) => 
            {
                return Wanderer.Software.EntityCls.Entities.Select(entity => entity).ToArray();
            });
        }

        private Service RobotApiEndpoint(WebApplication app)
        {
            return NewService(app, "/robot", () => Results.Extensions.Html(@$"<!doctype html>
            <html>
                <head><title>Robot at {Address}</title></head>
                <body>
                    <h1>Robot</h1>
                    <h2>Time</h2>
                    <p>The time on the server is {DateTime.Now:O}</p>
                    <h2>Entities</h2>
                    {CreateContentEntities()}
                    <h2>Devices</h2>
                    {CreateContentDevices()}
                    <h2>Modules</h2>
                    {CreateContentModules()}
                    <h2>Cameras</h2>
                    {CreateContentCamera(0, 320, 240, 100)}
                    {CreateContentCamera(1, 320, 240, 100)}
                    {CreateContentMap(2, 240, 240, 1000)}

            </body>
            </html>"));
        }
        private Service KaanApiEndpoint(WebApplication app)
        {
            return NewService(app, "/kaan", () => Results.Extensions.Html(@$"<!doctype html>
            <html>
                <head><title>Robot at {Address}</title></head>
                <body style=""background-color:rgb(21,32,43);"">
                    {CreateStyle()}
                    {CreateContentMap(2, 200, 200, 1000)}
                    {CreateContentCamera(0, 280, 200, 300)}
                    {CreateContentCamera(1, 280, 200, 300)}
                    {CreateRestGetButton("Hello", "/speak/Hello")}                
                    {CreateRestGetButton("Kaan", "/speak/Kaan")}
            </body>
            </html>"));
        }

        private object CreateStyle()
        {
            return @"<style>
                .button {
                  background-color: #4CAF50; /* Green */
                  border: none;
                  color: white;
                  padding: 15px 32px;
                  text-align: center;
                  text-decoration: none;
                  display: inline-block;
                  margin: 4px 2px;
                  cursor: pointer;
                }

                .button1 {font-size: 10px;}
                .button2 {font-size: 12px;}
                .button3 {font-size: 16px;}
                .button4 {font-size: 20px;width: 48%;}
                .button5 {font-size: 24px;}
                </style>";
        }

        private string CreateRestGetButton(string text, string url)
        {
            var functionName = text.Replace(" ", "");
            return $@"
                <button id='{functionName}'type='button' class=""button button4"" onclick=""function{functionName}()"">{text}</button>
                <script>
                async function function{functionName}() {{
                  const abc = await fetch('{url}');
                }}
                </script>";
        }

        private Service MapApiEndpoint(WebApplication app)
        {
            return NewService(app, "/map", () => Results.Extensions.Html(@$"<!doctype html>
            <html>
                <head><title>Robot at {Address}</title></head>
                <body>                    
                    <h1>Map</h1>
                    {CreateContentMap(2, 1200, 1200, 1000)}
                    {CreateContentCamera(0, 640, 480, 300)}
                    {CreateContentCamera(1, 640, 480, 300)}
            </body>
            </html>"));
        }
        private string CreateContentCamera(int cameraNo, int width, int height, int refreshMs)
        {
            //return $@"<div class=""split left"">
            //  <div class=""centered"">
            //    <h3>Color</h3>
            //    <img src=""/cameras/0"""" alt=""Color camera"">
            //  </div>
            //</div>

            //<div class=""split right"">
            //  <div class=""centered"">
            //    <h3>Depth</h3>
            //    <img src=""/cameras/01"" alt=""Depth camera"">
            //  </div>
            //</div>";
            return $@"<img src=""/cameras/{cameraNo}"" id=""reloader{cameraNo}"" alt=""Camera{cameraNo}"" onLoad=""setTimeout( () => 
            {{ document.getElementById('reloader{cameraNo}').src='/cameras/{cameraNo}' + '?' + new Date().getMilliseconds() }},{refreshMs})"" width=""{width}"" height=""{height}""/>";
        }

        private string CreateContentMap(int idNo, int width, int height, int refreshMs)
        {
            return $@"<img src=""/maps/{idNo}"" id=""reloader{idNo}"" alt=""Map{idNo}"" onLoad=""setTimeout( () => 
            {{ document.getElementById('reloader{idNo}').src='/maps/{idNo}' + '?' + new Date().getMilliseconds() }},{refreshMs})"" width=""{width}"" height=""{height}""/>";
        }

        public ContentResult GetHtml()
        {
            return new ContentResult
            {
                ContentType = "text/html",
                StatusCode = (int)HttpStatusCode.OK,
                Content = CreateContentEntities(),
            };
        }

        private string CreateContentEntities()
        {
            var htmlStr =
            @"<table width=""100%"" align=""center"" cellpadding=""2"" cellspacing=""2"" border=""0"" bgcolor=""#EAEAEA"" >
            <tr align=""""left"""" style=""""background-color:#004080;color:White;"""" >
                <b>
                    <td> <b>ID   <b/></td>                        
                    <td> <b>Name <b/></td>            
                    <td> <b>Type <b/></td>
                </b>
             </tr>";
            var entities = EntityCls.Entities;
            foreach (var entity in entities)
            {
                htmlStr += "<tr><td>" +entity.EntityNo + "</td><td>" + entity.Name + "</td><td>" + entity.GetType().Name + "</td></tr>";
            }
            htmlStr += "</table>";

            return "<html><body>" + htmlStr + "</body></html>";
        }
        private string CreateContentDevices()
        {
            var htmlStr =
            @"<table width=""100%"" align=""center"" cellpadding=""2"" cellspacing=""2"" border=""0"" bgcolor=""#EAEAEA"" >
            <tr align=""""left"""" style=""""background-color:#004080;color:White;"""" >
                 
                    <td><b> ID <b/> </td>                        
                    <td><b> Name <b/></td>            
                    <td><b> Type <b/></td>                        
                    <td><b> State<b/> </td>                        
             </tr>";
            var devices = Wanderer.Hardware.Device.Devices;
            foreach (var device in devices)
            {
                htmlStr += "<tr><td>" + device.DeviceNo + "</td>"+
                    "<td>" + device.Name + "</td>"+
                    "<td>" + device.DeviceType + "</td>" +
                    $"<td {StateColor(device.State)}>"  + device.State + "</td></tr>";
            }
            htmlStr += "</table>";

            return "<html><body>" + htmlStr + "</body></html>";
        }

        private string StateColor(Device.DeviceStateEnu state)
        {
            if (state == Device.DeviceStateEnu.Unknown ||
                state == Device.DeviceStateEnu.NotFound ||
                state == Device.DeviceStateEnu.Failed ||
                state == Device.DeviceStateEnu.Stopped )
            {
                return "bgcolor=\"pink\"";
            }
            else if (state == Device.DeviceStateEnu.Started)
            {
                return "bgcolor=\"green\"";
            }
            return "";
        }

        private string CreateContentModules()
        {
            var htmlStr =
            @"<table width=""100%"" align=""center"" cellpadding=""2"" cellspacing=""2"" border=""0"" bgcolor=""#EAEAEA"" >
            <tr align=""""left"""" style=""""background-color:#004080;color:White;"""" >
                    <td><b> ID <b/> </td>                        
                    <td><b> Name <b/></td>            
                    <td><b> State<b/> </td>                        
             </tr>";
            var modules = Wanderer.Software.ModuleCls.Modules;
            foreach (var module in modules)
            {
                htmlStr += "<tr><td>" + module.ModuleNo + "</td><td>" + module.Name + "</td><td>" + module.State + "</td></tr>";
            }
            htmlStr += "</table>";

            return "<html><body>" + htmlStr + "</body></html>";
        }
    }
    static class ResultsExtensions
    {
        public static IResult Html(this IResultExtensions resultExtensions, string html)
        {
            ArgumentNullException.ThrowIfNull(resultExtensions);

            return new HtmlResult(html);
        }
    }

    class HtmlResult : IResult
    {
        private readonly string _html;

        public HtmlResult(string html)
        {
            _html = html;
        }

        public Task ExecuteAsync(HttpContext httpContext)
        {
            httpContext.Response.ContentType = MediaTypeNames.Text.Html;
            httpContext.Response.ContentLength = Encoding.UTF8.GetByteCount(_html);
            return httpContext.Response.WriteAsync(_html);
        }
    }
}