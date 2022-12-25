using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net;
using System.Net.Mime;
using System.Text;
using Wanderer.Software;
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
            </body>
            </html>"));
            this.Name = $"{GetType().Name} on {Address}";
            app.Run(Address);
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
                htmlStr += "<tr><td>" + device.DeviceNo + "</td><td>" + device.Name + "</td><td>" + device.DeviceType + "</td><td>" + device.State + "</td></tr>";
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