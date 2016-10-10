using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace TheWorld
{
    public class Program
    {
        //Entry point: Where applications is actually starting up
        public static void Main(string[] args)
        {
            //Build up a web host that is going to start to listen to requests (makes it a web app). Sets up Initial server integration: Won't change often
            var host = new WebHostBuilder()                         
                .UseKestrel()                                       //Name of web server under ASP.NET Core
                .UseContentRoot(Directory.GetCurrentDirectory())    //Where content is for project
                .UseIISIntegration()                                //Support for IIS for optional features
                .UseStartup<Startup>()                              //Tell it you have a class called Startup to set up the web server and Instantiate it when you start up the web host
                .Build();                                           //Builds the host

            host.Run();                                             //Runs the host
        }
    }
}
