using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Core.Application.Helpers
{
    public static class FileValidationHelper
    {


        public static bool IsFileValid(IFormFile file, long maxSizeInBytes, string[] allowedExtensions)
        {
            if (file == null)
            {
                throw new ValidationException("File can not received");
            }

            if (!IsFileFormatValid(file, allowedExtensions))
            {
                throw new ValidationException($"File {file.FileName} with extensions {Path.GetExtension(file.FileName)} is not accepted");
            }

            if (!IsFileSizeWithinLimit(file, maxSizeInBytes))
            {
                throw new ValidationException($"File {file.FileName} size is over {maxSizeInBytes}");

            }

            return true;
        }


        private static bool IsFileExtensionAllowed(IFormFile file, string[] allowedExtensions)
        {
            var extension = Path.GetExtension(file.Name);
            return allowedExtensions.Contains(extension);
        }

        //check file size is not over size limit
        private static bool IsFileSizeWithinLimit(IFormFile file, long maxSizeInBytes)
        {
            return file.Length <= maxSizeInBytes;
        }


        private static bool IsFileFormatValid(IFormFile file, string[] allowedExtensions)
        {
            // Load the file into a memory stream
            using var memoryStream = new MemoryStream();
            file.CopyTo(memoryStream);

            // Reset the position of the memory stream
            memoryStream.Position = 0;

            // Create a dictionary with only the allowed extensions and their respective signatures
            var fileSignatures = new Dictionary<string, List<byte[]>>();
            foreach (var extension in allowedExtensions)
            {
                if (_fileSignatures.TryGetValue(extension, out var signaturesList))
                {
                    fileSignatures.Add(extension, signaturesList);
                }
            }

            // Flatten all allowed signatures into a single list of byte arrays
            var signatures = fileSignatures.Values.SelectMany(x => x).ToList();

            // Find the length of the longest allowed signature
            int maxSignatureLength = signatures.Max(s => s.Length);

            // Read the appropriate number of bytes from the file's header
            memoryStream.Position = 0;
            var headerBytes = new byte[maxSignatureLength];
            memoryStream.Read(headerBytes, 0, maxSignatureLength);

            // Check if any allowed signature matches the beginning of the file's header bytes
            return signatures.Any(signature => headerBytes.Take(signature.Length).SequenceEqual(signature));
        }



        // dictionary file signature of same file type
        private static Dictionary<string, List<byte[]>> _fileSignatures = new()
     {
        { ".jpeg", new List<byte[]>
            {
                new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
                new byte[] { 0xFF, 0xD8, 0xFF, 0xE2 },
                new byte[] { 0xFF, 0xD8, 0xFF, 0xE3 },
                new byte[] { 0xFF, 0xD8, 0xFF, 0xEE },
                new byte[] { 0xFF, 0xD8, 0xFF, 0xDB },
            }
        },
        { ".jpg", new List<byte[]>
            {
                new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
                new byte[] { 0xFF, 0xD8, 0xFF, 0xE1 },
                new byte[] { 0xFF, 0xD8, 0xFF, 0xE8 },
                new byte[] { 0xFF, 0xD8, 0xFF, 0xEE },
                new byte[] { 0xFF, 0xD8, 0xFF, 0xDB },
            }
        },
        { ".jpeg2000", new List<byte[]> { new byte[] { 0x00, 0x00, 0x00, 0x0C, 0x6A, 0x50, 0x20, 0x20, 0x0D, 0x0A, 0x87, 0x0A } } },
        { ".png", new List<byte[]> { new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A } } },
        { ".pdf", new List<byte[]> { new byte[] { 0x25, 0x50, 0x44, 0x46 } } },
        { ".txt", new List<byte[]>
             {
                 new byte[] { 0xEF, 0xBB , 0xBF },
                 new byte[] { 0xFF, 0xFE},
                 new byte[] { 0xFE, 0xFF },
                 new byte[] { 0x00, 0x00, 0xFE, 0xFF },
             }
         },

         { ".doc", new List<byte[]>  { new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 } } },
         { ".ppt", new List<byte[]>  { new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 } } },
        
        // { ".gif", new List<byte[]> { new byte[] { 0x47, 0x49, 0x46, 0x38 } } },
         { ".zip", new List<byte[]> //also docx, xlsx, pptx, ...
             {
                 new byte[] { 0x50, 0x4B, 0x03, 0x04 },
                 new byte[] { 0x50, 0x4B, 0x4C, 0x49, 0x54, 0x45 },
                 new byte[] { 0x50, 0x4B, 0x53, 0x70, 0x58 },
                 new byte[] { 0x50, 0x4B, 0x05, 0x06 },
                 new byte[] { 0x50, 0x4B, 0x07, 0x08 },
                 new byte[] { 0x57, 0x69, 0x6E, 0x5A, 0x69, 0x70 },
             }
         },

         
        // { ".z", new List<byte[]>
        //     {
        //         new byte[] { 0x1F, 0x9D },
        //         new byte[] { 0x1F, 0xA0 }
        //     }
        // },
        // { ".tar", new List<byte[]>
        //     {
        //         new byte[] { 0x75, 0x73, 0x74, 0x61, 0x72, 0x00, 0x30 , 0x30 },
        //         new byte[] { 0x75, 0x73, 0x74, 0x61, 0x72, 0x20, 0x20 , 0x00 },
        //     }
        // },
        // { ".tar.z", new List<byte[]>
        //     {
        //         new byte[] { 0x1F, 0x9D },
        //         new byte[] { 0x1F, 0xA0 }
        //     }
        // },
        // { ".tif", new List<byte[]>
        //     {
        //         new byte[] { 0x49, 0x49, 0x2A, 0x00 },
        //         new byte[] { 0x4D, 0x4D, 0x00, 0x2A }
        //     }
        // },
        // { ".tiff", new List<byte[]>
        //     {
        //         new byte[] { 0x49, 0x49, 0x2A, 0x00 },
        //         new byte[] { 0x4D, 0x4D, 0x00, 0x2A }
        //     }
        // },
         { ".rar", new List<byte[]>
             {
                 new byte[] { 0x52, 0x61, 0x72, 0x21, 0x1A, 0x07 , 0x00 },
                 new byte[] { 0x52, 0x61, 0x72, 0x21, 0x1A, 0x07 , 0x01, 0x00 },
             }
         },
        // { ".7z", new List<byte[]>
        //     {
        //         new byte[] { 0x37, 0x7A, 0xBC, 0xAF, 0x27 , 0x1C },
        //     }
        // },
         
        // { ".mp3", new List<byte[]>
        //     {
        //         new byte[] { 0xFF, 0xFB },
        //         new byte[] { 0xFF, 0xF3},
        //         new byte[] { 0xFF, 0xF2},
        //         new byte[] { 0x49, 0x44, 0x43},
        //     }
        // },
    };
    }
}
