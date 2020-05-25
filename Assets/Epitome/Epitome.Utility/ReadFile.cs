using System.IO;
using UnityEngine;

namespace Epitome.Utility
{
    public static class ReadFile
    {
        public static byte[] ReadFileStream(string path)
        {
            if (Project.FileExists(path))
            {
                FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);

                fileStream.Seek(0, SeekOrigin.Begin);

                byte[] bytes = new byte[fileStream.Length];

                fileStream.Read(bytes, 0, (int)bytes.Length);

                fileStream.Close();
                fileStream.Dispose();
                fileStream = null;

                return bytes;
            }
            return null;
        }
    }
}