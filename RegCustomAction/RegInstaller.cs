using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace RegCustomAction
{
    [RunInstaller(true)]
    public partial class RegInstaller : System.Configuration.Install.Installer
    {
        public RegInstaller()
        {
            InitializeComponent();
        }

        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);
        }

        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
        public override void Commit(IDictionary savedState)
        {
            base.Commit(savedState);
            
            string programFiles = ProgramFilesx86();
            string sqlPath = @"\Microsoft SQL Server\120\Tools\Binn\ManagementStudio\Extensions\SQL Pack\";
            string fileRegPath = "skipLoading.2014.reg";
            string fullPath = programFiles + sqlPath + fileRegPath;
            Process regeditProcess = Process.Start("regedit.exe", "/s " + fullPath);
            regeditProcess.WaitForExit();
            System.Diagnostics.Process.Start(programFiles + sqlPath);
        }

        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
        public override void Rollback(IDictionary savedState)
        {
            base.Rollback(savedState);
        }

        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
        public override void Uninstall(IDictionary savedState)
        {
            base.Uninstall(savedState);
        }

        public static string ProgramFilesx86()
        {
            if (8 == IntPtr.Size
                || (!String.IsNullOrEmpty(Environment.GetEnvironmentVariable("PROCESSOR_ARCHITEW6432"))))
            {
                return Environment.GetEnvironmentVariable("ProgramFiles(x86)");
            }

            return Environment.GetEnvironmentVariable("ProgramFiles");
        }

    }
}