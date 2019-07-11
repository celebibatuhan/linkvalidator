using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

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

                    string query = @"SELECT TOP(50) r.DisplayName as Restaurant, c.DisplayName AS Category, p.DisplayName AS Product, p.ImagePath, p.ProductId
		                             FROM YemekSepeti_productcatalog.dbo.[TR_ANTALYA_tr-TR] p (NOLOCK)
		                             INNER JOIN YemekSepeti_productcatalog.dbo.[TR_ANTALYA_tr-TR] c (NOLOCK) ON c.CategoryName = p.PrimaryParentCategory AND p.CatalogName = c.CatalogName
		                             INNER JOIN YemekSepeti_productcatalog.dbo.[TR_ANTALYA_tr-TR] r (NOLOCK) ON r.CategoryName = c.PrimaryParentCategory AND r.CatalogName = c.CatalogName
		                             WHERE p.DefinitionName = 'food' and p.ImagePath != '...' and p.ImagePath != '..' and p.ImagePath != '.' 
		                             AND p.ImagePath != '*' and p.ImagePath LIKE '/%'";

                string exportPath = @"C:\Users\batuhan.celebi\source\repos\linkvalidator\linkvalidator";
                string exportCsv = "personexport.csv";
                StreamWriter csvFile = null;
                try
                { 
                    SqlCommand sqlSelect = new SqlCommand(query, connection);
                    SqlDataReader reader = sqlSelect.ExecuteReader();

                    csvFile = new StreamWriter(@exportPath + exportCsv);
                    csvFile.WriteLine(String.Format("\"{0}\";\"{1}\";\"{2}\";" + "\"{3}\";\"{4}\"", reader.GetName(0), reader.GetName(1), reader.GetName(2), reader.GetName(3), reader.GetName(4)));
                    
                    while (reader.Read())
                    {
                        csvFile.WriteLine(String.Format("\"{0}\";\"{1}\";\"{2}\";" + "\"https://cdn.yemeksepeti.com{3}\";\"{4}\"", reader[0], reader[1], reader[2], reader[3], reader[4]));
                    }

                }
                catch (Exception)
                {
                    Console.WriteLine("Data export unsuccessful.");
                    Environment.Exit(0);
                }
                finally
                {
                    connection.Close();
                    csvFile.Close();
                }                
            }
            //listeye atmadan direk while loop ile sorgu gönder/çıkınca doğrudan csv file a gönder.
            using (var reader = new StreamReader(@"C:\Users\batuhan.celebi\source\repos\linkvalidator\linkvalidatorpersonexport.csv"))
            {
                Regex rx = new Regex(@"\b(http(s?):)([/|.|\w|\s|-])*\.(?:jpg|gif|png|jpeg)\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);

                HttpWebResponse httpRes;
                int ccount = 0;
                int scount = 0;
                int dcount = 0;
                int rcount = 0;
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] values = line.Split(';');
                    rcount++;
                    MatchCollection matches = rx.Matches(values[3]);
                    foreach (Match match in matches)
                    {
                        string deneme = values[3].ToString();
                        deneme = deneme.Substring(1);
                        string denemet = deneme.Remove(deneme.Length - 1, 1);
                        Console.WriteLine(denemet);

                        HttpWebRequest httpReq = (HttpWebRequest)WebRequest.Create(denemet);
                        try
                        {
                            httpRes = (HttpWebResponse)httpReq.GetResponse();
                            if (httpRes.StatusCode == HttpStatusCode.NotFound)
                            {
                                Console.WriteLine("404 Not Found");
                                dcount++;
                            }
                            else
                            {
                                Console.WriteLine("Succeed!");
                                scount++;
                            }
                        }
                        catch (WebException wec)
                        {
                            //Console.WriteLine(wec.Status.ToString());
                            if (wec.Status == WebExceptionStatus.ProtocolError)
                            {
                                Console.WriteLine(string.Format("404 {0} (Invalid Image URL)", ((HttpWebResponse)wec.Response).StatusDescription));
                                ccount++;
                            }
                        }
                    }
                }

                Console.WriteLine("404 veren sayısı" + ccount);
                Console.WriteLine("başarılı olan sayısı" + scount);
                Console.WriteLine("d counter " + dcount);
                Console.WriteLine("okunan satır sayısı" + rcount);
            }

        }
    }
}