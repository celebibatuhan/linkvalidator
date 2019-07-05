using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Data.SqlClient;





namespace linkvalidator 
{

    public class Program
    {
        


        public static void Main()
        {
            //string connetionString = "Data Source=sqltest;Database=YemekSepeti_productcatalog;User id=YEMEKSEPETI\batuhan.celebi";
            //SqlConnection cnn = new SqlConnection(connetionString);
            //cnn.Open();
          
            //SqlCommand cmd1 = new SqlCommand(
            //    "SELECT TOP(20) r.DisplayName AS Restaurant, c.DisplayName AS Category, p.DisplayName AS Product, p.ProductId, p.ImagePath " +
            //    "FROM YemekSepeti_productcatalog.dbo.[TR_ISTANBUL_tr - TR] p(NOLOCK) INNER JOIN YemekSepeti_productcatalog.dbo.[TR_ISTANBUL_tr - TR] c(NOLOCK) ON c.CategoryName = p.PrimaryParentCategory " +
            //    "AND p.CatalogName = c.CatalogName INNER JOIN YemekSepeti_productcatalog.dbo.[TR_ISTANBUL_tr - TR] r(NOLOCK) ON r.CategoryName = c.PrimaryParentCategory AND r.CatalogName = c.CatalogName " +
            //    "WHERE p.DefinitionName = 'food' and p.ImagePath != '...'", cnn);

            //Console.WriteLine(cmd1);
            //Console.WriteLine();

            //cnn.Close();

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