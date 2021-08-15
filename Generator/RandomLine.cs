using Shared;
using System;

namespace Generator
{
    internal class RandomLine
    {
        private static readonly string _newLine = Environment.NewLine;
        private static readonly Random _random = new Random(); 

        private const string _numbers = "0123456789";
        private const string _ascii = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!\"#$%&\'()*+,-./:;<=>?@[\\]^_`{|}~ ";
        
        private readonly int _usefulLength;
        private readonly int _maxStringLength;

        private string _stringPart;
        private string _numberPart;

        public RandomLine(int maxLength)
        {
            _usefulLength = maxLength - Settings.Delimiter.Length - _newLine.Length;
            _maxStringLength = _usefulLength - 1; //reserve one symbol for number part
            
            _stringPart = GenerateString();
        }

        public string Next()
        {
            _stringPart = GenerateString();
            _numberPart = GenerateNumber();
            return GetLine();
        }

        public string ChangeNumber()
        {
            _numberPart = GenerateNumber();
            return GetLine();
        }

        private string GetLine()
        {
            return $"{_numberPart}{Settings.Delimiter}{_stringPart}{_newLine}";
        }

        private string GenerateString()
        {
            int length = _random.Next(_maxStringLength) + 1;
            var chars = GetRandomChars(_ascii, length);

            return new string(chars);
        }

        private string GenerateNumber()
        {
            int maxLength = _usefulLength - _stringPart.Length;

            int length = _random.Next(maxLength) + 1;
            var chars = GetRandomChars(_numbers, length);

            if (chars[0] == '0')
                chars[0] = '1'; //first symbol should be positive

            return new string(chars);
        }

        private char[] GetRandomChars(string chars, int length)
        {
            var result = new char[length];

            for (var i = 0; i < result.Length; i++)
                result[i] = chars[_random.Next(chars.Length)];

            return result;
        }
    }
}
