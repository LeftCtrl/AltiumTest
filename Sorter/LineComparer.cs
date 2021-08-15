using Shared;
using System;
using System.Collections.Generic;

namespace Sorter
{
    public class LineComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            return Compare(x, y, StringComparison.Ordinal);
        }
        
        public int Compare(ReadOnlySpan<char> x, ReadOnlySpan<char> y, StringComparison comparison)
        {
            int delimeterIndexX = x.IndexOf(Settings.Delimiter);
            int delimeterIndexY = y.IndexOf(Settings.Delimiter);

            ReadOnlySpan<char> xString = x.Slice(delimeterIndexX + Settings.Delimiter.Length);
            ReadOnlySpan<char> yString = y.Slice(delimeterIndexY + Settings.Delimiter.Length);

            int stringPartsCompare = xString.CompareTo(yString, comparison);
            if (stringPartsCompare != 0)
                return stringPartsCompare;

            ReadOnlySpan<char> xNumber = x.Slice(0, delimeterIndexX);
            ReadOnlySpan<char> yNumber = y.Slice(0, delimeterIndexY);

            int numberLengthCompare = xNumber.Length - yNumber.Length;
            if (numberLengthCompare != 0)
                return numberLengthCompare;

            return xNumber.CompareTo(yNumber, comparison);
        }
    }
}
