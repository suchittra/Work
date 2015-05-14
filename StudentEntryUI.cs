using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;s
using System.Windows.Forms;

namespace UniversityManagementApp
{
    public partial class StudentEntryUI : Form
    {
        public StudentEntryUI()
        {
            InitializeComponent();
        }
        string connectionString =
               ConfigurationManager.ConnectionStrings["UniversityManagementConString"].ConnectionString;

        private bool isUpdateMode = false;

        private int studentId; 
        
        private void saveButton_Click(object sender, EventArgs e)
        {
            string name = nameTextBox.Text;
            string regNo = regNoTextBox.Text;
            string address = addressTextBox.Text;

            if (isUpdateMode)
            {
                SqlConnection connection = new SqlConnection(connectionString);


                //2. write query 

                string query = "UPDATE Students SET Name ='" + name + "', Address ='" + address + "' WHERE ID = '"+studentId+"'";


                // 3. execute query 

                SqlCommand command = new SqlCommand(query, connection);

                connection.Open();
                int rowAffected = command.ExecuteNonQuery();
                connection.Close();



                //4. see result

                if (rowAffected > 0)
                {
                    MessageBox.Show("Updated Successfully!");

                    saveButton.Text = "Save";
                    studentId = 0;
                    isUpdateMode = false;
                    regNoTextBox.Enabled = true;
                    ShowAllStudents();

                }
                else
                {
                    MessageBox.Show("Update Failed!");
                }
            }
            else
            {
                // Is RegNo Exists? if exists not insert, else insert

                if (IsRegNoExists(regNo))
                {
                    MessageBox.Show("Reg No already exists!");
                    return;
                }




                //1. connect to database - i.e. server, database, authentication (connectionstring)
                SqlConnection connection = new SqlConnection(connectionString);


                //2. write query 

                string query = "INSERT INTO Students VALUES('" + name + "','" + regNo + "','" + address + "')";


                // 3. execute query 

                SqlCommand command = new SqlCommand(query, connection);

                connection.Open();
                int rowAffected = command.ExecuteNonQuery();
                connection.Close();



                //4. see result

                if (rowAffected > 0)
                {
                    MessageBox.Show("Inserted Successfully!");
                    ShowAllStudents();

                }
                else
                {
                    MessageBox.Show("Insertion Failed!");
                }
            }



            
        }


        public bool IsRegNoExists(string regNo)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            
            //2. write query 

            string query = "SELECT * FROM Students WHERE RegNo ='" + regNo + "'";


            // 3. execute query 

            SqlCommand command = new SqlCommand(query,connection);

            bool isRegNoExists = false;

            connection.Open();
            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                isRegNoExists = true;
                break;
            }
            reader.Close();
            connection.Close();

            return isRegNoExists;

        }

        private void showButton_Click(object sender, EventArgs e)
        {
            ShowAllStudents();


        }

        public void LoadStudentListView(List<Student> students)
        {
            studentListView.Items.Clear();
            foreach (var student in students)
            {
                ListViewItem item = new ListViewItem(student.ID.ToString());
                item.SubItems.Add(student.Name);
                item.SubItems.Add(student.RegNo);
                item.SubItems.Add(student.Address);

                studentListView.Items.Add(item);
            }
        }

        public void ShowAllStudents()
        {
            SqlConnection connection = new SqlConnection(connectionString);

            //2. write query 

            string query = "SELECT * FROM Students";


            // 3. execute query 

            SqlCommand command = new SqlCommand(query, connection);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();

            List<Student> studentList = new List<Student>();

            while (reader.Read())
            {

                Student student = new Student();
                student.ID = int.Parse(reader["ID"].ToString());
                student.Name = reader["Name"].ToString();
                student.Address = reader["Address"].ToString();
                student.RegNo = reader["RegNo"].ToString();

                studentList.Add(student);

            }
            reader.Close();
            connection.Close();

            // populate list view with data 

            LoadStudentListView(studentList);
        }

        private void StudentEntryUI_Load(object sender, EventArgs e)
        {
            ShowAllStudents();
        }

        private void studentListView_DoubleClick(object sender, EventArgs e)
        {
            // 1. Select selected Student

            ListViewItem item = studentListView.SelectedItems[0];

            int id = int.Parse(item.Text.ToString());

            Student student = GetStudentByID(id);

            if (student != null)
            {
                //2. Enable update mode -- save button = update button, grab id

                isUpdateMode = true;

                saveButton.Text = "Update";
                regNoTextBox.Enabled = false;

                studentId = student.ID;

                //3. Fill Text with student data 

                nameTextBox.Text = student.Name;
                regNoTextBox.Text = student.RegNo;
                addressTextBox.Text = student.Address;
            }




           


           
        }

        public Student GetStudentByID(int id)
        {
            SqlConnection connection = new SqlConnection(connectionString);

            //2. write query 

            string query = "SELECT * FROM Students WHERE ID ='"+id+"'";


            // 3. execute query 

            SqlCommand command = new SqlCommand(query, connection);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();

            List<Student> studentList = new List<Student>();

            while (reader.Read())
            {

                Student student = new Student();
                student.ID = int.Parse(reader["ID"].ToString());
                student.Name = reader["Name"].ToString();
                student.Address = reader["Address"].ToString();
                student.RegNo = reader["RegNo"].ToString();

                studentList.Add(student);

            }
            reader.Close();
            connection.Close();

            return studentList.FirstOrDefault();


        }

        
    }

    



}
