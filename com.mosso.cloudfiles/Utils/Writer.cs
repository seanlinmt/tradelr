using System.IO;

namespace com.mosso.cloudfiles.utils
{
    public static class Writer
    {
        public static void WriteTo(this Stream source, Stream target)
        {
            WriteTo(source, target, 1024);
        }

        public static void WriteTo(this Stream source, Stream target, int bufferLength)
        {
            byte[] buffer = new byte[bufferLength];
            int bytesRead = 0;

            do
            {
                bytesRead = source.Read(buffer, 0, buffer.Length);
                target.Write(buffer, 0, bytesRead);
            } while (bytesRead > 0);
        }
    }
}