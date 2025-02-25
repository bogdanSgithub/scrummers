using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Dynamic;

// ============================================================================
// (c) Sandy Bultena 2018
// * Released under the GNU General Public License
// ============================================================================


namespace Budget
{
    // ====================================================================
    // CLASS: HomeBudget
    //        - Combines categories Class and expenses Class
    //        - One File defines Category and Budget File
    //        - etc
    // ====================================================================
    /// <summary>
    /// Main Budget Manager class. Tracks Categories, Expenses, allowing you to read from files and save to files the data.
    /// Provides important methods to get the budget items by different criteria.
    /// </summary>
    /// <example>
    /// <code>
    /// HomeBudget budget = new HomeBudget();
    /// budget.ReadFromFile("C:\\path\\to\\budget_file.txt");
    /// Access the categories and expenses
    /// var categories = budget.categories.List();
    /// var expenses = budget.expenses.List();
    /// Get a list of budget items filtered by a specific date range (2023)
    /// var startDate = new DateTime(2023, 1, 1);
    /// var endDate = new DateTime(2023, 12, 31);
    /// var budgetItems = budget.GetBudgetItems(startDate, endDate, false, 0);
    /// foreach (var item in budgetItems)
    /// {
    ///     Console.WriteLine($"Category: {item.Category}, Description: {item.ShortDescription}, Amount: {item.Amount}");
    /// }
    /// budget.SaveToFile("C:\\path\\to\\new_budget_file.txt");
    /// </code>
    /// </example>
    public class HomeBudget
    {
        private string _FileName;
        private string _DirName;
        private Categories _categories;
        private Expenses _expenses;

        // ====================================================================
        // Properties
        // ===================================================================

        // Properties (location of files etc)
        /// <summary>
        /// Stores the Budget File Name.
        /// </summary>
        /// <return>
        /// It returns a string representing the budget File Name or null if it's not set.
        /// </return>
        public String FileName { get { return _FileName; } }
        /// <summary>
        /// Stores the the name of the Directory.
        /// </summary>
        /// <return>
        /// Returns a string representing the directory Name or null if it's not set.
        /// </return>
        public String DirName { get { return _DirName; } }
        /// <summary>
        /// Stores the Directory name + File name together.
        /// </summary>
        public String PathName
        {
            get
            {
                if (_FileName != null && _DirName != null)
                {
                    return Path.GetFullPath(_DirName + "\\" + _FileName);
                }
                else
                {
                    return null;
                }
            }
        }

        // Properties (categories and expenses object)
        /// <summary>
        /// Stores the Categories of the Budget
        /// </summary>
        public Categories categories { get { return _categories; } }
        /// <summary>
        /// Stores the Expenses of the Budget
        /// </summary>
        public Expenses expenses { get { return _expenses; } }

        // -------------------------------------------------------------------
        // Constructor (new... default categories, no expenses)
        // -------------------------------------------------------------------
        /// <summary>
        /// Default Constructor. Sets the Categories and Expenses using their default constructors.
        /// </summary>
        /// <example>
        /// <code>
        /// HomeBudget homeBudget = new HomeBudget();
        /// </code>
        /// </example>
        public HomeBudget()
        {
            _categories = new Categories();
            _expenses = new Expenses();
        }

        // -------------------------------------------------------------------
        // Constructor (existing budget ... must specify file)
        // -------------------------------------------------------------------
        /// <summary>
        /// Constructor that takes in the budget File Name and uses that file to set the Categories and Expenses.
        /// </summary>
        /// <param name="budgetFileName"> string which represents the name of the budget file
        /// </param>
        /// <example>
        /// <code>
        /// HomeBudget myBudget = new HomeBudget("C:\\Users\\studentID\\Desktop\\Scrummers\\BudgetSolution\\test.budget");
        /// </code>
        /// </example>
        public HomeBudget(String budgetFileName)
        {
            _categories = new Categories();
            _expenses = new Expenses();
            ReadFromFile(budgetFileName);
        }

