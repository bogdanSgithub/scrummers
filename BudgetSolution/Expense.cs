using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// ============================================================================
// (c) Sandy Bultena 2018
// * Released under the GNU General Public License
// ============================================================================

namespace Budget
{
    // ====================================================================
    // CLASS: Expense
    //        - An individual expens for budget program
    // ====================================================================
    /// <summary>
    /// An individual expense for budget program
    /// </summary>
    public class Expense
    {
        // ====================================================================
        // Properties
        // ====================================================================
        /// <summary>
        /// Integer that represents the unique identifier for the expense
        /// </summary>
        public int Id { get; }
        /// <summary>
        /// DateTime object that represents the date of the expense
        /// </summary>
        public DateTime Date { get; }
        /// <summary>
        /// Double that represents the amount of the expense
        /// </summary>
        public Double Amount { get; }
        /// <summary>
        /// String that represents the description of the expense
        /// </summary>
        public String Description { get; }
        /// <summary>
        /// Integer that represents the category of the expense
        /// </summary>
        public int Category { get; }

        // ====================================================================
        // Constructor
        //    NB: there is no verification the expense category exists in the
        //        categories object
        // ====================================================================
        /// <summary>
        /// Parameterized constructor for the Expense class, simply assigns the values to the properties, no verification is done.
        /// </summary>
        /// <param name="id">Int parameter that Id Property will be set to</param>
        /// <param name="date">DateTime parameter that Date Property will be set to</param>
        /// <param name="category">Int parameter that Category Property will be set to</param>
        /// <param name="amount">Double parameter that Amount Property will be set to</param>
        /// <param name="description">String parameter that Description Property will be set to</param>
        /// <example>
        /// <code>
        /// Expense expense = new Expense(1, DateTime.Now, 1, 5.5, "Groceries");
        /// </code>
        /// </example>
        public Expense(int id, DateTime date, int category, Double amount, String description)
        {
            if (String.IsNullOrEmpty(description))
                throw new ArgumentNullException("description cannot be empty");

            this.Id = id;
            this.Date = date;
            this.Category = category;
            this.Amount = amount;
            this.Description = description;
        }

        // ====================================================================
        // Copy constructor - does a deep copy
        // ====================================================================
        /// <summary>
        /// Takes an expense object and does a deep copy of it.
        /// </summary>
        /// <param name="obj">Expense object</param>
        /// <example>
        /// <code>
        /// Expense expense = new Expense(1, DateTime.Now, 1, 5.5, "Groceries");
        /// Expense newExpense = new Expense(expense);
        /// </code>
        /// </example>
        public Expense(Expense obj)
        {
            this.Id = obj.Id;
            this.Date = obj.Date;
            this.Category = obj.Category;
            this.Amount = obj.Amount;
            this.Description = obj.Description;

        }
    }
}
