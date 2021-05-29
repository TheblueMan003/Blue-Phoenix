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
    public partial class NewFile : Form
    {
        public string filename;
        public Type type;
        public Type[] typeList = new Type[] { Type.EMPTY, Type.STRUCTURE, Type.SUBPROGRAMME, Type.RESOURCE, Type.EXTERNAL, Type.FOLDER }; 
        public enum Type
        {
            EMPTY,
            STRUCTURE,
            SUBPROGRAMME,
            RESOURCE,
            EXTERNAL,
            FOLDER
        }

        public NewFile()
        {
            InitializeComponent();
            listBox1.Items.Clear();
            foreach(Type t in typeList)
                listBox1.Items.Add(t);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            filename = textBox1.Text.ToLower();
            
            DialogResult = DialogResult.OK;
            if (listBox1.SelectedIndex > -1)
                type = typeList[listBox1.SelectedIndex];
            else
                type = Type.EMPTY;

            if (type == Type.RESOURCE && !filename.Contains('.'))
            {
                filename += ".txt";
            }

            Close();
        }
    }
}