        public HomeBudget(String databaseFile, String expensesXMLFile, bool newDB = false)
        {
            // if database exists, and user doesn't want a new database, open existing DB
            if (!newDB && File.Exists(databaseFile))
            {
                Database.existingDatabase(databaseFile);
            }

            // file did not exist, or user wants a new database, so open NEW DB
            else
            {
                Database.newDatabase(databaseFile);
                newDB = true;
            }

            // create the category object
            _categories = new Categories(Database.dbConnection, newDB);

            // create the _expenses course
            _expenses = new Expenses();
            _expenses.ReadFromFile(expensesXMLFile);
        }
        #region OpenNewAndSave
        // ---------------------------------------------------------------
        // Read
        // Throws Exception if any problem reading this file
        // ---------------------------------------------------------------
        /// <summary>
        /// Takes in the budget File Name and tries to get the Categories and Expenses from it. 
        /// </summary>
        /// <param name="budgetFileName">string which represents the name of the budget file.</param> 
        /// <exception cref="Exception">Exception thrown if the file doesn't exist or it's not in the right format.</exception> 
        /// <example>
        /// <code>
        /// HomeBudget myBudget = new HomeBudget();
        /// myBudget.ReadFromFile(@"C:\Users\studentID\Downloads\BudgetSolution\BudgetSolution\test.budget");
        /// // there is no output
        /// </code>
        /// </example>

        public void ReadFromFile(String budgetFileName)
        {
            // ---------------------------------------------------------------
            // read the budget file and process
            // ---------------------------------------------------------------
            try
            {
                // get filepath name (throws exception if it doesn't exist)
                budgetFileName = BudgetFiles.VerifyReadFromFileName(budgetFileName, "");

                // If file exists, read it
                string[] filenames = System.IO.File.ReadAllLines(budgetFileName);

                // ----------------------------------------------------------------
                // Save information about budget file
                // ----------------------------------------------------------------
                string folder = Path.GetDirectoryName(budgetFileName);
                _FileName = Path.GetFileName(budgetFileName);

                // read the expenses and categories from their respective files
                _categories.ReadFromFile(folder + "\\" + filenames[0]);
                _expenses.ReadFromFile(folder + "\\" + filenames[1]);

                // Save information about budget file
                _DirName = Path.GetDirectoryName(budgetFileName);
                _FileName = Path.GetFileName(budgetFileName);

            }

            // ----------------------------------------------------------------
            // throw new exception if we cannot get the info that we need
            // ----------------------------------------------------------------
            catch (Exception e)
            {
                throw new Exception("Could not read budget info: \n" + e.Message);
            }

        }

        // ====================================================================
        // save to a file
        // saves the following files:
        //  filepath_expenses.exps  # expenses file
        //  filepath_categories.cats # categories files
        //  filepath # a file containing the names of the expenses and categories files.
        //  Throws exception if we cannot write to that file (ex: invalid dir, wrong permissions)
        // ====================================================================
        /// <summary>
        /// Sets the Directory and File Name to null and after sets them to the provided filepath. Given the path and file which is really a prefix, takes that prefix to find the expenses and categories files
        /// and saves our homebudget's expenses and categories to those files. Finally it sets the DirName and FileName to the path and file.
        /// </summary>
        /// <param name="filepath">Represents the FilePath were to save</param>
        /// <exception cref="Exception">
        /// Thrown if the provided file path is invalid or cannot be written to.
        /// </exception>
        /// <example>
        /// <code>
        /// HomeBudget myBudget = new HomeBudget();
        /// myBudget.SaveToFile(@"C:\Users\studentID\Downloads\BudgetSolution\BudgetSolution\test.budget");
        /// // there is no output
        /// </code>
        /// </example>
        public void SaveToFile(String filepath)
        {

            // ---------------------------------------------------------------
            // just in case filepath doesn't exist, reset path info
            // ---------------------------------------------------------------
            _DirName = null;
            _FileName = null;

            // ---------------------------------------------------------------
            // get filepath name (throws exception if we can't write to the file)
            // ---------------------------------------------------------------
            filepath = BudgetFiles.VerifyWriteToFileName(filepath, "");

            String path = Path.GetDirectoryName(Path.GetFullPath(filepath));
            String file = Path.GetFileNameWithoutExtension(filepath);
            String ext = Path.GetExtension(filepath);

            // ---------------------------------------------------------------
            // construct file names for expenses and categories
            // ---------------------------------------------------------------
            String expensepath = path + "\\" + file + "_expenses" + ".exps";
            String categorypath = path + "\\" + file + "_categories" + ".cats";

            // ---------------------------------------------------------------
            // save the expenses and categories into their own files
            // ---------------------------------------------------------------
            _expenses.SaveToFile(expensepath);
            _categories.SaveToFile(categorypath);

            // ---------------------------------------------------------------
            // save filenames of expenses and categories to budget file
            // ---------------------------------------------------------------
            string[] files = { Path.GetFileName(categorypath), Path.GetFileName(expensepath) };
            System.IO.File.WriteAllLines(filepath, files);

            // ----------------------------------------------------------------
            // save filename info for later use
            // ----------------------------------------------------------------
            _DirName = path;
            _FileName = Path.GetFileName(filepath);
        }
        #endregion OpenNewAndSave

