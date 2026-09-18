using System;
using System.Security.Cryptography;
using System.Text;

namespace Myschools.Api.Data;

public sealed class LegacyPasswordCipher
{
	public string EncryptForLegacyLogin(string password)
	{
		byte[] key = MD5.HashData(Encoding.UTF8.GetBytes("06061982"));
		using TripleDES tripleDES = TripleDES.Create();
		tripleDES.Key = key;
		tripleDES.Mode = CipherMode.ECB;
		tripleDES.Padding = PaddingMode.PKCS7;
		using ICryptoTransform cryptoTransform = tripleDES.CreateEncryptor();
		byte[] bytes = Encoding.UTF8.GetBytes(password);
		return Convert.ToBase64String(cryptoTransform.TransformFinalBlock(bytes, 0, bytes.Length));
	}
}
