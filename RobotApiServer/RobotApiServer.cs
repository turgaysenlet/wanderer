using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Drawing;
using System.Net;
using System.Net.Mime;
using System.Text;
using Wanderer.Software;
using Wanderer.Software.ImageProcessing;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using Image = SixLabors.ImageSharp.Image;
using System.Drawing.Imaging;
using Wanderer.Hardware;

namespace Wanderer.Software.Api
{
    public class RobotApiServer : ApiServer
    {
        public int Port { get; set; } = 5000;
        public string Address
        {
            get
            {
                //return $"http://localhost:{Port}";
                return $"http://192.168.1.102:{Port}";
            }
        }
        public RobotApiServer()
        {
            State = ModuleStateEnu.Created;
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
            EntitiesApiEndpoint(app);
            EntitiesWithIdApiEndpoint(app);
            DevicesApiEndpoint(app);
            DevicesWithIdApiEndpoint(app);
            ModulesApiEndpoint(app);
            ModulesWithIdApiEndpoint(app);

            CameraViewEndpoint(app);

            this.Name = $"{GetType().Name} on {Address}";
            State = ModuleStateEnu.Started;
            app.Run(Address);
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

        private void ModulesWithIdApiEndpoint(WebApplication app)
        {
            app.MapGet("/modules/{id}", (HttpContext httpContext, string id) =>
            {
                return Wanderer.Software.Module.Modules.Where(module => module.ModuleNo.ToString() == id).ToArray();
            });
        }

        private void DevicesWithIdApiEndpoint(WebApplication app)
        {
            app.MapGet("/devices/{id}", (HttpContext httpContext, string id) =>
            {
                return Wanderer.Hardware.Device.Devices.Where(device => device.DeviceNo.ToString() == id).ToArray();
            });
        }

        private void EntitiesWithIdApiEndpoint(WebApplication app)
        {
            app.MapGet("/entities/{id}", (HttpContext httpContext, string id) =>
            {
                return Wanderer.Software.Entity.Entities.Where(entity => entity.EntityNo.ToString() == id).ToArray();
            });
        }
        private void ModulesApiEndpoint(WebApplication app)
        {
            app.MapGet("/modules", (HttpContext httpContext) =>
            {
                return Wanderer.Software.Module.Modules.Select(module => module).ToArray();
            });
        }

        private void DevicesApiEndpoint(WebApplication app)
        {
            app.MapGet("/devices", (HttpContext httpContext) =>
            {
                return Wanderer.Hardware.Device.Devices.Select(device => device).ToArray();
            });
        }

        private void EntitiesApiEndpoint(WebApplication app)
        {
            app.MapGet("/entities", (HttpContext httpContext) => 
            {
                return Wanderer.Software.Entity.Entities.Select(entity => entity).ToArray();
            });
        }

        private void RobotApiEndpoint(WebApplication app)
        {
            app.MapGet("/robot", () => Results.Extensions.Html(@$"<!doctype html>
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
                    {CreateContentCameras()}
            </body>
            </html>"));
        }

        private object CreateContentCameras()
        {
            return $@"<div class=""split left"">
              <div class=""centered"">
                <h3>Color</h3>
                <img src=""/cameras/0"""" alt=""Color camera"">
              </div>
            </div>

            <div class=""split right"">
              <div class=""centered"">
                <h3>Depth</h3>
                <img src=""/cameras/01"" alt=""Depth camera"">
              </div>
            </div>";
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
            var entities = Entity.Entities;
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
            var modules = Wanderer.Software.Module.Modules;
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