        #region GetList
        // ============================================================================
        // Get all expenses list
        // NOTE: VERY IMPORTANT... budget amount is the negative of the expense amount
        // Reasoning: an expense of $15 is -$15 from your bank account.
        // ============================================================================
        /// <summary>
        /// Returns a list of BudgetItems
        /// Sorting:
        /// The list is sorted in ascending order by Date.
        /// 
        /// Balance:
        /// The Balance represents the running total of the budget items up to that date.
        /// 
        /// Filter flag:
        /// If the filter flag is set to true, remove the budget items that belong to the specified Category ID from the output.
        /// 
        /// Category ID:
        /// The Category ID that we want to filter out from the output. If the filter flag is set to false, then it's never used.
        /// 
        /// Date range:
        /// - The Start represents the start date, filtering out all budget items that happened before that date. If it's null then
        ///  it will be set to the default which is January 1st 1900
        /// - The End represents the end date, filtering out all budget items that happened after that date. If it's null then
        ///  it will be set to the default which is January 1st 2500
        /// </summary>
        /// <param name="Start">Start Date Parameter used to get budget items after a certain date. If null, it defaults to 1/1/1900.</param>
        /// <param name="End">End Date Parameter used to get budget items before a certain date. If null, it defaults to 1/1/2500.</param>
        /// <param name="FilterFlag">Bool used to know if we should filter out by a certain category ID or not.</param>
        /// <param name="CategoryID">CategoryID which is used to filter out for the items of that category if FilterFlag is true.</param>
        /// <returns>A list of budget summaries (BudgetItemsByCategory objects) grouped by category, each containing total expenses and a list of BudgetItems, details.</returns>
        /// <returns></returns>
        /// 
        /// <example>
        /// 
        /// For all examples below, assume the budget file contains the
        /// following elements:
        /// 
        /// <code>
        /// Cat_ID Expense_ID Date Description Cost
        /// 10 1 1/10/2018 12:00:00 AM Clothes hat (on credit) 10
        /// 9 2 1/11/2018 12:00:00 AM Credit Card hat -10
        /// 10 3 1/10/2019 12:00:00 AM Clothes scarf(on credit) 15
        /// 9 4 1/10/2020 12:00:00 AM Credit Card scarf -15
        /// 14 5 1/11/2020 12:00:00 AM Eating Out McDonalds 45
        /// 14 7 1/12/2020 12:00:00 AM Eating Out Wendys 25
        /// 14 10 2/1/2020 12:00:00 AM Eating Out Pizza 33.33
        /// 9 13 2/10/2020 12:00:00 AM Credit Card mittens -15
        /// 9 12 2/25/2020 12:00:00 AM Credit Card Hat -25
        /// 14 11 2/27/2020 12:00:00 AM Eating Out Pizza 33.33
        /// 14 9 7/11/2020 12:00:00 AM Eating Out Cafeteria 11.11
        /// </code>
        /// 
        /// Getting a list of ALL budget items.
        /// 
        /// <code>
        /// <![CDATA[
        /// HomeBudget budget = new HomeBudget();
        /// budget.ReadFromFile(@"C:\Users\studentID\Downloads\BudgetSolution\test.budget");
        /// 
        /// // Get a list of all budget items
        /// var budgetItems = budget.GetBudgetItems(null, null, false, 0);
        /// 
        /// // print important information
        /// foreach (var bi in budgetItems)
        /// {
        ///     Console.WriteLine (
        ///         String.Format("{0} {1,-20} {2,8:C} {3,12:C}",
        ///         bi.Date.ToString("yyyy/MMM/dd"),
        ///         bi.ShortDescription,
        ///         bi.Amount, bi.Balance)
        ///     );
        /// }
        /// ]]>
        /// </code>
        /// 
        /// Sample output:
        /// <code>
        /// 2018-Jan-10 hat(on credit)       -$10.00      -$10.00
        /// 2018-Jan-11 hat                    $10.00        $0.00
        /// 2019-Jan-10 scarf(on credit)     -$15.00      -$15.00
        /// 2020-Jan-10 scarf                  $15.00        $0.00
        /// 2020-Jan-11 McDonalds             -$45.00      -$45.00
        /// 2020-Jan-12 Wendys                -$25.00      -$70.00
        /// 2020-Feb-01 Pizza                 -$33.33     -$103.33
        /// 2020-Feb-10 mittens                $15.00      -$88.33
        /// 2020-Feb-25 Hat                    $25.00      -$63.33
        /// 2020-Feb-27 Pizza                 -$33.33      -$96.66
        /// 2020-Jul-11 Cafeteria             -$11.11     -$107.77
        /// </code>
        /// 
        /// 
        /// Example: Filtering by Category ID 10
        /// 
        /// <code>
        /// <![CDATA[
        /// var filteredItems = budget.GetBudgetItems(null, null, true, 10);
        /// 
        /// foreach (var bi in filteredItems)
        /// {
        ///     Console.WriteLine (
        ///         String.Format("{0} {1,-20} {2,8:C} {3,12:C}",
        ///         bi.Date.ToString("yyyy/MMM/dd"),
        ///         bi.ShortDescription,
        ///         bi.Amount, bi.Balance)
        ///     );
        /// }
        /// ]]>
        /// </code>
        /// 
        /// Sample output:
        /// <code>
        /// 2018-Jan-10 hat(on credit)       -$10.00      -$10.00
        /// 2019-Jan-10 scarf(on credit)     -$15.00      -$25.00
        /// </code>
        /// 
        /// Example: Filtering by Date (before February 2, 2020)
        /// 
        /// <code>
        /// <![CDATA[
        /// DateTime end = new DateTime(2020, 2, 2);
        /// var filteredByDate = budget.GetBudgetItems(null, end, false, 0);
        /// 
        /// foreach (var bi in filteredByDate)
        /// {
        ///     Console.WriteLine (
        ///         String.Format("{0} {1,-20} {2,8:C} {3,12:C}",
        ///         bi.Date.ToString("yyyy/MMM/dd"),
        ///         bi.ShortDescription,
        ///         bi.Amount, bi.Balance)
        ///     );
        /// }
        /// ]]>
        /// </code>
        /// 
        /// Output
        /// <code>
        /// 2018-Jan-10 hat (on credit)       -$10.00      -$10.00
        /// 2018-Jan-11 hat                    $10.00        $0.00
        /// 2019-Jan-10 scarf(on credit)     -$15.00      -$15.00
        /// 2020-Jan-10 scarf                  $15.00        $0.00
        /// 2020-Jan-11 McDonalds             -$45.00      -$45.00
        /// 2020-Jan-12 Wendys                -$25.00      -$70.00
        /// 2020-Feb-01 Pizza                 -$33.33     -$103.33
        /// </code>
        /// </example>
        public List<BudgetItem> GetBudgetItems(DateTime? Start, DateTime? End, bool FilterFlag, int CategoryID)
        {
            // ------------------------------------------------------------------------
            // return joined list within time frame
            // ------------------------------------------------------------------------
            Start = Start ?? new DateTime(1900, 1, 1);
            End = End ?? new DateTime(2500, 1, 1);

            var query = from c in _categories.List()
                        join e in _expenses.List() on c.Id equals e.Category
                        where e.Date >= Start && e.Date <= End
                        select new { CatId = c.Id, ExpId = e.Id, e.Date, Category = c.Description, e.Description, e.Amount };

            // ------------------------------------------------------------------------
            // create a BudgetItem list with totals,
            // ------------------------------------------------------------------------
            List<BudgetItem> items = new List<BudgetItem>();
            Double total = 0;

            foreach (var e in query.OrderBy(q => q.Date))
            {
                // filter out unwanted categories if filter flag is on
                if (FilterFlag && CategoryID != e.CatId)
                {
                    continue;
                }

                // keep track of running totals
                total = total - e.Amount;
                items.Add(new BudgetItem
                {
                    CategoryID = e.CatId,
                    ExpenseID = e.ExpId,
                    ShortDescription = e.Description,
                    Date = e.Date,
                    Amount = -e.Amount,
                    Category = e.Category,
                    Balance = total
                });
            }

            return items;
        }

