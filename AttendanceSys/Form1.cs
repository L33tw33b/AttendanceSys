using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Windows.Forms;
using Microsoft.Office.Interop.Excel;
using _Excel = Microsoft.Office.Interop.Excel;

namespace AttendanceSys
{

    public partial class Form1 : Form
    {

        // Excel
        _Application excelApp = new _Excel.Application();
        Workbook wb;
        Worksheet ws;

        // App.config
        static readonly int namepos = Properties.Settings.Default.Namepos;
        static readonly int cardpos = Properties.Settings.Default.Cardnopos;
        static readonly int telepos = Properties.Settings.Default.Telepos;
        static readonly string textdir = ConfigurationManager.AppSettings["textpath"];
        static readonly string exceldir = ConfigurationManager.AppSettings["excelpath"];
        string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name.Split('\\')[1];

        // File
        string filen = $@"{textdir}{DateTime.UtcNow.Date.ToString("dd_MM_yyyy")}.txt";
        string[] pics = { "pics/1.jpg", "pics/2.jpg", "pics/3.jpg" };

        // Misc
        int i = 0;
        string[] name = new string[2000];
        string[] tele = new string[2000];
        string[] card = new string[2000];
        

        public Form1()
        {
            InitializeComponent();
            
        }

        private void nextImage() {
            if (i == 2)
            {
                i = 0;
            }
            else
            {
                i++;
            }

            pbox_slide.Image = Image.FromFile(pics[i]);


        }

        private void Form1_Load(object sender, EventArgs e) // Load
        {
            txt_input.Focus();
            lbl_welcome.Text = "";
            pbox_slide.ImageLocation = string.Format(@"pics/1.jpg");
            if (!File.Exists(filen)) {
                File.Create(filen);
            }

            if (excelApp == null) {
                MessageBox.Show("Excel is not installed!", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            wb = excelApp.Workbooks.Open($@"{exceldir}");
            ws = wb.ActiveSheet;
            for (int i = 0; i < ws.UsedRange.Rows.Count; i++) {
                name[i] = Convert.ToString(ws.Cells[i + 1, namepos].Value);
            }
            for (int i = 0; i < ws.UsedRange.Rows.Count  ; i++)
            {
                tele[i] = Convert.ToString(ws.Cells[i + 1, telepos].Value);
            }
            for (int i = 0; i < ws.UsedRange.Rows.Count ; i++)
            {
                card[i] = Convert.ToString(ws.Cells[i + 1, cardpos].Value);
            }
            wb.Close(0);
            excelApp.Quit();
        }

        private void timer1_Tick(object sender, EventArgs e) //Timer 
        {
            Image img = pbox_slide.Image;
            nextImage();
            img.Dispose();
        }

        private void txt_input_KeyDown(object sender, KeyEventArgs e) // Enter
        {
            if (e.KeyCode == Keys.Enter) {
                bool found = false;
                int i = 0;
                if (txt_input.TextLength == 8) { // User input == Telephone number
                    for (;i < tele.Length;i++) {
                        if (txt_input.Text == tele[i]) { // Excel Cells[y,x]
                            found = true;
                            using (StreamWriter sw = File.AppendText($@"{textdir}{DateTime.UtcNow.Date.ToString("dd_MM_yyyy")}.txt")) {
                                sw.WriteLine(/*ws.Cells[i, namepos].Value.ToString()*/string.Format("{0,-25}{1,-30}", name[i], DateTime.Now.ToString())) ;
                            }
                            
                                break;
                        }
                    }
                }
                else if (txt_input.TextLength == 10) { // User input == Card ID
                    for (; i < card.Length; i++)
                    {
                        if (txt_input.Text == card[i])
                        {
                            found = true;
                            using (StreamWriter sw = File.AppendText($@"{textdir}{DateTime.UtcNow.Date.ToString("dd_MM_yyyy")}.txt"))
                            {
                                try {
                                    sw.WriteLine(/*ws.Cells[i, namepos].Value.ToString()*/ string.Format("{0,-25}{1,-30}", name[i], DateTime.Now.ToString()));
                                }catch(Exception ex)
                                {
                                    MessageBox.Show(ex.Message);
                                }
                                
                            }
                            break;
                        }
                    }
                }
                
                if (!found) { MessageBox.Show("Invalid ID!", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
                else
                {
                    lbl_welcome.Text = $"歡迎你!{/*ws.Cells[i, namepos].Value.ToString()*/name[i]}";
                }

                txt_input.Text = "";
            }
        }


    }
}
