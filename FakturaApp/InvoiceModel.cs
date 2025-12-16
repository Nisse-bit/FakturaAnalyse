using System;
using System.Collections.Generic;
using System.Globalization;

namespace FakturaAnalyse
{
    public class InvoiceModel
    {
        public string InvoiceId { get; set; } = "";
        public string InvoiceDate { get; set; } = "";
        public string DueDate { get; set; } = "";
        public string VendorName { get; set; } = "";
        public string VendorAddress { get; set; } = "";
        public string CustomerName { get; set; } = "";
        public string CustomerAddress { get; set; } = "";
        public decimal? SubTotal { get; set; }
        public decimal? TaxAmount { get; set; }
        public decimal? TotalAmount { get; set; }
        public string Currency { get; set; } = "";
        public List<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();
        public string PaymentTerms { get; set; } = "";
        public string LanguageDetected { get; set; } = "";
        public string FileName { get; set; } = "";
        
        public Dictionary<string, string> GetBilingualDictionary()
        {
            return new Dictionary<string, string>
            {
                ["FakturaId/InvoiceId"] = InvoiceId ?? "Ikke fundet",
                ["Dato/Date"] = InvoiceDate ?? "Ikke fundet",
                ["Forfaldsdato/DueDate"] = DueDate ?? "Ikke fundet",
                ["Leverandør/Vendor"] = VendorName ?? "Ikke fundet",
                ["Leverandøradresse/VendorAddress"] = VendorAddress ?? "Ikke fundet",
                ["Kundenavn/CustomerName"] = CustomerName ?? "Ikke fundet",
                ["Kundeadresse/CustomerAddress"] = CustomerAddress ?? "Ikke fundet",
                ["Subtotal/SubTotal"] = SubTotal?.ToString("C", CultureInfo.GetCultureInfo("da-DK")) ?? "Ikke fundet",
                ["Moms/Tax"] = TaxAmount?.ToString("C", CultureInfo.GetCultureInfo("da-DK")) ?? "Ikke fundet",
                ["Total/TotalAmount"] = TotalAmount?.ToString("C", CultureInfo.GetCultureInfo("da-DK")) ?? "Ikke fundet",
                ["Valuta/Currency"] = Currency ?? "Ikke fundet",
                ["Betalingsbetingelser/PaymentTerms"] = PaymentTerms ?? "Ikke fundet",
                ["Sprog/Language"] = LanguageDetected ?? "Ikke fundet"
            };
        }
    }

    public class InvoiceItem
    {
        public string Description { get; set; } = "";
        public decimal? Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? TotalPrice { get; set; }
    }
}