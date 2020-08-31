using System;
using System.IO;
using System.Windows.Forms;

namespace MyNotepad
{
    public partial class MainForm : Form
    {
        static int count = 1;
        string path;
        static FontDialog fd = new FontDialog();
        static ColorDialog cd = new ColorDialog();

        public MainForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// MainForm load event handler, sets initial values.
        /// </summary>
        private void MainForm_Load(object sender, EventArgs e)
        {
            path = null;
            fd.Font = textBox.Font;
            cd.Color = textBox.ForeColor;
            ActiveControl = textBox;
        }

        /// <summary>
        /// MainForm closing event handler, calls method that check if the file is saved.
        /// </summary>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Unsave();
        }

        /// <summary>
        /// Menu new window clicked event handler, opens a new window.
        /// </summary>
        private void MenuNewWindow_Click(object sender, EventArgs e)
        {
            MainForm newMainForm = new MainForm();
            newMainForm.Show();
        }

        /// <summary>
        /// Menu open clicked event handler, opens file.
        /// </summary>
        private void MenuOpen_Click(object sender, EventArgs e)
        {
            Unsave();
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = Directory.GetCurrentDirectory();
                openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    path = openFileDialog.FileName;

                    var fileStream = openFileDialog.OpenFile();

                    StreamReader reader = new StreamReader(fileStream);
                    tabControl1.SelectedTab.Text = Path.GetFileName(path);
                    tabControl1.SelectedTab.Tag = path;
                    tabControl1.SelectedTab.Controls[0].Text = reader.ReadToEnd();
                    reader.Close();
                    path = string.Empty;
                }
            }
        }

        /// <summary>
        /// Menu save clicked event handler, calls Save method.
        /// </summary>
        private void MenuSave_Click(object sender, EventArgs e)
        {
            Save();
        }

        /// <summary>
        /// Menu save as clicked event handler, calls Save As method.
        /// </summary>
        private void MenuSaveAs_Click(object sender, EventArgs e)
        {
            SaveAs();
        }

        /// <summary>
        /// Menu exit clicked event handler, calls logout dialog.
        /// </summary>
        private void MenuExit_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Do you really want to go out?", Text,
                MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Close();
            }
        }

        /// <summary>
        /// Menu new clicked event handler, creates new page.
        /// </summary>
        private void MenuNew_Click(object sender, EventArgs e)
        {
            string name = "Untitled" + count.ToString();

            TextBox tb = new TextBox();
            tb.BorderStyle = System.Windows.Forms.BorderStyle.None;
            tb.Multiline = true;
            tb.Dock = System.Windows.Forms.DockStyle.Fill;
            tb.Name = "textBox" + count.ToString();
            tb.Size = new System.Drawing.Size(786, 15);
            tb.TabIndex = 0;
            tb.TextChanged += new System.EventHandler(this.textBox_TextChanged);

            TabPage tp = new TabPage(name);
            tp.BackColor = System.Drawing.SystemColors.Window;
            tp.Controls.Add(tb);
            tp.Name = "tabPage" + count.ToString();
            tp.Padding = new System.Windows.Forms.Padding(3);
            tp.Size = new System.Drawing.Size(792, 391);
            tp.TabIndex = 0;
            tp.Tag = string.Empty;

            tabControl1.Controls.Add(tp);
            tabControl1.SelectedIndex = tabControl1.Controls.Count - 1;
            ActiveControl = tabControl1.SelectedTab.Controls[0];
            count++;
        }

        /// <summary>
        /// TextBox text changed event handler, sets a marker that the file has changed.
        /// </summary>
        private void textBox_TextChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab.Text[0] != '*')
            {
                tabControl1.SelectedTab.Text = "*" + tabControl1.SelectedTab.Text;
            }
        }

        /// <summary>
        /// TextBox text changed event handler, calls font dialog and sets font.
        /// </summary>
        private void MenuFont_Click(object sender, EventArgs e)
        {
            if (fd.ShowDialog() == DialogResult.Cancel)
                return;
            foreach (TabPage i in tabControl1.Controls)
            {
                i.Controls[0].Font = fd.Font;
            }
        }

        /// <summary>
        /// TextBox text changed event handler, calls color dialog and sets color.
        /// </summary>
        private void MenuColor_Click(object sender, EventArgs e)
        {
            if (cd.ShowDialog() == DialogResult.Cancel)
                return;
            foreach (TabPage i in tabControl1.Controls)
            {
                i.Controls[0].ForeColor = cd.Color;
            }
        }

        /// <summary>
        /// Method, that call Save-method for all pages.
        /// </summary>
        private void SaveAll()
        {
            foreach (TabPage i in tabControl1.Controls)
            {
                tabControl1.SelectedTab = i;
                Save();
            }
        }

        /// <summary>
        /// Method, that save current file.
        /// </summary>
        private void Save()
        {
            if (tabControl1.SelectedTab.Text[0] != '*')
            {
                return;
            }
            if (tabControl1.SelectedTab.Tag.ToString() == string.Empty)
            {
                SaveAs();
                return;
            }

            StreamWriter writer = new StreamWriter(tabControl1.SelectedTab.Tag.ToString());
            writer.WriteLine(tabControl1.SelectedTab.Controls[0].Text);
            writer.Close();
            tabControl1.SelectedTab.Text = tabControl1.SelectedTab.Text.Remove(0,1);
        }

        /// <summary>
        /// Method, that call save file dialog and save file.
        /// </summary>
        private void SaveAs()
        {

            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.InitialDirectory = Directory.GetCurrentDirectory();
                saveFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                saveFileDialog.FilterIndex = 2;
                saveFileDialog.RestoreDirectory = true;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    path = saveFileDialog.FileName;

                    var fileStream = saveFileDialog.OpenFile();

                    StreamWriter writer = new StreamWriter(fileStream);
                    writer.WriteLine(tabControl1.SelectedTab.Controls[0].Text);
                    tabControl1.SelectedTab.Tag = path;
                    writer.Close();
                    tabControl1.SelectedTab.Text = Path.GetFileName(path);
                }
            }
        }

        /// <summary>
        /// Method, that check if the file is saved.
        /// </summary>
        private void Unsave()
        {
            foreach (TabPage i in tabControl1.Controls)
            {
                if (i.Text[0] == '*' && i.Controls[0].Text.Length > 0)
                {
                    
                    if (MessageBox.Show("Do you want to save changes to your text?", this.Text,
                MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        SaveAll();
                    }
                }
            }
        }
    }
}
