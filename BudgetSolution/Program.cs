using Budget;
using System;
using System.Runtime.CompilerServices;

// docfx "C:\Users\2276038\Source\Repos\scrummers\BudgetSolution\docfx.json" --serve

namespace BudgetSolution
{
    internal class Program
    {
        string budgetFile = "C:\\Users\\ALI\\source\\repos\\bogdanSgithub\\scrummers\\TestDatabase\\messy.db";
        DateTime? Start;
        DateTime? End;
        bool FilterFlag;
        int CategoryID;
        string chosenReport;
        HomeBudget homeBudget;

        static void Main(string[] args)
        {
            Program program = new Program();
        }

        public Program()
        {

            homeBudget = new HomeBudget(budgetFile);
            if (GetParameters())
            {
                GetMethodChoice();
                CallReport();
            }
        }

        public void TestDatabase()
        {   
            string filename = "..\\..\\..\\plzWork.db";
            Database.existingDatabase(filename);
            Expenses expenses = new Expenses();
            expenses.Add(DateTime.Now, 2, 10, "Old Description");
            foreach (Expense expense in expenses.List())
            {
                Console.WriteLine(expense);
            }
            //Database.existingDatabase(filename);
        }

        public DateTime? GetValidDate(string message)
        {
            DateTime? validDate = null;
            Console.WriteLine(message);
            string input = Console.ReadLine();
            if (!string.IsNullOrEmpty(input) && DateTime.TryParse(input, out DateTime result))
                validDate = result;
            return validDate;
        }

        public bool GetParameters()
        {
            bool isValid = true;

            Start = GetValidDate("Provide Start Date: ");
            End = GetValidDate("Provide End Date: ");

            try
            {
                Console.WriteLine("Provide Filter Flag: ");
                FilterFlag = bool.Parse(Console.ReadLine());

                Console.WriteLine("Provide CategoryID: ");
                CategoryID = int.Parse(Console.ReadLine());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                isValid = false;
            }
            return isValid;
        }

        public void GetMethodChoice()
        {
            Console.WriteLine("Which Report do you wish to have?");
            Console.WriteLine("i) GetBudgetItems\nii) GetBudgetItemsByMonth\niii) GetBudgetItemsByCategory\niv) GetBudgetDictionaryByCategoryAndMonth");
            Console.WriteLine("Your Choice (ex: ii ): ");
            chosenReport = Console.ReadLine();

        }

        public void CallReport()
        {
            switch (chosenReport)
            {
                case "i":
                    var budgetItems = homeBudget.GetBudgetItems(Start, End, FilterFlag, CategoryID);
                    foreach (var bi in budgetItems)
                        Console.WriteLine(String.Format("{0} {1,-20} {2,8:C} {3,12:C}", bi.Date.ToString("yyyy/MMM/dd"), bi.ShortDescription, bi.Amount, bi.Balance));

                    break;
                case "ii":
                    var budgetItemsByMonths = homeBudget.GetBudgetItemsByMonth(Start, End, FilterFlag, CategoryID);
                    foreach (var budgetItemsByMonth in budgetItemsByMonths)
                    {
                        Console.WriteLine($"Month: {budgetItemsByMonth.Month}");
                        Console.WriteLine($"Total: {budgetItemsByMonth.Total:C}");
                        Console.WriteLine("Details:");
                        foreach (var bi in budgetItemsByMonth.Details)
                        {
                            Console.WriteLine(
                                String.Format("{0} {1,-20} {2,8:C} {3,12:C}",
                                bi.Date.ToString("yyyy/MMM/dd"),
                                bi.ShortDescription,
                                bi.Amount, bi.Balance)
                            );
                        }

                        Console.WriteLine("----------------------------------------");
                    }
                    break;
                case "iii":
                    var budgetItemsByCategories = homeBudget.GetBudgetItemsByCategory(Start, End, FilterFlag, CategoryID);
                    foreach (var budgetItemsByCategory in budgetItemsByCategories)
                    {
                        Console.WriteLine($"Category: {budgetItemsByCategory.Category}");
                        Console.WriteLine($"Total: {budgetItemsByCategory.Total:C}");
                        Console.WriteLine("Details:");
                        foreach (var bi in budgetItemsByCategory.Details)
                        {
                            Console.WriteLine(
                                String.Format("{0} {1,-20} {2,8:C} {3,12:C}",
                                bi.Date.ToString("yyyy/MMM/dd"),
                                bi.ShortDescription,
                                bi.Amount, bi.Balance)
                            );
                        }
                        Console.WriteLine("----------------------------------------");
                    }
                    break;
                case "iv":
                    List<Dictionary<string, object>> budgetData = homeBudget.GetBudgetDictionaryByCategoryAndMonth(Start, End, FilterFlag, CategoryID);
                    foreach (var monthRecord in budgetData)
                    {
                        Console.WriteLine($"Month: {monthRecord["Month"]}");

                        foreach (var key in monthRecord.Keys)
                        {
                            if (key.StartsWith("details:"))
                            {
                                var categoryItems = monthRecord[key] as List<BudgetItem>;
                                Console.WriteLine($"Category Items: {key}");
                                foreach (var bi in categoryItems)
                                {
                                    Console.WriteLine(
                                          String.Format("{0} {1,-20} {2,8:C} {3,12:C}",
                                          bi.Date.ToString("yyyy/MMM/dd"),
                                          bi.ShortDescription,
                                          bi.Amount, bi.Balance)
                                      );
                                }
                            }
                        }
                    }
                    break;
            }
        }
    }
}