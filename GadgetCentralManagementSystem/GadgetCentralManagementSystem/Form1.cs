using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace GadgetCentralManagementSystem
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void dgvItems_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int r = e.RowIndex;
            int c = e.ColumnIndex;
            if (c == 0 && r >= 0)
            {
                ProductIdtxt.Text = dgvItems.Rows[r].Cells[1].Value.ToString();
                ProductGlobalIdtxt.Text = dgvItems.Rows[r].Cells[2].Value.ToString();
                ItemNametxt.Text = dgvItems.Rows[r].Cells[3].Value.ToString();
                UnitPricetxt.Text = dgvItems.Rows[r].Cells[4].Value.ToString();
                Inventorytxt.Text = dgvItems.Rows[r].Cells[5].Value.ToString();
                ProductDetailtxt.Text = dgvItems.Rows[r].Cells[5].Value.ToString();
                ProductCategorytxt.Text = dgvItems.Rows[r].Cells[5].Value.ToString();
                Thumbnailtxt.Text = dgvItems.Rows[r].Cells[5].Value.ToString();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadData();
        }
        private void LoadData()
        {
            string url = "https://localhost:7109/api/Product";
            HttpClient client = new HttpClient();
            var res = client.GetAsync(url);
            var result = res.Result;
            if (result.IsSuccessStatusCode)
            {
                var read = result.Content.ReadAsStringAsync();
                read.Wait();
                var products = read.Result;
                dgvItems.DataSource = null;
                dgvItems.DataSource = (new JavaScriptSerializer()).Deserialize<List<Product>>(products);
            }

        }

        private void Addbtn_Click(object sender, EventArgs e)
        {
            string url = "https://localhost:7109/api/Product";
            HttpClient client = new HttpClient();
            Product p = new Product();
            p.Id = int.Parse(ProductIdtxt.Text);
            p.GlobalId = ProductGlobalIdtxt.Text;
            p.ItemName = ItemNametxt.Text;
            p.UnitPrice = decimal.Parse(UnitPricetxt.Text);
            p.Inventory = int.Parse(Inventorytxt.Text);
            p.ProductDetails = ProductDetailtxt.Text;
            p.ProductCategory = ProductCategorytxt.Text;
            p.Thumbnail = Thumbnailtxt.Text;
            var json = (new JavaScriptSerializer()).Serialize(p);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            var res = client.PostAsync(url, data);
            var result = res.Result;
            if (result.IsSuccessStatusCode)
            {
                MessageBox.Show("Product Added Successfully");
                LoadData();
            }
            else
            {
                MessageBox.Show("Error while adding product");
            }
        }

        private void Deletebtn_Click(object sender, EventArgs e)
        {
            string url = "https://localhost:7109/api/Product9" + ProductIdtxt.Text;
            HttpClient client = new HttpClient();
            var res = client.DeleteAsync(url);
            var result = res.Result;
            if (result.IsSuccessStatusCode)
            {
                MessageBox.Show("Product Deleted Successfully");
                LoadData();
            }
            else
            {
                MessageBox.Show("Error while deleting product");
            }

        }

        private void Updatebtn_Click(object sender, EventArgs e)
        {
            string url = "https://localhost:7109/api/Product" + ProductIdtxt.Text;
            HttpClient client = new HttpClient();
            Product p = new Product();
            p.Id = int.Parse(ProductIdtxt.Text);
            p.GlobalId = ProductGlobalIdtxt.Text;
            p.ItemName = ItemNametxt.Text;
            p.UnitPrice = decimal.Parse(UnitPricetxt.Text);
            p.Inventory = int.Parse(Inventorytxt.Text);
            p.ProductDetails = ProductDetailtxt.Text;
            p.ProductCategory = ProductCategorytxt.Text;
            p.Thumbnail = Thumbnailtxt.Text;
            var json = (new JavaScriptSerializer()).Serialize(p);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            var res = client.PutAsync(url, data);
            var result = res.Result;
            if (result.IsSuccessStatusCode)
            {
                MessageBox.Show("Product Updated Successfully");
                LoadData();
            }
            else
            {
                MessageBox.Show("Error while updating product");
            }

        }
    }
}
