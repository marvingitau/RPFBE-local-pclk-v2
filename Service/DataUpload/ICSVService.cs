using System.Collections.Generic;
using System.IO;

namespace RPFBE.Service.DataUpload
{
    public interface ICSVService
    {
        IEnumerable<T> ReadCSV<T>(Stream file);
    }
}