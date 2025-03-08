using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Data.SQLite;

// ============================================================================
// (c) Sandy Bultena 2018
// * Released under the GNU General Public License
// ============================================================================

namespace Budget
{
    // ====================================================================
    // CLASS: categories
    //        - A collection of category items,
    //        - Read / write to file
    //        - etc
    // ====================================================================
    /// <summary>
    /// Object that has a list of categories
    /// </summary>
    public class Categories
    {
        private static String DefaultFileName = "budgetCategories.txt";
        private List<Category> _Cats = new List<Category>();
        private string _FileName;
        private string _DirName;

        // ====================================================================
        // Properties
        // ====================================================================
        /// <summary>
        /// string FileName representing the name of the file where the categories are saved
        /// </summary>
        public String FileName { get { return _FileName; } }
        /// <description>
        /// string directory name representing the name of the directory where the categories files is
        /// </description>
        public String DirName { get { return _DirName; } }

        // ====================================================================
        // Constructor
        // ====================================================================
        /// <summary>
        /// Default Constructor that calls the SetCategoriesToDefaults
        /// </summary>
        /// <example>
        /// <code>
        /// Categories categories = new Categories();
        /// </code>
        /// </example>
        public Categories()
        {
            SetCategoriesToDefaults();
        }

        // ====================================================================
        // get a specific category from the list where the id is the one specified
        // ====================================================================
        /// <summary>
        /// Given an int id, it looks for and returns the corresponding Category in the Categories list that has that id.
        /// </summary>
        /// <param name="i">the id of the wanted category</param>
        /// <returns>Returns the Category</returns>
        /// <exception cref="Exception">Thrown if there isn't a Category that has that id.</exception>
        /// <example>
        /// <code>
        /// Categories categories = new Categories();
        /// Category category = categories.GetCategoryFromId(1);
        /// // prints the description
        /// Console.WriteLine(category);
        /// </code>
        /// </example>
        public Category GetCategoryFromId(int i)
        {
            Category c = _Cats.Find(x => x.Id == i);
            if (c == null)
            {
                throw new Exception("Cannot find category with id " + i.ToString());
            }
            return c;
        }

        private void PopulateCategoriesList(SQLiteConnection conn)
        {
            _Cats.Clear();
            SQLiteCommand cmd = new SQLiteCommand(conn);

            cmd.CommandText = "SELECT Id, Description, TypeId FROM categories;";
            SQLiteDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                Category category = new Category(rdr.GetInt32(0), rdr.GetString(1), (Category.CategoryType)rdr.GetInt32(2) - 1);
                if (!_Cats.Contains(category))
                    _Cats.Add(category);
            }
        } 

        public Categories(SQLiteConnection conn, bool newDB)
        {
            if (newDB)
            {
                Database.newDatabase("default.db");
                SetInitialCategoryTypes();
                SetCategoriesToDefaults();
            }
            else
            {
                PopulateCategoriesList(conn);
            }
        }

        public void UpdateProperties(int id, string newDescr, Category.CategoryType type)
        {
            SQLiteCommand cmd = new SQLiteCommand(Database.dbConnection);

            cmd.CommandText = $"UPDATE categories SET Id = @id, Description = @description, TypeId = @type WHERE Id = @id;";
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@description", newDescr);
            cmd.Parameters.AddWithValue("@type", (int)type + 1);
            cmd.Prepare();
            cmd.ExecuteNonQuery();

            PopulateCategoriesList(Database.dbConnection);
        }

        private void ClearDBCategories()
        {
            SQLiteCommand cmd = new SQLiteCommand(Database.dbConnection);

            string query = $"DELETE FROM categories;";
            cmd.CommandText = query;
            cmd.ExecuteNonQuery();
        }

        private void SetInitialCategoryTypes()
        {
            SQLiteCommand cmd = new SQLiteCommand(Database.dbConnection);
            // add initial categoryTypes
            foreach (Category.CategoryType type in Enum.GetValues(typeof(Category.CategoryType)))
            {
                string query = $"INSERT INTO categoryTypes (Id, Description) VALUES({(int)type + 1}, '{type}');";
                cmd.CommandText = query;
                cmd.ExecuteNonQuery();
            }
        }

