using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Spookie_Bois_Modpack_Manager
{
    public partial class Form1 : Form
    {
        public bool downloadLocked = false;

        public Form1()
        {
            using (var client = new WebClient())
            {
                WebClient webClient = client;
                string sourceFile = @"http://conni.transvibe.club/clientFile";
                string destFile = @"./mods.zip";
                webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadCompleted);
                webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);
                webClient.DownloadFileAsync(new Uri(sourceFile), destFile);
            }
            InitializeComponent();
            textBox1.Text = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\.minecraft";
        }
        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
        void reloadInfomation(CheckedListBox cListBox, GroupBox groupBox, Label modCountLabel, string packName)
        {
            cListBox.Items.Clear();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(@"http://conni.transvibe.club/modPack-info?packID=" + packName);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string content = new StreamReader(response.GetResponseStream()).ReadToEnd();
            string[] args = content.Split(',');
            groupBox.Text = args[0];
            groupBox.Text += " - " + args[1];
            for (int i = 2; i < args.Count() - 1; i++)
            {
                cListBox.Items.Add(args[i]);
                cListBox.SetItemChecked(i - 2, true);
                modCountLabel.Text = i - 1 + " Loaded Mods";
            }
        }
        private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            downloadLocked = true;
            progressBar1.Value = e.ProgressPercentage;
        }
        private void DownloadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            downloadLocked = false;
            MessageBox.Show("The download is completed!");
        }
        void downloadMods(string packName)
        {
            if(File.Exists(@"./mods.zip"))
                File.Delete(@"./mods.zip");

            if (downloadLocked) {
                MessageBox.Show("currently downloading another file please wait. (if you believe this is a bug please contact the author.)");
            }
            using (var client = new WebClient())
            {
                WebClient webClient = client;
                string sourceFile = @"http://conni.transvibe.club/downloadMods?packID=" + packName;
                string destFile = @"./mods.zip";
                webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadCompleted);
                webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);
                webClient.DownloadFileAsync(new Uri(sourceFile), destFile);


           //     client.DownloadFile(@"http://conni.transvibe.club/downloadMods?packID=" + packName, "mods.zip");
            }
        }
        void extract(string packName)
        {
            string zipPath = @".\mods.zip";
            string extractPath = @".\Packs\" + packName;

            if(Directory.Exists(extractPath))
                Directory.Delete(extractPath, true);
            Directory.CreateDirectory(extractPath);

            try
            {
                ZipFile.ExtractToDirectory(zipPath, extractPath);

            }
            catch(IOException e)
            {

            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                reloadInfomation(checkedListBox1, groupBox2, label1, "PogCraft");

            }
            catch(WebException)
            {
                MessageBox.Show("This pack doesnt exist on the server please contact the server host asap!"); 
            }
        }
        private void label1_Click(object sender, EventArgs e)
        {

        }
        private void Form1_Load(object sender, EventArgs e)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(@"http://conni.transvibe.club/clientInfo");
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string content = new StreamReader(response.GetResponseStream()).ReadToEnd();
            if (content != "V1.0.0.0")
            {
                MessageBox.Show("Outdated Client");
                this.Close();
                this.Dispose();
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            downloadMods("PogCraft");
        }
        private void button4_Click(object sender, EventArgs e)
        {
            reloadInfomation(checkedListBox2, groupBox1, label2, "BugCraft");

        }
        private void button3_Click(object sender, EventArgs e)
        {
            downloadMods("BugCraft");
        }

        void addNewMods(string packName)
        {
            string mcPath = textBox1.Text + @"\mods";
            string modsPath = @"./Packs/" + packName + @"/mods";

            List<string> dirs = new List<string>();

            CheckedListBox.CheckedItemCollection checkedItems = checkedListBox2.CheckedItems;

            foreach ( string name in checkedItems)
            {
                dirs.Add(modsPath + "/" + name);
            }

            int Day = DateTime.Today.Day;
            foreach (string path in dirs)
            {
                if (File.Exists(path))
                {
                    if (path.Contains(".jar"))
                    {
                        string fName = Path.GetFileName(path);

                        try
                        {
                            File.Copy(path, mcPath + @"\" + fName);
                        }
                        catch (IOException e)
                        {
                            Console.WriteLine("Cannot create a file when that file already exists.");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("{ 0} is not a valid file.", path);
                }
            }

        }
        // C:\Users\frayb\AppData\Roaming\.minecraft
        void moveCurrentMods()
        {
            if(!Directory.Exists(textBox1.Text + @"\versions")) {
                MessageBox.Show("Given MC Path isnt valid");
            }

            string mcPath = textBox1.Text + @"\mods";
            string[] dirs = Directory.GetFiles(mcPath);

            int Day = DateTime.Today.Millisecond * DateTime.Today.Second * DateTime.Today.Day;
            foreach (string path in dirs)
            {
                if (File.Exists(path))
                {
                    // This path is a file
                    if (path.Contains(".jar"))
                    {
                        if (!Directory.Exists(mcPath + "/old/" + Day + "/"))
                        {
                            Directory.CreateDirectory(mcPath + "/old/" + Day + "/");
                        }
                        string b = mcPath + @"\old\" + Day + @"\";
                        if (!File.Exists(b + @"" + Path.GetFileName(path)))
                        {
                            File.Move(path, b + @"" + Path.GetFileName(path));
                        }
                        else
                        {
                            File.Delete(path);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("{ 0} is not a valid file.", path);
                }
            }
        }
        private void button5_Click(object sender, EventArgs e)
        {
            //copyToMc("PogCraft");
            moveCurrentMods();

            addNewMods("PogCraft");
        }
        private void button6_Click(object sender, EventArgs e)
        {
            //copyToMc("BugCraft");
            moveCurrentMods();

            addNewMods("BugCraft");

        }
        private void button7_Click(object sender, EventArgs e)
        {
            extract("PogCraft");
        }
        private void button8_Click(object sender, EventArgs e)
        {
            extract("BugCraft");

        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
        private void checkedListBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button12_Click(object sender, EventArgs e)
        {

        }

        private void statusStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {

        }

        private void button13_Click(object sender, EventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {

                System.Windows.Forms.DialogResult result = dialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    textBox1.Text = dialog.SelectedPath;
                }

            }
        }

        private void button14_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
        }
    }
}
