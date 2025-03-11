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
    //        - Read / write to file
    //        - etc
    // ====================================================================
    /// <summary>
    /// Object that has a list of expenses
    /// </summary>
    public class Expenses
    {
        private static String DefaultFileName = "budget.txt";
        //private List<Expense> _Expenses = new List<Expense>();
        private string _FileName;
        private string _DirName;

        public Expenses()
        {   
            if (Database.dbConnection is null)
            {
                Database.newDatabase("default.db");
            }
        }

        // ====================================================================
        // Properties
        // ====================================================================
        /// <summary>
        /// string FileName representing the name of the file where the expenses are saved
        /// </summary>
        public String FileName { get { return _FileName; } }
        /// <summary>
        /// string directory name representing the name of the directory where the expenses files is
        /// </summary>
        public String DirName { get { return _DirName; } }

        // ====================================================================
        // populate expenses from a file
        // if filepath is not specified, read/save in AppData file
        // Throws System.IO.FileNotFoundException if file does not exist
        // Throws System.Exception if cannot read the file correctly (parsing XML)
        // ====================================================================
        /// <summary>
        /// Populates the Expenses list by reading from the specified file and also saves the filename and directory name.
        /// If no filepath is provided, the default file in the AppData directory is used.
        /// </summary>
        /// <param name="filepath">The path to the file from which expenses will be read. If null, defaults to the AppData file.</param>
        /// <exception cref="System.IO.FileNotFoundException">Thrown by <see cref="BudgetFiles.VerifyReadFromFileName"/> if the specified file does not exist.</exception>
        /// <exception cref="System.Exception">Thrown if the file cannot be read correctly, such as if there is an issue with XML parsing.</exception>
        /// <example>
        /// <code>
        /// Expenses expenses = new Expenses();
        /// expenses.ReadFromFile("C:\\Users\\studentID\\Desktop\\Scrummers\\BudgetSolution\\test_expenses.exps");
        /// </code>
        /// </example>
        public void ReadFromFile(String filepath = null)
        {

            // ---------------------------------------------------------------
            // reading from file resets all the current expenses,
            // so clear out any old definitions
            // ---------------------------------------------------------------
            ClearDBExpenses();

            // ---------------------------------------------------------------
            // reset default dir/filename to null 
            // ... filepath may not be valid, 
            // ---------------------------------------------------------------
            _DirName = null;
            _FileName = null;

            // ---------------------------------------------------------------
            // get filepath name (throws exception if it doesn't exist)
            // ---------------------------------------------------------------
            filepath = BudgetFiles.VerifyReadFromFileName(filepath, DefaultFileName);

            // ---------------------------------------------------------------
            // read the expenses from the xml file
            // ---------------------------------------------------------------
            _ReadXMLFile(filepath);

            // ----------------------------------------------------------------
            // save filename info for later use?
            // ----------------------------------------------------------------
            _DirName = Path.GetDirectoryName(filepath);
            _FileName = Path.GetFileName(filepath);
        }

        private void ClearDBExpenses()
        {
            SQLiteCommand cmd = new SQLiteCommand(Database.dbConnection);

            string query = $"DELETE FROM expenses;";
            cmd.CommandText = query;
            cmd.ExecuteNonQuery();
        }

        // ====================================================================
        // save to a file
        // if filepath is not specified, read/save in AppData file
        // ====================================================================
        /// <summary>
        /// Saves the current expenses to the specified file and also saves the filename and directory name.
        /// If no filepath is provided, the default file in the AppData directory is used.
        /// </summary>
        /// <param name="filepath">The path to the file where expenses will be saved. If null, defaults to the last read file or AppData file.</param>
        /// <exception cref="System.IO.FileNotFoundException">Thrown by <see cref="BudgetFiles.VerifyWriteToFileName"/> if the specified file does not exist or cannot be accessed for writing.</exception>
        /// <exception cref="System.Exception">Thrown if the file cannot be saved correctly, such as if there is an issue with XML writing.</exception>
        /// <example>
        /// <code>
        /// Expenses expenses = new Expenses();
        /// expenses.SaveToFile("C:\\Users\\studentID\\Desktop\\Scrummers\\BudgetSolution\\test_expenses.exps");
        /// </code>
        /// </example>
        public void SaveToFile(String filepath = null)
        {
            // ---------------------------------------------------------------
            // if file path not specified, set to last read file
            // ---------------------------------------------------------------
            if (filepath == null && DirName != null && FileName != null)
            {
                filepath = DirName + "\\" + FileName;
            }

            // ---------------------------------------------------------------
            // just in case filepath doesn't exist, reset path info
            // ---------------------------------------------------------------
            _DirName = null;
            _FileName = null;

            // ---------------------------------------------------------------
            // get filepath name (throws exception if it doesn't exist)
            // ---------------------------------------------------------------
            filepath = BudgetFiles.VerifyWriteToFileName(filepath, DefaultFileName);

            // ---------------------------------------------------------------
            // save as XML
            // ---------------------------------------------------------------
            _WriteXMLFile(filepath);

            // ----------------------------------------------------------------
            // save filename info for later use
            // ----------------------------------------------------------------
            _DirName = Path.GetDirectoryName(filepath);
            _FileName = Path.GetFileName(filepath);
        }



        // ====================================================================
        // Add expense
        // ====================================================================
        private void Add(Expense exp)
        {
            //_Expenses.Add(exp);
        }

        /// <summary>
        /// Adds a new expense with the specified date, category, amount, and description 
        /// to the expense list. If there are existing expenses, the new expense is assigned 
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
            int newID = 0;

            SQLiteCommand cmd = new SQLiteCommand(Database.dbConnection);

            cmd.CommandText = "SELECT MAX(Id) FROM expenses;";
            object result = cmd.ExecuteScalar();

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

        public void UpdateProperties(int id, DateTime newDate, int newCategory, Double newAmount, String newDescription)
        {
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

        // ====================================================================
        // Delete expense
        // ====================================================================
        /// <summary>
        /// Tries to remove the expense with the specified ID from the expense list.
        /// If there isn't a expense in the list with the given ID then it isn't removed.
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
        /// Returns a new list containing copies of all expenses in the expense list.
        /// </summary>
        /// <returns>A new list of expenses, where each expense is a copy of the expense objects in the Expenses object's list.</returns>
        /// <example>
        /// <code>
        /// Expenses expenses = new Expenses();
        /// expenses.Add(DateTime.Now, 1, 5.5, "Groceries");
        /// foreach (Expense e in expenses.List()) {
        ///    Console.WriteLine(e);
        /// }
        /// Id INTEGER PRIMARY KEY,
        /// </code>
        /// </example>
        public List<Expense> List()
        {
            SQLiteCommand cmd = new SQLiteCommand(Database.dbConnection);
            cmd.CommandText = "SELECT Id, CategoryId, Amount, Date, Description FROM expenses;";
            SQLiteDataReader rdr = cmd.ExecuteReader();
            List<Expense> expenseList = new List<Expense>();

            while (rdr.Read())
            {
                expenseList.Add(new Expense(rdr.GetInt32(0), rdr.GetDateTime(3), rdr.GetInt32(1), rdr.GetDouble(2), rdr.GetString(4)));
            }

            return expenseList;
        }


        // ====================================================================
        // read from an XML file and add categories to our categories list
        // ====================================================================
        private void _ReadXMLFile(String filepath)
        {


            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(filepath);

                // Loop over each Expense
                foreach (XmlNode expense in doc.DocumentElement.ChildNodes)
                {
                    // set default expense parameters
                    int id = int.Parse((((XmlElement)expense).GetAttributeNode("ID")).InnerText);
                    String description = "";
                    DateTime date = DateTime.Parse("2000-01-01");
                    int category = 0;
                    Double amount = 0.0;

                    // get expense parameters
                    foreach (XmlNode info in expense.ChildNodes)
                    {
                        switch (info.Name)
                        {
                            case "Date":
                                date = DateTime.Parse(info.InnerText);
                                break;
                            case "Amount":
                                amount = Double.Parse(info.InnerText);
                                break;
                            case "Description":
                                description = info.InnerText;
                                break;
                            case "Category":
                                category = int.Parse(info.InnerText);
                                break;
                        }
                    }

                    // have all info for expense, so create new one
                    this.Add(new Expense(id, date, category, amount, description));

                }

            }
            catch (Exception e)
            {
                throw new Exception("ReadFromFileException: Reading XML " + e.Message);
            }
        }


        // ====================================================================
        // write to an XML file
        // if filepath is not specified, read/save in AppData file
        // ====================================================================
        private void _WriteXMLFile(String filepath)
        {
            // ---------------------------------------------------------------
            // loop over all categories and write them out as XML
            // ---------------------------------------------------------------
            try
            {
                CultureInfo.CurrentCulture = new("en-US", false);
                // create top level element of expenses
                XmlDocument doc = new XmlDocument();
                doc.LoadXml("<Expenses></Expenses>");

                // foreach Category, create an new xml element
                foreach (Expense exp in List())
                {
                    // main element 'Expense' with attribute ID
                    XmlElement ele = doc.CreateElement("Expense");
                    XmlAttribute attr = doc.CreateAttribute("ID");
                    attr.Value = exp.Id.ToString();
                    ele.SetAttributeNode(attr);
                    doc.DocumentElement.AppendChild(ele);

                    // child attributes (date, description, amount, category)
                    XmlElement d = doc.CreateElement("Date");
                    XmlText dText = doc.CreateTextNode(exp.Date.ToString("yyyy-MM-dd"));
                    ele.AppendChild(d);
                    d.AppendChild(dText);

                    XmlElement de = doc.CreateElement("Description");
                    XmlText deText = doc.CreateTextNode(exp.Description);
                    ele.AppendChild(de);
                    de.AppendChild(deText);

                    XmlElement a = doc.CreateElement("Amount");
                    XmlText aText = doc.CreateTextNode(exp.Amount.ToString());
                    ele.AppendChild(a);
                    a.AppendChild(aText);

                    XmlElement c = doc.CreateElement("Category");
                    XmlText cText = doc.CreateTextNode(exp.Category.ToString());
                    ele.AppendChild(c);
                    c.AppendChild(cText);

                }

                // write the xml to FilePath
                doc.Save(filepath);

            }
            catch (Exception e)
            {
                throw new Exception("SaveToFileException: Reading XML " + e.Message);
            }
        }

    }
}

