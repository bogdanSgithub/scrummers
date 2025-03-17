using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Threading;

// ===================================================================
// Very important notes:
// ... To keep everything working smoothly, you should always
//     dispose of EVERY SQLiteCommand even if you recycle a 
//     SQLiteCommand variable later on.
//     EXAMPLE:
//            Database.newDatabase(GetSolutionDir() + "\\" + filename);
//            var cmd = new SQLiteCommand(Database.dbConnection);
//            cmd.CommandText = "INSERT INTO categoryTypes(Description) VALUES('Whatever')";
//            cmd.ExecuteNonQuery();
//            cmd.Dispose();
//
// ... also dispose of reader objects
//
// ... by default, SQLite does not impose Foreign Key Restraints
//     so to add these constraints, connect to SQLite something like this:
//            string cs = $"Data Source=abc.sqlite; Foreign Keys=1";
//            var con = new SQLiteConnection(cs);
//
// ===================================================================


namespace Budget
{
    public static class Database
    {

        /// <summary>
        /// Connection to the SQLite Database. Readonly
        /// </summary>
        public static SQLiteConnection dbConnection { get { return _connection; } }
        private static SQLiteConnection _connection;
        private static List<string> _tables = new List<string> { "categories", "categoryTypes", "expenses" };

        // ===================================================================
        // create and open a new database
        // ===================================================================
        /// <summary>
        /// Creates a new .db file with the provided filename. Creates the tables with the foreign keys.
        /// </summary>
        /// <param name="filename">The Filename of the database.</param>
        /// <example>
        /// <code>
        /// Database.newDatabase("newDatabase");
        /// </code>
        /// </example>
        public static void newDatabase(string filename)
        {
            // If there was a database open before, close it and release the lock
            CloseDatabaseAndReleaseFile();
            SQLiteConnection.CreateFile(filename);
            existingDatabase(filename);
            CreateDatabaseTables();
            SetInitialCategoryTypes();
            SetCategoriesToDefaults();
        }

        // ===================================================================
        // open an existing database
        // ===================================================================
        /// 
        /// <summary>
        /// Sets the connection to the provided database filename. Sets foreign keys on. If the filename doesn't exist, it throws an FileNotFoundException.
        /// </summary>
        /// <param name="filename">The Filename of the database.</param>
        /// <example>
        /// <code>
        /// Database.existingDatabase("existingDatabase");
        /// </code>
        /// </example>
        public static void existingDatabase(string filename)
        {
            CloseDatabaseAndReleaseFile();

            if (!File.Exists(filename))
            {
                throw new FileNotFoundException("database file doesn't exist");
            }

            _connection = new SQLiteConnection($@"URI=file:{filename};Foreign Keys=1");
            _connection.Open();
        }

        // ===================================================================
        // Creates the default tables in the database with the foreign keys.
        // ===================================================================
        private static void CreateDatabaseTables()
        {   
            SQLiteCommand cmd = new SQLiteCommand(_connection);

            // categoryTypes
            cmd.CommandText = "DROP TABLE IF EXISTS categoryTypes";
            cmd.ExecuteNonQuery();

            cmd.CommandText = @"CREATE TABLE categoryTypes(Id INTEGER PRIMARY KEY,
            Description TEXT)";
            cmd.ExecuteNonQuery();

            // categories
            cmd.CommandText = "DROP TABLE IF EXISTS categories";
            cmd.ExecuteNonQuery();

            cmd.CommandText = @"CREATE TABLE categories(Id INTEGER PRIMARY KEY,
            Description TEXT, TypeId INTEGER, FOREIGN KEY(TypeId) REFERENCES categoryTypes(Id))";
            cmd.ExecuteNonQuery();

            // expenses
            cmd.CommandText = "DROP TABLE IF EXISTS expenses";
            cmd.ExecuteNonQuery();

