using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConverterInterface
{
    public interface IDocConverter
    {
        bool CSVToXML(string csvPath, string xmlPath, string timeStamp);
    }
}
