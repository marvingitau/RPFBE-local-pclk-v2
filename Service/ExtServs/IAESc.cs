using Microsoft.Extensions.Configuration;

namespace RPFBE.Service.ExtServs
{
    public interface IAESc
    {
        IConfiguration Configuration { get; }

        string DecryptStringFromBytes(string cipherText);
        string EncryptStringToBytes(string plainText);
    }
}