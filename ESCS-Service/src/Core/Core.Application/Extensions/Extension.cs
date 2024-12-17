using System.Security.Cryptography;

namespace Core.Application.Extensions
{
    public static class Extension
    {
        public static string GenerateSecureApiKey(int length = 32)
        {
            byte[] keyBytes = new byte[length];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(keyBytes);
            }
            return Convert.ToBase64String(keyBytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
        }


        public static string ExtractFileName(string fileName)
        {
            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException("File not found");
            }

            fileName = Path.GetFileName(fileName);

            var extension = Path.GetExtension(fileName);

            var fileElement = fileName.Split('_');


            fileName = "";

            for (int i = 0; i <= fileElement.Length - 2; i++)
            {
                if (i != fileElement.Length - 2)
                {
                    fileName += fileElement[i] + "_";
                }
                else
                {
                    fileName += fileElement[i];
                }
            }

            return fileName + extension;

        }
    }
}
