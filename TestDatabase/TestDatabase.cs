﻿using System;
using Xunit;
using System.IO;
using System.Collections.Generic;
using Budget;
using System.Data.SQLite;


namespace BudgetCodeTests
{
    [Collection("Sequential")]
    public class TestDatabase
    {


        [Fact]
        public void SQLite_TestNewDatabase_TablesCreated_newDBDoesNotExist()
        {
            // Arrange
            string path = TestConstants.GetSolutionDir();
            string filename = "newdb.db";
            List<String> tables = new List<String>() { "categoryTypes", "expenses", "categories" };
            Database.CloseDatabaseAndReleaseFile();
            if (File.Exists(path + "\\" + filename))
            {
                File.Delete(path + "\\" + filename);
            }

            // Act
            Database.newDatabase(path + "\\" + filename);

            // Assert
            string cmd = " .tables";
            List<String> databaseOutput = DatabaseCommandLine.ExecuteAndReturnOutput(path + "\\" + filename + cmd);
            if (databaseOutput.Count < 1)
            {
                Assert.True(false, "There were no tables created in new database ");
            }

            String table_string = databaseOutput[0];
            foreach (String table in tables)
            {
                Assert.True(table_string.Contains(table), $"table {table} in database");
            }

        }

        //DEPRECATED
        //[Fact]
        //public void SQLite_TestNewDatabase_newDBDoesExist_shouldHaveNoData()
        //{
        //    // Arrange
        //    string path = TestConstants.GetSolutionDir();
        //    string filename = "newdb.db";
        //    List<String> tables = new List<String>() { "categoryTypes", "expenses", "categories" };

        //    // open, add some stuff to the database directly, then
        //    // close it.
        //    Database.newDatabase(TestConstants.GetSolutionDir() + "\\" + filename);
        //    var cmd = new SQLiteCommand(Database.dbConnection);

        //    cmd.CommandText = "INSERT INTO categoryTypes(Description) VALUES('Whatever')";
        //    cmd.ExecuteNonQuery();
        //    cmd.Dispose();


        //    // Act
        //    Database.newDatabase(TestConstants.GetSolutionDir() + "\\" + filename);

        //    // Assert
        //    cmd = new SQLiteCommand("Select * from categoryTypes", Database.dbConnection);
        //    SQLiteDataReader rdr = cmd.ExecuteReader();
        //    Assert.False(rdr.HasRows, "There is data in the new databse");

        //}

        [Fact]
        public void SQLite_TestNewDatabase_ForeignKeyConstraintsEnabled()
        {

            // For SQLite, you need to use the following as a connection string
            // if you want your foreign key constraints to work.

            // string cs = $"Data Source={filepath}; Foreign Keys=1";

            // PS: Validate externally that having the above connection string does indeed
            //     turn on foreign key constraints :)

            // Arrange
            string path = TestConstants.GetSolutionDir();
            string filename = "newdb.db";

            // Act
            Database.newDatabase(path + "\\" + filename);

            // Assert
            String connectionString = Database.dbConnection.ConnectionString;
            Assert.True(connectionString.Contains("Foreign Keys=1"), "FK Constraints enabled");

        }

        [Fact]
        public void SQLite_TestNewDatabase_ColumnsInTableExpenses()
        {
            // Arrange
            string path = TestConstants.GetSolutionDir();
            string filename = "newdb.db";
            List<String> columns = new List<string>() { "Id", "CategoryId", "Amount", "Date", "Description" };

            // Act
            Database.newDatabase(path + "\\" + filename);

            // Assert
            string cmd = " \".mode list\" \"pragma table_info(expenses)\"";
            List<String> DatabaseOutput = DatabaseCommandLine.ExecuteAndReturnOutput(path + "\\" + filename + cmd);
            if (DatabaseOutput.Count < 1)
            {
                Assert.True(false, "There were no columns in table expenses ");
            }

            // Assert
            foreach (String column in columns)
            {
                int index = DatabaseOutput.FindIndex(s => s.Contains($"|{column}|"));
                Assert.NotEqual(-1, index);
            }

        }

