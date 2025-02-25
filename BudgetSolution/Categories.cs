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

        public Categories(SQLiteConnection conn, bool newDB)
        {

        }

        public void UpdateProperties(int id, string newDescr, Category.CategoryType type)
        {

        }

        // ====================================================================
        // populate categories from a file
        // if filepath is not specified, read/save in AppData file
        // Throws System.IO.FileNotFoundException if file does not exist
        // Throws System.Exception if cannot read the file correctly (parsing XML)
        // ====================================================================
        /// <summary>
        /// Populates the Categories list by reading from the specified file and also saves the filename and directory name.
        /// If no filepath is provided, the default file in the AppData directory is used.
        /// </summary>
        /// <param name="filepath">The path to the file from which categories will be read. If null, defaults to the AppData file.</param>
        /// <exception cref="System.IO.FileNotFoundException">Thrown by <see cref="BudgetFiles.VerifyReadFromFileName"/> if the specified file does not exist.</exception>
        /// <exception cref="System.Exception">Thrown if the file cannot be read correctly, such as if there is an issue with XML parsing.</exception>
        /// <example>
        /// <code>
        /// Categories categories = new Categories();
        /// categories.ReadFromFile("C:\\Users\\studentID\\Desktop\\Scrummers\\BudgetSolution\\test_categories.cats");
        /// </code>
        /// </example>
        public void ReadFromFile(String filepath = null)
        {

            // ---------------------------------------------------------------
            // reading from file resets all the current categories,
            // ---------------------------------------------------------------
            _Cats.Clear();

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
            // If file exists, read it
            // ---------------------------------------------------------------
            _ReadXMLFile(filepath);
            _DirName = Path.GetDirectoryName(filepath);
            _FileName = Path.GetFileName(filepath);
        }

        // ====================================================================
        // save to a file
        // if filepath is not specified, read/save in AppData file
        // ====================================================================
        /// <summary>
        /// Saves the current categories to the specified file and also saves the filename and directory name.
        /// If no filepath is provided, the default file in the AppData directory is used.
        /// </summary>
        /// <param name="filepath">The path to the file where categories will be saved. If null, defaults to the last read file or AppData file.</param>
        /// <exception cref="System.IO.FileNotFoundException">Thrown by <see cref="BudgetFiles.VerifyWriteToFileName"/> if the specified file does not exist or cannot be accessed for writing.</exception>
        /// <exception cref="System.Exception">Thrown if the file cannot be saved correctly, such as if there is an issue with XML writing.</exception>
        /// <example>
        /// <code>
        /// Categories categories = new Categories();
        /// categories.SaveToFile("C:\\Users\\studentID\\Desktop\\Scrummers\\BudgetSolution\\test_categories.cats");
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
            _Cats.Add(new Category(new_num, desc, type));
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
                _Cats.RemoveAt(i);
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

        // ====================================================================
        // read from an XML file and add categories to our categories list 
        // ====================================================================
        private void _ReadXMLFile(String filepath)
        {

            // ---------------------------------------------------------------
            // read the categories from the xml file, and add to this instance
            // ---------------------------------------------------------------
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(filepath);

                foreach (XmlNode category in doc.DocumentElement.ChildNodes)
                {
                    String id = (((XmlElement)category).GetAttributeNode("ID")).InnerText;
                    String typestring = (((XmlElement)category).GetAttributeNode("type")).InnerText;
                    String desc = ((XmlElement)category).InnerText;

                    Category.CategoryType type;
                    switch (typestring.ToLower())
                    {
                        case "income":
                            type = Category.CategoryType.Income;
                            break;
                        case "expense":
                            type = Category.CategoryType.Expense;
                            break;
                        case "credit":
                            type = Category.CategoryType.Credit;
                            break;
                        case "savings":
                            type = Category.CategoryType.Savings;
                            break;
                        default:
                            type = Category.CategoryType.Expense;
                            break;
                    }
                    this.Add(new Category(int.Parse(id), desc, type));
                }

            }
            catch (Exception e)
            {
                throw new Exception("ReadXMLFile: Reading XML " + e.Message);
            }

        }


        // ====================================================================
        // write all categories in our list to XML file
        // ====================================================================
        private void _WriteXMLFile(String filepath)
        {
            try
            {
                // create top level element of categories
                XmlDocument doc = new XmlDocument();
                doc.LoadXml("<Categories></Categories>");

                // foreach Category, create an new xml element
                foreach (Category cat in _Cats)
                {
                    XmlElement ele = doc.CreateElement("Category");
                    XmlAttribute attr = doc.CreateAttribute("ID");
                    attr.Value = cat.Id.ToString();
                    ele.SetAttributeNode(attr);
                    XmlAttribute type = doc.CreateAttribute("type");
                    type.Value = cat.Type.ToString();
                    ele.SetAttributeNode(type);

                    XmlText text = doc.CreateTextNode(cat.Description);
                    doc.DocumentElement.AppendChild(ele);
                    doc.DocumentElement.LastChild.AppendChild(text);

                }

                // write the xml to FilePath
                doc.Save(filepath);

            }
            catch (Exception e)
            {
                throw new Exception("_WriteXMLFile: Reading XML " + e.Message);
            }

        }

    }
}

