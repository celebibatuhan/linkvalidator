using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Text;
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
                Console.WriteLine("DB Connection : {0}", connection.State);

                string query = @"SELECT top(150) r.DisplayName as Restaurant, c.DisplayName AS Category, p.DisplayName AS Product, p.ImagePath, p.ProductId
		                         FROM YemekSepeti_productcatalog.dbo.[TR_ISTANBUL_tr-TR] p (NOLOCK)
		                         INNER JOIN YemekSepeti_productcatalog.dbo.[TR_ISTANBUL_tr-TR] c (NOLOCK) ON c.CategoryName = p.PrimaryParentCategory AND p.CatalogName = c.CatalogName
		                         INNER JOIN YemekSepeti_productcatalog.dbo.[TR_ISTANBUL_tr-TR] r (NOLOCK) ON r.CategoryName = c.PrimaryParentCategory AND r.CatalogName = c.CatalogName
		                         WHERE p.DefinitionName = 'food' AND p.ImagePath LIKE '/%'";

                string exportPath = @"C:\Users\batuhan.celebi\source\repos\linkvalidator\linkvalidator\";
                string exportCsv = "404List.csv";
                StreamWriter csvFile = null;
                HttpWebResponse httpRes;

                try
                {
                    SqlCommand sqlSelect = new SqlCommand(query, connection);
                    SqlDataReader reader = sqlSelect.ExecuteReader();

                    csvFile = new StreamWriter(@exportPath + exportCsv);
                    csvFile.WriteLine(String.Format("\"{0}\";\"{1}\";\"{2}\";" + "\"{3}\";\"{4}\"", reader.GetName(0), reader.GetName(1), reader.GetName(2), reader.GetName(3), reader.GetName(4)));

                    while (reader.Read())
                    {
                        string imgpat = reader[3].ToString();
                        string imgpath = "https://cdn.yemeksepeti.com" + imgpat;
                        Console.WriteLine(imgpath);
                        HttpWebRequest httpReq = (HttpWebRequest)WebRequest.Create(imgpath);

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
                            if (wec.Status == WebExceptionStatus.ProtocolError)
                            {
                                Console.WriteLine(string.Format("404 {0} (Invalid Image URL)", ((HttpWebResponse)wec.Response).StatusDescription));
                                csvFile.WriteLine(String.Format("\"{0}\";\"{1}\";\"{2}\";" + "\"{3}\";\"{4}\"", reader[0], reader[1], reader[2], reader[3], reader[4]));
                            }
                        } 
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
        }
    }
}