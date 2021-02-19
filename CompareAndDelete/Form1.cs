using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CompareAndDelete
{
    public partial class Form1 : Form
    {
        public CommonOpenFileDialog folderDialog = new CommonOpenFileDialog();
        public Dictionary<ulong, List<string>> hashCache = new Dictionary<ulong, List<string>>();
        public List<string> paths = new List<string>();

        public Form1()
        {
            InitializeComponent();
            folderDialog.IsFolderPicker = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AddFolder();
        }

        private void AddFolder()
        {
            if (folderDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                paths.Add(folderDialog.FileName);
                AddFolder();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            hashGatherer(paths.ToArray());
            foreach(var set in hashCache)
            {
                if(set.Value.Count > 1)
                {
                    for(int file = 0; file < set.Value.Count; file++)
                    {
                        if(file != 0)
                        {
                            if (File.Exists(set.Value[file]))
                            {
                                File.Delete(set.Value[file]);
                            }
                        }
                    }
                }
            }
        }

        private void hashGatherer(string[] paths)
        {
            for(int i = 0; i < paths.Length; i++)
            {
                hashGatherer(Directory.GetDirectories(paths[i]));
                string[] files = Directory.GetFiles(paths[i]);

                foreach(string file in files)
                {
                    var hash = Extensions.Data.XXHash.XXH64(File.ReadAllBytes(file), 0);
                    if(hashCache.ContainsKey(hash))
                    {
                        hashCache[hash].Add(file);
                    } else
                    {
                        hashCache.Add(hash, new List<string>() { file });
                    }
                }
            }
        }
    }
}
