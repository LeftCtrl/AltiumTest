using Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Sorter
{
    public class Sorter
    {
        private readonly LineComparer _lineComparer = new LineComparer();
        private readonly float _bufferSizeInBytes;

        public Sorter(float bufferSizeInGb)
        {
            _bufferSizeInBytes = bufferSizeInGb * 1024 * 1024 * 1024;
        }

        public void Sort(string inputPath, string outputPath, int splitCount)
        {
            if (File.Exists(outputPath))
                File.Delete(outputPath);

            string[] files = SplitByFirstSymbols(inputPath, outputPath, splitCount);
            
            foreach (var file in files)
            {
                SortAndAppendToOutputFile(file, outputPath);
                File.Delete(file);
            }
        }

        private string[] SplitByFirstSymbols(string inputPath, string outputPath, int symbolCount)
        {
            var writers = new Dictionary<string, (string Path, StreamWriter Writer)>();
            
            try
            {
                using LineReader reader = new LineReader(inputPath);

                int delimeterIndex;
                StreamWriter writer;
                string key;
                while (reader.Line != null)
                {
                    delimeterIndex = reader.Line.IndexOf(Settings.Delimiter);

                    key = reader.Line.Substring(delimeterIndex + Settings.Delimiter.Length);
                    if (key.Length > symbolCount)
                        key = key.Substring(0, symbolCount);

                    if (!writers.ContainsKey(key))
                    {
                        string tmpOutputPath = $"{outputPath}_{writers.Count}";
                        writers.Add(key, (tmpOutputPath, new StreamWriter(tmpOutputPath)));
                    }

                    writer = writers[key].Writer;
                    writer.WriteLine(reader.Line);
                    reader.ReadLine();
                }
            }
            finally
            {
                foreach (var writer in writers)
                    writer.Value.Writer.Dispose();
            }

            List<string> sortedKeys = writers.Keys.ToList();
            sortedKeys.Sort(StringComparer.Ordinal);

            return sortedKeys.Select(x => writers[x].Path).ToArray();
        }

        private void SortAndAppendToOutputFile(string inputPath, string outputPath)
        {
            int newLineLength = Environment.NewLine.Length;

            var sortedFiles = new List<string>();
            using (LineReader reader = new LineReader(inputPath))
            {
                var lines = new List<string>();
                float currentSize = 0;
                while (reader.Line != null)
                {
                    lines.Add(reader.Line);
                    currentSize += reader.Line.Length + newLineLength;

                    reader.ReadLine();
                    if (currentSize >= _bufferSizeInBytes)
                    {
                        currentSize = 0;
                        WriteToFile(sortedFiles, lines, outputPath, reader.Line == null);
                    }
                }

                if (lines.Count > 0)
                    WriteToFile(sortedFiles, lines, outputPath, true);
            }

            if (sortedFiles.Count > 0)
                MergeSortedFiles(sortedFiles, outputPath);
        }

        private void WriteToFile(
            List<string> sortedFiles, List<string> lines, string outputPath, bool isLastFile)
        {
            lines.Sort(_lineComparer);
            if (isLastFile && sortedFiles.Count == 0)
            {
                File.AppendAllLines(outputPath, lines, Settings.Encoding);
            }
            else
            {
                string sortedFile = $"{outputPath}_sorted{sortedFiles.Count}";
                sortedFiles.Add(sortedFile);
                File.WriteAllLines(sortedFile, lines, Settings.Encoding);
            }

            lines.Clear();
        }

        private void MergeSortedFiles(List<string> inputFiles, string outputPath)
        {
            int tempFileNumber = 0;
            string file1;
            string file2;
            string tempOutputPath;

            var filesToMerge = new Queue<string>(inputFiles);
            while (true)
            {
                file1 = filesToMerge.Dequeue();
                file2 = filesToMerge.Dequeue();

                if (filesToMerge.Count == 0)
                {
                    MergeSortedFiles(file1, file2, outputPath, true);
                    break;
                }

                tempOutputPath = filesToMerge.Count > 0 ? $"{outputPath}_merged{tempFileNumber++}" : outputPath;
                MergeSortedFiles(file1, file2, tempOutputPath, false);
                filesToMerge.Enqueue(tempOutputPath);
            }
        }

        private void MergeSortedFiles(string file1, string file2, string outputPath, bool append)
        {
            using (StreamWriter writer = new StreamWriter(outputPath, append, Settings.Encoding))
            using (LineReader reader1 = new LineReader(file1))
            using (LineReader reader2 = new LineReader(file2))
            {
                LineReader currentReader = reader1;
                var sb = new StringBuilder();
                bool lastReader = false;
                while (true)
                {
                    if (!lastReader)
                        currentReader = _lineComparer.Compare(reader1.Line, reader2.Line) < 0 ? reader1 : reader2;

                    sb.AppendLine(currentReader.Line);
                    if (sb.Length >= _bufferSizeInBytes)
                    {
                        writer.Write(sb);
                        sb.Clear();
                    }

                    currentReader.ReadLine();
                    if (currentReader.Line == null)
                    {
                        if (lastReader)
                            break;

                        lastReader = true;
                        currentReader = currentReader == reader1 ? reader2 : reader1;
                    }
                }

                writer.Write(sb);
            }

            File.Delete(file1);
            File.Delete(file2);
        }
    }
}
