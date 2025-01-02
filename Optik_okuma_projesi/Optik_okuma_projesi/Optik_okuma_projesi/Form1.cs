using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using OfficeOpenXml; // EPPlus i�in using y�nergesi


namespace Optik_okuma_projesi
{
    public partial class Form1 : Form
    {
        private string selectedFilePath = "";
        private string selectedExamType = "";

        public Form1()
        {

            InitializeComponent();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Lisans ayar�n� burada yap�n
            // Di�er ba�latma kodlar�...

            dataGridView1.ColumnCount = 5;
            dataGridView1.Columns[0].Name = "Numara";
            dataGridView1.Columns[1].Name = "Do�ru Say�s�";
            dataGridView1.Columns[2].Name = "Yanl�� Say�s�";
            dataGridView1.Columns[3].Name = "Bo� Say�s�";
            dataGridView1.Columns[4].Name = "Puan";
        }


        private void comboBoxGrupSay�s�_SelectedIndexChanged(object sender, EventArgs e)
        {
            int maxChar = 0;

            // Grup say�s�na g�re karakter s�n�r� belirle
            if (comboBoxGrupSay�s�.SelectedItem.ToString() == "25")
                maxChar = 25;
            else if (comboBoxGrupSay�s�.SelectedItem.ToString() == "50")
                maxChar = 50;
            else if (comboBoxGrupSay�s�.SelectedItem.ToString() == "100")
                maxChar = 100;

            // T�m textbox'lar i�in MaxLength ayarla
            textBoxCvpA.MaxLength = maxChar;
            textBoxCvpB.MaxLength = maxChar;
            textBoxCvpC.MaxLength = maxChar;
            textBoxCvpD.MaxLength = maxChar;
        }

