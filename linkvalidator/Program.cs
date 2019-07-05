using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace linkvalidator
{
    public class Program
    {
        public static void Main()
        {
            Regex rx = new Regex(@"\b(http(s?):)([/|.|\w|\s|-])*\.(?:jpg|gif|png|jpeg)\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);

            string path = @"C:\Users\batuhan.celebi\source\repos\linkvalidator\linkvalidator\Documents.txt";

            List<string> links = File.ReadLines(path).ToList();

            HttpWebResponse httpRes;

            for (var i = 0; i < links.Count; i++)
            {
                MatchCollection matches = rx.Matches(links[i]);

                foreach (Match match in matches)
                {
                    Console.WriteLine();
                    Console.WriteLine("Your URL: " + links[i]);
                    Console.WriteLine();

                    HttpWebRequest httpReq = (HttpWebRequest)WebRequest.Create(links[i]);

                    try
                    {
                        httpRes = (HttpWebResponse)httpReq.GetResponse();
                        if (httpRes.StatusCode == HttpStatusCode.NotFound)
                        {
                            Console.WriteLine("404 Not Found");
                        }
                        else
                        {
                            Console.WriteLine("Succeed!");
                        }
                    }

                    catch (WebException wec)
                    {
                        Console.WriteLine(wec.Status.ToString());
                        if (wec.Status == WebExceptionStatus.ProtocolError)
                        {
                            Console.WriteLine(string.Format("404 {0} (Invalid Image URL)", ((HttpWebResponse)wec.Response).StatusDescription));
                        }
                    }
                }
            }
        }
    }
}