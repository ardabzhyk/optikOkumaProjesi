using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Optik_okuma_projesi
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        [Obsolete]
        private void buttonGirisYap_Click(object sender, EventArgs e)
        {
            // Kullanıcı adı ve şifreyi al
            string kullaniciAdi = textBoxKullanıcıAdı.Text.Trim();
            string sifre = textBoxSifre.Text.Trim();

            // Boş giriş kontrolü
            if (string.IsNullOrEmpty(kullaniciAdi) || string.IsNullOrEmpty(sifre))
            {
                MessageBox.Show("Lütfen kullanıcı adı ve şifreyi doldurun.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // SQL Server bağlantı dizesi
            string connectionString = "Data Source=ARDA;Initial Catalog=optikOkuyucu;Integrated Security=True;"; 

            // Veritabanı bağlantısı
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // SQL sorgusu
                    string query = "SELECT COUNT(*) FROM kullanicilar WHERE kullaniciAdi = @kullaniciAdi AND sifre = @sifre";

                    // Komut oluştur
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Parametreleri ekle
                        command.Parameters.AddWithValue("@kullaniciAdi", kullaniciAdi);
                        command.Parameters.AddWithValue("@sifre", sifre);

                        // Sorguyu çalıştır ve sonuç al
                        int userCount = (int)command.ExecuteScalar();

                        if (userCount > 0)
                        {
                            MessageBox.Show("Giriş başarılı! Hoş geldiniz.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            Form1 yeniForm = new Form1();
                            yeniForm.Show();
                            this.Hide(); // Mevcut formu gizler

                        }
                        else
                        {
                            MessageBox.Show("Kullanıcı adı veya şifre hatalı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Bağlantı hatası: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
