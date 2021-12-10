using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.IO;

namespace podcast_project
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseKestrel() //Kestrel 是 ASP.NET Core 的跨平台網頁伺服器
                        .UseContentRoot(Directory.GetCurrentDirectory())  //獲取專案根目錄
                        .UseUrls("http://*:5000")  //表示伺服器應接聽任何 IP 位址或主機名稱上的要求
                        .UseStartup<Startup>();  //呼叫Startup類別
                });
    }
}
