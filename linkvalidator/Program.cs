using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using System.Data;
using nsexcel=Microsoft.Office.Interop.Excel;


namespace linkvalidator 
{

    public class Program
    { 
        public static void Main()
        {
            string connectionString = "Data Source=sqltest;Initial Catalog=YemekSepeti_productcatalog;" + "Integrated Security=true;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                Console.WriteLine("State: {0}", connection.State);

                string rname, cname, pname, ipath, pid;

                string query = @"SELECT top 1000 r.DisplayName as Restaurant, c.DisplayName AS Category, p.DisplayName AS Product, p.ImagePath, p.ProductId
                                FROM YemekSepeti_productcatalog.dbo.[TR_ISTANBUL_tr-TR] p(NOLOCK) 
                                INNER JOIN YemekSepeti_productcatalog.dbo.[TR_ISTANBUL_tr-TR] c(NOLOCK) ON c.CategoryName = p.PrimaryParentCategory
                                AND p.CatalogName = c.CatalogName 
                                INNER JOIN YemekSepeti_productcatalog.dbo.[TR_ISTANBUL_tr-TR] r(NOLOCK) ON r.CategoryName = c.PrimaryParentCategory 
                                AND r.CatalogName = c.CatalogName 
                                WHERE p.DefinitionName = 'food' and p.ImagePath != '...'";

                
                string exportPath = @"C:\Users\batuhan.celebi\source\repos\linkvalidator\linkvalidator";
                string exportCsv = "personexport.csv";
                StreamWriter csvFile = null;

                try
                    {
                        
                        SqlCommand sqlSelect = new SqlCommand(query, connection);

                        SqlDataReader reader = sqlSelect.ExecuteReader();

                        csvFile = new StreamWriter(@exportPath + exportCsv);

                        csvFile.WriteLine(String.Format("\"{0}\";\"{1}\";\"{2}\";" + "\"{3}\";\"{4}\"", reader.GetName(0), reader.GetName(1), reader.GetName(2),reader.GetName(3), reader.GetName(4)));

                        while (reader.Read())
                        {
                            csvFile.WriteLine(String.Format("\"{0}\";\"{1}\";\"{2}\";" + "\"{3}\";\"{4}\"", reader[0], reader[1], reader[2], reader[3], reader[4]));
                        }
                    }
                    catch (Exception e)
                    {
                    Console.WriteLine("Data export unsuccessful.");
                    System.Environment.Exit(0);
                }
                    finally
                    {
                    connection.Close();
                    csvFile.Close();

                }

                //while (dr.Read())
                //{
                //    rname = dr.GetString(0);
                //    cname = dr.GetString(1);
                //    pname = dr.GetString(2);
                //    ipath = dr.GetString(3);
                //    pid = dr.GetString(4);

                //    Console.WriteLine("Restoran adı {0}, Kategori{1}, Ürün{2}, ImagePath {3}, Ürün ID{4}", rname, cname, pname, ipath, pid);
                //}

                //dr.Close();
                //connection.Close();

            }

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