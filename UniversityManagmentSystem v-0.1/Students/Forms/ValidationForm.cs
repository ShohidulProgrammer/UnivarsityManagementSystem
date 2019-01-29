using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UniversityManagmentSystem
{
    public partial class ValidationForm : Form
    {
        public ValidationForm()
        {
            InitializeComponent();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if(IsValidated())
            {
                MessageBox.Show("New Record is Added successfully", "Record Added", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private bool IsValidated()
        {
            // if (nameTextBox.Text.Trim() == string.Empty)
            if (nameTextBox.Text == "")
            {
                ValidationMessage(nameTextBox, "Name Field is required");
                return false;
            }

            // if(ageTextBox.Text.Trim() ==string.Empty)
            if (ageTextBox.Text.Trim() == "")
            {
                ValidationMessage(ageTextBox, "Age Feild is Required");
                return false;
            }
            else
            {
                int outAge;
                // char c = ageTextBox.Text;
                // if(!IsDigit(Char c))
                if (!int.TryParse(ageTextBox.Text, out outAge))
                {
                    ValidationMessage(ageTextBox, "Age Field should only have numbers");
                    return false;
                }
            }

            if(qualificationComboBox.Text == "")
            {
                ValidationMessage(qualificationComboBox, "Please select hieghest qualification");
                return false;
            }

            if(DateTime.Today.AddYears(-18) < dateOfBirthDateTimePicker.Value.Date)
            {
                ValidationMessage(dateOfBirthDateTimePicker, "You Should be Over 18 to Register");
                return false;
            }
            return true;
        }

        private void ValidationMessage
            (Control ctrl, string validationMessage)
        {
            ctrl.BackColor = Color.LightPink;
            MessageBox.Show(validationMessage, "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
            ctrl.Focus();
        }

        private void ctrl_TextChanged(object sender, EventArgs e)
        {
            Control ctrl = (Control)sender;
            ctrl.BackColor = Color.White;
        }

        private void nameTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if((!char.IsLetter(e.KeyChar)) && (!char.IsWhiteSpace(e.KeyChar)) && (!char.IsControl(e.KeyChar)))
            {
                e.Handled = true;
            }
        }

        private void ageTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((!char.IsNumber(e.KeyChar)) && (!char.IsControl(e.KeyChar)))
            {
                e.Handled = true;
            }
        }

        private void emailTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((!char.IsLetterOrDigit(e.KeyChar)) && (!char.IsControl(e.KeyChar)) && (e.KeyChar != 46) && (e.KeyChar != 64))
            {
                e.Handled = true;
            }
        }

        private void ctrl_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                this.SelectNextControl((Control)sender, true, true, true, true);
            }
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
