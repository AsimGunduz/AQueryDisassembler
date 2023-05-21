# AQueryDisassembler

AQueryDisassembler helps you disassemble SQL queries to extract field names and table names. It provides the following public members:

## Installation
You can install the AQueryDisassembler package via NuGet. Use one of the following commands, depending on your preferred package manager:

#### Package Manager
Install-Package AQueryDisassembler -Version 1.0.0

#### .NET CLI
dotnet add package AQueryDisassembler --version 1.0.0


## SQLQueryParser Class

The `SQLQueryParser` class is the main class in the library.

### Constructors

- `SQLQueryParser(DbConnection connection)`: Initializes a new instance of the `SQLQueryParser` class with a database connection.

### Methods

- `List<string> ExtractFieldNames(string sqlQuery)`: Extracts the field names from a SQL query.
  - Parameters:
    - `sqlQuery` (string): The SQL query string.
  - Returns:
    - A list of field names.

- `List<string> ExtractTableNames(string sqlQuery)`: Extracts the table names from a SQL query.
  - Parameters:
    - `sqlQuery` (string): The SQL query string.
  - Returns:
    - A list of table names.

- `List<string> GetFieldNamesFromQuery(string sqlQuery)`: Gets the field names from a SQL query, either by extracting them directly from the query or by retrieving them from the metadata based on the table names used in the query.
  - Parameters:
    - `sqlQuery` (string): The SQL query string.
  - Returns:
    - A list of field names.

 ## Example
 
 ```csharp
 using AQueryDisassembler;

// Create a SQLQueryParser instance with a database connection
DbConnection connection = new SqlConnection(connectionString);
SQLQueryParser parser = new SQLQueryParser(connection);

// Example 1: Extract field names from a SQL query
string sqlQuery = "SELECT column1, column2 FROM table1";
List<string> fieldNames = parser.ExtractFieldNames(sqlQuery);
Console.WriteLine("Field Names: " + string.Join(", ", fieldNames));

// Example 2: Extract table names from a SQL query
string sqlQuery2 = "SELECT * FROM table1 INNER JOIN table2 ON table1.id = table2.id";
List<string> tableNames = parser.ExtractTableNames(sqlQuery2);
Console.WriteLine("Table Names: " + string.Join(", ", tableNames));

// Example 3: Get field names from a SQL query
string sqlQuery3 = "SELECT * FROM table1";
List<string> fieldNamesFromQuery = parser.GetFieldNamesFromQuery(sqlQuery3);
Console.WriteLine("Field Names from Query: " + string.Join(", ", fieldNamesFromQuery));

