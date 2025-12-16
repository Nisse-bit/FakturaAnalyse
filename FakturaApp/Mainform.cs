using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data;

namespace FakturaAnalyse
{
    public partial class MainForm : Form
    {
        private DataGridView dataGridView1;
        private Button btnOpenFile;
        private Button btnOpenPdf;
        private Button btnExport;
        private Label lblStatus;
        private DataTable currentData;

        public MainForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "📄 Faktura Analyzer (Prototype)";
            this.Size = new System.Drawing.Size(1600, 1000);   // Bigger window
            this.StartPosition = FormStartPosition.CenterScreen;

            // Use larger fonts for readability
            var buttonFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            var labelFont = new System.Drawing.Font("Segoe UI", 12F);
            var gridFont = new System.Drawing.Font("Segoe UI", 11F);

            btnOpenFile = new Button
            {
                Text = "📂 Åbn Excel",
                Location = new System.Drawing.Point(20, 20),
                Size = new System.Drawing.Size(200, 50),
                Font = buttonFont
            };
            btnOpenFile.Click += BtnOpenFile_Click;

            btnOpenPdf = new Button
            {
                Text = "📄 Åbn PDF (Azure AI)",
                Location = new System.Drawing.Point(240, 20),
                Size = new System.Drawing.Size(220, 50),
                Font = buttonFont
            };
            btnOpenPdf.Click += BtnOpenPdf_Click;

            btnExport = new Button
            {
                Text = "💾 Eksporter til Excel",
                Location = new System.Drawing.Point(480, 20),
                Size = new System.Drawing.Size(220, 50),
                Font = buttonFont,
                Enabled = false
            };
            btnExport.Click += BtnExport_Click;

            dataGridView1 = new DataGridView
            {
                Location = new System.Drawing.Point(20, 100),
                Size = new System.Drawing.Size(1500, 700),   // Much larger grid
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                Font = gridFont,
                RowTemplate = { Height = 35 }               // Taller rows
            };

            lblStatus = new Label
            {
                Location = new System.Drawing.Point(20, 820),
                Size = new System.Drawing.Size(800, 40),
                Text = "Ingen fil indlæst",
                Font = labelFont
            };

            this.Controls.Add(btnOpenFile);
            this.Controls.Add(btnOpenPdf);
            this.Controls.Add(btnExport);
            this.Controls.Add(dataGridView1);
            this.Controls.Add(lblStatus);
        }
        // Åbn Excel fil - await ProcessFile
        private async void BtnOpenFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = "Excel filer (*.xlsx)|*.xlsx";
                dialog.Title = "Vælg en Excel faktura";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    await ProcessFile(dialog.FileName, "excel");
                }
            }
        }

        // Åbn PDF med Azure AI - await ProcessFile
        private async void BtnOpenPdf_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = "PDF filer (*.pdf)|*.pdf|Billeder (*.jpg;*.png)|*.jpg;*.png";
                dialog.Title = "Vælg en faktura til AI-analyse";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    await ProcessFile(dialog.FileName, "azure");
                }
            }
        }

        // Behandle fil - async Task så vi kan await fra kaldere
        private async Task ProcessFile(string filePath, string type)
        {
            try
            {
                lblStatus.Text = type == "azure" ? "Analyserer med Azure AI..." : "Læser Excel fil...";
                this.Refresh();

                InvoiceModel invoice;

                
                    // Azure-analyse er allerede async og skal awaited
                    invoice = await AzureInvoiceAnalyzer.AnalyzeWithAzure(filePath);
                
               // else
              //  {
                    // Sørg for at klassen hedder korrekt: Analyzer eller Analyser
                   // var analyzer = new Analyzer(); // eller new Analyser() hvis det er korrekt i dit projekt
                   // invoice = await Task.Run(() => analyzer.ReadExcelFile(filePath));
               // }

                invoice.FileName = Path.GetFileName(filePath);

                // Vis resultat (UI-opdatering sker på UI-tråden pga. await)
                DisplayInvoice(invoice);
                btnExport.Enabled = true;
                lblStatus.Text = $"✅ Analyseret: {invoice.FileName}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fejl: {ex.Message}", "Fejl",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "Fejl under analyse";
            }
        }

        // Eksporter til Excel
        private void BtnExport_Click(object sender, EventArgs e)
        {
            if (currentData == null || currentData.Rows.Count == 0)
                return;

            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                dialog.Filter = "Excel fil (*.xlsx)|*.xlsx";
                dialog.FileName = $"faktura_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

              
            }
        }

        // Vis faktura i grid
        private void DisplayInvoice(InvoiceModel invoice)
        {
            currentData = new DataTable();
            currentData.Columns.Add("Felt", typeof(string));
            currentData.Columns.Add("Værdi", typeof(string));

            foreach (var kvp in invoice.GetBilingualDictionary())
            {
                currentData.Rows.Add(kvp.Key, kvp.Value ?? "-");
            }

            dataGridView1.DataSource = currentData;
        }
    }
}