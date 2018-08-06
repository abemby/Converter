using Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ConsoleApp1
{
    public class Helper
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

        public string APPBasePath { get; set; }
        public string CSVBasePath { get; set; }
        public string XMLBasePath { get; set; }
        public string LOGBasePath { get; set; }

        /// <summary>
        /// Initialise folder path
        /// Make sure directories exists
        /// </summary>
        /// 
        internal void Initialise()
        {
            APPBasePath = Directory.GetCurrentDirectory().ToLower().Replace("\\bin\\debug", "").Replace("\\bin\\release", "");

            CSVBasePath = string.Format("{0}\\CSV", APPBasePath);
            if (!Directory.Exists(CSVBasePath))
                Directory.CreateDirectory(CSVBasePath);

            XMLBasePath = string.Format("{0}\\XML", APPBasePath);
            if (!Directory.Exists(XMLBasePath))
                Directory.CreateDirectory(XMLBasePath);

            LOGBasePath = string.Format("{0}\\LOG", APPBasePath);
            if (!Directory.Exists(LOGBasePath))
                Directory.CreateDirectory(LOGBasePath);

        }

        internal void LogInFile(string path, string[] message)
        {
            var msg = message.ToList();
            msg.Insert(0, string.Format("{0}:", DateTime.Now));
            File.AppendAllLines(string.Format("{0}\\{1}.txt", LOGBasePath,path), msg);
        }

        internal void ProcessFiles(string timeStamp)
        {
            LogInFile(timeStamp, new string[] { "Convert Start..." });

            var folder = new DirectoryInfo(CSVBasePath);
            var filesToProcess = folder.GetFiles().Where(f => f.Name.EndsWith("csv"));
            var filesToProcessCount = filesToProcess.Count();
            if (filesToProcessCount <= 0)
            {
                LogInFile(timeStamp,
                    new string[] { string.Format("Error! CSV File(s) not found in '{0}' folder!!", folder.FullName) });
            }
            else
            {
                LogInFile(timeStamp,
                    new string[] { string.Format("{0} CSV File(s) found to process...", folder.Name) });

                foreach (var fileToProcess in filesToProcess)
                {
                    LogInFile(timeStamp, new string[] { fileToProcess.FullName });

                    var fileName = Path.GetFileNameWithoutExtension(fileToProcess.Name);
                    var success = DocumentConverter.Instance.CsvToXmlV4
                                    (fileToProcess.FullName, string.Format("{0}\\{1}.xml", XMLBasePath, fileName), timeStamp);

                    if (success)
                        new FileInfo(fileToProcess.FullName).RenameV2(string.Format("{0}.csvx", fileName));

                    LogInFile(timeStamp, new string[] { "Convert Success..." });
                }
            }

            LogInFile(timeStamp, new string[] { "Convert End." });
        }

    }
}

/// <summary>
///     Extension function
/// </summary>
namespace System.IO
{
    public static class ExtendedMethod
    {
        public static void RenameV2(this FileInfo fileInfo, string newName)
        {
            fileInfo.MoveTo(fileInfo.Directory.FullName + "\\" + newName);
        }
    }
}

