using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Statistics;
namespace KQStore.Models
{
    public class DatabaseHandler 
    {
        private string connectionString = "Server=MTHIEU\\SQLEXPRESS;Database=KQStore;User Id=kq;Password=Trihieu12a1!;";

        public DataTable GetProducts()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM Product", conn))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        DataTable productsTable = new DataTable();
                        adapter.Fill(productsTable);
                        return productsTable;
                    }
                }
            }
        }
        
    }
    public class Products
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Discount { get; set; }
        public string Img { get; set; }
        public int CateId { get; set; }

        public string CombineFeatures { get; set; }
    }
    public static class MatrixExtensions
    {
        public static Vector<double> GetRow(this double[,] matrix, int row)
        {
            int cols = matrix.GetLength(1);
            double[] rowData = new double[cols];
            for (int i = 0; i < cols; i++)
            {
                rowData[i] = matrix[row, i];
            }
            return Vector<double>.Build.Dense(rowData);
        }
    }
    public class ProductRecommender
    {
        public List<Products> Products { get; set; } = new List<Products>();

        public void LoadProducts(DataTable dataTable)
        {
            Products.Clear();
            foreach (DataRow row in dataTable.Rows)     
            {
                Products.Add(new Products
                {
                    ProductId = Convert.ToInt32(row["ProductId"]),
                    Name = row["Name"].ToString(),
                    Price = Convert.ToDecimal(row["Price"]),
                    Discount = Convert.ToInt32(row["Discount"]),
                    Img = row["Img"].ToString(),
                    CateId = Convert.ToInt32(row["CateId"]),
                    CombineFeatures = $"{row["Price"]} {row["CateId"]} {row["Name"]}"
                });
            }
        }

        public double[,] CalculateTfIdfMatrix()
        {
            var documents = Products.Select(p => p.CombineFeatures).ToArray();
            var tfidf = new TfIdfVectorizer();
            return tfidf.FitTransform(documents);
        }

        public double[,] CalculateCosineSimilarity(double[,] tfidfMatrix)
        {
            int n = tfidfMatrix.GetLength(0);
            double[,] cosineSimilarityMatrix = new double[n, n];

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    cosineSimilarityMatrix[i, j] = CosineSimilarity(tfidfMatrix.GetRow(i), tfidfMatrix.GetRow(j));
                }
            }

            return cosineSimilarityMatrix;
        }

        private double CosineSimilarity(Vector<double> vectorA, Vector<double> vectorB)
        {
            return vectorA.DotProduct(vectorB) / (vectorA.L2Norm() * vectorB.L2Norm());
        }

        public List<Products> GetTopSimilarProducts(int productIndex, int numberOfSimilarProducts)
        {
            var tfidfMatrix = CalculateTfIdfMatrix();
            var similarityMatrix = CalculateCosineSimilarity(tfidfMatrix);

            var similarProducts = similarityMatrix.GetRow(productIndex)
                .Select((score, index) => new { Index = index, Score = score })
                .OrderByDescending(x => x.Score)
                .Skip(1)
                .Take(numberOfSimilarProducts)
                .Select(x => Products[x.Index])
                .ToList();

            return similarProducts;
        }
    }
}