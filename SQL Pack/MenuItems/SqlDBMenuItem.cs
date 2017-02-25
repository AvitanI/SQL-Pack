using SQL_Pack.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQL_Pack.MenuItems
{
    /// <summary>
    /// database menu item extension
    /// </summary>
    class SqlDBMenuItem : MenuItemAbstract
    {
        #region Class Variables
        private ToolStripMenuItem mainItem;
        #endregion

        #region Constructor
        public SqlDBMenuItem() : base()
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
            return new SqlDBMenuItem();
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

            return new ToolStripItem[] { this.mainItem };
        }
        #endregion

        #region Custom Click Events
        public override void SearchValue(object sender, EventArgs e)
        {
            string value = Utils.ShowDialog("Insert value to search:", "Search Value In All DB");
            if (!string.IsNullOrEmpty(value))
            {
                Utils.CreateNewBlankScript(QueryBuilder.GetDBFullSearch(value), true);
            }
        }
        #endregion
    }
}
