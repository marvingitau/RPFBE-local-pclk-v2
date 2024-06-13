using System;
using System.Text;

namespace RPFBE
{
    public class Cryptography
	{
		public static string Hash(string value)
		{
			if (value != null)
			{
				return Convert.ToBase64String(System.Security.Cryptography.SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(value)));
			}
			else
			{
				return "";
			}

		}
	}
}