        [Fact]
        public void SQLite_TestNewDatabase_ColumnsInTableCategory()
        {
            // Arrange
            string path = TestConstants.GetSolutionDir();
            string filename = "newdb.db";
            List<String> columns = new List<string>() { "Id", "Description", "TypeId" };

            // Act
            Database.newDatabase(path + "\\" + filename);

            // Assert
            string cmd = " \".mode list\" \"pragma table_info(categories)\"";
            List<String> DatabaseOutput = DatabaseCommandLine.ExecuteAndReturnOutput(path + "\\" + filename + cmd);
            if (DatabaseOutput.Count < 1)
            {
                Assert.True(false, "There were no columns in table categories ");
            }

            // Assert
            foreach (String column in columns)
            {
                int index = DatabaseOutput.FindIndex(s => s.Contains($"|{column}|"));
                Assert.NotEqual(-1, index);
            }

        }

        [Fact]
        public void SQLite_TestNewDatabase_ColumnsInTableCategoryTypes()
        {
            // Arrange
            string path = TestConstants.GetSolutionDir();
            string filename = "newdb.db";
            List<String> columns = new List<string>() { "Id", "Description" };

            // Act
            Database.newDatabase(path + "\\" + filename);

            // Assert
            string cmd = " \".mode list\" \"pragma table_info(CategoryTypes)\"";
            List<String> DatabaseOutput = DatabaseCommandLine.ExecuteAndReturnOutput(path + "\\" + filename + cmd);
            if (DatabaseOutput.Count < 1)
            {
                Assert.True(false, "There were no columns in table types ");
            }

            // Assert
            foreach (String column in columns)
            {
                int index = DatabaseOutput.FindIndex(s => s.Contains($"|{column}|"));
                Assert.NotEqual(-1, index);
            }

        }

        [Fact]
        public void SQLite_TestNewDatabase_RequiredForeignKeysCategories()
        {
            // Arrange
            string path = TestConstants.GetSolutionDir();
            string filename = "newdb.db";
            Dictionary<String, String> FKtable = new Dictionary<String, String>()
            {
                {"table", "categoryTypes"},
                { "from", "TypeId" },
                {"to", "Id" },
            };

            // Act
            Database.newDatabase(path + "\\" + filename);

            // Assert
            string cmd = " \".mode line\" \"pragma foreign_key_list(categories)\"";
            List<String> DatabaseOutput = DatabaseCommandLine.ExecuteAndReturnOutput(path + "\\" + filename + cmd);
            if (DatabaseOutput.Count < 1)
            {
                Assert.True(false, "There were no foreign in table categories ");
            }

            // Assert
            foreach (KeyValuePair<string, string> kvp in FKtable)
            {
                String FKProperty = $"{kvp.Key} = {kvp.Value}";
                int index = DatabaseOutput.FindIndex(s => s.Contains(FKProperty));
                Assert.NotEqual(-1, index);
            }

        }

        [Fact]
        public void SQLite_TestNewDatabase_RequiredForeignKeysExpenses()
        {
            // Arrange
            string path = TestConstants.GetSolutionDir();
            string filename = "newdb.db";
            Dictionary<String, String> FKtable = new Dictionary<String, String>()
            {
                {"table", "categories"},
                { "from", "CategoryId" },
                {"to", "Id" },
            };

            // Act
            Database.newDatabase(path + "\\" + filename);

            // Assert
            string cmd = " \".mode line\" \"pragma foreign_key_list(expenses)\"";
            List<String> DatabaseOutput = DatabaseCommandLine.ExecuteAndReturnOutput(path + "\\" + filename + cmd);
            if (DatabaseOutput.Count < 1)
            {
                Assert.True(false, "There were no foreign in table expenses ");
            }

            // Assert
            foreach (KeyValuePair<string, string> kvp in FKtable)
            {
                String FKProperty = $"{kvp.Key} = {kvp.Value}";
                int index = DatabaseOutput.FindIndex(s => s.Contains(FKProperty));
                Assert.NotEqual(-1, index);
            }

        }

