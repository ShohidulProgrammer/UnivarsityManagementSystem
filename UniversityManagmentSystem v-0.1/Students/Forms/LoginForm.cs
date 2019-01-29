using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UniversityManagmentSystem
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private int numberOfWrongAttempts = 0;

        private void LoginForm_Load(object sender, EventArgs e)
        {
           // userNameTextBox.Text = Properties.Settings.Default.UserName;
           // passwordTextBox.Text = Properties.Settings.Default.Password;
        }

        private void signInbutton_Click(object sender, EventArgs e)
        {
            if(IsValidated())
            {
                try
                {
                    bool isUserNameCorrect, isPasswordCorrect, isActive;
                    GetIsUserLoginCorrect(out isUserNameCorrect, out isPasswordCorrect, out isActive);

                    if(isUserNameCorrect && isPasswordCorrect)
                    {
                        if(isActive )
                        {
                            this.Hide();

                            if (rememberMeCheckBox.Checked)
                            {
                             //   Properties.Settings.Default.UserName = userNameTextBox.Text;
                             //   Properties.Settings.Default.Password = passwordTextBox.Text;
                                Properties.Settings.Default.Save();
                            }
                            
                            ManageStudentsForm msf = new ManageStudentsForm();
                            msf.ShowDialog();
                        }
                        else
                        {
                            MessageBox.Show("Your Account is not active. Please consult adminstrator for further info.",
                                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            clearLoginInfoTextBox();
                        }
                    }
                    else
                    {
                        numberOfWrongAttempts++;

                        if (!isUserNameCorrect)
                        {
                            MessageBox.Show("User Name is not Correct.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            clearLoginInfoTextBox();
                        }
                        else
                        {
                            MessageBox.Show("Password is not Correct.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                            if(numberOfWrongAttempts >= 3)
                            {
                                DisableThisAccount();

                                MessageBox.Show("The account associated with this username is disabled now. \nPlease contact Admistrator.", 
                                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }

                            clearLoginInfoTextBox();
                        }
                    }
                }
                catch(ApplicationException ex)
                {
                    MessageBox.Show("Error: " +ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void DisableThisAccount()
        {
            string connString = ConfigurationManager.ConnectionStrings["dbx"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                using (SqlCommand cmd = new SqlCommand("usp_UserDisableThisAccount", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    conn.Open();

                    cmd.Parameters.AddWithValue("@UserName", userNameTextBox.Text);

                    cmd.ExecuteNonQuery();

                    // conection.close();
                }
            }
        }

        private void clearLoginInfoTextBox()
        {
            userNameTextBox.Clear();
            passwordTextBox.Clear();
            userNameTextBox.Focus();
            
        }

        private void clearPasswordTextBox()
        {
            passwordTextBox.Clear();
            passwordTextBox.Focus();
        }

        private void GetIsUserLoginCorrect(out bool isUserNameCorrect, out bool isPasswordCorrect, out  bool isActive)
        {
            string connString = ConfigurationManager.ConnectionStrings["dbx"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connString))
            {
                using (SqlCommand cmd = new SqlCommand("usp_UserCheckLoginDetails", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    conn.Open();

                    // Output Parameter
                    cmd.Parameters.Add("@IsUserNameCorrect", SqlDbType.Bit).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("@IsPasswordCorrect", SqlDbType.Bit).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("@IsActive", SqlDbType.Bit).Direction = ParameterDirection.Output;

                    // Paramer
                    cmd.Parameters.AddWithValue("@UserName", userNameTextBox.Text);
                    cmd.Parameters.AddWithValue("@Password", passwordTextBox.Text);

                    cmd.ExecuteNonQuery();

                    isUserNameCorrect = (bool)cmd.Parameters["@IsUserNameCorrect"].Value;
                    isPasswordCorrect = (bool)cmd.Parameters["@IsPasswordCorrect"].Value;
                    isActive = (bool)cmd.Parameters["@IsActive"].Value;

                    // conection.close();

                }
            }
        }

        private bool IsValidated()
        {
            if(userNameTextBox.Text.Trim() == string.Empty)
            {
                MessageBox.Show("UserName is Required.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                clearLoginInfoTextBox();

                return false;
            }

            if(passwordTextBox.Text.Trim() == string.Empty)
            {
                MessageBox.Show("password is Required.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                clearPasswordTextBox();

                return false;

            }

            return true;
        }



        private void exitButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void signUpButton_Click(object sender, EventArgs e)
        {
            this.Hide();

            ValidationForm vdf = new ValidationForm();
            vdf.ShowDialog();
           // Application.Run(new ValidationForm());
        }
    }
}
