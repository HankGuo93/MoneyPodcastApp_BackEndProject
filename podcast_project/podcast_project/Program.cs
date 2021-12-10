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
                    webBuilder.UseKestrel() //Kestrel �O ASP.NET Core ���󥭥x�������A��
                        .UseContentRoot(Directory.GetCurrentDirectory())  //����M�׮ڥؿ�
                        .UseUrls("http://*:5000")  //��ܦ��A������ť���� IP ��}�ΥD���W�٤W���n�D
                        .UseStartup<Startup>();  //�I�sStartup���O
                });
    }
}
