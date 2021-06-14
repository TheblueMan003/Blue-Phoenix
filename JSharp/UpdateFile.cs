using System;
using System.Windows.Forms;

namespace JSharp
{
    public partial class UpdateFile : Form
    {
        public Result result = Result.No;

        public static Result ShowDialog(string text)
        {
            UpdateFile file = new UpdateFile(text);
            file.ShowDialog();
            return file.result;
        }

        public UpdateFile(string text)
        {
            InitializeComponent();
            label1.Text = text;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            result = Result.Yes;
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            result = Result.YesForAll;
            Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            result = Result.No;
            Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            result = Result.NoForAll;
            Close();
        }

        public enum Result
        {
            Yes,
            YesForAll,
            No,
            NoForAll
        }
    }
}
