using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kurs2
{
    //public class Request<T>
    //{
    //    public string Mehtod { get; set; }
    //    public T Data { get; set; }
    //    public Request(string Method, T Data)
    //    {
    //        this.Mehtod = Mehtod;
    //        this.Data = Data;
    //    }
    //}
    public class Request
    {
        public string Method { get; set; }
        public string Data { get; set; }
    }
}
