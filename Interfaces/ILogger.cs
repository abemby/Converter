using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConverterInterface
{
   
    public interface ILogger
    {
        void Write(string path, List<string> messages);        
    }

    public enum LogDevice
    {
        Console = 0,
        File = 1
    }
}
