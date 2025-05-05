using System;
using System.Windows.Forms;


namespace _4080capstone
{
    public partial class TextInputForm : Form
    {
        public string UserInput { get; private set; }

        public TextInputForm(string existingText = "")
        {
            InitializeComponent();
            txtEditor.Text = existingText;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            UserInput = txtEditor.Text;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}