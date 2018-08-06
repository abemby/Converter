using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace ConsoleApp1
{
    public class DocumentConverter
    {        

        #region Singleton
        static DocumentConverter _documentConverter = null;

        private DocumentConverter() { }

        public static DocumentConverter Instance
        {
            get
            {
                if (_documentConverter == null)
                {
                    _documentConverter = new DocumentConverter();
                }

                return _documentConverter;
            }
        }
        #endregion

        private const string LOCAL_CURRENCY = "GBP";

        public bool CsvToXml(string sourcePath, string destinationPath)
        {
            var success = false;

            var fileExists = File.Exists(sourcePath);

            if (!fileExists)
            {
                return success;
            }

            try
            {


                var formatedLines = LoadCsv(sourcePath);
                var headers = formatedLines[0].Split(',').Select(x => x.Trim('\"').Replace(" ", string.Empty)).ToArray();

                var xml = new XElement("VendorParts",
                   formatedLines.Where((line, index) => index > 0).
                       Select(line => new XElement("Part",
                          line.Split(',').Select((field, index) => new XElement(headers[index], field)))));


                xml.Save(destinationPath);

                success = true;
            }

            catch (Exception ex)
            {
                success = false;

                var baseException = ex.GetBaseException();
                Debug.Write(baseException.Message);
            }

            return success;
        }
        
        public bool CsvToXmlV2(string csvPath, string xmlPath)
        {
            var success = false;

            var fileExists = File.Exists(csvPath);

            if (!fileExists)
            {
                return success;
            }


            try
            {

                var lines = File.ReadAllLines(csvPath).ToList();

                //get headers
                var headerLine = lines[0];
                var headers = headerLine.Split(',').Select(x => x.Trim('\"').Replace(" ", string.Empty)).ToArray();


                //remove header line
                lines.RemoveAt(0);


                var dataLines = lines.OrderBy(a => a[0]).ThenBy(b => b[1]).ThenBy(c => c[2]);
                                
                //group by elemnts

                XElement xml = new XElement("Orders",
                 from str in dataLines                 
                 let fields = str.Split(',')
                 select new XElement("Order",
                     new XAttribute(headers[0], fields[0]),
                     new XElement("Consignments",
                        new XElement("Consignment",
                            new XAttribute(headers[1], fields[1]),
                                new XElement("Parcels",
                                    new XElement("Parcel",
                                        new XAttribute(headers[2], fields[2]),
                                        new XElement("ParcelsItems",
                                            new XElement(headers[3], fields[3]),
                                            new XElement("Address1", fields[4]),                                            
                                            new XElement("Address2", fields[5]),
                                            new XElement("City", fields[6]),
                                            new XElement("State", fields[7]),
                                            new XElement("CountryCode", fields[8]),
                                            new XElement("ItemQuantity", fields[9]),
                                            new XElement("ItemValue", fields[10]),
                                            new XElement("ItemWeight", fields[11]),
                                            new XElement("ItemDescription", fields[12]),
                                            new XElement("ItemCurrency", (string.IsNullOrEmpty(fields[13]) ? LOCAL_CURRENCY : fields[13]))
                                        )
                                    )
                                )
                            )
                        )
                    )
                );



                xml.Save(xmlPath);

                //dump on screen
                Console.WriteLine(xml);

                success = true;
            }
            catch (Exception ex)
            {
                success = false;

                var baseException = ex.GetBaseException();
                Debug.Write(baseException.Message);
            }

            return success;
        }

        public bool CsvToXmlV3(string csvPath, string xmlPath)
        {
            var success = false;

            var fileExists = File.Exists(csvPath);

            if (!fileExists)
            {
                return success;
            }


            try
            {

                var lines = File.ReadAllLines(csvPath).ToList();

                //get headers
                var headerLine = lines[0];
                var headers = headerLine.Split(',').Select(x => x.Trim('\"').Replace(" ", string.Empty)).ToArray();


                //remove header line
                lines.RemoveAt(0);


                var dataLines = lines.OrderBy(a => a[0]).ThenBy(b => b[1]).ThenBy(c => c[2]);

                //group by elemnts

                XElement xml = new XElement("Orders",
                 from str in dataLines
                 let fields = str.Split(',')
                 select new XElement("Order",
                     new XAttribute(headers[0], fields[0]),
                     new XElement("Consignments",
                        new XElement("Consignment",
                            new XAttribute(headers[1], fields[1]),
                                new XElement("Parcels",
                                    new XElement("Parcel",
                                        new XAttribute(headers[2], fields[2]),
                                        new XElement("ParcelsItems",
                                            new XAttribute(headers[13], (string.IsNullOrEmpty(fields[13]) ? LOCAL_CURRENCY : fields[13])),
                                            new XAttribute(headers[12], fields[12]),
                                            new XAttribute(headers[11], fields[11]),
                                            new XAttribute(headers[10], fields[10]),
                                            new XAttribute(headers[9], fields[9]),
                                            new XAttribute(headers[8], fields[8]),
                                            new XAttribute(headers[7], fields[7]),
                                            new XAttribute(headers[6], fields[6]),
                                            new XAttribute(headers[5], fields[5]),
                                            new XAttribute(headers[4], fields[4]),
                                            new XAttribute(headers[3], fields[3])
                                        )
                                    )
                                )
                            )
                        )
                    )
                );


                var xml2 = xml.Elements("Parcels").Attributes("Length").Sum(a => (int)a);

                xml.Save(xmlPath);

                //dump on screen
                Console.WriteLine(xml);

                success = true;
            }
            catch (Exception ex)
            {
                success = false;

                var baseException = ex.GetBaseException();
                Debug.Write(baseException.Message);
            }

            return success;
        }

        public bool CsvToXmlV4(string csvPath, string xmlPath, string timeStamp)
        {
            var success = false;

            var fileExists = File.Exists(csvPath);

            if (!fileExists)
            {
                return success;
            }


            try
            {

                //read all lines from the file and remove any blank lines...
                var rows = File.ReadAllLines(csvPath).ToList();
                if (rows.Count > 0)
                    rows = rows.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();
                
                //get headers
                var headerRow = rows[0];

                //remove space or backspace as this will be used for xml node
                var headers = headerRow.Split(',').Select(x => x.Trim('\"').Replace(" ", string.Empty)).ToArray();

                //remove header line
                rows.RemoveAt(0);
                
                //group by order no then by consignment no then by parcel code
                XElement xml = new XElement("Orders",
                    from str in rows
                    let fields = str.Split(',')
                    group fields by fields[0] into gOrders
                    select new XElement("Order",
                        new XAttribute(headers[0], gOrders.Key),
                        new XAttribute("TotalValue", gOrders.Sum(g=>decimal.Parse(g[10]))),
                        new XAttribute("TotalWeight", gOrders.Sum(g => decimal.Parse(g[11]))),
                            from gOrd in gOrders           
                            group gOrd by gOrd[1] into gConsignments
                            select new XElement("Consignments",
                                    new XElement("Consignment",
                                    new XAttribute(headers[1], gConsignments.Key),
                                    from gCon in gConsignments
                                    group gCon by gCon[2] into gParcels
                                    select new XElement("Parcels",
                                        new XElement("Parcel",
                                        new XAttribute(headers[2], gParcels.Key),
                                        from gPar in gParcels
                                        group gPar by gPar[12] into gItems
                                        select new XElement("ParcelsItems",                                                                                       
                                             from gIte in gItems
                                             select new XElement("ParcelItem",
                                                new XAttribute(headers[13], (string.IsNullOrEmpty(gIte[13]) ? LOCAL_CURRENCY : gIte[13])),
                                                new XAttribute(headers[12], gIte[12]),
                                                new XAttribute(headers[11], gIte[11]),
                                                new XAttribute(headers[10], gIte[10]),
                                                new XAttribute(headers[9], gIte[9]),
                                                new XAttribute(headers[8], gIte[8]),
                                                new XAttribute(headers[7], gIte[7]),
                                                new XAttribute(headers[6], gIte[6]),
                                                new XAttribute(headers[5], gIte[5]),
                                                new XAttribute(headers[4], gIte[4]),
                                                new XAttribute(headers[3], gIte[3])
                                            )
                                        )
                                    )
                                )
                            )
                        )
                    )
                );

                 
                xml.Save(xmlPath);

                //dump on screen
                //Console.WriteLine(xml);

                success = true;
            }
            catch (Exception ex)
            {
                success = false;
                
                var baseException = ex.GetBaseException();
                //Debug.Write(baseException.Message);
                Helper.Instance.LogInFile(timeStamp, new string[] { baseException.Message });
            }

            return success;
        }

        private List<string> LoadCsv(string sourcePath)
        {
            var lines = File.ReadAllLines(sourcePath).ToList();

            var formatedLines = new List<string>();

            foreach (var line in lines)
            {
                var formatedLine = line.TrimEnd(',');
                formatedLines.Add(formatedLine);
            }
            return formatedLines;
        }

    }
}
