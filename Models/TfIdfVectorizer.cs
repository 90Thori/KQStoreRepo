using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KQStore.Models
{
    public class TfIdfVectorizer
    {
        private List<string> Vocabulary { get; set; }
        private Dictionary<string, double> IdfScores { get; set; }

        public TfIdfVectorizer()
        {
            Vocabulary = new List<string>();
            IdfScores = new Dictionary<string, double>();
        }

        public double[,] FitTransform(string[] documents)
        {
            var tokenizedDocs = documents.Select(doc => Tokenize(doc)).ToList();
            Vocabulary = tokenizedDocs.SelectMany(tokens => tokens).Distinct().ToList();
            CalculateIdf(tokenizedDocs);
            return Transform(tokenizedDocs);
        }

        private List<string> Tokenize(string document)
        {
            return document.ToLower().Split(new[] { ' ', '.', ',', ';', '!', '?' }, StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        private void CalculateIdf(List<List<string>> tokenizedDocs)
        {
            int totalDocs = tokenizedDocs.Count;
            foreach (var term in Vocabulary)
            {
                int docCount = tokenizedDocs.Count(doc => doc.Contains(term));
                IdfScores[term] = Math.Log((double)totalDocs / (1 + docCount)) + 1;
            }
        }

        private double[,] Transform(List<List<string>> tokenizedDocs)
        {
            int rows = tokenizedDocs.Count;
            int cols = Vocabulary.Count;
            double[,] tfidfMatrix = new double[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                var doc = tokenizedDocs[i];
                for (int j = 0; j < cols; j++)
                {
                    var term = Vocabulary[j];
                    double tf = (double)doc.Count(t => t == term) / doc.Count;
                    double idf = IdfScores[term];
                    tfidfMatrix[i, j] = tf * idf;
                }
            }

            return tfidfMatrix;
        }
    }
}