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
        private System.Windows.Forms.TextBox txtDecryptKey;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            lblDecryptKey = new Label();
            txtDecryptKey = new TextBox();
            menuStrip = new MenuStrip();
            userMenu = new ToolStripMenuItem();
            setUsernameMenuItem = new ToolStripMenuItem();
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
            menuStrip.SuspendLayout();
            SuspendLayout();
            // 
            // lblDecryptKey
            // 
            lblDecryptKey.Location = new Point(221, 383);
            lblDecryptKey.Name = "lblDecryptKey";
            lblDecryptKey.Size = new Size(135, 23);
            lblDecryptKey.TabIndex = 0;
            lblDecryptKey.Text = "Enter decryption key:";
            // 
            // txtDecryptKey
            // 
            txtDecryptKey.Location = new Point(221, 409);
            txtDecryptKey.Name = "txtDecryptKey";
            txtDecryptKey.Size = new Size(300, 25);
            txtDecryptKey.TabIndex = 1;
            // 
            // menuStrip
            // 
            menuStrip.Items.AddRange(new ToolStripItem[] { userMenu });
            menuStrip.Location = new Point(0, 0);
            menuStrip.Name = "menuStrip";
            menuStrip.Size = new Size(537, 25);
            menuStrip.TabIndex = 0;
            // 
            // userMenu
            // 
            userMenu.DropDownItems.AddRange(new ToolStripItem[] { setUsernameMenuItem });
            userMenu.Name = "userMenu";
            userMenu.Size = new Size(47, 21);
            userMenu.Text = "User";
            // 
            // setUsernameMenuItem
            // 
            setUsernameMenuItem.Name = "setUsernameMenuItem";
            setUsernameMenuItem.Size = new Size(157, 22);
            setUsernameMenuItem.Text = "Set Username";
            setUsernameMenuItem.Click += SetUsername_Click;
            // 
            // txtInput
            // 
            txtInput.Location = new Point(22, 95);
            txtInput.Multiline = true;
            txtInput.Name = "txtInput";
            txtInput.Size = new Size(300, 60);
            txtInput.TabIndex = 4;
            // 
            // txtFilePath
            // 
            txtFilePath.Location = new Point(22, 185);
            txtFilePath.Name = "txtFilePath";
            txtFilePath.ReadOnly = true;
            txtFilePath.Size = new Size(300, 25);
            txtFilePath.TabIndex = 6;
            // 
            // txtDecryptFile
            // 
            txtDecryptFile.Location = new Point(22, 355);
            txtDecryptFile.Name = "txtDecryptFile";
            txtDecryptFile.ReadOnly = true;
            txtDecryptFile.Size = new Size(300, 25);
            txtDecryptFile.TabIndex = 14;
            // 
            // txtKey
            // 
            txtKey.Location = new Point(128, 264);
            txtKey.Name = "txtKey";
            txtKey.Size = new Size(289, 25);
            txtKey.TabIndex = 11;
            // 
            // rbText
            // 
            rbText.Location = new Point(32, 45);
            rbText.Name = "rbText";
            rbText.Size = new Size(104, 24);
            rbText.TabIndex = 1;
            rbText.Text = "Text";
            // 
            // rbFile
            // 
            rbFile.Location = new Point(142, 45);
            rbFile.Name = "rbFile";
            rbFile.Size = new Size(104, 24);
            rbFile.TabIndex = 2;
            rbFile.Text = "File";
            // 
            // cmbMethod
            // 
            cmbMethod.Location = new Point(22, 235);
            cmbMethod.Name = "cmbMethod";
            cmbMethod.Size = new Size(150, 25);
            cmbMethod.TabIndex = 9;
            // 
            // cmbDecrypt
            // 
            cmbDecrypt.Location = new Point(22, 409);
            cmbDecrypt.Name = "cmbDecrypt";
            cmbDecrypt.Size = new Size(150, 25);
            cmbDecrypt.TabIndex = 17;
            // 
            // btnEncrypt
            // 
            btnEncrypt.Location = new Point(22, 295);
            btnEncrypt.Name = "btnEncrypt";
            btnEncrypt.Size = new Size(120, 30);
            btnEncrypt.TabIndex = 12;
            btnEncrypt.Text = "Encrypt & Save";
            btnEncrypt.Click += btnEncrypt_Click;
            // 
            // btnDecrypt
            // 
            btnDecrypt.Location = new Point(22, 440);
            btnDecrypt.Name = "btnDecrypt";
            btnDecrypt.Size = new Size(120, 30);
            btnDecrypt.TabIndex = 18;
            btnDecrypt.Text = "Decrypt & Save";
            btnDecrypt.Click += btnDecrypt_Click;
            // 
            // btnBrowseFile
            // 
            btnBrowseFile.Location = new Point(332, 185);
            btnBrowseFile.Name = "btnBrowseFile";
            btnBrowseFile.Size = new Size(75, 23);
            btnBrowseFile.TabIndex = 7;
            btnBrowseFile.Text = "Browse...";
            btnBrowseFile.Click += btnBrowseFile_Click;
            // 
            // btnBrowseDecryptFile
            // 
            btnBrowseDecryptFile.Location = new Point(332, 355);
            btnBrowseDecryptFile.Name = "btnBrowseDecryptFile";
            btnBrowseDecryptFile.Size = new Size(75, 23);
            btnBrowseDecryptFile.TabIndex = 15;
            btnBrowseDecryptFile.Text = "Browse...";
            btnBrowseDecryptFile.Click += btnBrowseDecryptFile_Click;
            // 
            // lblKey
            // 
            lblKey.Location = new Point(22, 265);
            lblKey.Name = "lblKey";
            lblKey.Size = new Size(100, 23);
            lblKey.TabIndex = 10;
            lblKey.Text = "Generated Key:";
            // 
            // lblInputType
            // 
            lblInputType.Location = new Point(22, 25);
            lblInputType.Name = "lblInputType";
            lblInputType.Size = new Size(114, 23);
            lblInputType.TabIndex = 0;
            lblInputType.Text = "Select input type:";
            // 
            // lblTextInput
            // 
            lblTextInput.Location = new Point(22, 69);
            lblTextInput.Name = "lblTextInput";
            lblTextInput.Size = new Size(141, 23);
            lblTextInput.TabIndex = 3;
            lblTextInput.Text = "Enter text to encrypt:";
            // 
            // lblFileInput
            // 
            lblFileInput.Location = new Point(22, 159);
            lblFileInput.Name = "lblFileInput";
            lblFileInput.Size = new Size(174, 23);
            lblFileInput.TabIndex = 5;
            lblFileInput.Text = "Or choose a file to encrypt:";
            // 
            // lblEncryptMethod
            // 
            lblEncryptMethod.Location = new Point(22, 209);
            lblEncryptMethod.Name = "lblEncryptMethod";
            lblEncryptMethod.Size = new Size(174, 23);
            lblEncryptMethod.TabIndex = 8;
            lblEncryptMethod.Text = "Choose encryption method:";
            // 
            // lblDecryptFile
            // 
            lblDecryptFile.Location = new Point(22, 329);
            lblDecryptFile.Name = "lblDecryptFile";
            lblDecryptFile.Size = new Size(141, 23);
            lblDecryptFile.TabIndex = 13;
            lblDecryptFile.Text = "Select file to decrypt:";
            // 
            // lblDecryptMethod
            // 
            lblDecryptMethod.Location = new Point(22, 383);
            lblDecryptMethod.Name = "lblDecryptMethod";
            lblDecryptMethod.Size = new Size(193, 23);
            lblDecryptMethod.TabIndex = 16;
            lblDecryptMethod.Text = "Method used for decryption:";
            // 
            // MainForm
            // 
            ClientSize = new Size(537, 509);
            Controls.Add(lblDecryptKey);
            Controls.Add(txtDecryptKey);
            Controls.Add(menuStrip);
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
            MainMenuStrip = menuStrip;
            Name = "MainForm";
            Text = "EncryptorApp";
            menuStrip.ResumeLayout(false);
            menuStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        private MenuStrip menuStrip;
        private ToolStripMenuItem userMenu;
        private ToolStripMenuItem setUsernameMenuItem;
        private Label lblDecryptKey;
    }
}
