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
    public partial class ManageStudentsForm : Form
    {
        public ManageStudentsForm()
        {
            InitializeComponent();
        }

        private DataTable dtStudents = new DataTable();

        private void ManageStudentsForm_Load(object sender, EventArgs e)
        {
            LoadDataIntoDataGridView();

        }

        private void LoadDataIntoDataGridView()
        {
            dtStudents = GetStudentsList();
            studentListDataGridView.DataSource = dtStudents;
        }

        private DataTable GetStudentsList()
        {
            DataTable dtStudents = new DataTable();
            string conString = ConfigurationManager.ConnectionStrings["dbx"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(conString))
            {
                using (SqlCommand cmd = new SqlCommand("usp_StudentGetAllStudentsInfo", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    conn.Open();

                    SqlDataReader reader = cmd.ExecuteReader();

                    dtStudents.Load(reader);

                    // conection.close();
                }
            }

            return dtStudents;
        }

        private void addNewStudentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // StudentInfoForm sif = new StudentInfoForm();
            //sif.ShowDialog();

            ShowStudentInfoForm(0, false);
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void nameTextBox_TextChanged(object sender, EventArgs e)
        {
            FilterStringByColumn("Name", nameTextBox);
        }

        private void emailTextBox_TextChanged(object sender, EventArgs e)
        {
            FilterStringByColumn("Email", emailTextBox);
        }

        private void addressTextBox_TextChanged(object sender, EventArgs e)
        {
            FilterStringByColumn("Address", addressTextBox);
        }

        private void FilterStringByColumn(string columnName, TextBox txtBox)
        {
            DataView dvStudents = dtStudents.DefaultView;

            dvStudents.RowFilter = columnName + " LIKE '%" + txtBox.Text + "%'";

        }

        private void resetFormToolStripMenuItem_Click(object sender, EventArgs e)
        {
            nameTextBox.Clear();
            emailTextBox.Clear();
            addressTextBox.Clear();
        }

        private void studentListDataGridView_DoubleClick(object sender, EventArgs e)
        {
            int rowtoUpdate = studentListDataGridView.Rows.GetFirstRow(DataGridViewElementStates.Selected);
            int studentId = Convert.ToInt16(studentListDataGridView.Rows[rowtoUpdate].Cells["StudentId"].Value);
            ShowStudentInfoForm(studentId, true);
        }

        private void ShowStudentInfoForm(int studentId, bool isUpdate)
        {
            StudentAdmissionForm sif = new StudentAdmissionForm();
            sif.StudentId = studentId;
            sif.IsUpdate = isUpdate;
            sif.ShowDialog();

            LoadDataIntoDataGridView();
        }
    }
}
