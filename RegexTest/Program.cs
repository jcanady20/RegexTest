using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;

namespace RegexTest
{
    public class Program
    {
        static readonly string email_pattern = @"[\w\.+-]+@[\w\.-]+\.[\w\.-]+";
        static readonly string uri_pattern = @"[\w]+://[^/\s?#]+[^\s?#]+(?:\?[^\s#]*)?(?:#[^\s]*)?";
        static readonly string ip_pattern = @"(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9])\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9])";

        static void Main(string[] args)
        {
            var requestPattern = (args.Length > 0) ? args[0] : "email";
            string pattern = null;
            switch(requestPattern)
            {
                case "email":
                    pattern = email_pattern;
                    break;
                case "uri":
                    pattern = uri_pattern;
                    break;
                    case "ip":
                    pattern = ip_pattern;
                    break;
                default:
                    Console.WriteLine("Regex name must be: email, uri or ip.");
                    Environment.Exit(2);
                    break;
            }
            RunRegExMatches(requestPattern, pattern);
            Console.ReadKey();
        }

        private static void RunRegExMatches(string requestPattern, string pattern, int pass = 0)
        {
            if (pass >= 9) return;
            var regex = new Regex(pattern, RegexOptions.Compiled);
            var filePath = @".\input-text.txt";
            var data = FetchData(filePath);
            var sw = System.Diagnostics.Stopwatch.StartNew();
            var matches = Measure(data, regex);
            sw.Stop();
            var elapsed = sw.Elapsed;
            Console.WriteLine($"{pass} -- {requestPattern} Found {matches} matches in {elapsed.ToString(@"hh\:mm\:ss\:fff")}");
            pass += 1;
            RunRegExMatches(requestPattern, pattern, pass);
        }
        private static IEnumerable<string> FetchData(string filePath)
        {
            var data = new List<string>();
            using (var reader = new StreamReader(filePath))
            {
                string line = null;
                while ((line = reader.ReadLine()) != null)
                {
                    data.Add(line);
                }
            }
            return data;
        }
        private static int Measure(IEnumerable<string> data, Regex regex)
        {
            var obj = new Object();
            var matches = 0;
            Parallel.ForEach(data, line =>
            {
                var results = regex.Matches(line)?.Count ?? 0;
                lock(obj)
                {
                    matches += results;
                }
            });
            return matches;
        }
    }
}
