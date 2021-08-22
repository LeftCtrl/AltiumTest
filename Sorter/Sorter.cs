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
        private readonly int _newLineLength = Environment.NewLine.Length;

        public Sorter(float bufferSizeInGb)
        {
            _bufferSizeInBytes = bufferSizeInGb * 1024 * 1024 * 1024;
        }

        public void Sort(string inputPath, string outputPath)
        {
            List<string> sortedFiles = SplitByBufferSize(inputPath, outputPath);
            MergeSortedFiles(sortedFiles, outputPath);
        }

        private List<string> SplitByBufferSize(string inputPath, string outputPath)
        {
            using LineReader reader = new LineReader(inputPath);

            var sortedFiles = new List<string>();
            var lines = new List<string>();
            float currentSize = 0;
            while (reader.Line != null)
            {
                lines.Add(reader.Line);
                currentSize += reader.Line.Length + _newLineLength;

                reader.ReadLine();
                if (currentSize >= _bufferSizeInBytes)
                {
                    currentSize = 0;
                    SortAndWriteToFile(outputPath, lines, sortedFiles);
                }
            }

            if (lines.Count > 0)
                SortAndWriteToFile(outputPath, lines, sortedFiles);

            return sortedFiles;
        }

        private void SortAndWriteToFile(string outputPath, List<string> lines, List<string> sortedFiles)
        {
            string sortedFile = $"{outputPath}_sorted{sortedFiles.Count}";
            sortedFiles.Add(sortedFile);

            var sortedList = lines
                .AsParallel()
                .OrderBy(x => x, _lineComparer)
                .ToArray();

            lines.Clear();

            File.WriteAllLines(sortedFile, sortedList);
        }

        private void MergeSortedFiles(List<string> inputFiles, string outputPath)
        {
            if (inputFiles.Count == 1)
            {
                File.Move(inputFiles[0], outputPath);
                return;
            }

            List<LineReader> readers = inputFiles
                .Select(file => new LineReader(file))
                .ToList();

            try
            {
                readers.Sort((x, y) => _lineComparer.Compare(x.Line, y.Line));

                using StreamWriter writer = new StreamWriter(outputPath, false, Settings.Encoding);
                var sb = new StringBuilder();
                LineReader currentReader;
                while (readers.Count > 0)
                {
                    currentReader = readers.Pop();

                    sb.AppendLine(currentReader.Line);
                    if (sb.Length >= _bufferSizeInBytes)
                    {
                        writer.Write(sb);
                        sb.Clear();
                    }

                    currentReader.ReadLine();
                    if (currentReader.Line != null)
                    {
                        var i = 0;
                        while (i < readers.Count) //TODO: binary search? It is not a bottleneck
                        {
                            if (_lineComparer.Compare(currentReader.Line, readers[i].Line) <= 0)
                                break;

                            i++;
                        }

                        readers.Insert(i, currentReader);
                    }
                    else
                        currentReader.Dispose();
                }

                writer.Write(sb);
            }
            finally
            {
                foreach (var reader in readers)
                    reader.Dispose();

                foreach (var file in inputFiles)
                    File.Delete(file);
            } 
        }
    }
}
