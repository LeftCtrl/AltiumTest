using System;
using System.Globalization;

namespace Generator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            float sizeInGb;
            int maxLineLength;
            int duplicatesPercent;
            if (args.Length < 4
                || !float.TryParse(args[1], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out sizeInGb)
                || !int.TryParse(args[2], out maxLineLength)
                || !int.TryParse(args[3], out duplicatesPercent))
            {
                Console.WriteLine("Usage with arguments: outputFile sizeInGb maxLineLength duplicatesPercent");
                Console.WriteLine(@"Example: C:\unsorted.txt 0.1 1024 20");
                return;
            }

            Console.WriteLine($"Creating {args[0]}...");
            Generator.CreateFile(args[0], sizeInGb, maxLineLength, duplicatesPercent);
            Console.WriteLine($"File {args[0]} was created!");
        }
    }
}
