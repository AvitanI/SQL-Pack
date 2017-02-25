using Microsoft.SqlServer.Management.UI.VSIntegration.ObjectExplorer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SQL_Pack.Models
{
    public enum NodeType { Server, Database, Schema, Table, Column }

    public class NodeContextInfo
    {
        /// <summary>
        /// Class variables :  @"Server[@Name='MYYOGACOMPUTER\SQLEXPRESS']/Database[@Name='FinalDB']/Table[@Name='Charge' and @Schema='dbo']/Column[@Name='CostPerHour']"
        /// </summary>
        public static readonly string Server_Pattern = @"^Server\[\@Name=\'(?<Server>[^\]]*)\'\]$";
        public static readonly string DataBase_Pattern = @"^Server\[\@Name=\'(?<Server>[^\]]*)\'\]\/Database\[\@Name=\'(?<Database>[^']*)\'\]$";
        public static readonly string Table_Pattern = @"^Server\[\@Name=\'(?<Server>[^\]]*)\'\]\/Database\[\@Name=\'(?<Database>[^']*)\'\]\/Table\[\@Name=\'(?<Table>[^']*)\'\s+and\s+\@Schema=\'(?<Schema>[^']*)\'\]$";
        public static readonly string Column_Pattern = @"^Server\[\@Name=\'(?<Server>[^\]]*)\'\]\/Database\[\@Name=\'(?<Database>[^']*)\'\]\/Table\[\@Name=\'(?<Table>[^']*)\'\s+and\s+\@Schema=\'(?<Schema>[^']*)\'\]\/Column\[\@Name=\'(?<Column>[^']*)\'\]$";

        public string Server { get; private set; }
        public string Database { get; private set; }
        public string Schema { get; private set; }
        public string Table { get; private set; }
        public string Column { get; private set; }
        public string ConnectionString { get { return this.nodeContext.Connection.ConnectionString + ";Database=" + Database; } }

        /// <summary>
        /// Private members
        /// </summary>
        private INodeInformation nodeContext;

        public NodeContextInfo(NodeType type, INodeInformation ctx)
        {
            this.nodeContext = ctx;
            this.BuildNodeInfoByType(type);
        }

        private void BuildNodeInfoByType(NodeType type)
        {
            if (string.IsNullOrEmpty(this.nodeContext.Context)) { return; }

            Regex r = null;
            Match match = null;

            switch (type)
            {
                case NodeType.Server:
                    r = new Regex(NodeContextInfo.Server_Pattern);
                    break;
                case NodeType.Database:
                    r = new Regex(NodeContextInfo.DataBase_Pattern);
                    break;
                case NodeType.Table:
                    r = new Regex(NodeContextInfo.Table_Pattern);
                    break;
                case NodeType.Column:
                    r = new Regex(NodeContextInfo.Column_Pattern);
                    break;
                default:
                    return;
            }

            //GroupCollection groups = r.Match(this.nodeContext).Groups;
            //foreach (string groupName in r.GetGroupNames())
            //{
            //    this[groupName] = groups[groupName].Value;
            //}
            match = r.Match(this.nodeContext.Context);
            Server = match.Groups["Server"].Value;
            Table = match.Groups["Table"].Value;
            Schema = match.Groups["Schema"].Value;
            Database = match.Groups["Database"].Value;
            Column = match.Groups["Column"].Value;
        }
    }
}
