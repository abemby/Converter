using System;
using System.Timers;
using System.Threading.Tasks;
using ConverterInterface;
using System.IO;
using System.Collections.Generic;
using System.Configuration;
using ConverterClasses;

namespace ConverterConsole
{
    class Program
    {

        // timer could also be part of helper class...
        // 1000 is the number of milliseconds in a second.
        // 60 is the number of seconds in a minute
        // 30 is the number of minutes.
        // for 2 minutes = 1000 * 60 * 2 = 120000

        static Timer _timer = null;
        static Logger _logger = null;

        static void Main(string[] args)
        {
            try
            {

                string appBasePath = Directory.GetCurrentDirectory().ToLower().
                                        Replace("\\bin\\debug", "").Replace("\\bin\\release", "");

                _logger = new Logger(LogDevice.Console, appBasePath);

                //initialise helper - set directories
                Helper.Instance.Initialise(_logger, appBasePath);                

                //time interval comes from app configuration file
                var timerInterval = ConfigurationSettings.AppSettings["TimerInterval"].ToString();
                if (string.IsNullOrEmpty(timerInterval))
                    timerInterval = "2";      //default to 2 minutes...               

                _timer = new Timer();
                _timer.Interval = TimeSpan.FromMinutes(double.Parse(timerInterval)).TotalMilliseconds;
                _timer.Elapsed += Timer_Elapsed;
                _timer.Start();

                _logger.Write(DateTime.Now.ToFileTime().ToString(),
                    new List<string>() { string.Format("Console is up and timer is ready set to fire every {0} minute(s)...", timerInterval.ToString()) });

                //xmlPath = string.Format("{0}\\{1}", basePath, "V1.xml");
                //success = DocumentConverter.Instance.CsvToXml(csvPath, xmlPath);

                //xmlPath = string.Format("{0}\\{1}", basePath, "V2.xml");
                //success = DocumentConverter.Instance.CsvToXmlV2(csvPath , xmlPath);


                //xmlPath = string.Format("{0}\\{1}", basePath, "V3.xml");
                //success = DocumentConverter.Instance.CsvToXmlV3(csvPath, xmlPath);


                //rename file after process is done...
                Console.ReadKey();                
            }
            catch (Exception ex)
            {
                var err = new List<string>();
                err.Add(ex.Message); err.Add(ex.InnerException.Message); err.Insert(0, "Main()");
                _logger.Write(DateTime.Now.ToFileTime().ToString(), err);
            }

            finally
            {                
                _logger.Write(DateTime.Now.ToFileTime().ToString(), new List<string>() { "Console down." });
                _timer.Dispose(); _logger = null; Helper.Instance.Dispose(); GC.Collect();
            }

        }

        private static void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                _timer.Stop();
                Helper.Instance.ProcessFiles(DateTime.Now.ToFileTime().ToString());
                _timer.Start();
            }
            catch (Exception ex)
            {
                var err = new List<string>();
                err.Add(ex.Message); err.Add(ex.InnerException.Message); err.Insert(0, "Timer_Elapsed()");
                _logger.Write(DateTime.Now.ToFileTime().ToString(), err);
                _timer.Dispose(); _logger = null; Helper.Instance.Dispose(); GC.Collect();
            }

        }               
    }
}

