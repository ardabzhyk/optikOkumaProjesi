using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using OfficeOpenXml; // EPPlus için using yönergesi


namespace Optik_okuma_projesi
{
    public partial class Form1 : Form
    {
        private string selectedFilePath = "";
        private string selectedExamType = "";

        public Form1()
        {

            InitializeComponent();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Lisans ayarýný burada yapýn
            // Diðer baþlatma kodlarý...

            dataGridView1.ColumnCount = 5;
            dataGridView1.Columns[0].Name = "Numara";
            dataGridView1.Columns[1].Name = "Doðru Sayýsý";
            dataGridView1.Columns[2].Name = "Yanlýþ Sayýsý";
            dataGridView1.Columns[3].Name = "Boþ Sayýsý";
            dataGridView1.Columns[4].Name = "Puan";
        }


        private void comboBoxGrupSayýsý_SelectedIndexChanged(object sender, EventArgs e)
        {
            int maxChar = 0;

            // Grup sayýsýna göre karakter sýnýrý belirle
            if (comboBoxGrupSayýsý.SelectedItem.ToString() == "25")
                maxChar = 25;
            else if (comboBoxGrupSayýsý.SelectedItem.ToString() == "50")
                maxChar = 50;
            else if (comboBoxGrupSayýsý.SelectedItem.ToString() == "100")
                maxChar = 100;

            // Tüm textbox'lar için MaxLength ayarla
            textBoxCvpA.MaxLength = maxChar;
            textBoxCvpB.MaxLength = maxChar;
            textBoxCvpC.MaxLength = maxChar;
            textBoxCvpD.MaxLength = maxChar;
        }