        // ====================================================================
        // set categories to default
        // ====================================================================
        /// <summary>
        /// Resets the current categories and populates the list with some default Category objects.
        /// </summary>
        /// <example>
        /// <code>
        /// Categories categories = new Categories();
        /// categories.SetCategoriesToDefaults();
        /// </code>
        /// </example>
        public void SetCategoriesToDefaults()
        {
            // ---------------------------------------------------------------
            // reset any current categories,
            // ---------------------------------------------------------------
            _Cats.Clear();
            ClearDBCategories();

            // ---------------------------------------------------------------
            // Add Defaults
            // ---------------------------------------------------------------
            Add("Utilities", Category.CategoryType.Expense);
            Add("Rent", Category.CategoryType.Expense);
            Add("Food", Category.CategoryType.Expense);
            Add("Entertainment", Category.CategoryType.Expense);
            Add("Education", Category.CategoryType.Expense);
            Add("Miscellaneous", Category.CategoryType.Expense);
            Add("Medical Expenses", Category.CategoryType.Expense);
            Add("Vacation", Category.CategoryType.Expense);
            Add("Credit Card", Category.CategoryType.Credit);
            Add("Clothes", Category.CategoryType.Expense);
            Add("Gifts", Category.CategoryType.Expense);
            Add("Insurance", Category.CategoryType.Expense);
            Add("Transportation", Category.CategoryType.Expense);
            Add("Eating Out", Category.CategoryType.Expense);
            Add("Savings", Category.CategoryType.Savings);
            Add("Income", Category.CategoryType.Income);
        }

        // ====================================================================
        // Add category
        // ====================================================================
        private void Add(Category cat)
        {
            _Cats.Add(cat);
        }

        /// <summary>
        /// Adds a new category with the specified description and type to the category list.
        /// If there are existing categories, the new category is assigned a unique ID, which is 1 bigger than the highest existing ID.
        /// </summary>
        /// <param name="desc">The description of the category being added.</param>
        /// <param name="type">The type of the category</param>
        /// <example>
        /// <code>
        /// Categories categories = new Categories();
        /// categories.Add("Food", Category.CategoryType.Expense);
        /// </code>
        /// </example>
        public void Add(String desc, Category.CategoryType type)
        {
            int new_num = 1;
            if (_Cats.Count > 0)
            {
                new_num = (from c in _Cats select c.Id).Max();
                new_num++;
            }
            Category newCategory = new Category(new_num, desc, type);
            _Cats.Add(newCategory);
            InsertIntoDB(newCategory);
        }

        private void InsertIntoDB(Category category)
        {
            SQLiteCommand cmd = new SQLiteCommand(Database.dbConnection);

            string query = $"INSERT INTO categories (Id, Description, TypeId) VALUES(@id, @description, @type);";
            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@id", category.Id);
            cmd.Parameters.AddWithValue("@description", category.Description);
            cmd.Parameters.AddWithValue("@type", (int)category.Type + 1);
            cmd.Prepare();
            cmd.ExecuteNonQuery();
        }

        // ====================================================================
        // Delete category
        // ====================================================================

        /// <summary>
        /// Tries to remove the category with the specified ID from the category list.
        /// If there isn't a Category in the list with the given ID then it isn't removed.
        /// </summary>
        /// <param name="Id">The ID of the category to be deleted.</param>
        /// <example>
        /// <code>
        /// Categories categories = new Categories();
        /// // will delete Utilities expense since it was added by SetCategoriesToDefaults which was called in constructor
        /// categories.Delete(1);
        /// </code>
        /// </example>
        public void Delete(int Id)
        {
            int i = _Cats.FindIndex(x => x.Id == Id);
            if (i != -1)
            {
                _Cats.RemoveAt(i);


                SQLiteCommand cmd = new SQLiteCommand(Database.dbConnection);
                cmd.CommandText = $"DELETE FROM categories WHERE Id = @id;";
                cmd.Parameters.AddWithValue("@id", Id);
                cmd.Prepare();
                cmd.ExecuteNonQuery();
            }

        }

        // ====================================================================
        // Return list of categories
        // Note:  make new copy of list, so user cannot modify what is part of
        //        this instance
        // ====================================================================
        /// <summary>
        /// Returns a new list containing copies of all categories in the category list.
        /// </summary>
        /// <returns>A new list of categories, where each category is a copy of the Category objects in the Categories object's list.</returns>
        /// <example>
        /// <code>
        /// Categories categories = new Categories();
        /// categories.Add("Food", Category.CategoryType.Expense);
        /// foreach (Category c in categories.List()) {
        ///    Console.WriteLine(c);
        /// }
        /// </code>
        /// </example>
        public List<Category> List()
        {   

            List<Category> newList = new List<Category>();
            foreach (Category category in _Cats)
            {
                newList.Add(new Category(category));
            }
            return newList;
        }
    }
}

