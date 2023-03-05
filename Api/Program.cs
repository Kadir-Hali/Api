using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Api
{
    class Program
    {
        static readonly HttpClient client = new HttpClient();
        static void Main(string[] args)
        {
            

            List<Brand> brands = GetBrands(Strings.BaseURL, Strings.ApiKey);

            if (brands != null)
            {
                SaveBrandsToDatabase(brands);
                Console.WriteLine($"{brands.Count} brands saved to database!");
            }
            else
            {
                Console.WriteLine("Failed to get brands from API!");
            }

            Console.ReadLine();
        }

        static List<Brand> GetBrands(string url, string apiKey)
        {
            client.DefaultRequestHeaders.Add("Api-Key", apiKey);

            try
            {
                HttpResponseMessage response = client.GetAsync(url).Result;

                if (response.IsSuccessStatusCode)
                {
                    string responseString = response.Content.ReadAsStringAsync().Result;
                    var brandsResponse = JsonConvert.DeserializeObject<BrandsResponse>(responseString);
                    return brandsResponse.Brands;
                }
                else
                {
                    Console.WriteLine($"Error: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return null;
        }

        static void SaveBrandsToDatabase(List<Brand> brands)
        {
            string connectionString = @"Server=(localdb)\mssqllocaldb;Database=ApiDb;Trusted_Connection=true";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                foreach (Brand brand in brands)
                {
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = "INSERT INTO Brands (Id, Name) VALUES (@id, @name)";
                        command.Parameters.AddWithValue("@id", brand.Id);
                        command.Parameters.AddWithValue("@name", brand.Name);
                        command.ExecuteNonQuery();
                    }
                }
            }
        }
    }

    class BrandsResponse
    {
        public List<Brand> Brands { get; set; }
    }

    class Brand
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }
}
