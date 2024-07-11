using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;


namespace sem3plink
{
    class Progam
    {
        static string site = "ya.ru";
        static object lockObj = new object();

        public static async Task Main(String[] args)
        {

            /* Напишите многопоточное приложение, которое определяет все IP-адреса интернет-ресурса и определяет до которого из них лучше Ping. Приложение должно работать с помощью Task.
             */

            await Task1();


        }

        public static async Task Task1()
        {
            IPAddress[] ips = Dns.GetHostAddresses(site, System.Net.Sockets.AddressFamily.InterNetwork);

            Dictionary<IPAddress, long> pings = new Dictionary<IPAddress, long>();

            List<Task> tasks = new List<Task>();
            foreach (IPAddress ip in ips)
            {
                var task = Task.Run(async() =>
                {
                    Ping p = new Ping();
                    PingReply pingReply = await p.SendPingAsync(ip);

                    lock (lockObj)
                    {
                        pings.Add(ip, pingReply.RoundtripTime);
                    }
                    Console.WriteLine($"{ip} : {pingReply.RoundtripTime}");
                });
                tasks.Add(task);               
            }

            Task.WaitAll(tasks.ToArray());

            long minPing = pings.Min(x => x.Value);

            Console.WriteLine(minPing);
        }
    }
}
