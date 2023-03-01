using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JakaAPI
{
    public class JakaPainter : JakaRobot
    {
        public JakaPainter(string domain, int port = 10001) : base(domain, port) { }
    }
}