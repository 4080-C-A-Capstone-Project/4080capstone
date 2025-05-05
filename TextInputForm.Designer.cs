using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _4080capstone
{
    partial class TextInputForm
    {
        private System.ComponentModel.IContainer components = null;
        private TextBox txtEditor;
        private Button btnSave;
        private Button btnCancel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            txtEditor = new TextBox();
            btnSave = new Button();
            btnCancel = new Button();

            SuspendLayout();

            // txtEditor
            txtEditor.Multiline = true;
            txtEditor.Location = new System.Drawing.Point(12, 12);
            txtEditor.Size = new System.Drawing.Size(360, 180);
            txtEditor.ScrollBars = ScrollBars.Vertical;

            // btnSave
            btnSave.Text = "Save";
            btnSave.Location = new System.Drawing.Point(216, 200);
            btnSave.Click += new EventHandler(btnSave_Click);

            // btnCancel
            btnCancel.Text = "Cancel";
            btnCancel.Location = new System.Drawing.Point(297, 200);
            btnCancel.Click += new EventHandler(btnCancel_Click);

            // Form
            ClientSize = new System.Drawing.Size(384, 240);
            Controls.Add(txtEditor);
            Controls.Add(btnSave);
            Controls.Add(btnCancel);
            Text = "Edit Text";

            ResumeLayout(false);
            PerformLayout();
        }
    }
}
