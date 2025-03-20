using System.Globalization;

namespace Budget
{
    internal class Program
    {
        static void Main(string[] args)
        {
            HomeBudget budget = new HomeBudget("test.budget");
            const int BudgetItems = 1, BudgetItemsMonth = 2, BudgetItemsCategory = 3, BudgetItemCategoryMonth = 4, Exit = 5;
            int userInput = 0;

            while (userInput != Exit)
            {
                Console.WriteLine("Welcome to the budget testing app. Please choose one of the below options:\n\n1. Get Budget Items.\n2. Get Budget Items by Month.\n3. Get Budget Items by Category.\n4. Get Budget Items by Category and Month.\n5. Exit\n\nPlease enter your choice: ");

                userInput = GetIntInput(Exit, BudgetItems);

                switch (userInput)
                {
                    case BudgetItems:
                        GetBudgetItemsTable(budget);
                        break;

                    case BudgetItemsMonth:
                        GetBudgetItemsMonthTable(budget);
                        break;

                    case BudgetItemsCategory:
                        GetBudgetItemsCategoryTable(budget);
                        break;

                    case BudgetItemCategoryMonth:
                        GetBudgetItemCategoryMonthTable(budget);
                        break;

                    case Exit:
                        Environment.Exit(0);
                        break;

                }
            }


        }

        static void GetBudgetItemCategoryMonthTable(HomeBudget budget)
        {
            const int StringSpacing = -20;
            const int IntSpacing = -10;

            DateTime? startDate = null;
            DateTime? endDate = null;
            bool filterFlag = false;
            int categoryID = 0;

            GetUserOptions(ref startDate, ref endDate, ref filterFlag, ref categoryID);

            List<Dictionary<string, object>> items = budget.GetBudgetDictionaryByCategoryAndMonth(startDate, endDate, filterFlag, categoryID);

            Dictionary<string, object> totalDict = items[items.Count - 1];

            items.Remove(totalDict);

            Console.Clear();

            Console.WriteLine("=========================================================================");
            Console.WriteLine("|   Amount   |         Date         | Expense ID |     Description      |");
            Console.WriteLine("=========================================================================");


            foreach (Dictionary<string, object> item in items)
            {
                string category = "";

                foreach (string key in totalDict.Keys)
                {
                    if (item.ContainsKey(key))
                        category = key;
                }

                Console.WriteLine($"Month: {item["Month"]}, Category: {category}");

                Console.WriteLine("=========================================================================");
                foreach (BudgetItem budgetItem in item[$"details:{category}"] as List<BudgetItem>)
                {
                    Console.WriteLine($"| {budgetItem.Amount,IntSpacing} | {budgetItem.Date.ToString("yyyy/MM/dd"),StringSpacing} | {budgetItem.ExpenseID,IntSpacing} | {budgetItem.ShortDescription,StringSpacing} |");
                }
                Console.WriteLine("=========================================================================");

            }

            Console.WriteLine("Press any key to continue.");

            Console.ReadKey();

            Console.Clear();
        }

        static void GetBudgetItemsTable(HomeBudget budget)
        {
            DateTime? startDate = null;
            DateTime? endDate = null;
            bool filterFlag = false;
            int categoryID = 0;

            GetUserOptions(ref startDate, ref endDate, ref filterFlag, ref categoryID);

            List<BudgetItem> items = budget.GetBudgetItems(startDate, endDate, filterFlag, categoryID);

            DisplayItems(items);
        }

        static void GetBudgetItemsMonthTable(HomeBudget budget)
        {
            DateTime? startDate = null;
            DateTime? endDate = null;
            bool filterFlag = false;
            int categoryID = 0;

            GetUserOptions(ref startDate, ref endDate, ref filterFlag, ref categoryID);

            List<BudgetItemsByMonth> items = budget.GetBudgetItemsByMonth(startDate, endDate, filterFlag, categoryID);

            DisplayItemsByMonthTable(items);
        }

        static void GetBudgetItemsCategoryTable(HomeBudget budget)
        {
            DateTime? startDate = null;
            DateTime? endDate = null;
            bool filterFlag = false;
            int categoryID = 0;

            GetUserOptions(ref startDate, ref endDate, ref filterFlag, ref categoryID);

            List<BudgetItemsByCategory> items = budget.GetBudgetItemsByCategory(startDate, endDate, filterFlag, categoryID);

            DisplayItemsByCategoryTable(items);
        }

