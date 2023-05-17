using System;
using System.Threading.Tasks;

namespace Bing
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Server server = new();
            await server.StartAsync(8080);
        }
    }
}
