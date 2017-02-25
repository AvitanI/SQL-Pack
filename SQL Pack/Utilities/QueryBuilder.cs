using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQL_Pack.Utilities
{
    public class QueryBuilder
    {
        public static readonly string countQuery = "SELECT COUNT(*) FROM [{0}].[{1}].[{2}]";
        public static readonly string duplicateTableQuery = "SELECT * INTO {3} FROM [{0}].[{1}].[{2}] WHERE 1=2";
        public static readonly string duplicateTableQueryWithData = "SELECT * INTO {3} FROM [{0}].[{1}].[{2}]";

        public static StringBuilder GetDBFullSearch(string searchValue)
        {
            return new StringBuilder(string.Format(@"DECLARE @SearchStr nvarchar(100)
SET @SearchStr = '{0}'
CREATE TABLE #Results (ColumnName nvarchar(370), ColumnValue nvarchar(3630))
SET NOCOUNT ON
DECLARE @TableName nvarchar(256), @ColumnName nvarchar(128), @SearchStr2 nvarchar(110)
SET  @TableName = ''
SET @SearchStr2 = QUOTENAME('%' + @SearchStr + '%', '''')
WHILE @TableName IS NOT NULL
BEGIN
SET @ColumnName = ''
SET @TableName =
(
SELECT MIN(QUOTENAME(TABLE_SCHEMA) + '.' + QUOTENAME(TABLE_NAME))
FROM     INFORMATION_SCHEMA.TABLES
WHERE TABLE_TYPE = 'BASE TABLE'
AND QUOTENAME(TABLE_SCHEMA) + '.' + QUOTENAME(TABLE_NAME) > @TableName
AND OBJECTPROPERTY(
OBJECT_ID(
QUOTENAME(TABLE_SCHEMA) + '.' + QUOTENAME(TABLE_NAME)), 'IsMSShipped') = 0)
WHILE(@TableName IS NOT NULL) AND(@ColumnName IS NOT NULL)
BEGIN
    SET @ColumnName =
    (
    SELECT MIN(QUOTENAME(COLUMN_NAME))
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = PARSENAME(@TableName, 2)
    AND TABLE_NAME = PARSENAME(@TableName, 1)
    AND DATA_TYPE IN('char', 'varchar', 'nchar', 'nvarchar', 'int', 'decimal')
    AND QUOTENAME(COLUMN_NAME) > @ColumnName
    )
     IF @ColumnName IS NOT NULL
    BEGIN
        INSERT INTO #Results
        EXEC
        (
        'SELECT ''' + @TableName + '.' + @ColumnName + ''', LEFT(' + @ColumnName + ', 3630) FROM ' + @TableName + ' (NOLOCK) ' +
        ' WHERE ' + @ColumnName + ' LIKE ' + @SearchStr2)
    END
END
END
SELECT ColumnName, ColumnValue FROM #Results
DROP TABLE #Results", searchValue));
        }

        public static StringBuilder GetTableFullSearch(string searchValue, string table)
        {
            return new StringBuilder(string.Format(@"CREATE TABLE #Results (ColumnName nvarchar(370), ColumnValue nvarchar(3630))
SET NOCOUNT ON
declare @columnName nvarchar(128) = ''
DECLARE @SearchStr2 nvarchar(110) = QUOTENAME('%{0}%', '''')
while @columnName IS NOT NULL
BEGIN
    SET @columnName= (SELECT MIN(QUOTENAME(COLUMN_NAME))
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = '{1}' 
    AND DATA_TYPE IN('char', 'varchar', 'nchar', 'nvarchar', 'int', 'decimal')
    AND QUOTENAME(COLUMN_NAME) > @ColumnName)
    IF @ColumnName IS NOT NULL
    BEGIN
        INSERT INTO #Results
        EXEC
        ('SELECT ''' + '{1}' + '.' + @ColumnName + ''', LEFT(' + @ColumnName + ', 3630) FROM ' + '{1}' + ' (NOLOCK) ' + 
        ' WHERE ' + @ColumnName + ' LIKE ' + @SearchStr2)
    END
END
SELECT ColumnName, ColumnValue FROM #Results
DROP TABLE #Results", searchValue, table));
        }

        public static StringBuilder GetColumnFullSearch(string searchValue, string db, string schema, string table, string column)
        {
            return new StringBuilder(string.Format(@"SELECT [{3}] FROM [{0}].[{1}].[{2}] WHERE [{3}] LIKE '%{4}%'", db, schema, table, column, searchValue));
        }

        public static string GetQuoteName(string valueToQuote)
        {
            return "[" + valueToQuote + "]";
        }

        public static int GetCountOF(string connectionString, string sqlStatement)
        {
            SqlCommand command = new SqlCommand(sqlStatement);
            command.Connection = new SqlConnection(connectionString);
            command.Connection.Open();
            string result = command.ExecuteScalar().ToString();

            if (string.IsNullOrEmpty(result))
            {
                return -1;
            }

            int count = int.Parse(result);
            command.Connection.Close();
            return count;
        }

        public static void AddDuplicateTable(string connectionString, string sqlStatement)
        {
            try
            {
                SqlCommand command = new SqlCommand(sqlStatement);
                command.Connection = new SqlConnection(connectionString);
                command.Connection.Open();
                command.ExecuteReader();
                command.Connection.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
    }
}
