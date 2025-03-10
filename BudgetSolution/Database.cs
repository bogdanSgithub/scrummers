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
            CreateDatabase();
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
        private static void CreateDatabase()
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
    }

}