        private void buttonDegerlendirme_Click(object sender, EventArgs e)
        {

           
                // Zorunlu alan kontrolü
                if (string.IsNullOrEmpty(textBoxSýnavNo.Text) ||
                    string.IsNullOrEmpty(textBoxSýnavAd.Text) ||
                    string.IsNullOrEmpty(textBoxDersiVeren.Text) ||
                    comboBoxGrupSayýsý.SelectedIndex == -1 ||
                    comboBoxSoruSayýsý.SelectedIndex == -1 ||
                    string.IsNullOrEmpty(textBoxSoruPuaný.Text) ||
                    (!radioButtonVize.Checked && !radioButtonFinal.Checked &&
                     !radioButtonQuiz.Checked && !radioButtonUyg.Checked &&
                     !radioButtonBüt.Checked && !radioButtonEk1.Checked &&
                     !radioButtonEk2.Checked && !radioButtonMazeret.Checked))
                {
                    MessageBox.Show("Tüm zorunlu alanlarý doldurun ve bir sýnav türü seçin!", "Uyarý", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Cevap anahtarý kontrolü
                bool isTextBoxFilled = !string.IsNullOrEmpty(textBoxCvpA.Text) ||
                                       !string.IsNullOrEmpty(textBoxCvpB.Text) ||
                                       !string.IsNullOrEmpty(textBoxCvpC.Text) ||
                                       !string.IsNullOrEmpty(textBoxCvpD.Text);
                bool isFileSelected = !string.IsNullOrEmpty(selectedFilePath);

                if ((isTextBoxFilled && isFileSelected) || (!isTextBoxFilled && !isFileSelected))
                {
                    MessageBox.Show("Cevap anahtarýný belirlemek için yalnýzca bir yöntemi kullanýn: \n1. TextBox ile cevap anahtarýný girin.\n2. Dosya seçin.",
                        "Uyarý", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Cevap anahtarýný dosyadan yükleme
                string[] cevapAnahtarý = new string[4];
                if (isFileSelected)
                {
                    try
                    {
                        string fileContent = File.ReadAllText(selectedFilePath);
                        cevapAnahtarý = fileContent.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Cevap anahtarý dosyasý okunamadý: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(textBoxCvpA.Text)) cevapAnahtarý[0] = textBoxCvpA.Text;
                    if (!string.IsNullOrEmpty(textBoxCvpB.Text)) cevapAnahtarý[1] = textBoxCvpB.Text;
                    if (!string.IsNullOrEmpty(textBoxCvpC.Text)) cevapAnahtarý[2] = textBoxCvpC.Text;
                    if (!string.IsNullOrEmpty(textBoxCvpD.Text)) cevapAnahtarý[3] = textBoxCvpD.Text;
                }

                // Sýnav türünü belirleme
                selectedExamType = radioButtonVize.Checked ? "Vize" :
                                   radioButtonFinal.Checked ? "Final" :
                                   radioButtonQuiz.Checked ? "Quiz" :
                                   radioButtonUyg.Checked ? "Uygulama" :
                                   radioButtonBüt.Checked ? "Bütünleme" :
                                   radioButtonEk1.Checked ? "Ek Sýnav-1" :
                                   radioButtonEk2.Checked ? "Ek Sýnav-2" :
                                   radioButtonMazeret.Checked ? "Mazeret" : "";

                try
                {
                    // Öðrenci cevaplarýný dosyadan okuma
                    string[] lines = File.ReadAllLines(selectedFilePath);
                    int maxQuestions = int.Parse(comboBoxSoruSayýsý.SelectedItem.ToString());
                    double questionScore = double.Parse(textBoxSoruPuaný.Text);

                    dataGridView1.Rows.Clear(); // Önceki deðerlendirmeleri temizle
                    foreach (string line in lines)
                    {
                        if (line.Length < 16)
                        {
                            MessageBox.Show("Satýr uygun formatta deðil: " + line, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            continue;
                        }

                        string studentNumber = line.Substring(0, 9);
                        string bookletType = line[15].ToString();

                        // Doðru cevap anahtarýný belirle
                        string correctAnswers = "";
                        if (bookletType == "A") correctAnswers = cevapAnahtarý[0];
                        else if (bookletType == "B") correctAnswers = cevapAnahtarý[1];
                        else if (bookletType == "C") correctAnswers = cevapAnahtarý[2];
                        else if (bookletType == "D") correctAnswers = cevapAnahtarý[3];

                        if (string.IsNullOrEmpty(correctAnswers))
                        {
                            // Kitapçýk türü yanlýþ veya cevap anahtarý eksik
                            listBox1.Items.Add($"Öðrenci No: {studentNumber}, Kitapçýk türü yanlýþ veya eksik!");
                            dataGridView1.Rows.Add(studentNumber, 0, 0, maxQuestions, 0); // Yanlýþ cevaplar için sýfýr puan
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
                        listBox1.Items.Add($"Öðrenci No: {studentNumber}, Doðru: {correctCount}, Yanlýþ: {incorrectCount}, Boþ: {emptyCount}, Puan: {totalScore}");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }


        private void buttonExcellYazdýr_Click(object sender, EventArgs e)
        {
            try
            {
                // Dosya kaydetme
                string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "sýnavDeðerlendirmeleri");
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath); // Klasör yoksa oluþtur
                }

                // Yeni bir Excel dosyasý oluþtur
                using (var package = new ExcelPackage())
                {
                    // Yeni bir çalýþma kitabý oluþtur
                    var workbook = package.Workbook;
                    var worksheet = workbook.Worksheets.Add("Sýnav Deðerlendirmesi");

                    // Excel dosyasýnýn ilk satýrýna baþlýklarý yaz
                    worksheet.Cells[1, 1].Value = "Sýnav No:";
                    worksheet.Cells[1, 2].Value = textBoxSýnavNo.Text;
                    worksheet.Cells[1, 3].Value = "Sýnav Adý:";
                    worksheet.Cells[1, 4].Value = textBoxSýnavAd.Text;
                    worksheet.Cells[1, 5].Value = "Dersi Veren:";
                    worksheet.Cells[1, 6].Value = textBoxDersiVeren.Text;
                    worksheet.Cells[1, 7].Value = "Tarih:";
                    worksheet.Cells[1, 8].Value = dateTimePicker1.Value.ToShortDateString();

                    // Baþlýklarý kalýn yap
                    using (var range = worksheet.Cells["A1:H1"])
                    {
                        range.Style.Font.Bold = true;
                    }

                    // 2. satýrý boþ býrak
                    // 3. satýra baþlýklarý yaz ve kalýn yap
                    worksheet.Cells[3, 1].Value = "Öðrenci Numarasý";
                    worksheet.Cells[3, 2].Value = "Doðru";
                    worksheet.Cells[3, 3].Value = "Yanlýþ";
                    worksheet.Cells[3, 4].Value = "Boþ";
                    worksheet.Cells[3, 5].Value = "Puan"; // Yeni Puan baþlýðý

                    using (var headerRange = worksheet.Cells["A3:E3"]) // Baþlýk alanýný güncelledik
                    {
                        headerRange.Style.Font.Bold = true;
                    }

                    // Öðrenci verilerini yazma (5. satýrdan itibaren)
                    int row = 5;
                    foreach (DataGridViewRow dataGridViewRow in dataGridView1.Rows)
                    {
                        if (dataGridViewRow.IsNewRow) continue; // Yeni satýr kontrolü

                        worksheet.Cells[row, 1].Value = dataGridViewRow.Cells[0].Value; // Öðrenci No
                        worksheet.Cells[row, 2].Value = dataGridViewRow.Cells[1].Value; // Doðru
                        worksheet.Cells[row, 3].Value = dataGridViewRow.Cells[2].Value; // Yanlýþ
                        worksheet.Cells[row, 4].Value = dataGridViewRow.Cells[3].Value; // Boþ
                        worksheet.Cells[row, 5].Value = dataGridViewRow.Cells[4].Value; // Puan

                        row++;
                    }

                    string fileName = $"{textBoxSýnavNo.Text}_{textBoxSýnavAd.Text}_{dateTimePicker1.Value.ToString("yyyyMMdd")}.xlsx";
                    string filePath = Path.Combine(folderPath, fileName);

                    // Dosyayý kaydet
                    package.SaveAs(new FileInfo(filePath));

                    // Excel dosyasýný aç
                    System.Diagnostics.Process.Start(new ProcessStartInfo
                    {
                        FileName = filePath,
                        UseShellExecute = true // Bu, dosyayý varsayýlan programla açar
                    });

                    MessageBox.Show($"Excel dosyasý '{filePath}' olarak kaydedildi.", "Baþarýlý", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }
        private void ButtonÖgrenciCevaplarýSec_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Metin Dosyalarý (*.txt)|*.txt|Tüm Dosyalar (*.*)|*.*", // Filtreyi ayarlýyoruz
                Title = "Bir TXT dosyasý seçin" // Pencerenin baþlýðýný ayarlýyoruz
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                selectedFilePath = openFileDialog.FileName;
            }
        }

        private void buttonCevapAnahtarýSec_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Metin Dosyalarý (*.txt)|*.txt|Tüm Dosyalar (*.*)|*.*", // Filtreyi ayarlýyoruz
                Title = "Bir TXT dosyasý seçin" // Pencerenin baþlýðýný ayarlýyoruz
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                selectedFilePath = openFileDialog.FileName;
            }
        }
    }
}
