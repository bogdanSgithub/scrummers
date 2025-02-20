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
    // CLASS: BudgetItem
    //        A single budget item, includes Category and Expense
    // ====================================================================
    /// <summary>
    /// A budget item that includes Category and Expense.
    /// </summary>

    public class BudgetItem
    {
        /// <summary>
        /// Integer that represents the CategoryID of the BudgetItem.
        /// </summary>
        public int CategoryID { get; set; }
        /// <summary>
        /// Integer that represents the ExpenseID of the BudgetItem.
        /// </summary>
        public int ExpenseID { get; set; }
        /// <summary>
        /// DateTime that represents the Date of the BudgetItem, when the BudgetItem was completed.
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// String that represents the Category name of the BudgetItem.
        /// </summary>
        public String Category { get; set; }
        /// <summary>
        /// String that gives a short description of the BudgetItem.
        /// </summary>
        public String ShortDescription { get; set; }
        /// <summary>
        /// Double that represents the Amount ($) of the BudgetItem (absolute value).
        /// </summary>
        public Double Amount { get; set; }
        /// <summary>
        /// Double that represents the Balance ($) of the BudgetItem, if expense then negative else, positive.
        /// </summary>
        public Double Balance { get; set; }
    }

    /// <summary>
    /// Has a list of BudgetItems and the total amount of the BudgetItems for a certain Month.
    /// </summary>
    public class BudgetItemsByMonth
    {
        /// <summary>
        /// String that represents the Month of the BudgetItems.
        /// </summary>
        public String Month { get; set; }
        /// <summary>
        /// List of BudgetItems.
        /// </summary>
        public List<BudgetItem> Details { get; set; }
        /// <summary>
        /// Double that represents the total amount of the BudgetItems that are in the Details list.
        /// </summary>
        public Double Total { get; set; }
    }


    /// <summary>
    /// Has a list of BudgetItems and the total value of the BudgetItems that are of a certain Category.
    /// </summary>
    public class BudgetItemsByCategory
    {
        /// <summary>
        /// String that represents the Category of the BudgetItems.
        /// </summary>
        public String Category { get; set; }
        /// <summary>
        /// List of BudgetItems.
        /// </summary>
        public List<BudgetItem> Details { get; set; }
        /// <summary>
        /// Double that represents the total value of the BudgetItems that are in the Details list.
        /// </summary>
        public Double Total { get; set; }
    }


}
