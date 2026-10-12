using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace InventoryWinForms
{
    public partial class Form1 : Form
    {
        private static readonly HttpClient client = new HttpClient() { BaseAddress = new Uri("https://localhost:7228/api/") };

        public Form1()
        {
            InitializeComponent();
            txtId.ReadOnly = true;
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            await LoadProductsAsync();
        }

        private async Task LoadProductsAsync()
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync("products");
                if (response.IsSuccessStatusCode)
                {
                    string jsonResult = await response.Content.ReadAsStringAsync();
                    var products = JsonConvert.DeserializeObject<List<Product>>(jsonResult);
                    dgvProducts.DataSource = products;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message);
            }
        }

        private async void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                var newProduct = new Product
                {
                    Name = txtName.Text,
                    Category = txtCategory.Text,
                    Price = decimal.Parse(txtPrice.Text),
                    StockQuantity = int.Parse(txtStock.Text)
                };

                HttpResponseMessage response = await client.PostAsJsonAsync("products", newProduct);
                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Product added successfully!");
                    ClearFields();
                    await LoadProductsAsync();
                }
                else
                {
                    MessageBox.Show("Failed to add product.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Please check your input values: " + ex.Message);
            }
        }

        private async void btnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtId.Text))
            {
                MessageBox.Show("Please select a product from the table first.");
                return;
            }

            try
            {
                var updatedProduct = new Product
                {
                    Id = int.Parse(txtId.Text),
                    Name = txtName.Text,
                    Category = txtCategory.Text,
                    Price = decimal.Parse(txtPrice.Text),
                    StockQuantity = int.Parse(txtStock.Text)
                };

                HttpResponseMessage response = await client.PutAsJsonAsync($"products/{updatedProduct.Id}", updatedProduct);
                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Product updated successfully!");
                    ClearFields();
                    await LoadProductsAsync();
                }
                else
                {
                    MessageBox.Show("Failed to update product.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating: " + ex.Message);
            }
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtId.Text))
            {
                MessageBox.Show("Please select a product to delete.");
                return;
            }

            int id = int.Parse(txtId.Text);
            HttpResponseMessage response = await client.DeleteAsync($"products/{id}");
            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Product deleted successfully!");
                ClearFields();
                await LoadProductsAsync();
            }
            else
            {
                MessageBox.Show("Failed to delete product.");
            }
        }

        private void dgvProducts_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow selectedRow = dgvProducts.Rows[e.RowIndex];
                txtId.Text = selectedRow.Cells["Id"].Value.ToString();
                txtName.Text = selectedRow.Cells["Name"].Value.ToString();
                txtCategory.Text = selectedRow.Cells["Category"].Value.ToString();
                txtPrice.Text = selectedRow.Cells["Price"].Value.ToString();
                txtStock.Text = selectedRow.Cells["StockQuantity"].Value.ToString();
            }
        }

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            await LoadProductsAsync();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        private void ClearFields()
        {
            txtId.Clear();
            txtName.Clear();
            txtCategory.Clear();
            txtPrice.Clear();
            txtStock.Clear();
        }
    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
    }
}
