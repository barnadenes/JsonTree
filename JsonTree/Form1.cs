using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Windows.Forms;

namespace JsonTree
{
    public partial class JSONTree : Form
    {
        private OpenFileDialog dialog;

        public JSONTree()
        {
            InitializeComponent();
        }

        private void openJSONToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.dialog = new OpenFileDialog();
            dialog.Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*";
            dialog.ShowDialog();

            string path = dialog.FileName;
            string extension = Path.GetExtension(path);

            if (extension != ".json")
            {
                MessageBox.Show("Not a JSON file");
            }
            else
            {               
                using (var reader = new StreamReader(path))
                using (var jsonReader = new JsonTextReader(reader))
                {
                    var root = JToken.Load(jsonReader);
                    DisplayTreeView(root, Path.GetFileNameWithoutExtension(path));                 
                }               
            }
        }

        private void DisplayTreeView(JToken root, string rootName)
        {
            treeView.BeginUpdate();
            try
            {
                treeView.Nodes.Clear();
                var tNode = treeView.Nodes[treeView.Nodes.Add(new TreeNode(rootName))];
                tNode.Tag = root;

                AddNode(root, tNode);              
            }
            finally
            {
                treeView.EndUpdate();
            }
        }

        private void AddNode(JToken token, TreeNode inTreeNode)
        {
            if (token == null)
                return;
            if (token is JValue)
            {
                var childNode = inTreeNode.Nodes[inTreeNode.Nodes.Add(new TreeNode(token.ToString()))];
                childNode.Tag = token;
            }
            else if (token is JObject)
            {
                var obj = (JObject)token;
                foreach (var property in obj.Properties())
                {
                    var childNode = inTreeNode.Nodes[inTreeNode.Nodes.Add(new TreeNode(property.Name))];
                    childNode.Tag = property;
                    AddNode(property.Value, childNode);
                }
            }
            else if (token is JArray)
            {
                var array = (JArray)token;
                for (int i = 0; i < array.Count; i++)
                {
                    var childNode = inTreeNode.Nodes[inTreeNode.Nodes.Add(new TreeNode(i.ToString()))];
                    childNode.Tag = array[i];
                    AddNode(array[i], childNode);
                }
            }
        }
    }
}