        // ============================================================================
        // Group all expenses month by month (sorted by year/month)
        // returns a list of BudgetItemsByMonth which is 
        // "year/month", list of budget items, and total for that month
        // ============================================================================
        /// <summary>
        /// Retrieves a summary of budget items grouped by month within the specified date range.
        /// The method calculates the total expenses for each month and returns a list of budget summaries (BudgetItemsByMonth objects) which are grouped by month.
        /// </summary>
        /// <param name="Start">Start Date Parameter used to get budget items after a certain date. If null, it defaults to 1/1/1900.</param>
        /// <param name="End">End Date Parameter used to get budget items before a certain date. If null, it defaults to 1/1/2500.</param>
        /// <param name="FilterFlag">Bool used to know if we should filter out by a certain category ID or not.</param>
        /// <param name="CategoryID">CategoryID which is used to filter out for the items of that category if FilterFlag is true.</param>
        /// <returns>A list of budget summaries (BudgetItemsByCategory objects) grouped by category, each containing total expenses and a list of BudgetItems, details.</returns>
        /// <example> 
        /// Getting a list of BudgetItemsByCategory
        /// <code>
        /// <![CDATA[
        /// HomeBudget budget = new HomeBudget();
        /// budget.ReadFromFile(@"C:\Users\Bogdan\source\repos\testPLZ\test.budget");
        /// var budgetItemsByCategories = budget.GetBudgetItemsByCategory(null, null, false, 10);
        /// foreach (var budgetItemsByCategory in budgetItemsByCategories)
        /// {
        ///     Console.WriteLine($"Category: {budgetItemsByCategory.Category}");
        ///     Console.WriteLine($"Total: {budgetItemsByCategory.Total:C}");
        ///     Console.WriteLine("Details:");
        ///     foreach (var bi in budgetItemsByCategory.Details)
        ///     {
        ///         Console.WriteLine(
        ///             String.Format("{0} {1,-20} {2,8:C} {3,12:C}",
        ///             bi.Date.ToString("yyyy/MMM/dd"),
        ///             bi.ShortDescription,
        ///             bi.Amount, bi.Balance)
        ///         );
        ///     }
        /// 
        ///     Console.WriteLine("----------------------------------------");
        /// }
        /// ]]>
        /// </code>
        /// 
        /// Output
        /// <code>
        /// Category: Clothes
        /// Total: -$25.00
        /// Details:
        /// 2018-Jan-10 hat (on credit)       -$10.00      -$10.00
        /// 2019-Jan-10 scarf (on credit)     -$15.00      -$15.00
        /// ----------------------------------------
        /// Category: Credit Card
        /// Total: $65.00
        /// Details:
        /// 2018-Jan-11 hat                    $10.00        $0.00
        /// 2020-Jan-10 scarf                  $15.00        $0.00
        /// 2020-Feb-10 mittens                $15.00      -$88.33
        /// 2020-Feb-25 Hat                    $25.00      -$63.33
        /// ----------------------------------------
        /// Category: Eating Out
        /// Total: -$147.77
        /// Details:
        /// 2020-Jan-11 McDonalds             -$45.00      -$45.00
        /// 2020-Jan-12 Wendys                -$25.00      -$70.00
        /// 2020-Feb-01 Pizza                 -$33.33     -$103.33
        /// 2020-Feb-27 Pizza                 -$33.33      -$96.66
        /// 2020-Jul-11 Cafeteria             -$11.11     -$107.77
        /// ----------------------------------------
        /// </code>
        /// </example>
        public List<BudgetItemsByMonth> GetBudgetItemsByMonth(DateTime? Start, DateTime? End, bool FilterFlag, int CategoryID)
        {
            // -----------------------------------------------------------------------
            // get all items first
            // -----------------------------------------------------------------------
            List<BudgetItem> items = GetBudgetItems(Start, End, FilterFlag, CategoryID);

            // -----------------------------------------------------------------------
            // Group by year/month
            // -----------------------------------------------------------------------
            var GroupedByMonth = items.GroupBy(c => c.Date.Year.ToString("D4") + "/" + c.Date.Month.ToString("D2"));

            // -----------------------------------------------------------------------
            // create new list
            // -----------------------------------------------------------------------
            var summary = new List<BudgetItemsByMonth>();
            foreach (var MonthGroup in GroupedByMonth)
            {
                // calculate total for this month, and create list of details
                double total = 0;
                var details = new List<BudgetItem>();
                foreach (var item in MonthGroup)
                {
                    total = total + item.Amount;
                    details.Add(item);
                }

                // Add new BudgetItemsByMonth to our list
                summary.Add(new BudgetItemsByMonth
                {
                    Month = MonthGroup.Key,
                    Details = details,
                    Total = total
                });
            }

            return summary;
        }

