using ConverterClasses;
using ConverterInterface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConverterClasses
{
    public class Helper : IDisposable
    {
        #region Singleton

        static Helper _helper = null;

        private Helper() { }

        public static Helper Instance
        {
            get
            {
                if (_helper == null)
                    _helper = new Helper();

                return _helper;
            }
        }

        #endregion

        public ILogger _iLogger = null;

        public string APPBasePath { get; set; }
        public string CSVBasePath { get; set; }
        public string XMLBasePath { get; set; }
        public string LOGBasePath { get; set; }

        public void Initialise(ILogger iLogger, string APPBasePath)
        {
            CSVBasePath = string.Format("{0}\\CSV", APPBasePath);
            if (!Directory.Exists(CSVBasePath))
                Directory.CreateDirectory(CSVBasePath);

            XMLBasePath = string.Format("{0}\\XML", APPBasePath);
            if (!Directory.Exists(XMLBasePath))
                Directory.CreateDirectory(XMLBasePath);

            LOGBasePath = string.Format("{0}\\LOG", APPBasePath);
            if (!Directory.Exists(LOGBasePath))
                Directory.CreateDirectory(LOGBasePath);

            _iLogger = iLogger;
        }

        public void ProcessFiles(string timeStamp)
        {

            _iLogger.Write(timeStamp, new List<string>() { "Convert Start..." });

            var filesToProcess = GetFilesToProcess(CSVBasePath);
            if (filesToProcess.Count() <= 0)            
            {
                _iLogger.Write(timeStamp,
                    new List<string>() { string.Format("CSV File(s) not found in '{0}' folder!!", CSVBasePath) });
            }
            else
            {
                _iLogger.Write(timeStamp,
                    new List<string>() { string.Format("{0} CSV File(s) found to process...", filesToProcess.Count().ToString()) });

                using (var dc = new Transform(_iLogger))
                {
                    foreach (var fileToProcess in filesToProcess)
                    {
                        _iLogger.Write(timeStamp, new List<string>() { fileToProcess.FullName });

                        var fileName = Path.GetFileNameWithoutExtension(fileToProcess.Name);
                        if (dc.CSVToXML(fileToProcess.FullName, string.Format("{0}\\{1}.xml", XMLBasePath, fileName), timeStamp))                        
                            fileToProcess.Rename(string.Format("{0}.csvx", fileName));

                        _iLogger.Write(timeStamp, new List<string>() { "Convert Success..." });
                    }
                }
            }

            _iLogger.Write(timeStamp, new List<string>() { "Convert End." });
        }
       
        private IEnumerable<FileInfo> GetFilesToProcess(string cSVBasePath)
        {
            var folder = new DirectoryInfo(CSVBasePath);
            return folder.GetFiles().Where(f => f.Name.EndsWith("csv"));            
        }

        public void Dispose()
        {
            _helper = null;
            _iLogger = null;
            APPBasePath = string.Empty;
            CSVBasePath = string.Empty;
            XMLBasePath = string.Empty;
        }
    }
}
