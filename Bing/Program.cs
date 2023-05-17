using System;
using System.Threading.Tasks;

namespace Kurs2
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
