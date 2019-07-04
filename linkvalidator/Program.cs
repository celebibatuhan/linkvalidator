using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace linkvalidator
{
    public class Program
    {

        public static void Main()
        {
            Regex rx = new Regex(@"\b(\w*.jpg\w*)|(\w*.png\w*)|(\w*.jpeg\w*)|(\w*.gif\w*)\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            
            string path = @"C:\Users\batuhan.celebi\source\repos\linkvalidator\linkvalidator\Documents.txt";
            StreamReader text = new StreamReader(path);

            
            List<string> links = File.ReadLines(path).ToList();
            string output;
            for (var i = 0; i < links.Count; i++)
            {
                MatchCollection matches = rx.Matches(links[i]);
                
                foreach (Match match in matches)
                {
                    Console.WriteLine("That is what your looking for : {0} ",links[i]);
                }
            }
            
            
            
            

        }


    }
}
