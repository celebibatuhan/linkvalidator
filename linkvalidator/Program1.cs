using System;
using System.Data.SqlClient;
using System.IO;
using System.Net;


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

                string query = @"SELECT top 10
                                  [RestaurantName]
                                 ,[RestaurantCategoryName]
                                 ,[ProductName]
                                 ,[ProductImage]
                                 ,[ProductId]
                                 FROM [YemekSepeti_productcatalog].[dbo].[All_Products]
                                 WHERE [ProductImage] LIKE '/%'";

                string exportPath = @"C:\Users\";
                string exportCsv = "404List.csv";
                StreamWriter csvFile = null;
                HttpWebResponse httpRes;
                
                try
                {   
                    SqlCommand sqlSelect = new SqlCommand(query, connection);
                    SqlDataReader reader = sqlSelect.ExecuteReader();

                    csvFile = new StreamWriter(@exportPath + exportCsv);
                    csvFile.WriteLine(String.Format("\"{0}\";\"{1}\";\"{2}\";\"{3}\";\"{4}\"", reader.GetName(0), reader.GetName(1), reader.GetName(2), reader.GetName(3), reader.GetName(4)));

                    while (reader.Read())
                    {
                        string ImgPath = "https://cdn.yemeksepeti.com" + reader[3].ToString();
                        //Console.WriteLine(ImgPath);
                        HttpWebRequest httpReq = (HttpWebRequest)WebRequest.Create(ImgPath);
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
                                csvFile.WriteLine(String.Format("\"{0}\";\"{1}\";\"{2}\";\"{3}\";\"{4}\"", reader[0], reader[1], reader[2], reader[3], reader[4]));
                            }
                        }
                    }
                }
                catch (Exception ex)
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