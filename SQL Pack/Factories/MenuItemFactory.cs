using Microsoft.SqlServer.Management.UI.VSIntegration.ObjectExplorer;
using SQL_Pack.MenuItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQL_Pack.Factories
{
    public static class MenuItemFactory
    {
        private static bool IsTableMenuAdded = false;
        private static bool IsColumnMenuAdded = false;
        private static bool IsDBMenuAdded = false;

        public static MenuItemAbstract CreateMenuItem(INodeInformation node)
        {
            MenuItemAbstract menuItem = null;
            switch (node.Parent.InvariantName)
            {
                case "Databases":
                    if (!IsDBMenuAdded)
                    {
                        menuItem = new SqlDBMenuItem();
                        //AddMenuItemToHierarchyObject(node, new SqlDBMenuItem());
                        IsDBMenuAdded = true;
                    }
                    break;
                case "UserTables":
                    if (!IsTableMenuAdded)
                    {
                        menuItem = new SqlTableMenuItem();
                        //AddMenuItemToHierarchyObject(node, new SqlTableMenuItem());
                        IsTableMenuAdded = true;
                    }
                    break;
                case "Columns":
                    if (!IsColumnMenuAdded)
                    {
                        menuItem = new SqlColumnMenuItem();
                        //AddMenuItemToHierarchyObject(node, new SqlColumnMenuItem());
                        IsColumnMenuAdded = true;
                    }
                    break;
                default:
                    return null;
            }
            return menuItem;
        }
    }
}
