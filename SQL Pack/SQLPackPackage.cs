//------------------------------------------------------------------------------
// <copyright file="SQLPackPackage.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;
using Microsoft.SqlServer.Management.UI.VSIntegration.ObjectExplorer;
using Microsoft.SqlServer.Management.SqlStudio.Explorer;
using Microsoft.SqlServer.Management;
using SQL_Pack.MenuItems;
using System.Windows.Forms;
using EnvDTE;
using Microsoft.VisualStudio.CommandBars;
using System.Reflection;

namespace SQL_Pack
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [Guid(SQLPackPackage.PackageGuidString)]
    [ProvideAutoLoad("d114938f-591c-46cf-a785-500a82d97410")] //CommandGuids.ObjectExplorerToolWindowIDString
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    public sealed class SQLPackPackage : Package
    {
        /// <summary>
        /// SQLPackPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "49dfca8b-294e-4fa6-8ef5-f59fcaffdf01";
        private HierarchyObject dbMenu = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="SQLPackPackage"/> class.
        /// </summary>
        public SQLPackPackage()
        {
            // Inside this method you can place any initialization code that does not require
            // any Visual Studio service because at this point the package object is created but
            // not sited yet inside Visual Studio environment. The place to do all the other
            // initialization is the Initialize method.
        }

        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            ContextService contextService = GetService(typeof(IContextService)) as ContextService;
            contextService.ActionContext.CurrentContextChanged += ActionContextOnCurrentContextChanged;

            // Reg setting is removed after initialize. Wait short delay then recreate it.
            DelayAddSkipLoadingReg();

            //dte = (EnvDTE.DTE)GetService(typeof(EnvDTE.DTE));

            //Window window = dte.Windows.Item(EnvDTE.Constants.vsWindowKindOutput);
            //OutputWindow outputWindow = (OutputWindow)window.Object;
            //OutputWindowPane owp;
            ////outputWindow.ActivePane.Activate();

            //owp = outputWindow.OutputWindowPanes.Add("new pane");
            //owp.OutputString("hello");
            //owp.Activate();
            try
            {
                //Microsoft.VisualStudio.CommandBars.CommandBar sqlQueryGridPane = ((CommandBars)dte.CommandBars)["SQL Results Grid Tab Context"];
                //MessageBox.Show(sqlQueryGridPane.);
                //CommandBarControl cmdBarControl2 = sqlQueryGridPane.Controls.Add(MsoControlType.msoControlButton, Missing.Value, Missing.Value, Missing.Value, true);
                
                //var myButton = (CommandBarButton)cmdBarControl2;
                //myButton.Visible = true;
                //myButton.Enabled = true;
                //myButton.Caption = "XXX SQL";
                //myButton.FaceId = 224;
                //myButton.Style = MsoButtonStyle.msoButtonIconAndCaption;
                //myButton.Click += new _CommandBarButtonEvents_ClickEventHandler(btnMEssageBoxxResults_Click);
            }
            catch(Exception ee) { MessageBox.Show(ee.ToString()); }


        }

        private void btnMEssageBoxxResults_Click(CommandBarButton Ctrl, ref bool CancelDefault)
        {
            MessageBox.Show("kokokokoko");
        }

        private void ActionContextOnCurrentContextChanged(object sender, EventArgs e)
        {
            try
            {
                INodeInformation[] nodes;
                INodeInformation node;
                int nodeCount;
                IObjectExplorerService objectExplorer = GetService(typeof(IObjectExplorerService)) as ObjectExplorerService;

                if (objectExplorer != null)
                {
                    objectExplorer.GetSelectedNodes(out nodeCount, out nodes);
                    node = nodeCount > 0 ? nodes[0] : null;

                    if (node != null)
                    {
                        MenuItemAbstract menuItem = Factories.MenuItemFactory.CreateMenuItem(node);
                        AddMenuItemToHierarchyObject(node, menuItem);
                    }
                }
            }
            #pragma warning disable 0169
#pragma warning disable CS0168 // Variable is declared but never used
            catch (Exception ObjectExplorerContextException)
#pragma warning restore CS0168 // Variable is declared but never used
            {
                //MessageBox.Show("ObjectExplorerContextException: " + ObjectExplorerContextException.Message);
            }
            #pragma warning restore 0169
        }

        private void AddMenuItemToHierarchyObject(INodeInformation node, object item)
        {
            dbMenu = (HierarchyObject)node.GetService(typeof(IMenuHandler));
            dbMenu.AddChild(string.Empty, item);
        }

        private void AddSkipLoadingReg()
        {
            var myPackage = this.UserRegistryRoot.CreateSubKey(@"Packages\{" + SQLPackPackage.PackageGuidString + "}");
            myPackage.SetValue("SkipLoading", 1);
        }

        private void DelayAddSkipLoadingReg()
        {
            var delay = new Timer();
            delay.Tick += delegate (object o, EventArgs e)
            {
                delay.Stop();
                AddSkipLoadingReg();
            };
            delay.Interval = 1000;
            delay.Start();
        }

        #endregion
    }
}
