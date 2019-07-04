using System;
using System.IO;
using System.Text.RegularExpressions;

namespace linkvalidator
{
    public class Program
    {

        public static void Main()
        {

            Regex rx = new Regex(@"\b(\w*.jpg\w*)\b",RegexOptions.Compiled | RegexOptions.IgnoreCase);

            string path = @"C:\Users\batuhan.celebi\source\repos\linkvalidator\linkvalidator\Documents.txt";
            StreamReader text = new StreamReader(path);
            
            MatchCollection matches = rx.Matches(path);

            Console.WriteLine("{0} matches found in:\n   {1}",matches.Count,text);

            foreach (Match match in matches)
            {
                GroupCollection groups = match.Groups;
                Console.WriteLine("'{0}' repeated at positions {1} and {2}",groups["word"].Value,groups[0].Index, groups[1].Index);
            }

        }

    }
}
