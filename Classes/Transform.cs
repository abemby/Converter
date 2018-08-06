using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using ConverterInterface;

namespace ConverterClasses
{
    public class Transform : IDocConverter, IDisposable
    {

        private ILogger _iLogger = null;
        private const string LOCAL_CURRENCY = "GBP";

        public Transform(ILogger iLogger)
        {
            _iLogger = iLogger;
        }

        public bool CSVToXML(string csvPath, string xmlPath, string timeStamp)
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
                        new XAttribute("TotalWeight", gOrders.Sum(g => decimal.Parse(g[11]))),
                        new XAttribute("TotalValue", gOrders.Sum(g => decimal.Parse(g[10]))),                        
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

                success = true;
            }
            catch (Exception ex)
            {
                success = false;

                var baseException = ex.GetBaseException();
                _iLogger.Write(timeStamp, new List<string>() { baseException.Message });
            }

            return success;

        }

        public void Dispose()
        {
            _iLogger = null;    
        }
    }
}
