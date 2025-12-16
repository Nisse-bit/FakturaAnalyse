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
            this.Text = "📄 Faktura Analyzer";
            this.Size = new System.Drawing.Size(900, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            btnOpenFile = new Button
            {
                Text = "📂 Åbn Excel",
                Location = new System.Drawing.Point(20, 20),
                Size = new System.Drawing.Size(150, 40),
                Font = new System.Drawing.Font("Segoe UI", 10F)
            };
            btnOpenFile.Click += BtnOpenFile_Click;

            btnOpenPdf = new Button
            {
                Text = "📄 Åbn PDF (Azure AI)",
                Location = new System.Drawing.Point(180, 20),
                Size = new System.Drawing.Size(170, 40),
                Font = new System.Drawing.Font("Segoe UI", 10F)
            };
            btnOpenPdf.Click += BtnOpenPdf_Click;

            btnExport = new Button
            {
                Text = "💾 Eksporter til Excel",
                Location = new System.Drawing.Point(360, 20),
                Size = new System.Drawing.Size(170, 40),
                Font = new System.Drawing.Font("Segoe UI", 10F),
                Enabled = false
            };
            btnExport.Click += BtnExport_Click;

            dataGridView1 = new DataGridView
            {
                Location = new System.Drawing.Point(20, 80),
                Size = new System.Drawing.Size(840, 450),
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            lblStatus = new Label
            {
                Location = new System.Drawing.Point(20, 540),
                Size = new System.Drawing.Size(400, 30),
                Text = "Ingen fil indlæst",
                Font = new System.Drawing.Font("Segoe UI", 10F)
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