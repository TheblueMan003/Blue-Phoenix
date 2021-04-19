using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JSharp
{
    public partial class ForceOffuscation : Form
    {
        Dictionary<string, string> offuscationMap;
        Dictionary<string, Compiler.Variable> variables;
        public ForceOffuscation(Dictionary<string, Compiler.Variable> variables, Dictionary<string,string> offuscationMap)
        {
            InitializeComponent();
            this.offuscationMap = offuscationMap;
            this.variables = variables;

            foreach (string key in offuscationMap.Keys)
            {
                dataGridView1.Rows.Add(key, offuscationMap[key]);
            }
            Filter();
        }

        private void ForceOffuscation_Load(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            offuscationMap.Clear();
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells[0].Value != null && row.Cells[0].Value.ToString() != "")
                {
                    offuscationMap[row.Cells[0].Value.ToString()] = row.Cells[1].Value.ToString();
                }
            }
            Close();
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex > -1)
            {
                string key = listBox1.Items[listBox1.SelectedIndex].ToString();
                dataGridView1.Rows.Add(key, variables[key].name);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            Filter();
        }

        private void Filter()
        {
            listBox1.Items.Clear();
            if (variables != null)
            {
                foreach (string key in variables.Keys)
                {
                    if (key.Contains(textBox1.Text) && (!checkBox1.Checked || variables[key].entity))
                    {
                        listBox1.Items.Add(key);
                    }
                }
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Filter();
        }
    }
}
