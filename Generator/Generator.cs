using Shared;
using System;
using System.IO;

namespace Generator
{
    public static class Generator
    {
        public static void CreateFile(string path, float sizeInGb, int maxLineLength, int duplicatesPercent)
        {
            float sizeInBytes = sizeInGb * 1024 * 1024 * 1024;
            var randomLine = new RandomLine(maxLineLength);
            var randomPercent = new Random();

            string duplicatesPath = path + ".dup";

            using (StreamWriter mainWriter = new StreamWriter(path, false, Settings.Encoding))
            using (StreamWriter dupWriter = new StreamWriter(duplicatesPath, false, Settings.Encoding))
            {
                StreamWriter currentWriter;
                string line;
                float currentSize = 0;
                while (currentSize < sizeInBytes)
                {
                    bool isDuplicate = randomPercent.Next(100) < duplicatesPercent;

                    if (isDuplicate)
                    {
                        line = randomLine.ChangeNumber();
                        currentWriter = dupWriter;
                    }
                    else
                    {
                        line = randomLine.Next();
                        currentWriter = mainWriter;
                    }

                    currentWriter.Write(line);
                    currentSize += line.Length;
                }
            }

            MergeFiles(path, duplicatesPath);
            File.Delete(duplicatesPath);
        }

        private static void MergeFiles(string file1, string file2)
        {
            using StreamWriter writer = new StreamWriter(file1, true, Settings.Encoding);
            using StreamReader reader = new StreamReader(file2, Settings.Encoding);

            string line;
            while ((line = reader.ReadLine()) != null)
                writer.WriteLine(line);
        }
    }
}
