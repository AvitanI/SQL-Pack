using Microsoft.SqlServer.Management.Smo;
using SQL_Pack.Models;
using SQL_Pack.Utilities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQL_Pack.MenuItems
{
    /// <summary>
    /// table menu item extension
    /// </summary>
    class SqlColumnMenuItem : MenuItemAbstract
    {
        #region Class Variables
        private const string DUPLICATE = "_Duplicate";
        private ToolStripMenuItem mainItem;
        #endregion

        #region Constructor
        public SqlColumnMenuItem() : base()
        {

        }
        #endregion

        #region Override Methods
        /// <summary>
        /// Invoke
        /// </summary>
        protected override void Invoke() { }

        /// <summary>
        /// Clone
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            return new SqlColumnMenuItem();
        }
        #endregion

        #region IWinformsMenuHandler Members
        public override ToolStripItem[] GetMenuItems()
        {
            this.mainItem = base.GetMainItem();

            /*context submenu item - search*/
            ToolStripMenuItem insertItem = new ToolStripMenuItem("Search Value");
            insertItem.Image = SQL_Pack.Properties.Resources.search;
            insertItem.Tag = false;
            insertItem.Click += new EventHandler(SearchValue);
            this.mainItem.DropDownItems.Add(insertItem);

            /*context submenu item - search*/
            ToolStripMenuItem duplicateItem = new ToolStripMenuItem("Duplicate Column");
            duplicateItem.Image = SQL_Pack.Properties.Resources.duplicate;
            duplicateItem.Tag = false;
            duplicateItem.Click += new EventHandler(Duplicate);
            this.mainItem.DropDownItems.Add(duplicateItem);

            return new ToolStripItem[] { this.mainItem };
        }
        #endregion

        #region Custom Click Events
        public override void SearchValue(object sender, EventArgs e)
        {
            NodeContextInfo nodeCtx = new NodeContextInfo(NodeType.Column, this.Parent);

            string value = Utils.ShowDialog("Insert value to search:", string.Format("Search Value In Column {0}", nodeCtx.Column));
            if (!string.IsNullOrEmpty(value))
            {
                Utils.CreateNewBlankScript(QueryBuilder.GetColumnFullSearch(value, nodeCtx.Database, nodeCtx.Schema, nodeCtx.Table, nodeCtx.Column), true);
            }
        }

        private void Duplicate(object sender, EventArgs e)
        {
            try
            {
                NodeContextInfo nodeCtx = new NodeContextInfo(NodeType.Column, this.Parent);
                Server server = new Server(nodeCtx.Server);
                Database db = server.Databases[nodeCtx.Database];
                // get current table
                Table currentTable = db.Tables[nodeCtx.Table, nodeCtx.Schema];
                // get columns of current table
                ColumnCollection col = currentTable.Columns;

                // in case of there is already column that 
                if (nodeCtx.Column.Contains(DUPLICATE) ||
                    (col[nodeCtx.Column + DUPLICATE]) != null)
                {
                    MessageBox.Show("There Is Already Column With The Same Name");
                    return;
                }

                DialogResult dialogResult = MessageBox.Show("Copy Data Also?", "Duplicate Column", MessageBoxButtons.YesNo);
                bool copyData = (dialogResult == DialogResult.Yes);

                // get current column by name
                Column currentColumn = col[nodeCtx.Column];

                // create duplicated column
                Column duplicatedColumn = new Column(currentTable, nodeCtx.Column + "_Duplicate");
                duplicatedColumn.DataType = currentColumn.DataType;
                duplicatedColumn.Nullable = true;//currentColumn.Nullable;
                duplicatedColumn.DataType.MaximumLength = currentColumn.DataType.MaximumLength;

                // add the new column
                currentTable.Columns.Add(duplicatedColumn);
                duplicatedColumn.Create();

                // copy data
                if (copyData)
                {
                    string sqlStatement = "UPDATE [{0}].[{1}].[{2}] SET {3} = {4}";
                    SqlCommand command = new SqlCommand(string.Format(sqlStatement, nodeCtx.Database, nodeCtx.Schema, nodeCtx.Table, nodeCtx.Column + DUPLICATE, nodeCtx.Column));
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
