using Microsoft.SqlServer.Management.UI.VSIntegration.ObjectExplorer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQL_Pack.Models
{
    class ObjectExplorerExtender : IObjectExplorerExtender
    {
        # region private members
        private IServiceProvider Package { get; }
        #endregion

        # region constructors
        public ObjectExplorerExtender(IServiceProvider package)
        {
            Package = package;
        }
        #endregion

        # region methods
        public TreeView GetObjectExplorerTreeView()
        {
            var objectExplorerService = (IObjectExplorerService)Package.GetService(typeof(IObjectExplorerService));
            if (objectExplorerService != null)
            {
                var oesTreeProperty = objectExplorerService.GetType().GetProperty("Tree", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (oesTreeProperty != null)
                    return (TreeView)oesTreeProperty.GetValue(objectExplorerService, null);
                //else
                //    debug_message("Object Explorer Tree property not found.");
            }
            //else
            //    debug_message("objectExplorerService == null");

            return null;
        }

        public INodeInformation GetNodeInformation(TreeNode node)
        {
            INodeInformation result = null;
            IServiceProvider serviceProvider = node as IServiceProvider;
            if (serviceProvider != null)
            {
                result = (serviceProvider.GetService(typeof(INodeInformation)) as INodeInformation);
                //debug_message("NodeInformation\n UrnPath:{0}\n Name:{1}\n InvariantName:{2}\n Context:{3}\n NavigationContext:{4}", ni.UrnPath, ni.Name, ni.InvariantName, ni.Context, ni.NavigationContext);
            }
            return result;
        }

        # endregion
    }
}
