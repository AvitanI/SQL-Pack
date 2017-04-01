using Microsoft.SqlServer.Management.UI.VSIntegration.ObjectExplorer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQL_Pack.MenuItems
{
    public abstract class MenuItemAbstract : ToolsMenuItemBase, IWinformsMenuHandler, IContextMenuItem
    {
        /// <summary>
        /// Main context menu item
        /// </summary>
        //protected ToolStripMenuItem mainItem;

        /// <summary>
        /// contructors
        /// </summary>
        public MenuItemAbstract() {}

        protected ToolStripMenuItem GetMainItem()
        {
            ToolStripMenuItem mainItem = new ToolStripMenuItem("SQL Pack Tool");
            mainItem.Image = SQL_Pack.Properties.Resources.package;
            return mainItem;
        }

        /// <summary>
        /// abstract methods
        /// </summary>
        /// <returns></returns>
        public abstract ToolStripItem[] GetMenuItems();
        public abstract void SearchValue(object sender, EventArgs e);

        #region Override Methods
        /// <summary>
        /// Invokes this instance.
        /// </summary>
        protected override void Invoke() { }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        public abstract override object Clone();
        #endregion
    }
}
