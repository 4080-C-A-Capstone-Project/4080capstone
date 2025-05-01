namespace EncryptorApp
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.TextBox txtInput;
        private System.Windows.Forms.TextBox txtFilePath;
        private System.Windows.Forms.TextBox txtDecryptFile;
        private System.Windows.Forms.TextBox txtKey;
        private System.Windows.Forms.RadioButton rbText;
        private System.Windows.Forms.RadioButton rbFile;
        private System.Windows.Forms.ComboBox cmbMethod;
        private System.Windows.Forms.ComboBox cmbDecrypt;
        private System.Windows.Forms.Button btnEncrypt;
        private System.Windows.Forms.Button btnDecrypt;

        private System.Windows.Forms.Button btnBrowseFile;
        private System.Windows.Forms.Button btnBrowseDecryptFile;

        private System.Windows.Forms.Label lblKey;
        private System.Windows.Forms.Label lblInputType;
        private System.Windows.Forms.Label lblTextInput;
        private System.Windows.Forms.Label lblFileInput;
        private System.Windows.Forms.Label lblEncryptMethod;
        private System.Windows.Forms.Label lblDecryptFile;
        private System.Windows.Forms.Label lblDecryptMethod;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            txtInput = new TextBox();
            txtFilePath = new TextBox();
            txtDecryptFile = new TextBox();
            txtKey = new TextBox();
            rbText = new RadioButton();
            rbFile = new RadioButton();
            cmbMethod = new ComboBox();
            cmbDecrypt = new ComboBox();
            btnEncrypt = new Button();
            btnDecrypt = new Button();
            btnBrowseFile = new Button();
            btnBrowseDecryptFile = new Button();
            lblKey = new Label();
            lblInputType = new Label();
            lblTextInput = new Label();
            lblFileInput = new Label();
            lblEncryptMethod = new Label();
            lblDecryptFile = new Label();
            lblDecryptMethod = new Label();
            SuspendLayout();
            // 
            // txtInput
            // 
            txtInput.Location = new Point(20, 80);
            txtInput.Multiline = true;
            txtInput.Name = "txtInput";
            txtInput.Size = new Size(300, 60);
            txtInput.TabIndex = 4;
            // 
            // txtFilePath
            // 
            txtFilePath.Location = new Point(20, 170);
            txtFilePath.Name = "txtFilePath";
            txtFilePath.ReadOnly = true;
            txtFilePath.Size = new Size(300, 25);
            txtFilePath.TabIndex = 6;
            // 
            // txtDecryptFile
            // 
            txtDecryptFile.Location = new Point(20, 340);
            txtDecryptFile.Name = "txtDecryptFile";
            txtDecryptFile.ReadOnly = true;
            txtDecryptFile.Size = new Size(300, 25);
            txtDecryptFile.TabIndex = 14;
            // 
            // txtKey
            // 
            txtKey.Location = new Point(150, 250);
            txtKey.Name = "txtKey";
            txtKey.Size = new Size(170, 25);
            txtKey.TabIndex = 11;
            // 
            // rbText
            // 
            rbText.Location = new Point(30, 30);
            rbText.Name = "rbText";
            rbText.Size = new Size(104, 24);
            rbText.TabIndex = 1;
            rbText.Text = "Text";
            // 
            // rbFile
            // 
            rbFile.Location = new Point(140, 30);
            rbFile.Name = "rbFile";
            rbFile.Size = new Size(104, 24);
            rbFile.TabIndex = 2;
            rbFile.Text = "File";
            // 
            // cmbMethod
            // 
            cmbMethod.Location = new Point(20, 220);
            cmbMethod.Name = "cmbMethod";
            cmbMethod.Size = new Size(150, 25);
            cmbMethod.TabIndex = 9;
            // 
            // cmbDecrypt
            // 
            cmbDecrypt.Location = new Point(20, 390);
            cmbDecrypt.Name = "cmbDecrypt";
            cmbDecrypt.Size = new Size(150, 25);
            cmbDecrypt.TabIndex = 17;
            // 
            // btnEncrypt
            // 
            btnEncrypt.Location = new Point(20, 280);
            btnEncrypt.Name = "btnEncrypt";
            btnEncrypt.Size = new Size(120, 30);
            btnEncrypt.TabIndex = 12;
            btnEncrypt.Text = "Encrypt & Save";
            btnEncrypt.Click += btnEncrypt_Click;
            // 
            // btnDecrypt
            // 
            btnDecrypt.Location = new Point(20, 420);
            btnDecrypt.Name = "btnDecrypt";
            btnDecrypt.Size = new Size(120, 30);
            btnDecrypt.TabIndex = 18;
            btnDecrypt.Text = "Decrypt & Save";
            btnDecrypt.Click += btnDecrypt_Click;
            // 
            // btnBrowseFile
            // 
            btnBrowseFile.Location = new Point(330, 170);
            btnBrowseFile.Name = "btnBrowseFile";
            btnBrowseFile.Size = new Size(75, 23);
            btnBrowseFile.TabIndex = 7;
            btnBrowseFile.Text = "Browse...";
            btnBrowseFile.Click += btnBrowseFile_Click;
            // 
            // btnBrowseDecryptFile
            // 
            btnBrowseDecryptFile.Location = new Point(330, 340);
            btnBrowseDecryptFile.Name = "btnBrowseDecryptFile";
            btnBrowseDecryptFile.Size = new Size(75, 23);
            btnBrowseDecryptFile.TabIndex = 15;
            btnBrowseDecryptFile.Text = "Browse...";
            btnBrowseDecryptFile.Click += btnBrowseDecryptFile_Click;
            // 
            // lblKey
            // 
            lblKey.Location = new Point(20, 250);
            lblKey.Name = "lblKey";
            lblKey.Size = new Size(124, 23);
            lblKey.TabIndex = 10;
            lblKey.Text = "Key (for AES only):";
            // 
            // lblInputType
            // 
            lblInputType.Location = new Point(20, 10);
            lblInputType.Name = "lblInputType";
            lblInputType.Size = new Size(114, 23);
            lblInputType.TabIndex = 0;
            lblInputType.Text = "Select input type:";
            // 
            // lblTextInput
            // 
            lblTextInput.Location = new Point(20, 54);
            lblTextInput.Name = "lblTextInput";
            lblTextInput.Size = new Size(141, 23);
            lblTextInput.TabIndex = 3;
            lblTextInput.Text = "Enter text to encrypt:";
            // 
            // lblFileInput
            // 
            lblFileInput.Location = new Point(20, 144);
            lblFileInput.Name = "lblFileInput";
            lblFileInput.Size = new Size(174, 23);
            lblFileInput.TabIndex = 5;
            lblFileInput.Text = "Or choose a file to encrypt:";
            // 
            // lblEncryptMethod
            // 
            lblEncryptMethod.Location = new Point(20, 194);
            lblEncryptMethod.Name = "lblEncryptMethod";
            lblEncryptMethod.Size = new Size(174, 23);
            lblEncryptMethod.TabIndex = 8;
            lblEncryptMethod.Text = "Choose encryption method:";
            // 
            // lblDecryptFile
            // 
            lblDecryptFile.Location = new Point(20, 314);
            lblDecryptFile.Name = "lblDecryptFile";
            lblDecryptFile.Size = new Size(141, 23);
            lblDecryptFile.TabIndex = 13;
            lblDecryptFile.Text = "Select file to decrypt:";
            // 
            // lblDecryptMethod
            // 
            lblDecryptMethod.Location = new Point(20, 364);
            lblDecryptMethod.Name = "lblDecryptMethod";
            lblDecryptMethod.Size = new Size(193, 23);
            lblDecryptMethod.TabIndex = 16;
            lblDecryptMethod.Text = "Method used for decryption:";
            // 
            // MainForm
            // 
            ClientSize = new Size(450, 480);
            Controls.Add(lblInputType);
            Controls.Add(rbText);
            Controls.Add(rbFile);
            Controls.Add(lblTextInput);
            Controls.Add(txtInput);
            Controls.Add(lblFileInput);
            Controls.Add(txtFilePath);
            Controls.Add(btnBrowseFile);
            Controls.Add(lblEncryptMethod);
            Controls.Add(cmbMethod);
            Controls.Add(lblKey);
            Controls.Add(txtKey);
            Controls.Add(btnEncrypt);
            Controls.Add(lblDecryptFile);
            Controls.Add(txtDecryptFile);
            Controls.Add(btnBrowseDecryptFile);
            Controls.Add(lblDecryptMethod);
            Controls.Add(cmbDecrypt);
            Controls.Add(btnDecrypt);
            Name = "MainForm";
            Text = "EncryptorApp";
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