        // ============================================================================
        // Group all expenses by category (ordered by category name)
        // ============================================================================
        /// <summary>
        /// Retrieves a summary of budget items grouped by category within the specified date range.
        /// The method calculates the total expenses for each category and returns a list of budget summaries (BudgetItemsByCategory objects) which are grouped by category.
        /// </summary>
        /// <param name="Start">Start Date Parameter used to get budget items after a certain date. If null, it defaults to 1/1/1900.</param>
        /// <param name="End">End Date Parameter used to get budget items before a certain date. If null, it defaults to 1/1/2500.</param>
        /// <param name="FilterFlag">Bool used to know if we should filter out by a certain category ID or not.</param>
        /// <param name="CategoryID">CategoryID which is used to filter out for the items of that category if FilterFlag is true.</param>
        /// <returns>A list of budget summaries (BudgetItemsByCategory objects) grouped by category, each containing total expenses and a list of BudgetItems, details.</returns>
        /// <example>
        /// 
        /// For all examples below, assume the budget file contains the
        /// following elements:
        /// 
        /// <code>
        /// Cat_ID Expense_ID Date Description Cost
        /// 10 1 1/10/2018 12:00:00 AM Clothes hat (on credit) 10
        /// 9 2 1/11/2018 12:00:00 AM Credit Card hat -10
        /// 10 3 1/10/2019 12:00:00 AM Clothes scarf(on credit) 15
        /// 9 4 1/10/2020 12:00:00 AM Credit Card scarf -15
        /// 14 5 1/11/2020 12:00:00 AM Eating Out McDonalds 45
        /// 14 7 1/12/2020 12:00:00 AM Eating Out Wendys 25
        /// 14 10 2/1/2020 12:00:00 AM Eating Out Pizza 33.33
        /// 9 13 2/10/2020 12:00:00 AM Credit Card mittens -15
        /// 9 12 2/25/2020 12:00:00 AM Credit Card Hat -25
        /// 14 11 2/27/2020 12:00:00 AM Eating Out Pizza 33.33
        /// 14 9 7/11/2020 12:00:00 AM Eating Out Cafeteria 11.11
        /// </code>
        /// 
        /// Getting a list of BudgetItemsByCategory
        /// <code>
        /// <![CDATA[
        /// HomeBudget budget = new HomeBudget();
        /// budget.ReadFromFile(@"C:\Users\Bogdan\source\repos\testPLZ\test.budget");
        /// var budgetItemsByCategories = budget.GetBudgetItemsByCategory(null, null, false, 10);
        /// foreach (var budgetItemsByCategory in budgetItemsByCategories)
        /// {
        ///     Console.WriteLine($"Category: {budgetItemsByCategory.Category}");
        ///     Console.WriteLine($"Total: {budgetItemsByCategory.Total:C}");
        ///     Console.WriteLine("Details:");
        ///     foreach (var bi in budgetItemsByCategory.Details)
        ///     {
        ///         Console.WriteLine(
        ///             String.Format("{0} {1,-20} {2,8:C} {3,12:C}",
        ///             bi.Date.ToString("yyyy/MMM/dd"),
        ///             bi.ShortDescription,
        ///             bi.Amount, bi.Balance)
        ///         );
        ///     }
        /// 
        ///     Console.WriteLine("----------------------------------------");
        /// }
        /// ]]>
        /// </code>
        /// 
        /// Output
        /// <code>
        /// Category: Clothes
        /// Total: -$25.00
        /// Details:
        /// 2018-Jan-10 hat (on credit)       -$10.00      -$10.00
        /// 2019-Jan-10 scarf (on credit)     -$15.00      -$15.00
        /// ----------------------------------------
        /// Category: Credit Card
        /// Total: $65.00
        /// Details:
        /// 2018-Jan-11 hat                    $10.00        $0.00
        /// 2020-Jan-10 scarf                  $15.00        $0.00
        /// 2020-Feb-10 mittens                $15.00      -$88.33
        /// 2020-Feb-25 Hat                    $25.00      -$63.33
        /// ----------------------------------------
        /// Category: Eating Out
        /// Total: -$147.77
        /// Details:
        /// 2020-Jan-11 McDonalds             -$45.00      -$45.00
        /// 2020-Jan-12 Wendys                -$25.00      -$70.00
        /// 2020-Feb-01 Pizza                 -$33.33     -$103.33
        /// 2020-Feb-27 Pizza                 -$33.33      -$96.66
        /// 2020-Jul-11 Cafeteria             -$11.11     -$107.77
        /// ----------------------------------------
        /// </code>
        /// </example>

