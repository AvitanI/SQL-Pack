using EnvDTE;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Smo;
using SQL_Pack.Models;
using SQL_Pack.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQL_Pack.MenuItems
{
    class SqlTableMenuItem : MenuItemAbstract
    {
        #region Class Variables

        #endregion
        private ToolStripMenuItem mainItem;

        #region Constructor
        /// <summary>
        /// init base constructor
        /// </summary>
        public SqlTableMenuItem() : base() { }
        #endregion

        public override object Clone() { return new SqlTableMenuItem(); }

        #region IWinformsMenuHandler Members
        /// <summary>
        /// Gets the menu items.
        /// </summary>
        /// <returns></returns>
        public override ToolStripItem[] GetMenuItems()
        {
            this.mainItem = base.GetMainItem();

            /*context submenu item - search*/
            ToolStripMenuItem insertItem = new ToolStripMenuItem("Search Value");
            insertItem.Image = SQL_Pack.Properties.Resources.search;
            insertItem.Tag = false;
            insertItem.Click += new EventHandler(SearchValue);
            this.mainItem.DropDownItems.Add(insertItem);

            /*context submenu item - count*/
            ToolStripMenuItem countItem = new ToolStripMenuItem("Count");
            countItem.Image = SQL_Pack.Properties.Resources.count;
            countItem.Tag = false;
            countItem.Click += new EventHandler(Count);
            this.mainItem.DropDownItems.Add(countItem);

            /*context submenu item - count*/
            ToolStripMenuItem duplicateItem = new ToolStripMenuItem("Duplicate Table");
            duplicateItem.Image = SQL_Pack.Properties.Resources.duplicate;
            duplicateItem.Tag = false;
            duplicateItem.Click += new EventHandler(Duplicate);
            this.mainItem.DropDownItems.Add(duplicateItem);

            ///*context submenu item - count*/
            //ToolStripMenuItem testItem = new ToolStripMenuItem("Test");
            ////testItem.Image = SQL_Pack.Properties.Resources.duplicate;
            //testItem.Tag = false;
            //testItem.Click += new EventHandler(Test);
            //this.mainItem.DropDownItems.Add(testItem);

            return new ToolStripItem[] { this.mainItem };
        }

        private void Test(object sender, EventArgs e)
        {
            NodeContextInfo nodeCtx = new NodeContextInfo(NodeType.Table, this.Parent);
            //Server server = new Server(nodeCtx.Server);
            //Database db = server.Databases[nodeCtx.Database];

            //db.ExecuteWithResults(string.Format("SELECT * FROM [{0}].[{1}].[{2}]", nodeCtx.Database, nodeCtx.Schema, nodeCtx.Table));
            try
            {
                //if (dte == null) { MessageBox.Show("nullllllllllllll"); }
                //Window window = dte.Windows.Item(Constants.vsWindowKindOutput);
                //OutputWindow outputWindow = (OutputWindow)window.Object;
                //OutputWindowPane owp;
                //owp = outputWindow.OutputWindowPanes.Add("new pane");
                //owp.OutputString("hello");
                MessageBox.Show(nodeCtx.ConnectionString);
            }
            catch(Exception er) { MessageBox.Show(er.ToString()); }
        }

        #endregion

        #region Custom Click Events
        /// <summary>
        /// Handles the Click event of the Count control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        public override void SearchValue(object sender, EventArgs e)
        {
            NodeContextInfo nodeCtx = new NodeContextInfo(NodeType.Table, this.Parent);

            string value = Utils.ShowDialog("Insert value to search:", string.Format("Search Value In Table {0}", nodeCtx.Table));
            if (!string.IsNullOrEmpty(value))
            {
                Utils.CreateNewBlankScript(QueryBuilder.GetTableFullSearch(value, nodeCtx.Table), true);
            }
        }

        private void Count(object sender, EventArgs e)
        {
            NodeContextInfo nodeCtx = new NodeContextInfo(NodeType.Table, this.Parent);
            var sqlStatement = string.Format(QueryBuilder.countQuery, nodeCtx.Database, nodeCtx.Schema, nodeCtx.Table);
            var count = QueryBuilder.GetCountOF(nodeCtx.ConnectionString, sqlStatement);
            StringBuilder result = new StringBuilder();
            result.AppendLine("-- Counting Rows In Table *" + nodeCtx.Table + "*");
            result.AppendLine("-- Result: " + count);
            result.AppendLine("-- Running Query: " + sqlStatement);
            Utils.CreateNewBlankScript(result);
        }

        private void Duplicate(object sender, EventArgs e)
        {
            try
            {
                NodeContextInfo nodeCtx = new NodeContextInfo(NodeType.Table, this.Parent);
                Server server = new Server(nodeCtx.Server);
                Database db = server.Databases[nodeCtx.Database];

                // in case of there is already column that 
                if (nodeCtx.Table.Contains("_Duplicate") ||
                    (db.Tables[nodeCtx.Table + "_Duplicate"]) != null)
                {
                    MessageBox.Show("There Is Already Table With The Same Name");
                    return;
                }

                DialogResult dialogResult = MessageBox.Show("Copy data also?", "Duplicate Table", MessageBoxButtons.YesNo);
                bool copyData = (dialogResult == DialogResult.Yes);

                Table currentTable = db.Tables[nodeCtx.Table, nodeCtx.Schema]; // get current table
                ColumnCollection columns = currentTable.Columns; // get columns of current table
                Table newTable = new Table(db, nodeCtx.Table + "_Duplicate"); // create new table to add it to current db

                foreach (Column col in columns)
                {
                    // create new column
                    Column c = new Column(newTable, col.Name);
                    c.DataType = col.DataType;
                    c.Nullable = true;
                    c.DataType.MaximumLength = col.DataType.MaximumLength;

                    // add the new column
                    newTable.Columns.Add(c);
                }

                db.Tables.Add(newTable);
                newTable.Create();

                if (copyData)
                {
                    string sqlStatement = "INSERT INTO {0} SELECT * FROM [{1}].[{2}].[{3}]";
                    SqlCommand command = new SqlCommand(string.Format(sqlStatement, newTable.Name, nodeCtx.Database, nodeCtx.Schema, nodeCtx.Table));
                    command.Connection = new SqlConnection(nodeCtx.ConnectionString);
                    command.Connection.Open();
                    command.ExecuteReader();
                    command.Connection.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        #endregion
    }
}
