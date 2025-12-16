using System;
using System.Windows.Forms;

namespace FakturaAnalyse
{
    
    // exceptions to terminal and text, should be copyable
    // error handling. testing
    //fixing warnings. -- added ignore nullable warnings to proj or sln
    // cooler window  add css somehow? window builder or scene builder
    // research auditor job things
    // works somewhat with pdf.
    // does not work with excel
    //eksporter virker ikke
    
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            
            
            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm());
        }
    }
}