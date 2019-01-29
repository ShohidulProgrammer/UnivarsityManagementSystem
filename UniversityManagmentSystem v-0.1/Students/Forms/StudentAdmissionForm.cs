using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using UniversityManagmentSystem.SystemConstants;
using System.IO;
using UniversityManagmentSystem.Properties;
using System.Collections;

namespace UniversityManagmentSystem
{
    public partial class StudentAdmissionForm : Form
    {
        public StudentAdmissionForm()
        {
            InitializeComponent();
        }

        // StudentId
        private int studentId = 0;
        public int StudentId
        {
            get { return studentId; }
            set { studentId = value; }
        }

        // IsUpdate
        private bool isUpdate = false;
        public bool IsUpdate
        {
            get { return isUpdate; }
            set { isUpdate = value; }
        }

        // Original Row Version
        public byte[] OriginalRowVersion { get; set; }


        public enum Gender
        {
            NoSelection = 0,
            Male        = 1,
            Female      = 2
        }

        public enum MarritalStatus
        {
            NoSelection = 0,
            Single = 1,
            Married = 2
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }


        private void studentNameTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void emailTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if(this.IsUpdate)
            {
                // If Data is already Updated then the user message and reload the refresh data for re-update.

                if(IfDataNotUpdated(OriginalRowVersion, GetCurrentRowVersion()))
                {
                     // Execute Update Code
                      UpdateStudentDetails();

                       MessageBox.Show("Student Information is Updated Succesfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("The record is updated by another user. Record will be reloaded now. Please update the record again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    // Reload the data
                    LoadAndMapDataToControlIfUpdate();
                }
                
            }
            else
            {
                // Execute Insert Code
                InsertStudentDetails(out this.studentId);

                this.IsUpdate = true;

                MessageBox.Show("Student Info added to the System.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private byte[] GetCurrentRowVersion()
        {
            byte[] currentRowVersion = new byte[8];

            string connString = ConfigurationManager.ConnectionStrings["dbx"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connString))
            {
                using (SqlCommand cmd = new SqlCommand("usp_GetCurrentRowVersion", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@StudentId", this.StudentId);

                    conn.Open();

                    currentRowVersion = (byte[])cmd.ExecuteScalar();
                }
            }

            return currentRowVersion;
        }

        private bool IfDataNotUpdated(byte[] originalRowVersion, byte[] currentRowVersion)
        {
            return StructuralComparisons.StructuralEqualityComparer.Equals(originalRowVersion,currentRowVersion);
        }

        private void UpdateStudentDetails()
        {
            string connString = ConfigurationManager.ConnectionStrings["dbx"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                using (SqlCommand cmd = new SqlCommand("usp_StudentUpdateDetails", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // parameter
                    cmd.Parameters.AddWithValue("@Name", studentNameTextBox.Text);
                    cmd.Parameters.AddWithValue("@SemisterId", (semisterNameComboBox.SelectedIndex == -1) ? 0 : semisterNameComboBox.SelectedValue);
                    cmd.Parameters.AddWithValue("@DepartmentId", (departmentNameComboBox.SelectedIndex == -1) ? 0 : departmentNameComboBox.SelectedValue);

                    cmd.Parameters.AddWithValue("@GenderId", GetGender());
                    cmd.Parameters.AddWithValue("@DateOfBirth", (dateOfBirthDateTimePicker.Text.Trim() == string.Empty) ? (DateTime?)null : dateOfBirthDateTimePicker.Value.Date);
                    cmd.Parameters.AddWithValue("@MaritalStatusId", MaritalStatus());
                    cmd.Parameters.AddWithValue("@DepartmentId", (bloodGroupComboBox.SelectedIndex == -1) ? 0 : bloodGroupComboBox.SelectedValue);
                    cmd.Parameters.AddWithValue("@DepartmentId", (religionComboBox.SelectedIndex == -1) ? 0 : religionComboBox.SelectedValue);
                    cmd.Parameters.AddWithValue("@Nationality", nationalityTextBox.Text);
                    cmd.Parameters.AddWithValue("@ContactNo", contactNoTextBox.Text);
                    cmd.Parameters.AddWithValue("Email", emailTextBox.Text);

                    cmd.Parameters.AddWithValue("@FathersName", fathersNameTextBox.Text);
                    cmd.Parameters.AddWithValue("@FathersMobileNo", fathersMobileNoTextBox.Text);
                    cmd.Parameters.AddWithValue("@MothersName", mothersNameTextBox.Text);
                    cmd.Parameters.AddWithValue("@MothersMobileNo", mothersMobileNoTextBox.Text);
                    cmd.Parameters.AddWithValue("@GuardianName", guardianNameTextBox.Text);
                    cmd.Parameters.AddWithValue("@GuardianMobileNo", guardiansMobileNoTextBox.Text);
                    cmd.Parameters.AddWithValue("@GuardianRelation", guardianRelationTextBox.Text);
                    cmd.Parameters.AddWithValue("@GuardianOccupation", guardiansOccupationTextBox.Text);

                    cmd.Parameters.AddWithValue("@presentHouseNo", presentHouseNoTextBox.Text);
                    cmd.Parameters.AddWithValue("@PresentLocalityId", (presentLocalityComboBox.SelectedIndex == -1) ? 0 : presentLocalityComboBox.SelectedValue);
                    cmd.Parameters.AddWithValue("@PresentCityId", (presentCityComboBox.SelectedIndex == -1) ? 0 : presentCityComboBox.SelectedValue);
                    cmd.Parameters.AddWithValue("@PresentPostCode", presentPostCodeTextBox.Text);

                    cmd.Parameters.AddWithValue("@PermanentHouseNo", permanentHouseNoTextBox.Text);
                    cmd.Parameters.AddWithValue("@PermanentLocalityId", (permanentLocalityComboBox.SelectedIndex == -1) ? 0 : permanentLocalityComboBox.SelectedValue);
                    cmd.Parameters.AddWithValue("@PermanentCityId", (permanentCityComboBox.SelectedIndex == -1) ? 0 : permanentCityComboBox.SelectedValue);
                    cmd.Parameters.AddWithValue("@PermanentPostCode", permanentPostCodeTextBox.Text);

                    cmd.Parameters.AddWithValue("@PSCYear", pscYearTextBox.Text);
                    cmd.Parameters.AddWithValue("@JSCYear", jscYearTextBox.Text);
                    cmd.Parameters.AddWithValue("@SSCYear", sscYearTextBox.Text);
                    cmd.Parameters.AddWithValue("@HSCYear", hscYearTextBox.Text);
                    cmd.Parameters.AddWithValue("@BSCYear", hscYearTextBox.Text);


                    cmd.Parameters.AddWithValue("@PSCBoardId", (pscBoardComboBox.SelectedIndex == -1) ? 0 : pscBoardComboBox.SelectedValue);
                    cmd.Parameters.AddWithValue("@JSCBoardId", (jscBoardComboBox.SelectedIndex == -1) ? 0 : jscBoardComboBox.SelectedValue);
                    cmd.Parameters.AddWithValue("@SSCBoardId", (sscBoardComboBox.SelectedIndex == -1) ? 0 : sscBoardComboBox.SelectedValue);
                    cmd.Parameters.AddWithValue("@HSCBoardId", (hscBoardComboBox.SelectedIndex == -1) ? 0 : hscBoardComboBox.SelectedValue);
                    cmd.Parameters.AddWithValue("@BSCBoard", bscBoardTextBox.Text);

                    cmd.Parameters.AddWithValue("@PSCGpa", pscGpaTextBox.Text);
                    cmd.Parameters.AddWithValue("@JSCGpa", jscGpaTextBox.Text);
                    cmd.Parameters.AddWithValue("@SSCGpa", sscGpaTextBox.Text);
                    cmd.Parameters.AddWithValue("@HSCGpa", hscGpaTextBox.Text);
                    cmd.Parameters.AddWithValue("@BSCGpa", hscGpaTextBox.Text);

                    cmd.Parameters.AddWithValue("@WaiverPercentage", waiverPercentageTextBox.Text);
                    cmd.Parameters.AddWithValue("@IsInterestedInCSharp", isInterestedInCSharpCheckBox.Checked);
                    cmd.Parameters.AddWithValue("@IsInterestedInVB", isIntestedInVBCheckBox.Checked);
                    cmd.Parameters.AddWithValue("@IsInterestedInJava", isIntestedInJavaCheckBox.Checked);
                    cmd.Parameters.AddWithValue("@StartTime", (startTimeDateTimePicker.Text.Trim() == string.Empty) ? (TimeSpan?)null : startTimeDateTimePicker.Value.TimeOfDay);
                    cmd.Parameters.AddWithValue("@EndTime", (endTimeDateTimePicker.Text.Trim() == string.Empty) ? (TimeSpan?)null : endTimeDateTimePicker.Value.TimeOfDay);
                    cmd.Parameters.AddWithValue("@FundTypeId", (fundTypeComboBox.SelectedIndex == -1) ? 0 : fundTypeComboBox.SelectedValue);
                    cmd.Parameters.AddWithValue("@FeesPaymentId", (feesPaymentComboBox.SelectedIndex == -1) ? 0 : feesPaymentComboBox.SelectedValue);
                    cmd.Parameters.AddWithValue("@Comments", commentsTextBox.Text);

                    cmd.Parameters.AddWithValue("@Photo", SavePhoto());

                    //Open Connection
                    conn.Open();

                    // ExecuteReader (Select statement)
                    // ExecuteScalar (Select statement)
                    // ExecuteNoQuery (Insert, Update or Delete)

                    cmd.ExecuteNonQuery();
                    
                }
            }
        }

       
        private void InsertStudentDetails(out int studentId)
        {

            string connString = ConfigurationManager.ConnectionStrings["dbx"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                using (SqlCommand cmd = new SqlCommand("usp_StudentInsertDetails", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@StudentId", SqlDbType.Int).Direction = ParameterDirection.Output;

                    // parameter
                    cmd.Parameters.AddWithValue("@Name", studentNameTextBox.Text);
                    cmd.Parameters.AddWithValue("@SemisterId", (semisterNameComboBox.SelectedIndex == -1) ? 0 : semisterNameComboBox.SelectedValue);
                    cmd.Parameters.AddWithValue("@DepartmentId", (departmentNameComboBox.SelectedIndex == -1) ? 0 : departmentNameComboBox.SelectedValue);

                    cmd.Parameters.AddWithValue("@GenderId", GetGender());
                    cmd.Parameters.AddWithValue("@DateOfBirth", (dateOfBirthDateTimePicker.Text.Trim() == string.Empty) ? (DateTime?)null : dateOfBirthDateTimePicker.Value.Date);
                    cmd.Parameters.AddWithValue("@MaritalStatusId", MaritalStatus());
                    cmd.Parameters.AddWithValue("@DepartmentId", (bloodGroupComboBox.SelectedIndex == -1) ? 0 : bloodGroupComboBox.SelectedValue);
                    cmd.Parameters.AddWithValue("@DepartmentId", (religionComboBox.SelectedIndex == -1) ? 0 : religionComboBox.SelectedValue);
                    cmd.Parameters.AddWithValue("@Nationality", nationalityTextBox.Text);
                    cmd.Parameters.AddWithValue("@ContactNo", contactNoTextBox.Text);
                    cmd.Parameters.AddWithValue("Email", emailTextBox.Text);

                    cmd.Parameters.AddWithValue("@FathersName", fathersNameTextBox.Text);
                    cmd.Parameters.AddWithValue("@FathersMobileNo", fathersMobileNoTextBox.Text);
                    cmd.Parameters.AddWithValue("@MothersName", mothersNameTextBox.Text);
                    cmd.Parameters.AddWithValue("@MothersMobileNo", mothersMobileNoTextBox.Text);
                    cmd.Parameters.AddWithValue("@GuardianName", guardianNameTextBox.Text);
                    cmd.Parameters.AddWithValue("@GuardianMobileNo", guardiansMobileNoTextBox.Text);
                    cmd.Parameters.AddWithValue("@GuardianRelation", guardianRelationTextBox.Text);
                    cmd.Parameters.AddWithValue("@GuardianOccupation", guardiansOccupationTextBox.Text);

                    cmd.Parameters.AddWithValue("@presentHouseNo", presentHouseNoTextBox.Text);
                    cmd.Parameters.AddWithValue("@PresentLocalityId", (presentLocalityComboBox.SelectedIndex == -1) ? 0 : presentLocalityComboBox.SelectedValue);
                    cmd.Parameters.AddWithValue("@PresentCityId", (presentCityComboBox.SelectedIndex == -1) ? 0 : presentCityComboBox.SelectedValue);
                    cmd.Parameters.AddWithValue("@PresentPostCode", presentPostCodeTextBox.Text);

                    cmd.Parameters.AddWithValue("@PermanentHouseNo", permanentHouseNoTextBox.Text);
                    cmd.Parameters.AddWithValue("@PermanentLocalityId", (permanentLocalityComboBox.SelectedIndex == -1) ? 0 : permanentLocalityComboBox.SelectedValue);
                    cmd.Parameters.AddWithValue("@PermanentCityId", (permanentCityComboBox.SelectedIndex == -1) ? 0 : permanentCityComboBox.SelectedValue);
                    cmd.Parameters.AddWithValue("@PermanentPostCode", permanentPostCodeTextBox.Text);

                    cmd.Parameters.AddWithValue("@PSCYear", pscYearTextBox.Text);
                    cmd.Parameters.AddWithValue("@JSCYear", jscYearTextBox.Text);
                    cmd.Parameters.AddWithValue("@SSCYear", sscYearTextBox.Text);
                    cmd.Parameters.AddWithValue("@HSCYear", hscYearTextBox.Text);
                    cmd.Parameters.AddWithValue("@BSCYear", hscYearTextBox.Text);


                    cmd.Parameters.AddWithValue("@PSCBoardId", (pscBoardComboBox.SelectedIndex == -1) ? 0 : pscBoardComboBox.SelectedValue);
                    cmd.Parameters.AddWithValue("@JSCBoardId", (jscBoardComboBox.SelectedIndex == -1) ? 0 : jscBoardComboBox.SelectedValue);
                    cmd.Parameters.AddWithValue("@SSCBoardId", (sscBoardComboBox.SelectedIndex == -1) ? 0 : sscBoardComboBox.SelectedValue);
                    cmd.Parameters.AddWithValue("@HSCBoardId", (hscBoardComboBox.SelectedIndex == -1) ? 0 : hscBoardComboBox.SelectedValue);
                    cmd.Parameters.AddWithValue("@BSCBoard",    bscBoardTextBox.Text);

                    cmd.Parameters.AddWithValue("@PSCGpa", pscGpaTextBox.Text);
                    cmd.Parameters.AddWithValue("@JSCGpa", jscGpaTextBox.Text);
                    cmd.Parameters.AddWithValue("@SSCGpa", sscGpaTextBox.Text);
                    cmd.Parameters.AddWithValue("@HSCGpa", hscGpaTextBox.Text);
                    cmd.Parameters.AddWithValue("@BSCGpa", hscGpaTextBox.Text);

                    cmd.Parameters.AddWithValue("@WaiverPercentage", waiverPercentageTextBox.Text);
                    cmd.Parameters.AddWithValue("@IsInterestedInCSharp", isInterestedInCSharpCheckBox.Checked);
                    cmd.Parameters.AddWithValue("@IsInterestedInVB", isIntestedInVBCheckBox.Checked);
                    cmd.Parameters.AddWithValue("@IsInterestedInJava", isIntestedInJavaCheckBox.Checked);
                    cmd.Parameters.AddWithValue("@StartTime", (startTimeDateTimePicker.Text.Trim() == string.Empty) ? (TimeSpan?)null : startTimeDateTimePicker.Value.TimeOfDay);
                    cmd.Parameters.AddWithValue("@EndTime", (endTimeDateTimePicker.Text.Trim() == string.Empty) ? (TimeSpan?)null : endTimeDateTimePicker.Value.TimeOfDay);
                    cmd.Parameters.AddWithValue("@FundTypeId", (fundTypeComboBox.SelectedIndex == -1) ? 0 : fundTypeComboBox.SelectedValue);
                    cmd.Parameters.AddWithValue("@FeesPaymentId", (feesPaymentComboBox.SelectedIndex == -1) ? 0 : feesPaymentComboBox.SelectedValue);
                    cmd.Parameters.AddWithValue("@Comments", commentsTextBox.Text);
                    
                    cmd.Parameters.AddWithValue("@Photo", SavePhoto());

                    //Open Connection
                    conn.Open();

                    // ExecuteReader (Select statement or searching)
                    // ExecuteScalar (Select only one statement or searching one row)
                    // ExecuteNoQuery (Insert, Update or Delete)

                    cmd.ExecuteNonQuery();

                    studentId = Convert.ToInt16(cmd.Parameters["@StudentId"].Value);

                }
            }
        }

       

        private object MaritalStatus()
        {
            if (singleRadioButton.Checked)
            {
                return (int)MarritalStatus.Single;
            }
            else if (femaleRadioButton.Checked)
                return (int)MarritalStatus.Married;
            else
                return (int)MarritalStatus.NoSelection;
        }

        private byte[] SavePhoto()
        {
            MemoryStream ms = new MemoryStream();
            studentImagePictureBox.Image.Save(ms, studentImagePictureBox.Image.RawFormat);
            return ms.GetBuffer();
        }

        private int GetGender()
        {
            if (maleRadioButton.Checked)
            {
                return (int)Gender.Male;
            }
            else if (femaleRadioButton.Checked)
                return (int)Gender.Female;
            else
                return (int)Gender.NoSelection;
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void isInterstedInCSharpCheckBox_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void isIntestedInVBCheckBox_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void isIntestedInJavaCheckBox_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void dateOfBirthDateTimePicker_ValueChanged(object sender, EventArgs e)
        {
            if(dateOfBirthDateTimePicker.Value == dateOfBirthDateTimePicker.MinDate)
            {
                dateOfBirthDateTimePicker.CustomFormat = " ";
            }
            else
                dateOfBirthDateTimePicker.CustomFormat = "dd/MM/yyyy";
        }

        private void dateOfBirthDateTimePicker_KeyDown(object sender, KeyEventArgs e)
        {
            if((e.KeyCode == Keys.Back)||(e.KeyCode == Keys.Delete))
            {
                dateOfBirthDateTimePicker.CustomFormat = " "; 
            }
        }

       
        
        private void Time_ValueChanged(object sender, EventArgs e)
        {
            DateTimePicker dtp = (DateTimePicker)sender;

            if (dtp.Value == dtp.MinDate)
                GetCustomTimeFormat(sender, " ");
            else
                GetCustomTimeFormat(sender, "HH:mm");
        }

        private void TimePicker_MouseDown(object sender, MouseEventArgs e)
        {
            GetCustomTimeFormat(sender, "HH:mm");
        }

        private void GetCustomTimeFormat(object sender, string format)
        {
            DateTimePicker dtp = (DateTimePicker)sender;
            dtp.CustomFormat = format;
        }

        private void TimePiker_KyeDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Back) || (e.KeyCode == Keys.Delete))
            {
                GetCustomTimeFormat(sender, " ");
            }
        }

        private void StudentInfoForm_Load(object sender, EventArgs e)
        {
            // LoadDataIntoComboBoxes();
            LoadAndMapDataToControlIfUpdate();


        }

        private void LoadAndMapDataToControlIfUpdate()
        {
            if (this.IsUpdate)
            {
                DataTable dtStudentInfo = GetStudentInfoById(this.StudentId);
                DataRow row = dtStudentInfo.Rows[0];

                studentNameTextBox.Text = row["Name"].ToString();
                emailTextBox.Text = row["Email"].ToString();
                isInterestedInCSharpCheckBox.Checked = (row["IsInterestedInCSharp"] is DBNull) ? false : Convert.ToBoolean(row["IsInterestedInCSharp"]);
                isIntestedInVBCheckBox.Checked = (row["IsInterestedInVB"] is DBNull) ? false : Convert.ToBoolean(row["IsInterestedInCSharp"]);
                isIntestedInJavaCheckBox.Checked = (row["IsInterestedInJava"] is DBNull) ? false : Convert.ToBoolean(row["IsInterestedInJava"]);

                maleRadioButton.Checked = (row["GenderId"] is DBNull) ? false : (Convert.ToInt16(row["GenderId"]) == 1) ? true : false;
                femaleRadioButton.Checked = (row["GenderId"] is DBNull) ? false : (Convert.ToInt16(row["GenderId"]) == 2) ? true : false;

                dateOfBirthDateTimePicker.Value = (row["DateOfBirth"] is DBNull) ? dateOfBirthDateTimePicker.MinDate : Convert.ToDateTime(row["DateOfBirth"]).Date;
                startTimeDateTimePicker.Value = (row["StartTime"] is DBNull) ? startTimeDateTimePicker.MinDate : Convert.ToDateTime(row["StartTime"]);
                endTimeDateTimePicker.Value = (row["EndTime"] is DBNull) ? endTimeDateTimePicker.MinDate : Convert.ToDateTime(row["EndTime"]);

                fundTypeComboBox.SelectedValue = row["FundTypeId"];
                feesPaymentComboBox.SelectedValue = row["FeesPaymentId"];

                presentHouseNoTextBox.Text = row["Address"].ToString();
                presentCityComboBox.SelectedValue = row["CityId"];
                presentLocalityComboBox.SelectedValue = row["LocalityId"];
                presentPostCodeTextBox.Text = row["PostCode"].ToString();

                commentsTextBox.Text = row["Comments"].ToString();

                //studentImagePictureBox.Image = (row["Photo"] is DBNull) ? Resources.shoikat : GetPhoto((byte[])row["Photo"]);
                studentImagePictureBox.Image = (row["Photo"] is DBNull) ? GetPhoto((byte[])row["Photo"]) : GetPhoto((byte[])row["Photo"]);

                this.OriginalRowVersion = (byte[])row["OriginalRowVersion"];

            }
        }

        private Image GetPhoto(byte[] photo)
        {
            MemoryStream ms = new MemoryStream(photo);
            return Image.FromStream(ms);
        }

        private DataTable GetStudentInfoById(int studentId)
        {
            DataTable dtStudentById = new DataTable();
            string connString = ConfigurationManager.ConnectionStrings["dbx"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                using (SqlCommand cmd = new SqlCommand("usp_StudentGetStudentInfoById", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@StudentId", studentId);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    dtStudentById.Load(reader);
                }
            }

            return dtStudentById;
            
            
        }

       /* private void LoadDataIntoComboBoxes()
        {
            fundTypeComboBox.DataSource = GetListData((int)ListDataTypes.FundType);
            fundTypeComboBox.DisplayMember = "Description";
            fundTypeComboBox.ValueMember = "ListDataId";
            fundTypeComboBox.SelectedIndex = -1;

            feesPaymentComboBox.DataSource = GetListData((int)ListDataTypes.FeesPayment);
            feesPaymentComboBox.DisplayMember = "Description";
            feesPaymentComboBox.ValueMember = "ListDataId";
            feesPaymentComboBox.SelectedIndex = -1;

            presentCityComboBox.SelectedValueChanged -= new EventHandler(cityComboBox_SelectedValueChanged);

            presentCityComboBox.DataSource = GetAllCities();
            presentCityComboBox.DisplayMember = "CityName";
            presentCityComboBox.ValueMember = "CityId";
            presentCityComboBox.SelectedIndex = -1;

            presentCityComboBox.SelectedValueChanged += new EventHandler(cityComboBox_SelectedValueChanged);

        }

        private DataTable GetAllCities()
        {
            DataTable dtCities = new DataTable();

            string connString = ConfigurationManager.ConnectionStrings["dbx"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connString))
            {
                using (SqlCommand cmd = new SqlCommand("usp_GetAllCities", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    conn.Open();

                    SqlDataReader reader = cmd.ExecuteReader();

                    dtCities.Load(reader);
                }
            }

            return dtCities;
        }


        // GetFundTypes is a list type of data.
        // So it Can be use with......
        // DataTable
        // Objects
        // DataSet
        // Array
        // Collection
        // Generics

        /*
        private DataTable GetListData(int listDataTypeId)
        {
            DataTable dtListData = new DataTable();

            string connString = ConfigurationManager.ConnectionStrings["dbx"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connString))
            {
                using (SqlCommand cmd = new SqlCommand("usp_GetAllListData", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@ListDataTypeId", listDataTypeId);

                    conn.Open();

                    SqlDataReader reader = cmd.ExecuteReader();

                    dtListData.Load(reader);
                }
            }

            return dtListData;
        }
        */

        
        private void ExtendedDetailsTabPage_Click(object sender, EventArgs e)
        {

        }

        private void cityComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            presentLocalityComboBox.DataSource = GetAllLocalitiesByCityId((int)presentCityComboBox.SelectedValue);
            presentLocalityComboBox.DisplayMember = "LocalityName";
            presentLocalityComboBox.ValueMember = "LocalityId";
        }

        private object GetAllLocalitiesByCityId(int cityId)
        {
            DataTable dtLocalities = new DataTable();

            string connString = ConfigurationManager.ConnectionStrings["dbx"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connString))
            {
                using (SqlCommand cmd = new SqlCommand("usp_GetAllLocalitiesByCityId", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@CityId", cityId);

                    conn.Open();

                    SqlDataReader reader = cmd.ExecuteReader();

                    dtLocalities.Load(reader);
                }
            }

            return dtLocalities;
        }

        private void cityComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void studentImagePictureBox_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Select the Photo";
            // ofd.Filter = "PNG File(*.png)|*.png|JPG File(*.jgp)|*.jpg|BMP File(*.bmp)|*.bmp|Gif File(*.gif)|*.gif";
            ofd.Filter = "Image File (*.png;*.jgp;*.bmp;*.gif)|*.png;*.jgp;*.bmp;*.gif";
           
            if(ofd.ShowDialog() == DialogResult.OK)
            {
                studentImagePictureBox.Image = new Bitmap(ofd.FileName);
            }
        }

        private void addressTabPage_Click(object sender, EventArgs e)
        {

        }

        private void guardiansOccupationTextBox_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
