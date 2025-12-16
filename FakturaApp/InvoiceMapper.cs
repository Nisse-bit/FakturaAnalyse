using System;
using System.Collections.Generic;
using System.Globalization;
using Azure.AI.FormRecognizer.DocumentAnalysis;

namespace FakturaAnalyse
{
    public static class InvoiceMapper
    {
        public static InvoiceModel Map(AnalyzedDocument doc)
        {
            var invoice = new InvoiceModel();
            
            // Get basic fields from Content
            GetStringField(doc, "InvoiceId", value => invoice.InvoiceId = value);
            GetStringField(doc, "InvoiceDate", value => invoice.InvoiceDate = value);
            GetStringField(doc, "DueDate", value => invoice.DueDate = value);
            GetStringField(doc, "VendorName", value => invoice.VendorName = value);
            GetStringField(doc, "VendorAddress", value => invoice.VendorAddress = value);
            GetStringField(doc, "CustomerName", value => invoice.CustomerName = value);
            GetStringField(doc, "CustomerAddress", value => invoice.CustomerAddress = value);
            GetStringField(doc, "Currency", value => invoice.Currency = value);
            GetStringField(doc, "PaymentTerm", value => invoice.PaymentTerms = value);
            
            // Get numeric fields
            GetDecimalField(doc, "SubTotal", value => invoice.SubTotal = value);
            GetDecimalField(doc, "TotalTax", value => invoice.TaxAmount = value);
            GetDecimalField(doc, "InvoiceTotal", value => invoice.TotalAmount = value);
            
            // Default to Danish
            invoice.LanguageDetected = "Danish/da-DK";
            
            return invoice;
        }
        
        private static void GetStringField(AnalyzedDocument doc, string fieldName, Action<string> setter)
        {
            if (doc.Fields.TryGetValue(fieldName, out var field) && !string.IsNullOrEmpty(field.Content))
            {
                setter(field.Content);
            }
        }
        
        private static void GetDecimalField(AnalyzedDocument doc, string fieldName, Action<decimal?> setter)
        {
            if (doc.Fields.TryGetValue(fieldName, out var field) && !string.IsNullOrEmpty(field.Content))
            {
                // Clean the string: remove currency symbols, replace comma with dot
                var cleanString = field.Content
                    .Replace("kr", "")
                    .Replace("$", "")
                    .Replace("€", "")
                    .Replace("£", "")
                    .Replace(",", ".")
                    .Trim();
                
                if (decimal.TryParse(cleanString, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal result))
                {
                    setter(result);
                }
                else
                {
                    setter(null);
                }
            }
            else
            {
                setter(null);
            }
        }
    }
}