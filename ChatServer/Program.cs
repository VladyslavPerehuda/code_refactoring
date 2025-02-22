using ChatLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace ChatServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = new ServiceHost(typeof(ChatService));
            host.AddServiceEndpoint(
                typeof(IChatService),
                new NetTcpBinding(),
                "net.tcp://localhost:8080/ChatService");

            host.Open();
            Console.WriteLine("Сервер запущено. Натисніть будь-яку клавішу для завершення.");
            Console.ReadKey();
            host.Close();
        }
    }
}