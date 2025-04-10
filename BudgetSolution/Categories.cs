using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Data.SQLite;
using System.Xml.Linq;

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
    /// Object that implements all operations for the categories database.
    /// </summary>
    public class Categories
    {

        /// <summary>
        /// Given an int id, it looks for and returns the corresponding Category in the database that has that id.
        /// </summary>
        /// <param name="i">the id of the wanted category</param>
        /// <returns>The matching category.</returns>
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
            //select the matching category
            SQLiteCommand cmd = new SQLiteCommand(Database.dbConnection);
            cmd.CommandText = "SELECT Id, Description, TypeId FROM categories WHERE Id = @id;";
            cmd.Parameters.AddWithValue("@id", i);


            SQLiteDataReader rdr = cmd.ExecuteReader();

            Category category = null;

            //change the fields of the category if a match is found
            while (rdr.Read())
            {
                category = new Category(rdr.GetInt32(0), rdr.GetString(1), (Category.CategoryType)rdr.GetInt32(2) - 1);
            }

            if (category == null)
            {
                throw new Exception("Cannot find category with id " + i.ToString());
            }

            return category;
        }

        /// <summary>
        /// Constructor that initializes the database.
        /// </summary>
        /// <param name="newDB">A flag that represents if a new database should be created or not.</param>
        public Categories(SQLiteConnection conn, bool newDB)
        {
            
        }
        /// <summary>
        /// Updates a matching category within the database.
        /// </summary>
        /// <param name="id">The ID of the category to update.</param>
        /// <param name="newDescr">The new description of the category.</param>
        /// <param name="type">The new type of the category.</param>
        /// <example>
        /// <code>
        /// //Initialize categories, sets categories to defaults.
        /// Categories example = new Categories(Database.dbConnection);
        /// 
        /// //The first category will have the new fields.
        /// example.UpdateProperties(1, "New Description", Category.CategoryType.Income);
        /// </code>
        /// </example>
        public void UpdateProperties(int id, string newDescr, Category.CategoryType type)
        {
            SQLiteCommand cmd = new SQLiteCommand(Database.dbConnection);

            cmd.CommandText = $"UPDATE categories SET Description = @description, TypeId = @type WHERE Id = @id;";
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@description", newDescr);
            cmd.Parameters.AddWithValue("@type", (int)type + 1);
            cmd.Prepare();
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Adds a new category with the specified description and type to the category database.
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
            int newID = 0;

            SQLiteCommand cmd = new SQLiteCommand(Database.dbConnection);

            cmd.CommandText = "SELECT MAX(Id) FROM categories;";
            object result = cmd.ExecuteScalar();

            if (result != DBNull.Value) 
                newID = int.Parse(result.ToString());

            Category newCategory = new Category(newID + 1, desc, type);
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
        /// Tries to remove the category with the specified ID from the category database.
        /// If there isn't a Category in the database with the given ID then it isn't removed.
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
            SQLiteCommand cmd = new SQLiteCommand(Database.dbConnection);
            cmd.CommandText = $"DELETE FROM categories WHERE Id = @id;";
            cmd.Parameters.AddWithValue("@id", Id);
            cmd.Prepare();
            cmd.ExecuteNonQuery();
        }

        // ====================================================================
        // Return list of categories
        // ====================================================================
        /// <summary>
        /// Returns a new list containing copies of all categories in the category database.
        /// </summary>
        /// <returns>A new list of categories, where each category is a copy of the Category objects in the database.</returns>
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
            SQLiteCommand cmd = new SQLiteCommand(Database.dbConnection);
            cmd.CommandText = "SELECT Id, Description, TypeId FROM categories;";
            SQLiteDataReader rdr = cmd.ExecuteReader();
            List<Category> categoryList = new List<Category>();

            while (rdr.Read())
            {
                categoryList.Add(new Category(rdr.GetInt32(0), rdr.GetString(1), (Category.CategoryType)rdr.GetInt32(2) - 1));
            }

            return categoryList;
        }
    }
}

