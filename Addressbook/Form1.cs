using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Addressbook
{
    public partial class Form1 : Form
    {
        int addressId = 0;
        //pravi konekciju ka bazi
        SqlConnection sqlConn = new SqlConnection(@"Data Source=DESKTOP-L7QN8D1;Initial Catalog=Addressbook;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
        public void openConn() {
        
                if (sqlConn.State == ConnectionState.Closed)
                    sqlConn.Open();
        }
        public Form1()
        {
            InitializeComponent();
        }

        private void CBoxGender_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        public void LblGender_Click(object sender, EventArgs e) {
               
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            btnDelete.Enabled = false;
            openConn();
            fillDgv();
            cbGender.Items.Add("muski");
            cbGender.Items.Add("zenski");

        }
        //izbacuje tabelu u gridu
        void fillDgv() {
            openConn();
            SqlDataAdapter sqlDa = new SqlDataAdapter("addressVieworSearch", sqlConn);
            sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;
            sqlDa.SelectCommand.Parameters.AddWithValue("@name", tbSearch.Text.Trim());
            DataTable dTable = new DataTable();
            sqlDa.Fill(dTable);
            dgv.DataSource = dTable;
            dgv.Columns[0].Visible = false;
            sqlConn.Close();
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            openConn();
            SqlCommand sqlCmd = new SqlCommand("addressSaveOrEdit", sqlConn);
            sqlCmd.CommandType = CommandType.StoredProcedure;
            if (btnSave.Text == "Save")
            {
                sqlCmd.Parameters.AddWithValue("@mode", "Save");
                sqlCmd.Parameters.AddWithValue("@address_id", 0);
                sqlCmd.Parameters.AddWithValue("@name", tbName.Text.Trim());
                sqlCmd.Parameters.AddWithValue("@last_name", tbLastName.Text.Trim());
                sqlCmd.Parameters.AddWithValue("@phone_number", tbPhoneNum.Text.Trim());
                sqlCmd.Parameters.AddWithValue("@address", tbAddress.Text.Trim());
                sqlCmd.Parameters.AddWithValue("@gender", cbGender.Text);
                sqlCmd.ExecuteNonQuery();
                fillDgv();
                MessageBox.Show("Saved Successfully.");
                reset();
            }
            else {
                sqlCmd.Parameters.AddWithValue("@mode", "Update");
                sqlCmd.Parameters.AddWithValue("@address_id", addressId);
                sqlCmd.Parameters.AddWithValue("@name", tbName.Text.Trim());
                sqlCmd.Parameters.AddWithValue("@last_name", tbLastName.Text.Trim());
                sqlCmd.Parameters.AddWithValue("@phone_number", tbPhoneNum.Text.Trim());
                sqlCmd.Parameters.AddWithValue("@address", tbAddress.Text.Trim());
                sqlCmd.Parameters.AddWithValue("@gender", cbGender.Text);
                sqlCmd.ExecuteNonQuery();
                fillDgv();
                MessageBox.Show("Updated Successfully.");
                reset();
            }
            sqlConn.Close();
            

        }
        //Pretrazuje po imenu
        private void BtnSearch_Click(object sender, EventArgs e)
        {
            openConn();
            SqlDataAdapter sqlDa = new SqlDataAdapter("addressViewOrSearch", sqlConn);
            sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;
            sqlDa.SelectCommand.Parameters.AddWithValue("@name", tbSearch.Text.Trim());
            DataTable dTable = new DataTable();
            sqlDa.Fill(dTable);
            dgv.DataSource = dTable;
            sqlConn.Close();
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            reset();
            sqlConn.Close();
        }
        //vracamo sve na pocetno stanje
        public void reset() {
            tbName.Text = "";
            tbLastName.Text = "";
            tbPhoneNum.Text = "";
            tbAddress.Text = "";
            cbGender.Text = "";
            tbSearch.Text = "";
            addressId = 0;
            btnSave.Text = "Save";
            btnDelete.Enabled = false;
            fillDgv();
            
        }
        //kada se klikne dva puta na red u gridu, mozemo da apdejtujemo ili da brisemo red iz tabele
        private void Dgv_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (dgv.CurrentRow.Index != -1) {
                tbName.Text = dgv.CurrentRow.Cells[1].Value.ToString();
                tbLastName.Text = dgv.CurrentRow.Cells[2].Value.ToString();
                tbPhoneNum.Text = dgv.CurrentRow.Cells[3].Value.ToString();
                tbAddress.Text = dgv.CurrentRow.Cells[4].Value.ToString();
                cbGender.Text = dgv.CurrentRow.Cells[5].Value.ToString(); 
            }
            addressId = Convert.ToInt32(dgv.CurrentRow.Cells[0].Value);
            btnSave.Text = "Update";
            btnDelete.Enabled = true;

        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            openConn();
            SqlCommand sqlCmd = new SqlCommand("deleteRow", sqlConn);
            sqlCmd.CommandType = CommandType.StoredProcedure;
            sqlCmd.Parameters.AddWithValue("@address_id", addressId);
            sqlCmd.ExecuteNonQuery();
            MessageBox.Show("Row Successfully Deleted.");
            reset();
            fillDgv();
            sqlConn.Close();


        }
    }
}
