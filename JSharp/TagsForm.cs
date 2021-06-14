using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace JSharp
{
    public partial class TagsForm : Form
    {
        public Dictionary<string, Dictionary<string, TagsList>> data;

        public TagsForm(Dictionary<string, Dictionary<string, TagsList>> data)
        {
            InitializeComponent();
            this.data = data;
            foreach (string key in data.Keys)
            {
                comboBox1.Items.Add(key);
            }
        }

        private void TagsForm_Load(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateAll();
        }

        private void UpdateAll()
        {
            if (data.ContainsKey(comboBox1.Text))
            {
                listBox2.Items.Clear();
                richTextBox1.Clear();
                foreach (string key in data[comboBox1.Text].Keys)
                {
                    listBox2.Items.Add(key);
                }
            }
            else
            {
                listBox2.Items.Clear();
                richTextBox1.Clear();
            }
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox2.SelectedIndex > -1)
            {
                richTextBox1.Lines = data[comboBox1.Text][listBox2.SelectedItem.ToString()].values.ToArray();
                textBox1.Text = listBox2.SelectedItem.ToString();
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            if (data.ContainsKey(comboBox1.Text) && listBox2.SelectedIndex > -1)
            {
                data[comboBox1.Text][listBox2.SelectedItem.ToString()].values = new List<string>(richTextBox1.Lines);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (!data[comboBox1.Text].ContainsKey(textBox1.Text))
            {
                data[comboBox1.Text].Add(textBox1.Text, new TagsList());
                textBox1.Text = "";
                listBox2.Items.Clear();
                richTextBox1.Clear();
                foreach (string key in data[comboBox1.Text].Keys)
                {
                    listBox2.Items.Add(key);
                }

                richTextBox1.Clear();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (listBox2.SelectedIndex > -1)
            {
                data[comboBox1.Text].Remove(listBox2.SelectedItem.ToString());
                UpdateAll();
            }
        }
    }
}
