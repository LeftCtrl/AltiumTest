using NUnit.Framework;
using Sorter;
using System;
using System.Diagnostics;
using System.IO;

namespace Tests
{
    class SorterTests
    {
        private string _inputFile = @"unsorted.txt";
        private string _outputFile = @"sorted.txt";
        private TimeSpan _elapsedBySort;
        private float _partSizeInGb = 0.5f;
        private float _peakWorkingSet64;

        [OneTimeSetUp]
        public void Init()
        {
            Generator.Generator.CreateFile(_inputFile, 10, 1024, 25);

            var timer = new Stopwatch();
            timer.Start();

            new Sorter.Sorter(_partSizeInGb).Sort(_inputFile, _outputFile);
            
            _elapsedBySort = timer.Elapsed;
            _peakWorkingSet64 = (float)Process.GetCurrentProcess().PeakWorkingSet64 / (1024 * 1024 * 1024);
        }

        [OneTimeTearDown]
        public void DeleteTestFiles()
        {
            File.Delete(_inputFile);
            File.Delete(_outputFile);
        }

        [Test]
        public void OutputFileShouldBeSorted()
        {
            using LineReader reader = new LineReader(_outputFile);

            var comparer = new LineComparer();
            
            string prevLine = reader.Line;
            reader.ReadLine();

            while (reader.Line != null)
            {
                Assert.IsTrue(comparer.Compare(prevLine, reader.Line) <= 0, $"{prevLine} <<< {reader.Line}");
                prevLine = reader.Line;
                reader.ReadLine();
            }
        }

        [Test]
        public void InputAndOutputFilesShouldHaveSameLineCount()
        {
            long check = 0;
            using LineReader reader = new LineReader(_inputFile);
            while (reader.Line != null)
            {
                check++;
                reader.ReadLine();
            }

            long result = 0;
            using LineReader reader2 = new LineReader(_outputFile); 
            while (reader2.Line != null)
            {
                result++;
                reader2.ReadLine();
            }

            Assert.AreEqual(check, result);
        }

        [Test]
        public void ShouldSortOneGbPerMinute()
        {
            double gb = (double) new FileInfo(_inputFile).Length / (1024 * 1024 * 1024);
            double minutes = _elapsedBySort.TotalMinutes;

            Assert.LessOrEqual(minutes, gb);
        }

        [Test]
        [Ignore("For debug. Result depends on the input parameters and pc configuration")]
        public void ShouldUseNotMoreThan5PartSizeMemory()
        {
            Assert.LessOrEqual(_peakWorkingSet64, _partSizeInGb * 5);
        }
    }
}
