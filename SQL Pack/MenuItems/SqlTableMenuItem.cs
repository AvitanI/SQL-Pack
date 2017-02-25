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

            return new ToolStripItem[] { this.mainItem };
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
            //string sqlStatement = string.Format("SELECT COUNT(*) FROM {0}.{1}", schema, tableName);
            //Utils.GetTableFullSearch();
            NodeContextInfo nodeCtx = new NodeContextInfo(NodeType.Table, this.Parent);
            //MessageBox.Show("table: " + nodeCtx.Table);
            string value = Utils.ShowDialog("Insert value to search:", string.Format("Search Value In Table {0}", nodeCtx.Table));
            if (!string.IsNullOrEmpty(value))
            {
                Utils.CreateNewBlankScript(QueryBuilder.GetTableFullSearch(value, nodeCtx.Table), true);
            }
            //string tableName = nodeCtx.Table;
            //string schema = nodeCtx.Schema;
            //string database = nodeCtx.Database;
            //string connectionString = nodeCtx.ConnectionString;
            //string sqlStatement = string.Format("SELECT COUNT(*) FROM {0}.{1}", schema, tableName);

            //SqlCommand command = new SqlCommand(sqlStatement);
            //command.Connection = new SqlConnection(connectionString);
            //command.Connection.Open();
            //int tableCount = int.Parse(command.ExecuteScalar().ToString());
            //command.Connection.Close();

            //StringBuilder resultCaption = new StringBuilder().AppendFormat("{0} /*{1:n0}*/", sqlStatement, tableCount);

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




            //string s = "";
            //foreach (Column item in columns)
            //{
            //    //Column duplicatedColumn = new Column(currentTable, nodeCtx.Column + "_Duplicate");
            //    //duplicatedColumn.DataType = currentColumn.DataType;
            //    //duplicatedColumn.Nullable = true;//currentColumn.Nullable;
            //    //duplicatedColumn.DataType.MaximumLength = currentColumn.DataType.MaximumLength;

            //    //// add the new column
            //    //currentTable.Columns.Add(duplicatedColumn);
            //    //duplicatedColumn.Create();
            //}
            //MessageBox.Show(s);



            /*
            DialogResult dialogResult = MessageBox.Show("Copy data also?", "Duplicate Table", MessageBoxButtons.YesNo);
            bool copyData = (dialogResult == DialogResult.Yes);
            var sqlDuplicateQuery = (copyData) ? QueryBuilder.duplicateTableQueryWithData : QueryBuilder.duplicateTableQuery;
            NodeContextInfo nodeCtx = new NodeContextInfo(NodeType.Table, this.Parent);
            var sqlStatement = string.Format(sqlDuplicateQuery, nodeCtx.Database, nodeCtx.Schema, nodeCtx.Table, nodeCtx.Table + "_Duplicate");
            QueryBuilder.AddDuplicateTable(nodeCtx.ConnectionString, sqlStatement);
            MessageBox.Show("Table " + nodeCtx.Table + " Was Duplicated! \n refresh schema to see changes");
            */
        }
        #endregion

        private static StringBuilder SMOGenerateSQL(string currenServerName, string dbName, string tableName, string tableSchema)
        {
            Server server = new Server(currenServerName);
            Database db = server.Databases[dbName];
            List<Urn> list = new List<Urn>();


            list.Add(db.Tables[tableName, tableSchema].Urn);

            foreach (Index index in db.Tables[tableName, tableSchema].Indexes)
            {
                MessageBox.Show("index.Urn: " + index.Urn);
                list.Add(index.Urn);
            }

            foreach (ForeignKey foreignKey in db.Tables[tableName, tableSchema].ForeignKeys)
            {
                list.Add(foreignKey.Urn);
            }

            foreach (Trigger triggers in db.Tables[tableName, tableSchema].Triggers)
            {
                list.Add(triggers.Urn);
            }

            Scripter scripter = new Scripter();
            scripter.Server = server;
            scripter.Options.IncludeHeaders = true;
            scripter.Options.SchemaQualify = true;
            scripter.Options.SchemaQualifyForeignKeysReferences = true;
            scripter.Options.NoCollation = true;
            scripter.Options.DriAllConstraints = true;
            scripter.Options.DriAll = true;
            scripter.Options.DriAllKeys = true;
            scripter.Options.DriIndexes = true;
            scripter.Options.ClusteredIndexes = true;
            scripter.Options.NonClusteredIndexes = true;
            scripter.Options.ToFileOnly = false;
            StringCollection scriptedSQL = scripter.Script(list.ToArray());

            StringBuilder sb = new StringBuilder();

            foreach (string s in scriptedSQL)
            {
                sb.AppendLine(s);
            }

            return sb;
        }

        private static StringBuilder SMODuplicateTable(string currenServerName, string dbName, string tableName, string tableSchema)
        {
            Server server = new Server(currenServerName);
            Database db = server.Databases[dbName];
            //Table t = new Table(db, tableName + "_Duplicated");
            List<Urn> list = new List<Urn>();


            Table t = db.Tables[tableName, tableSchema];
            t.Rename(tableName + "_Duplicated");
            list.Add(t.Urn);
            MessageBox.Show("my urn: " + t.Urn);
            //t.Create();
            foreach (Index index in db.Tables[tableName, tableSchema].Indexes)
            {
                list.Add(index.Urn);
            }

            foreach (ForeignKey foreignKey in db.Tables[tableName, tableSchema].ForeignKeys)
            {
                list.Add(foreignKey.Urn);
            }

            foreach (Trigger triggers in db.Tables[tableName, tableSchema].Triggers)
            {
                list.Add(triggers.Urn);
            }

            Scripter scripter = new Scripter();
            scripter.Server = server;
            scripter.Options.IncludeHeaders = true;
            scripter.Options.SchemaQualify = true;
            scripter.Options.SchemaQualifyForeignKeysReferences = true;
            scripter.Options.NoCollation = true;
            scripter.Options.DriAllConstraints = true;
            scripter.Options.DriAll = true;
            scripter.Options.DriAllKeys = true;
            scripter.Options.DriIndexes = true;
            scripter.Options.ClusteredIndexes = true;
            scripter.Options.NonClusteredIndexes = true;
            scripter.Options.ToFileOnly = false;
            StringCollection scriptedSQL = scripter.Script(list.ToArray());

            StringBuilder sb = new StringBuilder();

            foreach (string s in scriptedSQL)
            {
                sb.AppendLine(s);
            }

            return sb;
        }
    }
}
