using System.IO;
using System.IO.Compression;

namespace Simply.Common
{
    /// <summary>
    /// Defines the <see cref="ByteArrayExtensions"/>.
    /// </summary>
    public static class ByteArrayExtensions
    {
        /// <summary>
        /// Compresses the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public static byte[] Compress(this byte[] data)
        {
            using (MemoryStream output = new MemoryStream())
            using (GZipStream gzip = new GZipStream(output, CompressionMode.Compress, true))
            {
                gzip.Write(data, 0, data.Length);
                gzip.Close();
                return output.ToArray();
            }
        }

        /// <summary>
        /// Decompresses the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public static byte[] Decompress(this byte[] data)
        {
            using (MemoryStream input = new MemoryStream())
            {
                input.Write(data, 0, data.Length);
                input.Position = 0;
                using (GZipStream gzip = new GZipStream(input, CompressionMode.Decompress, true))
                {
                    MemoryStream output = new MemoryStream();
                    byte[] buff = new byte[64];
                    int read = -1;
                    read = gzip.Read(buff, 0, buff.Length);
                    while (read > 0)
                    {
                        output.Write(buff, 0, read);
                        read = gzip.Read(buff, 0, buff.Length);
                    }
                    gzip.Close();
                    return output.ToArray();
                }
            }
        }
    }
}