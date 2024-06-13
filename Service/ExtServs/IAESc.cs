namespace RPFBE.Service.ExtServs
{
    public interface IAESc
    {
       public string DecryptStringFromBytes(byte[] cipherText,string Key);
       public byte[] EncryptStringToBytes(string plainText, string Key);
    }
}