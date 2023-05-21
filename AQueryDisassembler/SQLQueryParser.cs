using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace AQueryDisassembler
{
    /// <summary>
    /// Helper class for parsing and extracting information from SQL queries.
    /// </summary>
    public class SQLQueryParser
    {
        private readonly DbConnection _connection;

        /// <summary>
        /// Initializes a new instance of the SQLQueryParser class with a database connection.
        /// </summary>
        /// <param name="connection">The database connection to use for metadata retrieval.</param>
        public SQLQueryParser(DbConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        /// <summary>
        /// Extracts the field names from a SQL query.
        /// </summary>
        /// <param name="sqlQuery">The SQL query string.</param>
        /// <returns>A list of field names.</returns>
        public List<string> ExtractFieldNames(string sqlQuery)
        {
            var parser = new TSql150Parser(false);
            IList<ParseError> parseErrors;
            var fragment = parser.Parse(new StringReader(sqlQuery), out parseErrors);

            var visitor = new FieldNameVisitor();
            fragment.Accept(visitor);

            return visitor.FieldNames;
        }

        /// <summary>
        /// Extracts the table names from a SQL query.
        /// </summary>
        /// <param name="sqlQuery">The SQL query string.</param>
        /// <returns>A list of table names.</returns>
        public List<string> ExtractTableNames(string sqlQuery)
        {
            var parser = new TSql150Parser(false);
            IList<ParseError> parseErrors;
            var fragment = parser.Parse(new StringReader(sqlQuery), out parseErrors);

            var visitor = new TableNameVisitor();
            fragment.Accept(visitor);

            return visitor.TableNames;
        }

        /// <summary>
        /// Gets the field names from a SQL query, either by extracting them directly from the query
        /// or by retrieving them from the metadata based on the table names used in the query.
        /// </summary>
        /// <param name="sqlQuery">The SQL query string.</param>
        /// <returns>A list of field names.</returns>
        public List<string> GetFieldNamesFromQuery(string sqlQuery)
        {
            if (sqlQuery.Contains("*"))
            {
                var tableNames = ExtractTableNames(sqlQuery);
                return GetFieldNamesFromMetaData(tableNames.ToArray());
            }
            else
            {
                return ExtractFieldNames(sqlQuery);
            }
        }

        /// <summary>
        /// Gets the field names from the metadata for the specified table names.
        /// </summary>
        /// <param name="tableNames">The table names.</param>
        /// <returns>A list of field names.</returns>
        internal List<string> GetFieldNamesFromMetaData(params string[] tableNames)
        {
            var fieldNames = new List<string>();

            using (var connection = OpenConnection())
            {
                var schema = connection.GetSchema("Columns");

                foreach (var tableName in tableNames)
                {
                    var tableFieldNames = schema.AsEnumerable()
                        .Where(row => string.Equals(row["TABLE_NAME"].ToString(), tableName, StringComparison.OrdinalIgnoreCase))
                        .Select(row => row["COLUMN_NAME"].ToString())
                        .ToList();

                    fieldNames.AddRange(tableFieldNames);
                }
            }

            return fieldNames;
        }

        /// <summary>
        /// Opens the database connection if it's not already open.
        /// </summary>
        /// <returns>The opened database connection.</returns>
        private DbConnection OpenConnection()
        {
            if (_connection.State != ConnectionState.Open)
                _connection.Open();

            return _connection;
        }
    }
}