        [Fact]
        public void SQLite_TestExistingDatabase_shouldHaveData()
        {
            // Arrange
            string path = TestConstants.GetSolutionDir();
            string filename = "newdb.db";
            List<String> tables = new List<String>() { "categoryTypes", "expenses", "categories" };

            // open, add some stuff to the database directly, then
            // close it.
            Database.newDatabase(path + "\\" + filename);
            var cmd = new SQLiteCommand(Database.dbConnection);

            cmd.CommandText = "INSERT INTO categoryTypes(Description) VALUES('Whatever')";
            cmd.ExecuteNonQuery();
            cmd.Dispose();

            // Act
            Database.existingDatabase(path + "\\" + filename);

            // Assert
            cmd = new SQLiteCommand("Select * from categoryTypes", Database.dbConnection);
            SQLiteDataReader rdr = cmd.ExecuteReader();
            Assert.True(rdr.HasRows, "There is data in the new databse");

        }

        [Fact]
        public void SQLite_TestIsValidIdInTable()
        {
            // Arrange
            string path = TestConstants.GetSolutionDir();
            string filename = "newdb.db";

            // Act
            // by calling newDatabase, we add the default 4 categoryTypes and also add the 16 default categories, i am just checking that the method works
            Database.newDatabase(path + "\\" + filename);
            bool validCategories = Database.IsValidIdInTable(1, "categories");
            bool validCategoryTypes = Database.IsValidIdInTable(1, "categoryTypes");
            bool invalidCategories = Database.IsValidIdInTable(999999999, "categories");

            Assert.True(validCategories);
            Assert.True(validCategoryTypes);
            Assert.False(invalidCategories);
        }

        [Fact]
        public void SQLite_TestInvalidDatabaseThrow()
        {
            // Arrange
            string path = TestConstants.GetSolutionDir();
            string filename = "invalidDatabase.db";

            // Act
            try
            {
                Database.existingDatabase(path + "\\" + filename);
            }
            catch
            {
                Assert.True(true);
            }
        }

        [Fact]
        public void SQLite_TestInvalidIsValidIdInTable_ToThrow()
        {
            // Arrange
            string path = TestConstants.GetSolutionDir();
            string filename = "newdb.db";

            Database.newDatabase(path + "\\" + filename);

            // Act
            try
            {
                Database.IsValidIdInTable(1, "invalidTable");
            }
            catch
            {
                Assert.True(true);
            }
        }

        [Fact]
        public void SQLite_TestInvalidClearDBTable_ToThrow()
        {
            // Arrange
            string path = TestConstants.GetSolutionDir();
            string filename = "newdb.db";

            Database.newDatabase(path + "\\" + filename);

            // Act
            try
            {
                Database.ClearDBTable("invalidTable");
            }
            catch
            {
                Assert.True(true);
            }
        }
    }




    public class DatabaseCommandLine
    {
        static public List<String> ExecuteAndReturnOutput(string DatabaseCmd)
        {
            // https://stackoverflow.com/questions/206323/how-to-execute-command-line-in-c-get-std-out-results

            //Create process
            System.Diagnostics.Process pProcess = new System.Diagnostics.Process();

            //strCommand is path and file name of command to run
            pProcess.StartInfo.FileName = "sqlite3";

            //strCommandParameters are parameters to pass to program
            pProcess.StartInfo.Arguments = DatabaseCmd;

            pProcess.StartInfo.UseShellExecute = false;

            //Set output of program to be written to process output stream
            pProcess.StartInfo.RedirectStandardOutput = true;

            //Start the process
            pProcess.Start();

            //Wait for process to finish
            pProcess.WaitForExit();

            //Get program output
            string strOutput = pProcess.StandardOutput.ReadToEnd();

            // Convert the output to a list of strings
            List<String> output = new List<string>();
            using (System.IO.StringReader reader = new System.IO.StringReader(strOutput))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    output.Add(line);
                }
            }

            return output;

        }


    }
}
