using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileScanner
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private async void button1_Click(object sender, EventArgs e)
        {
            this.button1.Enabled = false;

            string path = this.textBox1.Text;
            string pattern = this.textBox2.Text;

            var progress = new Progress<string>(i => this.label2.Text = "Прогресс: " + i.ToString());
            DataTable table = await Utils.Search(path, pattern, progress);

            foreach (DataRow row in table.Rows)
            {
                ListViewItem item = new ListViewItem(row[0].ToString());
                for (int i = 1; i < table.Columns.Count; i++)
                {
                    item.SubItems.Add(row[i].ToString());
                }
                this.listView1.Items.Add(item);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ColumnHeader columnheader;// Used for creating column headers.

            // Ensure that the view is set to show details.
            listView1.View = View.Details;

            // Create some column headers for the data.
            columnheader = new ColumnHeader();
            columnheader.Text = "Файл";
            columnheader.Width = 600;
            this.listView1.Columns.Add(columnheader);

            columnheader = new ColumnHeader();
            columnheader.Text = "Матч";
            columnheader.Width = 300;
            this.listView1.Columns.Add(columnheader);
        }
    }
}
