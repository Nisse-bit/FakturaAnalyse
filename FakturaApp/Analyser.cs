using System;
using System.IO;
using System.Threading.Tasks;
using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;

namespace FakturaAnalyse
{
    public class AzureInvoiceAnalyzer
    {
        public static async Task<InvoiceModel> AnalyzeWithAzure(string filePath)
        {
            var (endpoint, key) = AzureKeyManager.GetCredentials();

            
            var credential = new AzureKeyCredential(key);
            var client = new DocumentAnalysisClient(new Uri(endpoint), credential);
            
            using var fileStream = File.OpenRead(filePath);
            
            var operation = await client.AnalyzeDocumentAsync(
                WaitUntil.Completed,
                "prebuilt-invoice", 
                fileStream);
            
            var result = operation.Value;
            
            if (result.Documents.Count > 0)
            {
                return InvoiceMapper.Map(result.Documents[0]);
            }
            
            return new InvoiceModel();
        }
    }
}