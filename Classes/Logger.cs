using ConverterInterface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConverterClasses
{
    public class Logger : ILogger
    {
        /// <summary>
        /// Private class level variable - cannot be accessed
        /// </summary>
        private LogDevice LogDevice { get; set; }

        /// <summary>
        /// Public property - can be accessed
        /// </summary>
        public string LOGBasePath { get; set; }


        /// <summary>
        /// Constructor
        /// </summary>
        public Logger(LogDevice console) : this(LogDevice.Console, string.Empty) { }

        public Logger(LogDevice logDevice, string appBasePath)
        {
            LogDevice = logDevice;

            LOGBasePath = string.Format("{0}\\LOG", appBasePath);
            if (!Directory.Exists(LOGBasePath))
                Directory.CreateDirectory(LOGBasePath);
        }

        public void Write(string path, List<string> messages)
        {
            if (LogDevice == LogDevice.Console)
                WriteToConsole(messages);
            else
                WriteToFile(path, messages);
        }

        public void WriteToFile(string path, List<string> messages)
        {
            messages.Insert(0, string.Format("{0}:", DateTime.Now));
            File.AppendAllLines(string.Format("{0}\\{1}.txt", LOGBasePath, path), messages);
        }

        public void WriteToConsole(List<string> messages)
        {
            messages.Insert(0, string.Format("{0}:", DateTime.Now));
            foreach (var msg in messages) { Console.WriteLine(msg); }
                
        }
    }
}
