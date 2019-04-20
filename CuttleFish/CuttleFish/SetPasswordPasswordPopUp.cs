using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CuttleFish
{
    public partial class SetPasswordPasswordPopUp : Form
    {
        public string password = "";
        string setTitle = "";
        public SetPasswordPasswordPopUp(string title)
        {
            setTitle = title;
            InitializeComponent();
        }
        private void SetPasswordPasswordPopUp_Load(object sender, EventArgs e)
        {
            this.ActiveControl = txtPass;
            OkBtn.Enabled = false;
            txtPass.MaxLength = 32;
            this.Text = setTitle;
        }
        private void OkBtn_Click(object sender, EventArgs e)
        {
            if (txtPass.Text != "")
            {
                password = txtPass.Text;
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show("Password cannot be empty", "CuttleFish", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void txtPass_TextChanged(object sender, EventArgs e)
        {
            if (txtPass.TextLength >= 8)
            {
                OkBtn.Enabled = true;
            }
            else
            {
                OkBtn.Enabled = false;
            }
        }
    }
}