        static void DisplayItemsByCategoryTable(List<BudgetItemsByCategory> items)
        {
            const int StringSpacing = -20;
            const int IntSpacing = -10;
            Console.Clear();

            Console.WriteLine("================================================================================================");
            Console.WriteLine("|       Category       |   Amount   |         Date         | Expense ID |     Description      |");
            Console.WriteLine("================================================================================================");

            foreach (BudgetItemsByCategory item in items)
            {
                Console.WriteLine();
                Console.WriteLine($"For {item.Category}, Total: {item.Total}");
                Console.WriteLine();

                Console.WriteLine("================================================================================================");
                foreach (BudgetItem budgetItem in item.Details)
                {
                    Console.WriteLine($"| {budgetItem.Category,StringSpacing} | {budgetItem.Amount,IntSpacing} | {budgetItem.Date.ToString("yyyy/MM/dd"),StringSpacing} | {budgetItem.ExpenseID,IntSpacing} | {budgetItem.ShortDescription,StringSpacing} |");

                }
                Console.WriteLine("================================================================================================");

            }

            Console.WriteLine("Press enter to continue.");

            Console.ReadKey();

            Console.Clear();
        }

        static void DisplayItemsByMonthTable(List<BudgetItemsByMonth> items)
        {
            const int StringSpacing = -20;
            const int IntSpacing = -10;
            Console.Clear();

            Console.WriteLine("================================================================================================");
            Console.WriteLine("|       Category       |   Amount   |         Date         | Expense ID |     Description      |");
            Console.WriteLine("================================================================================================");

            foreach (BudgetItemsByMonth item in items)
            {
                Console.WriteLine();
                Console.WriteLine($"For {item.Month}, Total: {item.Total}");
                Console.WriteLine();

                Console.WriteLine("================================================================================================");
                foreach (BudgetItem budgetItem in item.Details)
                {
                    Console.WriteLine($"| {budgetItem.Category,StringSpacing} | {budgetItem.Amount,IntSpacing} | {budgetItem.Date.ToString("yyyy/MM/dd"),StringSpacing} | {budgetItem.ExpenseID,IntSpacing} | {budgetItem.ShortDescription,StringSpacing} |");

                }
                Console.WriteLine("================================================================================================");

            }

            Console.WriteLine("Press enter to continue.");

            Console.ReadKey();

            Console.Clear();
        }

        static void GetUserOptions(ref DateTime? startDate, ref DateTime? endDate, ref bool filterFlag, ref int categoryID)
        {
            Console.Clear();

            Console.WriteLine("What date should the budget items start from? (YYYY/MM/DD) (leave empty if you don't want to filter by date): ");

            startDate = GetValidDate();

            if (startDate.HasValue)
            {
                Console.WriteLine("What date should the budget items end from? (YYYY/MM/DD): ");

                endDate = GetValidDate();
            }

            Console.WriteLine("Do you want to only see a single category (true/false)?: ");

            filterFlag = GetFlag();

            if (filterFlag)
            {
                Console.WriteLine("What is the ID of the category you want to see?: ");

                categoryID = GetIntInput();
            }
        }

        static void DisplayItems(List<BudgetItem> items)
        {
            const int StringSpacing = -20;
            const int IntSpacing = -10;
            Console.Clear();

            Console.WriteLine("================================================================================================");
            Console.WriteLine("|       Category       |   Amount   |         Date         | Expense ID |     Description      |");
            Console.WriteLine("================================================================================================");

            foreach (BudgetItem item in items)
            {
                Console.WriteLine($"| {item.Category,StringSpacing} | {item.Amount,IntSpacing} | {item.Date.ToString("yyyy/MM/dd"),StringSpacing} | {item.ExpenseID,IntSpacing} | {item.ShortDescription,StringSpacing} |");
            }

            Console.WriteLine("================================================================================================");

            Console.WriteLine("Press enter to continue.");

            Console.ReadKey();

            Console.Clear();
        }

        static bool GetFlag()
        {
            bool flag = false;

            while (!bool.TryParse(Console.ReadLine(), out flag))
            {
                Console.WriteLine("Invalid format for flag. (true/false) Please try again: ");
            }

            return flag;
        }

        static DateTime? GetValidDate()
        {
            DateTime date;
            string input = Console.ReadLine();

            while (!DateTime.TryParse(input, out date))
            {
                if (string.IsNullOrEmpty(input))
                {
                    return null;
                }

                Console.WriteLine("Date format invalid. (MM-DD-YYYY) Please try again: ");

                input = Console.ReadLine();
            }

            return date;
        }

        static int GetIntInput(int upperLimit = int.MaxValue, int lowerLimit = int.MinValue)
        {
            int userInput;

            while (!int.TryParse(Console.ReadLine(), out userInput) || userInput > upperLimit || userInput < lowerLimit)
            {
                Console.WriteLine("Please provide a valid integer greater than 0: ");
            }

            return userInput;
        }
    }
}