        public List<BudgetItemsByCategory> GetBudgetItemsByCategory(DateTime? Start, DateTime? End, bool FilterFlag, int CategoryID)
        {
            // -----------------------------------------------------------------------
            // get all items first
            // -----------------------------------------------------------------------
            List<BudgetItem> items = GetBudgetItems(Start, End, FilterFlag, CategoryID);

            // -----------------------------------------------------------------------
            // Group by Category
            // -----------------------------------------------------------------------
            var GroupedByCategory = items.GroupBy(c => c.Category);

            // -----------------------------------------------------------------------
            // create new list
            // -----------------------------------------------------------------------
            var summary = new List<BudgetItemsByCategory>();
            foreach (var CategoryGroup in GroupedByCategory.OrderBy(g => g.Key))
            {
                // calculate total for this category, and create list of details
                double total = 0;
                var details = new List<BudgetItem>();
                foreach (var item in CategoryGroup)
                {
                    total = total + item.Amount;
                    details.Add(item);
                }

                // Add new BudgetItemsByCategory to our list
                summary.Add(new BudgetItemsByCategory
                {
                    Category = CategoryGroup.Key,
                    Details = details,
                    Total = total
                });
            }
            return summary;
        }


        // ============================================================================
        // Group all events by category and Month
        // creates a list of Dictionary objects (which are objects that contain key value pairs).
        // The list of Dictionary objects includes:
        //          one dictionary object per month with expenses,
        //          and one dictionary object for the category totals
        // 
        // Each per month dictionary object has the following key value pairs:
        //           "Month", <the year/month for that month as a string>
        //           "Total", <the total amount for that month as a double>
        //            and for each category for which there is an expense in the month:
        //             "items:category", a List<BudgetItem> of all items in that category for the month
        //             "category", the total amount for that category for this month
        //
        // The one dictionary for the category totals has the following key value pairs:
        //             "Month", the string "TOTALS"
        //             for each category for which there is an expense in ANY month:
        //             "category", the total for that category for all the months
        // ============================================================================
        /// <summary>
        /// Groups all budget events by category and month, creating a list of dictionaries containing key-value pairs. 
        /// The dictionaries provide a detailed breakdown of monthly expenses and category totals.
        /// </summary>
        /// <param name="Start">Start Date Parameter used to get budget items after a certain date. If null, it defaults to 1/1/1900.</param>
        /// <param name="End">End Date Parameter used to get budget items before a certain date. If null, it defaults to 1/1/2500.</param>
        /// <param name="FilterFlag">Bool used to know if we should filter out by a certain category ID or not.</param>
        /// <param name="CategoryID">CategoryID which is used to filter out for the items of that category if FilterFlag is true.</param>
        /// <returns>A list of dictionaries, each containing monthly expense details and category totals. Each dictionary includes 
        /// the month's expenses, categorized expenses, and a final summary of category totals across all months.</returns>
        /// <example>
        /// Getting a list of BudgetItemsByCategory
        /// <code>
        /// <![CDATA[
        /// HomeBudget budget = new HomeBudget();
        /// budget.ReadFromFile(@"C:\Users\Bogdan\source\repos\testPLZ\test.budget");
        /// var budgetItemsByCategories = budget.GetBudgetItemsByCategory(null, null, false, 10);
        /// foreach (var budgetItemsByCategory in budgetItemsByCategories)
        /// {
        ///     Console.WriteLine($"Category: {budgetItemsByCategory.Category}");
        ///     Console.WriteLine($"Total: {budgetItemsByCategory.Total:C}");
        ///     Console.WriteLine("Details:");
        ///     foreach (var bi in budgetItemsByCategory.Details)
        ///     {
        ///         Console.WriteLine(
        ///             String.Format("{0} {1,-20} {2,8:C} {3,12:C}",
        ///             bi.Date.ToString("yyyy/MMM/dd"),
        ///             bi.ShortDescription,
        ///             bi.Amount, bi.Balance)
        ///         );
        ///     }
        /// 
        ///     Console.WriteLine("----------------------------------------");
        /// }
        /// ]]>
        /// </code>
        /// output
        /// <code>
        /// Month: 2018/01
        /// Category Items: details:Clothes
        /// 2018-Jan-10 hat (on credit)       -$10.00      -$10.00
        /// Category Items: details:Credit Card
        /// 2018-Jan-11 hat                    $10.00        $0.00
        /// Month: 2019/01
        /// Category Items: details:Clothes
        /// 2019-Jan-10 scarf (on credit)     -$15.00      -$15.00
        /// Month: 2020/01
        /// Category Items: details:Credit Card
        /// 2020-Jan-10 scarf                  $15.00        $0.00
        /// Category Items: details:Eating Out
        /// 2020-Jan-11 McDonalds             -$45.00      -$45.00
        /// 2020-Jan-12 Wendys                -$25.00      -$70.00
        /// Month: 2020/02
        /// Category Items: details:Credit Card
        /// 2020-Feb-10 mittens                $15.00      -$88.33
        /// 2020-Feb-25 Hat                    $25.00      -$63.33
        /// Category Items: details:Eating Out
        /// 2020-Feb-01 Pizza                 -$33.33     -$103.33
        /// 2020-Feb-27 Pizza                 -$33.33      -$96.66
        /// Month: 2020/07
        /// Category Items: details:Eating Out
        /// 2020-Jul-11 Cafeteria             -$11.11     -$107.77
        /// </code>
        /// </example>
        public List<Dictionary<string, object>> GetBudgetDictionaryByCategoryAndMonth(DateTime? Start, DateTime? End, bool FilterFlag, int CategoryID)
        {
            // -----------------------------------------------------------------------
            // get all items by month 
            // -----------------------------------------------------------------------
            List<BudgetItemsByMonth> GroupedByMonth = GetBudgetItemsByMonth(Start, End, FilterFlag, CategoryID);

            // -----------------------------------------------------------------------
            // loop over each month
            // -----------------------------------------------------------------------
            var summary = new List<Dictionary<string, object>>();
            var totalsPerCategory = new Dictionary<String, Double>();

            foreach (var MonthGroup in GroupedByMonth)
            {
                // create record object for this month
                Dictionary<string, object> record = new Dictionary<string, object>();
                record["Month"] = MonthGroup.Month;
                record["Total"] = MonthGroup.Total;

                // break up the month details into categories
                var GroupedByCategory = MonthGroup.Details.GroupBy(c => c.Category);

                // -----------------------------------------------------------------------
                // loop over each category
                // -----------------------------------------------------------------------
                foreach (var CategoryGroup in GroupedByCategory.OrderBy(g => g.Key))
                {

                    // calculate totals for the cat/month, and create list of details
                    double total = 0;
                    var details = new List<BudgetItem>();

                    foreach (var item in CategoryGroup)
                    {
                        total = total + item.Amount;
                        details.Add(item);
                    }

                    // add new properties and values to our record object
                    record["details:" + CategoryGroup.Key] = details;
                    record[CategoryGroup.Key] = total;

                    // keep track of totals for each category
                    if (totalsPerCategory.TryGetValue(CategoryGroup.Key, out Double CurrentCatTotal))
                    {
                        totalsPerCategory[CategoryGroup.Key] = CurrentCatTotal + total;
                    }
                    else
                    {
                        totalsPerCategory[CategoryGroup.Key] = total;
                    }
                }

                // add record to collection
                summary.Add(record);
            }
            // ---------------------------------------------------------------------------
            // add final record which is the totals for each category
            // ---------------------------------------------------------------------------
            Dictionary<string, object> totalsRecord = new Dictionary<string, object>();
            totalsRecord["Month"] = "TOTALS";

            foreach (var cat in categories.List())
            {
                try
                {
                    totalsRecord.Add(cat.Description, totalsPerCategory[cat.Description]);
                }
                catch { }
            }
            summary.Add(totalsRecord);


            return summary;
        }
        #endregion GetList

    }
}
