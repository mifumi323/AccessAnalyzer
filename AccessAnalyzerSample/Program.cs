using System;
using MifuminLib.AccessAnalyzer;

namespace AccessAnalyzerSample
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(RefererAnalyzer.GetSearchPhrase("https://www.google.co.jp/search?q=%E3%82%A2%E3%83%B3%E3%83%91%E3%83%B3%E3%83%9E%E3%83%B3&ie=utf-8"));
        }
    }
}
