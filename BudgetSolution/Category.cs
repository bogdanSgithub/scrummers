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
    // CLASS: Category
    //        - An individual category for budget program
    //        - Valid category types: Income, Expense, Credit, Saving
    // ====================================================================
    /// <summary>
    /// An individual category for budget program
    /// </summary>
    public class Category
    {
        // ====================================================================
        // Properties
        // ====================================================================
        /// <summary>
        /// Integer that represents the unique identifier of the category.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// String that gives a description of the Category.
        /// </summary>
        public String Description { get; set; }
        /// <summary>
        /// Enum representing the Category Type of this category
        /// </summary>
        public CategoryType Type { get; set; }
        /// <summary>
        /// Enum representing the possible types of categories
        /// </summary>
        public enum CategoryType
        {
            Income,
            Expense,
            Credit,
            Savings
        };

        // ====================================================================
        // Constructor
        // ====================================================================
        /// <summary>
        /// Parameterized constructor for the Expense class, simply assigns the values to the properties, no verification is done.
        /// </summary>
        /// <param name="id">Int parameter that Id Property will be set to</param>
        /// <param name="description">String parameter that Description Property will be set to</param>
        /// <param name="type">Enum parameter that Type Property will be set to. Optional, default is Expense</param>
        /// <example>
        /// <code>
        /// Category myCategory = new Category(1, "Utilities");
        /// </code>
        /// </example>
        public Category(int id, String description, CategoryType type = CategoryType.Expense)
        {
            this.Id = id;
            this.Description = description;
            this.Type = type;
        }

        // ====================================================================
        // Copy Constructor
        // ====================================================================
        /// <summary>
        /// Takes a Category object and does a deep copy of it.
        /// </summary>
        /// <param name="category">Category object</param>
        /// <example>
        /// <code>
        /// Category cat = new Category(1, "Utilities");
        /// Category category = new Category(cat);
        /// </code>
        /// </example>
        public Category(Category category)
        {
            this.Id = category.Id;
            this.Description = category.Description;
            this.Type = category.Type;
        }
        // ====================================================================
        // String version of object
        // ====================================================================
        /// <summary>
        /// Returns the Description
        /// </summary>
        /// <returns>Returns the Description</returns>
        public override string ToString()
        {
            return Description;
        }

    }
}

