using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Windows.Forms;

namespace AddNewUser
{
    public partial class Form1 : Form
    {
        public ContextItems contextItems;
        public char[] alphabet;
        public Form1()
        {
            InitializeComponent();
            contextItems = new ContextItems();
            alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz".ToArray();
            for (int i = 0; i < contextItems.order.Count; i++)
            {
                treeView1.Nodes.Add(contextItems.visualNames[i]);
            }
        }
        private void RefreshTreeView()
        {
            treeView1.Nodes.Clear();
            for (int i = 0; i < contextItems.order.Count; i++)
            {
                treeView1.Nodes.Add(contextItems.visualNames[i]);
            }
            removeButton.Enabled = false;
            GoButton.Enabled = false;
            txtName.Text = "";
            txtCmd.Text = "";
            checkBox1.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (txtName.Text == "")
            {
                MessageBox.Show("Entry needs to have a name!");
            }
            else
            {
                int index = treeView1.SelectedNode.Index;
                contextItems.visualNames[index] = txtName.Text;
                contextItems.commands[index] = txtCmd.Text;
                contextItems.luaShield[index] = checkBox1.Checked;
                RefreshTreeView();
                //contextItems.visualNames[]
                //string regName = txtName.Text;
                //string regCmd = txtCmd.Text;
                //string newReg = regTemplate.Replace("{0}", regName);
                //newReg = newReg.Replace("{1}", regCmd);
                //string regKeyOne = @"Directory\Background\shell\" + regName;
                //string regKeyTwo = regKeyOne + @"\command";
                //string fullRegKey = @"Software\Classes\directory\Background\shell\" + regName + @"\command";
                //RegistryKey keyFull = Registry.CurrentUser.CreateSubKey(fullRegKey);
                //keyFull.SetValue(null, regCmd);
                //keyFull.Close();
                //MessageBox.Show("done");
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog browseDialog = new OpenFileDialog();
            browseDialog.DefaultExt = "*.exe";
            browseDialog.Filter = "Executable Files|*.exe";
            if (browseDialog.ShowDialog() == DialogResult.OK && browseDialog.FileName.Length > 0)
            {
                string browsePath = browseDialog.FileName;
                txtCmd.Text = browsePath;
            }
                
        }

        private void button1_Click_1(object sender, EventArgs e) // Commit to registry
        {
            if (MessageBox.Show("This is a destructive process and may lead to loss of context menu items! Are you sure you want to apply your changes?", "Sure?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                RegistryKey contextItemsInReg = Registry.CurrentUser.OpenSubKey(@"Software\Classes\Directory\Background\shell", true);
                foreach (string k in contextItemsInReg.GetSubKeyNames())
                {
                    contextItemsInReg.DeleteSubKeyTree(k); // Clear all the context items in registry
                }
                for (int i = 0; i < contextItems.order.Count; i++) // For each context item in the thing currently
                {
                    string v = contextItems.visualNames[i];
                    v = v.Replace(" ", "_"); // For the internal name, remove all spaces.
                    v = (alphabet[i]) + v; // Also affix the order number to the front of the item (Windows orders them alphabetically...)

                    RegistryKey thisKey = contextItemsInReg.CreateSubKey(v, true); // create the key, its name is devoid of spaces and has a number.
                    thisKey.SetValue(null, contextItems.visualNames[i]); // Set the (Default) value to the visual name (what the user will see)
                    RegistryKey thisCommand = thisKey.CreateSubKey("command", true); // Create the "command" key
                    thisCommand.SetValue(null, contextItems.commands[i]); // Inside the "command" key, set its (Default) value to the command entered by user.

                    if (contextItems.luaShield[i])
                    {
                        thisKey.SetValue("HasLUAShield", "", RegistryValueKind.String);
                    }
                }
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            int index = e.Node.Index;
            txtName.Text = contextItems.visualNames[index];
            txtCmd.Text = contextItems.commands[index];
            if (index == 0)
            {
                upButton.Enabled = false;
                downButton.Enabled = true;
            }
            else if (index+1 == treeView1.GetNodeCount(true))
            {
                downButton.Enabled = false;
                upButton.Enabled = true;
            }
            else
            {
                upButton.Enabled = true;
                downButton.Enabled = true;
            }
            if(treeView1.GetNodeCount(true) == 1)
            {
                downButton.Enabled = false;
                upButton.Enabled = false;
            }
            GoButton.Enabled = true;
            removeButton.Enabled = true;
            checkBox1.Enabled = true;
            checkBox1.Checked = contextItems.luaShield[index];
        }

        private void upButton_Click(object sender, EventArgs e)
        {
            int index = treeView1.SelectedNode.Index;
            int newIndex = index - 1;
            
            string prevIName = contextItems.internalNames[index - 1];
            string prevVName = contextItems.visualNames[index - 1];
            string prevCmd = contextItems.commands[index - 1];

            contextItems.internalNames[newIndex] = contextItems.internalNames[index]; // Set previous command in list
            contextItems.visualNames[newIndex] = contextItems.visualNames[index]; // To the one being moved up
            contextItems.commands[newIndex] = contextItems.commands[index];

            // Now we need to put the command that was there into the old index
            contextItems.internalNames[index] = prevIName;
            contextItems.visualNames[index] = prevVName;
            contextItems.commands[index] = prevCmd;

            RefreshTreeView();
            treeView1.SelectedNode = treeView1.Nodes[newIndex];
        }

        private void downButton_Click(object sender, EventArgs e)
        {
            int index = treeView1.SelectedNode.Index;
            int newIndex = index + 1;

            string prevIName = contextItems.internalNames[newIndex];
            string prevVName = contextItems.visualNames[newIndex];
            string prevCmd = contextItems.commands[newIndex];

            contextItems.internalNames[newIndex] = contextItems.internalNames[index]; // Set previous command in list
            contextItems.visualNames[newIndex] = contextItems.visualNames[index]; // To the one being moved up
            contextItems.commands[newIndex] = contextItems.commands[index];

            // Now we need to put the command that was there into the old index
            contextItems.internalNames[index] = prevIName;
            contextItems.visualNames[index] = prevVName;
            contextItems.commands[index] = prevCmd;

            RefreshTreeView();
            treeView1.SelectedNode = treeView1.Nodes[newIndex];
        }

        private void removeButton_Click(object sender, EventArgs e)
        {
            int indexToBeRemoved = treeView1.SelectedNode.Index;
            contextItems.internalNames.RemoveAt(indexToBeRemoved);
            contextItems.visualNames.RemoveAt(indexToBeRemoved);
            contextItems.commands.RemoveAt(indexToBeRemoved);
            contextItems.order.RemoveAt(indexToBeRemoved);
            for(int i = indexToBeRemoved; i < contextItems.order.Count; i++)
            {
                contextItems.order[i] = i + 1;
            }
            RefreshTreeView();
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            contextItems.order.Add(contextItems.order.Count + 1);
            contextItems.visualNames.Add("New Item");
            contextItems.commands.Add("");
            contextItems.internalNames.Add("newitem");
            contextItems.luaShield.Add(false);
            RefreshTreeView();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            contextItems.luaShield[treeView1.SelectedNode.Index] = !contextItems.luaShield[treeView1.SelectedNode.Index];
        }
    }
    public class ContextItems // Really shoulda made a struct to hold the actual values but eh
    {
        public List<int> order;
        public List<string> internalNames;
        public List<string> visualNames;
        public List<string> commands;
        public List<bool> luaShield;
        public ContextItems()
        {
            order = new List<int>();
            internalNames = new List<string>();
            visualNames = new List<string>();
            commands = new List<string>();
            luaShield = new List<bool>();
            RegistryKey contextItemsInReg = Registry.CurrentUser.OpenSubKey(@"Software\Classes\Directory\Background\shell");
            string[] subKeyNames = contextItemsInReg.GetSubKeyNames();
            for (int i = 0; i < contextItemsInReg.SubKeyCount; i++)
            {
                order.Add(i + 1);
                internalNames.Add(subKeyNames[i]);

                RegistryKey thisKey = contextItemsInReg.OpenSubKey(subKeyNames[i]);
                object thisKeyName = thisKey.GetValue(null);
                if (thisKeyName == null)
                {
                    visualNames.Add(subKeyNames[i]);
                }
                else
                {
                    visualNames.Add(thisKeyName.ToString());
                }

                RegistryKey commandKey = thisKey.OpenSubKey("command");
                string command = commandKey.GetValue(null).ToString();
                commands.Add(command);

                if(thisKey.GetValue("HasLUAShield") == null)
                {
                    luaShield.Add(false);
                }
                else
                {
                    luaShield.Add(true);
                }
            }
        }
    };
}