        private void buttonDegerlendirme_Click(object sender, EventArgs e)
        {

           
                // Zorunlu alan kontrol�
                if (string.IsNullOrEmpty(textBoxS�navNo.Text) ||
                    string.IsNullOrEmpty(textBoxS�navAd.Text) ||
                    string.IsNullOrEmpty(textBoxDersiVeren.Text) ||
                    comboBoxGrupSay�s�.SelectedIndex == -1 ||
                    comboBoxSoruSay�s�.SelectedIndex == -1 ||
                    string.IsNullOrEmpty(textBoxSoruPuan�.Text) ||
                    (!radioButtonVize.Checked && !radioButtonFinal.Checked &&
                     !radioButtonQuiz.Checked && !radioButtonUyg.Checked &&
                     !radioButtonB�t.Checked && !radioButtonEk1.Checked &&
                     !radioButtonEk2.Checked && !radioButtonMazeret.Checked))
                {
                    MessageBox.Show("T�m zorunlu alanlar� doldurun ve bir s�nav t�r� se�in!", "Uyar�", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Cevap anahtar� kontrol�
                bool isTextBoxFilled = !string.IsNullOrEmpty(textBoxCvpA.Text) ||
                                       !string.IsNullOrEmpty(textBoxCvpB.Text) ||
                                       !string.IsNullOrEmpty(textBoxCvpC.Text) ||
                                       !string.IsNullOrEmpty(textBoxCvpD.Text);
                bool isFileSelected = !string.IsNullOrEmpty(selectedFilePath);

                if ((isTextBoxFilled && isFileSelected) || (!isTextBoxFilled && !isFileSelected))
                {
                    MessageBox.Show("Cevap anahtar�n� belirlemek i�in yaln�zca bir y�ntemi kullan�n: \n1. TextBox ile cevap anahtar�n� girin.\n2. Dosya se�in.",
                        "Uyar�", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Cevap anahtar�n� dosyadan y�kleme
                string[] cevapAnahtar� = new string[4];
                if (isFileSelected)
                {
                    try
                    {
                        string fileContent = File.ReadAllText(selectedFilePath);
                        cevapAnahtar� = fileContent.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Cevap anahtar� dosyas� okunamad�: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(textBoxCvpA.Text)) cevapAnahtar�[0] = textBoxCvpA.Text;
                    if (!string.IsNullOrEmpty(textBoxCvpB.Text)) cevapAnahtar�[1] = textBoxCvpB.Text;
                    if (!string.IsNullOrEmpty(textBoxCvpC.Text)) cevapAnahtar�[2] = textBoxCvpC.Text;
                    if (!string.IsNullOrEmpty(textBoxCvpD.Text)) cevapAnahtar�[3] = textBoxCvpD.Text;
                }

                // S�nav t�r�n� belirleme
                selectedExamType = radioButtonVize.Checked ? "Vize" :
                                   radioButtonFinal.Checked ? "Final" :
                                   radioButtonQuiz.Checked ? "Quiz" :
                                   radioButtonUyg.Checked ? "Uygulama" :
                                   radioButtonB�t.Checked ? "B�t�nleme" :
                                   radioButtonEk1.Checked ? "Ek S�nav-1" :
                                   radioButtonEk2.Checked ? "Ek S�nav-2" :
                                   radioButtonMazeret.Checked ? "Mazeret" : "";

                try
                {
                    // ��renci cevaplar�n� dosyadan okuma
                    string[] lines = File.ReadAllLines(selectedFilePath);
                    int maxQuestions = int.Parse(comboBoxSoruSay�s�.SelectedItem.ToString());
                    double questionScore = double.Parse(textBoxSoruPuan�.Text);

                    dataGridView1.Rows.Clear(); // �nceki de�erlendirmeleri temizle
                    foreach (string line in lines)
                    {
                        if (line.Length < 16)
                        {
                            MessageBox.Show("Sat�r uygun formatta de�il: " + line, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            continue;
                        }

                        string studentNumber = line.Substring(0, 9);
                        string bookletType = line[15].ToString();

                        // Do�ru cevap anahtar�n� belirle
                        string correctAnswers = "";
                        if (bookletType == "A") correctAnswers = cevapAnahtar�[0];
                        else if (bookletType == "B") correctAnswers = cevapAnahtar�[1];
                        else if (bookletType == "C") correctAnswers = cevapAnahtar�[2];
                        else if (bookletType == "D") correctAnswers = cevapAnahtar�[3];

                        if (string.IsNullOrEmpty(correctAnswers))
                        {
                            // Kitap��k t�r� yanl�� veya cevap anahtar� eksik
                            listBox1.Items.Add($"��renci No: {studentNumber}, Kitap��k t�r� yanl�� veya eksik!");
                            dataGridView1.Rows.Add(studentNumber, 0, 0, maxQuestions, 0); // Yanl�� cevaplar i�in s�f�r puan
                            continue;
                        }

                        int correctCount = 0, incorrectCount = 0, emptyCount = 0;
                        for (int i = 0; i < maxQuestions; i++)
                        {
                            if (line.Length <= 16 + i)
                            {
                                emptyCount += (maxQuestions - i);
                                break;
                            }

                            char studentAnswer = line[16 + i];
                            char correctAnswer = correctAnswers.Length > i ? correctAnswers[i] : ' ';

                            if (studentAnswer == ' ')
                                emptyCount++;
                            else if (studentAnswer == correctAnswer)
                                correctCount++;
                            else
                                incorrectCount++;
                        }

                        double totalScore = correctCount * questionScore;
                        dataGridView1.Rows.Add(studentNumber, correctCount, incorrectCount, emptyCount, totalScore);
                        listBox1.Items.Add($"��renci No: {studentNumber}, Do�ru: {correctCount}, Yanl��: {incorrectCount}, Bo�: {emptyCount}, Puan: {totalScore}");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }


        private void buttonExcellYazd�r_Click(object sender, EventArgs e)
        {
            try
            {
                // Dosya kaydetme
                string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "s�navDe�erlendirmeleri");
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath); // Klas�r yoksa olu�tur
                }

                // Yeni bir Excel dosyas� olu�tur
                using (var package = new ExcelPackage())
                {
                    // Yeni bir �al��ma kitab� olu�tur
                    var workbook = package.Workbook;
                    var worksheet = workbook.Worksheets.Add("S�nav De�erlendirmesi");

                    // Excel dosyas�n�n ilk sat�r�na ba�l�klar� yaz
                    worksheet.Cells[1, 1].Value = "S�nav No:";
                    worksheet.Cells[1, 2].Value = textBoxS�navNo.Text;
                    worksheet.Cells[1, 3].Value = "S�nav Ad�:";
                    worksheet.Cells[1, 4].Value = textBoxS�navAd.Text;
                    worksheet.Cells[1, 5].Value = "Dersi Veren:";
                    worksheet.Cells[1, 6].Value = textBoxDersiVeren.Text;
                    worksheet.Cells[1, 7].Value = "Tarih:";
                    worksheet.Cells[1, 8].Value = dateTimePicker1.Value.ToShortDateString();

                    // Ba�l�klar� kal�n yap
                    using (var range = worksheet.Cells["A1:H1"])
                    {
                        range.Style.Font.Bold = true;
                    }

                    // 2. sat�r� bo� b�rak
                    // 3. sat�ra ba�l�klar� yaz ve kal�n yap
                    worksheet.Cells[3, 1].Value = "��renci Numaras�";
                    worksheet.Cells[3, 2].Value = "Do�ru";
                    worksheet.Cells[3, 3].Value = "Yanl��";
                    worksheet.Cells[3, 4].Value = "Bo�";
                    worksheet.Cells[3, 5].Value = "Puan"; // Yeni Puan ba�l���

                    using (var headerRange = worksheet.Cells["A3:E3"]) // Ba�l�k alan�n� g�ncelledik
                    {
                        headerRange.Style.Font.Bold = true;
                    }

                    // ��renci verilerini yazma (5. sat�rdan itibaren)
                    int row = 5;
                    foreach (DataGridViewRow dataGridViewRow in dataGridView1.Rows)
                    {
                        if (dataGridViewRow.IsNewRow) continue; // Yeni sat�r kontrol�

                        worksheet.Cells[row, 1].Value = dataGridViewRow.Cells[0].Value; // ��renci No
                        worksheet.Cells[row, 2].Value = dataGridViewRow.Cells[1].Value; // Do�ru
                        worksheet.Cells[row, 3].Value = dataGridViewRow.Cells[2].Value; // Yanl��
                        worksheet.Cells[row, 4].Value = dataGridViewRow.Cells[3].Value; // Bo�
                        worksheet.Cells[row, 5].Value = dataGridViewRow.Cells[4].Value; // Puan

                        row++;
                    }

                    string fileName = $"{textBoxS�navNo.Text}_{textBoxS�navAd.Text}_{dateTimePicker1.Value.ToString("yyyyMMdd")}.xlsx";
                    string filePath = Path.Combine(folderPath, fileName);

                    // Dosyay� kaydet
                    package.SaveAs(new FileInfo(filePath));

                    // Excel dosyas�n� a�
                    System.Diagnostics.Process.Start(new ProcessStartInfo
                    {
                        FileName = filePath,
                        UseShellExecute = true // Bu, dosyay� varsay�lan programla a�ar
                    });

                    MessageBox.Show($"Excel dosyas� '{filePath}' olarak kaydedildi.", "Ba�ar�l�", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }
        private void Button�grenciCevaplar�Sec_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Metin Dosyalar� (*.txt)|*.txt|T�m Dosyalar (*.*)|*.*", // Filtreyi ayarl�yoruz
                Title = "Bir TXT dosyas� se�in" // Pencerenin ba�l���n� ayarl�yoruz
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                selectedFilePath = openFileDialog.FileName;
            }
        }

        private void buttonCevapAnahtar�Sec_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Metin Dosyalar� (*.txt)|*.txt|T�m Dosyalar (*.*)|*.*", // Filtreyi ayarl�yoruz
                Title = "Bir TXT dosyas� se�in" // Pencerenin ba�l���n� ayarl�yoruz
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                selectedFilePath = openFileDialog.FileName;
            }
        }
    }
}