            cmd.CommandText = @"CREATE TABLE expenses(Id INTEGER PRIMARY KEY,
            CategoryId INTEGER, Amount DECIMAL(10,2), Date date, Description TEXT, FOREIGN KEY(CategoryId) REFERENCES categories(Id))";
            cmd.ExecuteNonQuery();
        }

        public static void ClearDBTable(string table)
        {
            if (!_tables.Contains(table))
                throw new ArgumentException("Invalid table");

            SQLiteCommand cmd = new SQLiteCommand(dbConnection);

            cmd.CommandText = $"DELETE FROM {table};";
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Inserts the default category types 
        /// </summary>
        public static void SetInitialCategoryTypes()
        {
            SQLiteCommand cmd = new SQLiteCommand(dbConnection);

            // add initial categoryTypes
            foreach (Category.CategoryType type in Enum.GetValues(typeof(Category.CategoryType)))
            {
                string query = $"INSERT INTO categoryTypes (Id, Description) VALUES({(int)type + 1}, '{type}');";
                cmd.CommandText = query;
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Clears the categories table and then adds the default categories to it.
        /// </summary>
        public static void SetCategoriesToDefaults()
        {
            // ---------------------------------------------------------------
            // reset any current categories,
            // ---------------------------------------------------------------
            ClearDBTable("categories");

            // ---------------------------------------------------------------
            // Add Defaults
            // ---------------------------------------------------------------
            SQLiteCommand cmd = new SQLiteCommand(_connection);
            string[] statements = { 
                "INSERT INTO categories (Id, Description, TypeId) VALUES(1, 'Utilities', 2);",
                "INSERT INTO categories (Id, Description, TypeId) VALUES(2, 'Rent', 2);",
                "INSERT INTO categories (Id, Description, TypeId) VALUES(3, 'Food', 2);",
                "INSERT INTO categories (Id, Description, TypeId) VALUES(4, 'Entertainment', 2);",
                "INSERT INTO categories (Id, Description, TypeId) VALUES(5, 'Education', 2);",
                "INSERT INTO categories (Id, Description, TypeId) VALUES(6, 'Miscellaneous', 2);",
                "INSERT INTO categories (Id, Description, TypeId) VALUES(7, 'Medical Expenses', 2);",
                "INSERT INTO categories (Id, Description, TypeId) VALUES(8, 'Vacation', 2);",
                "INSERT INTO categories (Id, Description, TypeId) VALUES(9, 'Credit Card', 3);",
                "INSERT INTO categories (Id, Description, TypeId) VALUES(10, 'Clothes', 2);",
                "INSERT INTO categories (Id, Description, TypeId) VALUES(11, 'Gifts', 2);",
                "INSERT INTO categories (Id, Description, TypeId) VALUES(12, 'Insurance', 2);",
                "INSERT INTO categories (Id, Description, TypeId) VALUES(13, 'Transportation', 2);",
                "INSERT INTO categories (Id, Description, TypeId) VALUES(14, 'Eating Out', 2);",
                "INSERT INTO categories (Id, Description, TypeId) VALUES(15, 'Savings', 4);",
                "INSERT INTO categories (Id, Description, TypeId) VALUES(16, 'Income', 1);"
            }; 

            foreach (string statement in statements)
            {
                cmd.CommandText = statement;
                cmd.ExecuteNonQuery();
            }
        }

        // ===================================================================
        // close existing database, wait for garbage collector to
        // release the lock before continuing
        // ===================================================================
        /// <summary>
        /// It closes the connection the database and waits for the garbage collector to remove the lock from the database file.
        /// </summary>
        static public void CloseDatabaseAndReleaseFile()
        {
            if (Database.dbConnection != null)
            {
                // close the database connection
                Database.dbConnection.Close();
                

                // wait for the garbage collector to remove the
                // lock from the database file
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        /// <summary>
        /// Given an int id and string table name, checks to see if there is a row with that id in the given table.
        /// Makes sure the given table is valid. Returns if there was a row in the table with that id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        static public bool IsValidIdInTable(int id, string table)
        {
            if (!_tables.Contains(table))
                throw new ArgumentException("Not a valid table");

            SQLiteCommand cmd = new SQLiteCommand(dbConnection);

            // is still safe because we check initially if it's a valid table
            cmd.CommandText = $"SELECT COUNT(Id) FROM {table} WHERE Id = @id;";
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Prepare();
            object result = cmd.ExecuteScalar();

            return int.Parse(result.ToString()) == 1;
        }
    }

}
