using Microsoft.SqlServer.Management.UI.VSIntegration.ObjectExplorer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQL_Pack.Models
{
    interface IObjectExplorerExtender
    {
        TreeView GetObjectExplorerTreeView();
        INodeInformation GetNodeInformation(TreeNode node);
    }
}
