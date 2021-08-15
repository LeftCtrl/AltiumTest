using System;
using System.Diagnostics;
using System.Globalization;

namespace Sorter
{
    public class Program
    {
        public static void Main(string[] args)
        {
            float bufferSizeInGb;
            int splitCount;
            if (args.Length < 4
                || !float.TryParse(args[2], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out bufferSizeInGb)
                || !int.TryParse(args[3], out splitCount))
            {
                Console.WriteLine("Usage with arguments: inputFile outputFile bufferSizeInGb splitCount");
                Console.WriteLine(@"Example: C:\unsorted.txt C:\sorted.txt 0.5 1");
                return;
            }

            var timer = new Stopwatch();
            timer.Start();

            Console.WriteLine($"Creating {args[1]}...");
            new Sorter(bufferSizeInGb).Sort(args[0], args[1], splitCount);
            Console.WriteLine($"Sort time: {timer.Elapsed.ToString("h'h 'm'm 's's'")}");
        }
    }
}
