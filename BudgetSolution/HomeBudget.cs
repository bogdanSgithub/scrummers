using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Dynamic;
using System.Data.SQLite;

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
    /// Main Budget Manager class. Tracks Categories, Expenses, allowing you to read from database and save to database the data.
    /// Provides important methods to get the budget items by different criteria.
    /// </summary>
    /// <example>
    /// <code>
    /// HomeBudget budget = new HomeBudget("pathToFile.db");
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
    /// </code>
    /// </example>
    public class HomeBudget
    {
        private Categories _categories;
        private Expenses _expenses;

        // ====================================================================
        // Properties
        // ===================================================================

        // Properties (location of files etc)

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
        // Constructor (existing budget ... must specify file)
        // -------------------------------------------------------------------
        /// <summary>
        /// Constructor that takes in the database File Name and uses that file to set the Categories and Expenses.
        /// </summary>
        /// <param name="databaseFile"> string which represents the name of the database file
        /// </param>
        /// <example>
        /// <code>
        /// HomeBudget myBudget = new HomeBudget("C:\\Users\\studentID\\Desktop\\Scrummers\\BudgetSolution\\test.db");
        /// </code>
        /// </example>
        public HomeBudget(String databaseFile, bool newDB = false)
        {
            // if database exists, and user doesn't want a new database, open existing DB
            if (!newDB && File.Exists(databaseFile))
            {
                Database.existingDatabase(databaseFile);
            }

            // file did not exist, or user wants a new database, so open NEW DB
            if (newDB)
            {
                Database.newDatabase(databaseFile);
            }

            // create the category object
            _categories = new Categories(Database.dbConnection, newDB);

            // create the _expenses course
            _expenses = new Expenses();
        }

        #region GetList
        // ============================================================================
        // Get all expenses list
        // NOTE: VERY IMPORTANT... budget amount is the negative of the expense amount
        // Reasoning: an expense of $15 is -$15 from your bank account.
        // ============================================================================
        /// <summary>
        /// Returns a list of BudgetItems queried from the database
        /// 
        /// NOTE: VERY IMPORTANT... budget amount is the negative of the expense amount
        /// Reasoning: an expense of $15 is -$15 from your bank account.
        /// 
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
        /// HomeBudget myBudget = new HomeBudget("C:\\Users\\studentID\\Desktop\\Scrummers\\BudgetSolution\\test.db");
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

            SQLiteCommand cmd = new SQLiteCommand(Database.dbConnection);

            //get all budget items from db that match the given parameters
            cmd.CommandText = $"SELECT e.Id, e.CategoryId, e.Description, e.Date, e.Amount, c.Description FROM categories c, expenses e WHERE e.CategoryId = c.Id AND e.Date > @start AND e.Date < @end {(FilterFlag ? "AND e.CategoryId = @filteredID": "")} ORDER BY e.Date;";

            cmd.Parameters.AddWithValue("@start", Start);
            cmd.Parameters.AddWithValue("@end", End);
            cmd.Parameters.AddWithValue("@filteredID", CategoryID);

            SQLiteDataReader rdr = cmd.ExecuteReader();

            // ------------------------------------------------------------------------
            // create a BudgetItem list with totals,
            // ------------------------------------------------------------------------
            List<BudgetItem> items = new List<BudgetItem>();
            Double total = 0;

            //loop through the reader to populate the budget item list
            while (rdr.Read())
            {
                // set fields from database to variables to increase clarity
                int expID = rdr.GetInt32(0);
                int dbCatID = rdr.GetInt32(1);
                string itemDescription = rdr.GetString(2);
                DateTime date = rdr.GetDateTime(3);
                double amount = rdr.GetDouble(4);
                string categoryDescription = rdr.GetString(5);

                // keep track of running totals
                total += amount;
                items.Add(new BudgetItem
                {
                    CategoryID = dbCatID,
                    ExpenseID = expID,
                    ShortDescription = itemDescription,
                    Date = date,
                    Amount = amount,
                    Category = categoryDescription,
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
        /// Retrieves a summary of budget items grouped by month within the specified date range by querying the database.
        /// The method calculates the total expenses for each month and returns a list of budget summaries (BudgetItemsByMonth objects) which are grouped by month.
        /// 
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
        /// HomeBudget myBudget = new HomeBudget("C:\\Users\\studentID\\Desktop\\Scrummers\\BudgetSolution\\test.db");
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
            
            // ------------------------------------------------------------------------
            // return joined list within time frame
            // ------------------------------------------------------------------------
            Start = Start ?? new DateTime(1900, 1, 1);
            End = End ?? new DateTime(2500, 1, 1);

            SQLiteCommand cmd = new SQLiteCommand(Database.dbConnection);
            
            //Get all the budget items, but with the month and year of when the expense was made.
            cmd.CommandText = $"SELECT e.Id, e.CategoryId, e.Description, e.Date, e.Amount, c.Description, strftime('%Y/%m', e.Date) AS MonthYear FROM categories c, expenses e WHERE e.CategoryId = c.Id AND e.Date > @start AND e.Date < @end {(FilterFlag ? "AND e.CategoryId = @filteredID" : "")} ORDER BY e.Date;";

            cmd.Parameters.AddWithValue("@start", Start);
            cmd.Parameters.AddWithValue("@end", End);
            cmd.Parameters.AddWithValue("@filteredID", CategoryID);

            SQLiteDataReader rdr = cmd.ExecuteReader();

            // ------------------------------------------------------------------------
            // create a BudgetItem list with totals,
            // ------------------------------------------------------------------------
            List<BudgetItemsByMonth> listBudgetItemsByMonth = new List<BudgetItemsByMonth>();
            BudgetItemsByMonth budgetItemsByMonth = new BudgetItemsByMonth();
            string previousMonth = "";

            //loop over reader to add the fields to create budget items by month and add them to the list
            while (rdr.Read())
            {  
                // set fields from database to variables to increase clarity
                int expID = rdr.GetInt32(0);
                int dbCatID = rdr.GetInt32(1);
                string itemDescription = rdr.GetString(2);
                DateTime date = rdr.GetDateTime(3);
                double amount = rdr.GetDouble(4);
                string categoryDescription = rdr.GetString(5);
                string currentMonth = rdr.GetString(6);

                // if the month is not the same as the previous one, add the built budget item by month to the list and resert it
                if (previousMonth != currentMonth)
                {
                    if (previousMonth != "")
                        listBudgetItemsByMonth.Add(budgetItemsByMonth);

                    budgetItemsByMonth = new BudgetItemsByMonth
                    {
                        Month = currentMonth,
                        Total = 0,
                        Details = new List<BudgetItem>()
                    };
                    previousMonth = currentMonth;
                }

                // update the single budget item by month every iteration of the same month
                budgetItemsByMonth.Total += amount;
                budgetItemsByMonth.Details.Add(new BudgetItem
                    {
                        CategoryID = dbCatID,
                        ExpenseID = expID,
                        ShortDescription = itemDescription,
                        Date = date,
                        Amount = amount,
                        Category = categoryDescription,
                        Balance = -budgetItemsByMonth.Total
                    });
            }
            //make sure the last budget item is not null before adding it
            if (budgetItemsByMonth.Month is not null)
                listBudgetItemsByMonth.Add(budgetItemsByMonth);

            return listBudgetItemsByMonth;
        }

        // ============================================================================
        // Group all expenses by category (ordered by category name)
        // ============================================================================
        /// <summary>
        /// Retrieves a summary of budget items grouped by category within the specified date range by querying the database.
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
        /// HomeBudget myBudget = new HomeBudget("C:\\Users\\studentID\\Desktop\\Scrummers\\BudgetSolution\\test.db");
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
            Start = Start ?? new DateTime(1900, 1, 1);
            End = End ?? new DateTime(2500, 1, 1);

            SQLiteCommand cmd = new SQLiteCommand(Database.dbConnection);

            //get all the budget items to be able to add them to the list
            cmd.CommandText = $"SELECT e.Id, e.CategoryId, e.Description, e.Date, e.Amount, c.Description, strftime('%Y/%m', e.Date) AS MonthYear FROM categories c, expenses e WHERE e.CategoryId = c.Id AND e.Date > @start AND e.Date < @end {(FilterFlag ? "AND e.CategoryId = @filteredID" : "")} ORDER BY c.Description, e.Date;";

            cmd.Parameters.AddWithValue("@start", Start);
            cmd.Parameters.AddWithValue("@end", End);
            cmd.Parameters.AddWithValue("@filteredID", CategoryID);

            SQLiteDataReader rdr = cmd.ExecuteReader();

            List<BudgetItemsByCategory> listBudgetItemsByCategory = new List<BudgetItemsByCategory>();
            BudgetItemsByCategory budgetItemsByCategory = new BudgetItemsByCategory();
            string previousCategory = "";
            while (rdr.Read())
            {
                // set fields from database to variables to increase clarity
                int expID = rdr.GetInt32(0);
                int dbCatID = rdr.GetInt32(1);
                string itemDescription = rdr.GetString(2);
                DateTime date = rdr.GetDateTime(3);
                double amount = rdr.GetDouble(4);
                string categoryDescription = rdr.GetString(5);
                string currentMonth = rdr.GetString(6);


                // if the category is not the same as the previous one, add the built budget item by category to the list and resert it
                if (previousCategory != categoryDescription)
                {
                    if (previousCategory != "")
                        listBudgetItemsByCategory.Add(budgetItemsByCategory);

                    budgetItemsByCategory = new BudgetItemsByCategory
                    {
                        Category = categoryDescription,
                        Total = 0,
                        Details = new List<BudgetItem>()
                    };
                    previousCategory = categoryDescription;
                }

                // update the single budget item by category every iteration of the same month
                budgetItemsByCategory.Total += amount;
                budgetItemsByCategory.Details.Add(new BudgetItem
                {
                    CategoryID = dbCatID,
                    ExpenseID = expID,
                    ShortDescription = itemDescription,
                    Date = date,
                    Amount = amount,
                    Category = categoryDescription,
                    Balance = -budgetItemsByCategory.Total
                });
            }
            //make sure the last budget item is not null before adding it
            if (budgetItemsByCategory.Category is not null)
                listBudgetItemsByCategory.Add(budgetItemsByCategory);

            return listBudgetItemsByCategory;
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
        /// Groups all budget events retrieved from the database by category and month, creating a list of dictionaries containing key-value pairs. 
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
        /// HomeBudget myBudget = new HomeBudget("C:\\Users\\studentID\\Desktop\\Scrummers\\BudgetSolution\\test.db");
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
