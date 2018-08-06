using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConverterClasses
{
    class Extensions
    {
    }
}

/// <summary>
/// Extension to rename file name - uses inbuild IO.Move
/// </summary>
namespace System.IO
{
    public static class ExtendedMethod
    {
        public static void Rename(this FileInfo fileInfo, string newName)
        {
            fileInfo.MoveTo(fileInfo.Directory.FullName + "\\" + newName);
        }
    }
}