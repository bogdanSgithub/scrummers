using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Globalization;
using System.Data.SQLite;

// ============================================================================
// (c) Sandy Bultena 2018
// * Released under the GNU General Public License
// ============================================================================

namespace Budget
{
    // ====================================================================
    // CLASS: expenses
    //        - A collection of expense items,
    //        - Read / write to database
    //        - etc
    // ====================================================================
    /// <summary>
    /// Object that allows the user to interact with the expenses table in the database.
    /// </summary>
    public class Expenses
    {
        public Expenses()
        {   
        }

        /// <summary>
        /// Gets the expense that matches the passed the id from the database. Throws if the id doesnt have any matches.
        /// </summary>
        /// <param name="i">The ID to find a match for.</param>
        /// <returns>The matching expense, if found inside the database.</returns>
        /// <exception cref="Exception"></exception>
        /// <example>
        /// <code>
        /// Expenses expenses = new Expenses();
        /// 
        /// //e will contain the matching expense from the database.
        /// Expense e = expenses.GetExpenseFromId(1);
        /// </code>
        /// </example>
        public Expense GetExpenseFromId(int i)
        {
            //select the matching category
            SQLiteCommand cmd = new SQLiteCommand(Database.dbConnection);
            cmd.CommandText = "SELECT Id, CategoryId, Amount, Date, Description FROM expenses WHERE Id = @id;";
            cmd.Parameters.AddWithValue("@id", i);


            SQLiteDataReader rdr = cmd.ExecuteReader();

            Expense expense = null;

            //change the fields of the category if a match is found
            while (rdr.Read())
            {
                expense = new Expense(new Expense(rdr.GetInt32(0), rdr.GetDateTime(3), rdr.GetInt32(1), rdr.GetDouble(2), rdr.GetString(4)));
            }

            if (expense == null)
            {
                throw new Exception("Cannot find expense with id " + i.ToString());
            }

            return expense;
        }

        /// <summary>
        /// Adds a new expense with the specified date, category, amount, and description 
        /// to the expense database. If there are existing expenses, the new expense is assigned 
        /// a unique ID, which is 1 more than the highest existing ID.
        /// </summary>
        /// <param name="date">The date of the expense.</param>
        /// <param name="category">The category ID associated with the expense.</param>
        /// <param name="amount">The amount of the expense.</param>
        /// <param name="description">A brief description of the expense.</param>
        /// <example>
        /// <code>
        /// Expenses expenses = new Expenses();
        /// expenses.Add(DateTime.Now, 1, 5.5, "Groceries");
        /// </code>
        /// </example>
        public void Add(DateTime date, int category, Double amount, String description)
        {
            if (!Database.IsValidIdInTable(category, "categories"))
                throw new ArgumentException("Invalid categoryId.");

            int newID = 0;

            SQLiteCommand cmd = new SQLiteCommand(Database.dbConnection);

            cmd.CommandText = "SELECT MAX(Id) FROM expenses;";
            object result = cmd.ExecuteScalar();

            // if there are expenses in the db
            if (result != DBNull.Value)
                newID = int.Parse(result.ToString());

            Expense newExpense = new Expense(newID + 1, date, category, amount, description);
            InsertIntoDB(newExpense);
        }

        private void InsertIntoDB(Expense expense)
        {
            SQLiteCommand cmd = new SQLiteCommand(Database.dbConnection);
            string query = $"INSERT INTO expenses (Id, CategoryId, Amount, Date, Description) VALUES(@id, @categoryId, @amount, @date, @description);";
            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@id", expense.Id);
            cmd.Parameters.AddWithValue("@categoryId", expense.Category);
            cmd.Parameters.AddWithValue("@amount", expense.Amount);
            cmd.Parameters.AddWithValue("@date", expense.Date);
            cmd.Parameters.AddWithValue("@description", expense.Description);

            cmd.Prepare();
            cmd.ExecuteNonQuery();
        }


        /// <summary>
        /// Updates an expense in the database that matches the id passed with the new passed information.
        /// </summary>
        /// <param name="id">The id of the expense to update.</param>
        /// <param name="newDate">The new date of the expense.</param>
        /// <param name="newCategory">The new category of the expense.</param>
        /// <param name="newAmount">The new amount of the expense.</param>
        /// <param name="newDescription">The new description of the expense.</param>
        /// <exception cref="ArgumentException"></exception>
        /// <example>
        /// <code>
        /// Expenses expenses = new Expenses();
        /// 
        /// //The expense with an id of 1 will have the updated fields.
        /// expenses.UpdateProperties(1,DateTime.Now, 1, 5.5, "Groceries");
        /// </code>
        /// </example>
        public void UpdateProperties(int id, DateTime newDate, int newCategory, Double newAmount, String newDescription)
        {
            if (!Database.IsValidIdInTable(newCategory, "categories"))
                throw new ArgumentException("Invalid categoryId.");

            SQLiteCommand cmd = new SQLiteCommand(Database.dbConnection);

            cmd.CommandText = $"UPDATE expenses SET CategoryId = @categoryId, Description = @description, Amount = @amount, Date = @date WHERE Id = @id;";
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@categoryId", newCategory);
            cmd.Parameters.AddWithValue("@amount", newAmount);
            cmd.Parameters.AddWithValue("@date", newDate);
            cmd.Parameters.AddWithValue("@description", newDescription);
            cmd.Prepare();
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Tries to remove the expense with the specified ID from the expense database.
        /// If there isn't a expense in the database with the given ID then it isn't removed.
        /// </summary>
        /// <param name="Id">The ID of the expense to be deleted.</param>
        /// <example>
        /// <code>
        /// Expenses expenses = new Expenses();
        /// expenses.Add(DateTime.Now, 1, 5.5, "Groceries");
        /// expenses.Delete(1);
        /// </code>
        /// </example>
        public void Delete(int Id)
        {
            SQLiteCommand cmd = new SQLiteCommand(Database.dbConnection);
            cmd.CommandText = $"DELETE FROM expenses WHERE Id = @id;";
            cmd.Parameters.AddWithValue("@id", Id);
            cmd.Prepare();
            cmd.ExecuteNonQuery();
        }

        // ====================================================================
        // Return list of expenses
        // Note:  make new copy of list, so user cannot modify what is part of
        //        this instance
        // ====================================================================
        /// <summary>
        /// Returns a new list containing copies of all expenses in the expense database.
        /// </summary>
        /// <returns>A new list of expenses, where each expense is a copy of the expense objects in the Expenses object's database.</returns>
        /// <example>
        /// <code>
        /// Expenses expenses = new Expenses();
        /// expenses.Add(DateTime.Now, 1, 5.5, "Groceries");
        /// foreach (Expense e in expenses.List()) {
        ///    Console.WriteLine(e);
        /// }
        /// </code>
        /// </example>
        public List<Expense> List()
        {
            SQLiteCommand cmd = new SQLiteCommand(Database.dbConnection);
            cmd.CommandText = "SELECT Id, CategoryId, Amount, Date, Description FROM expenses;";
            SQLiteDataReader rdr = cmd.ExecuteReader();
            List<Expense> expenseList = new List<Expense>();

            //populate list with expenses from db.
            while (rdr.Read())
            {
                expenseList.Add(new Expense(rdr.GetInt32(0), rdr.GetDateTime(3), rdr.GetInt32(1), rdr.GetDouble(2), rdr.GetString(4)));
            }

            return expenseList;
        }
    }
}

