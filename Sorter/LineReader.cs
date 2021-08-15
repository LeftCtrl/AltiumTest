using Shared;
using System;
using System.IO;

namespace Sorter
{
    public class LineReader : IDisposable
    {
        private readonly StreamReader _reader;
        private string _line;

        public string Line => _line;

        public LineReader(string path)
        {
            _reader = new StreamReader(path, Settings.Encoding);
            ReadLine();
        }

        public void ReadLine()
        {
            _line = _reader.ReadLine();
        }

        public void Dispose()
        {
            _reader.Dispose();
        }
    }
}
