using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Prometheus;
using Prometheus.DotNetRuntime;

namespace api_under_test
{
    public class Program
    {

        public static Gauge ConfiguredRetries = Metrics.CreateGauge("polly_retries", "How many");
        public static Gauge ConfiguredTimeout = Metrics.CreateGauge("polly_timeout", "How many milliseconds");
        public static Gauge TcpConnections = Metrics.CreateGauge("tcpconnections", "How many");
        public static Gauge ExecutedRetries = Metrics.CreateGauge("executed_retries", "How many");
        public static Gauge GC0= Metrics.CreateGauge("GC0", "How many");
        public static Gauge GC1= Metrics.CreateGauge("GC1", "How many");
        public static Gauge GC2= Metrics.CreateGauge("GC2", "How many");

        private static Timer _timer;

        public static void Main(string[] args)
        {
            var collector = DotNetRuntimeStatsBuilder.Default().StartCollecting();
            //var server = new MetricServer(hostname: "*", port: 1234);
            var server = new MetricServer(hostname: "*", port: 1234);
            
            server.Start();
            
            _timer = new Timer(LogNetworkAndGcInfo, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls("http://*:5000");
                });
        private static void LogNetworkAndGcInfo(object state)
        {
            var tcpStat = IPGlobalProperties.GetIPGlobalProperties().GetTcpIPv4Statistics();
            TcpConnections.Set(tcpStat.CurrentConnections);
            GC0.Set(GC.CollectionCount(0));
            GC1.Set(GC.CollectionCount(1));
            GC2.Set(GC.CollectionCount(2));
        }

    }
}
