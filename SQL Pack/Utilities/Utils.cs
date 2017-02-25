using Microsoft.SqlServer.Management.UI.VSIntegration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQL_Pack.Utilities
{
    public class Utils
    {
        /// <summary>
        /// // Insert SQL definition to document & optional execute query
        /// </summary>
        /// <param name="query">query string</param>
        /// <param name="execute">optional execute, default is false</param>
        public static void CreateNewBlankScript(StringBuilder query, bool execute = false)
        {
            ServiceCache.ScriptFactory.CreateNewBlankScript(Microsoft.SqlServer.Management.UI.VSIntegration.Editors.ScriptType.Sql);
            EnvDTE.TextDocument doc = (EnvDTE.TextDocument)ServiceCache.ExtensibilityModel.Application.ActiveDocument.Object(null);
            doc.EndPoint.CreateEditPoint().Insert(query.ToString());

            if (execute)
            {
                doc.DTE.ExecuteCommand("Query.Execute");
            }
        }
        /// <summary>
        /// Open prompt window so user can insert value
        /// </summary>
        /// <param name="text"></param>
        /// <param name="caption"></param>
        /// <returns></returns>
        public static string ShowDialog(string text, string caption)
        {
            Form prompt = new Form()
            {
                Width = 500,
                Height = 150,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen
            };
            Label textLabel = new Label() { Left = 50, Top = 20, Text = text };
            TextBox textBox = new TextBox() { Left = 50, Top = 50, Width = 400 };
            Button confirmation = new Button() { Text = "Search", Left = 350, Width = 100, Top = 70, DialogResult = DialogResult.OK };
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.AcceptButton = confirmation;

            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : null;
        }
    }
}
