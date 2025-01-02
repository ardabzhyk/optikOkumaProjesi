namespace Optik_okuma_projesi
{
    partial class Form2
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            buttonGirisYap = new Button();
            textBoxKullanıcıAdı = new TextBox();
            textBoxSifre = new TextBox();
            label1 = new Label();
            label2 = new Label();
            SuspendLayout();
            // 
            // buttonGirisYap
            // 
            buttonGirisYap.Location = new Point(338, 263);
            buttonGirisYap.Name = "buttonGirisYap";
            buttonGirisYap.Size = new Size(75, 23);
            buttonGirisYap.TabIndex = 0;
            buttonGirisYap.Text = "Giriş yap";
            buttonGirisYap.UseVisualStyleBackColor = true;
            buttonGirisYap.Click += buttonGirisYap_Click;
            // 
            // textBoxKullanıcıAdı
            // 
            textBoxKullanıcıAdı.Location = new Point(318, 146);
            textBoxKullanıcıAdı.Name = "textBoxKullanıcıAdı";
            textBoxKullanıcıAdı.Size = new Size(158, 23);
            textBoxKullanıcıAdı.TabIndex = 1;
            // 
            // textBoxSifre
            // 
            textBoxSifre.Location = new Point(318, 194);
            textBoxSifre.Name = "textBoxSifre";
            textBoxSifre.Size = new Size(158, 23);
            textBoxSifre.TabIndex = 2;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(223, 149);
            label1.Name = "label1";
            label1.Size = new Size(76, 15);
            label1.TabIndex = 3;
            label1.Text = "Kullanıcı Adı:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(238, 202);
            label2.Name = "label2";
            label2.Size = new Size(33, 15);
            label2.TabIndex = 4;
            label2.Text = "Şifre:";
            // 
            // Form2
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(textBoxSifre);
            Controls.Add(textBoxKullanıcıAdı);
            Controls.Add(buttonGirisYap);
            Name = "Form2";
            Text = "Form2";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button buttonGirisYap;
        private TextBox textBoxKullanıcıAdı;
        private TextBox textBoxSifre;
        private Label label1;
        private Label label2;
    }
}