﻿using System;
using Xunit;
using System.IO;
using System.Collections.Generic;
using Budget;
using System.Data.SQLite;

namespace BudgetCodeTests
{
    [Collection("Sequential")]
    public class TestExpenses
    {
        int numberOfExpensesInFile = TestConstants.numberOfExpensesInFile;
        String testInputFile = TestConstants.testExpensesInputFile;
        int maxIDInExpenseFile = TestConstants.maxIDInExpenseFile;
        Expense firstExpenseInFile = new Expense(1, new DateTime(2021, 1, 10), 10, 12, "hat (on credit)");


        // ========================================================================

        [Fact]
        public void ExpensesObject_New()
        {
            // Arrange

            // Act
            Expenses expenses = new Expenses();

            // Assert
            Assert.IsType<Expenses>(expenses);
        }


        // ========================================================================
        /*
        [Fact]
        public void ExpensesMethod_ReadFromFile_NotExist_ThrowsException()
        {
            // Arrange
            String badFile = "abc.txt";
            Expenses expenses = new Expenses();

            // Act and Assert
            Assert.Throws<System.IO.FileNotFoundException>(() => expenses.ReadFromFile(badFile));

        }*/

        // ========================================================================

        /*[Fact]
        public void ExpensesMethod_ReadFromFile_ValidateCorrectDataWasRead()
        {
            // Arrange
            String dir = TestConstants.GetSolutionDir();
            Expenses expenses = new Expenses();

            // Act
            expenses.ReadFromFile(dir + "\\" + testInputFile);
            List<Expense> list = expenses.List();
            Expense firstExpense = list[0];

            // Assert
            Assert.Equal(numberOfExpensesInFile, list.Count);
            Assert.Equal(firstExpenseInFile.Id, firstExpense.Id);
            Assert.Equal(firstExpenseInFile.Amount, firstExpense.Amount);
            Assert.Equal(firstExpenseInFile.Description, firstExpense.Description);
            Assert.Equal(firstExpenseInFile.Category, firstExpense.Category);

            String fileDir = Path.GetFullPath(Path.Combine(expenses.DirName, ".\\"));
            Assert.Equal(dir, fileDir);
            Assert.Equal(testInputFile, expenses.FileName);

        }*/

        // ========================================================================

        [Fact]
        public void ExpensesMethod_List_ReturnsListOfExpenses()
        {
            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{folder}\\messy.db";
            System.IO.File.Copy(goodDB, messyDB, true);
            Database.existingDatabase(messyDB);
            SQLiteConnection conn = Database.dbConnection;
            Expenses expenses = new Expenses();
            DateTime date = DateTime.Now;
            string descr = "New Expense";

            // should be 6 before adding anything
            List<Expense> expensesList = expenses.List();
            Assert.Equal(expensesList.Count, numberOfExpensesInFile);

            // Act
            expenses.Add(date, 1, 10, descr);
            expensesList = expenses.List();
            int sizeOfList = expensesList.Count;

            // Assert
            Assert.Equal(numberOfExpensesInFile + 1, sizeOfList);
            Assert.Equal(descr, expensesList[sizeOfList - 1].Description);

        }

        // ========================================================================

        //DEPRECATED TEST
        //[Fact]
        //public void ExpensesMethod_List_ModifyListDoesNotModifyExpensesInstance()
        //{
        //    // Arrange
        //    String dir = TestConstants.GetSolutionDir();
        //    Expenses expenses = new Expenses();
        //    expenses.ReadFromFile(dir + "\\" + testInputFile);
        //    List<Expense> list = expenses.List();

        //    // Act
        //    list[0].Amount = list[0].Amount + 21.03;

        //    // Assert
        //    Assert.NotEqual(list[0].Amount, expenses.List()[0].Amount);

        //}

        // ========================================================================

        // Deprecated

        //[Fact]

        //public void ExpensesMethod_Add()
        //{
        //    // Arrange
        //    String dir = TestConstants.GetSolutionDir();
        //    Expenses expenses = new Expenses();
        //    expenses.ReadFromFile(dir + "\\" + testInputFile);
        //    int category = 1;
        //    double amount = 98.1;

        //    // Act
        //    expenses.Add(DateTime.Now,category,amount,"new expense");
        //    List<Expense> expensesList = expenses.List();
        //    int sizeOfList = expenses.List().Count;

        //    // Assert
        //    Assert.Equal(numberOfExpensesInFile+1, sizeOfList);
        //    Assert.Equal(maxIDInExpenseFile + 1, expensesList[sizeOfList - 1].Id);
        //    Assert.Equal(amount, expensesList[sizeOfList - 1].Amount);

        //}

        [Fact]
        public void ExpensesMethod_AddValid()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{folder}\\messy.db";
            System.IO.File.Copy(goodDB, messyDB, true);
            Database.existingDatabase(messyDB);
            SQLiteConnection conn = Database.dbConnection;
            Expenses expenses = new Expenses();
            DateTime date = DateTime.Now;
            string descr = "New Expense";
            Category.CategoryType type = Category.CategoryType.Income;

            // Act
            expenses.Add(date, 1, 10, descr);
            List<Expense> expensesList = expenses.List();
            int sizeOfList = expensesList.Count;

            // Assert
            Assert.Equal(numberOfExpensesInFile + 1, sizeOfList);
            Assert.Equal(descr, expensesList[sizeOfList - 1].Description);

        }

        [Fact]
        public void ExpensesMethod_AddInvalidDescription()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{folder}\\messy.db";
            System.IO.File.Copy(goodDB, messyDB, true);
            Database.existingDatabase(messyDB);
            SQLiteConnection conn = Database.dbConnection;
            Expenses expenses = new Expenses();
            DateTime date = DateTime.Now;
            string descr = "";

            try
            {
                expenses.Add(date, 1, 10, descr);
            }
            catch
            {
                Assert.True(true);
            }
        }

        // ========================================================================

        [Fact]
        public void ExpensesMethod_UpdateCategory()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String newDB = $"{folder}\\newDB.db";
            Database.newDatabase(newDB);
            SQLiteConnection conn = Database.dbConnection;
            Expenses expenses = new Expenses();
            String newDescr = "New Description";
            DateTime newDate = DateTime.Now;
            int id = 1;
            int newCategory = 9;
            int newAmount = 20;

            expenses.Add(DateTime.Now, 2, 10, "Old Description");

            // Act
            expenses.UpdateProperties(id, newDate, newCategory, newAmount, newDescr);
            Expense expense = expenses.GetExpenseFromId(id);

            // Assert 
            Assert.Equal(newDate, expense.Date);
            Assert.Equal(newDescr, expense.Description);
            Assert.Equal(newCategory, expense.Category);
            Assert.Equal(newAmount, expense.Amount);
        }

        [Fact]
        public void ExpensesMethod_Delete()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{folder}\\messy.db";
            System.IO.File.Copy(goodDB, messyDB, true);
            Database.existingDatabase(messyDB);
            SQLiteConnection conn = Database.dbConnection;
            Expenses expenses = new Expenses();
            int IdToDelete = 3;

            // Act
            expenses.Delete(IdToDelete);
            List<Expense> expensesList = expenses.List();
            int sizeOfList = expensesList.Count;

            // Assert
            Assert.Equal(numberOfExpensesInFile - 1, sizeOfList);
            Assert.False(expensesList.Exists(e => e.Id == IdToDelete), "correct expense item deleted");

        }

        // ========================================================================

        [Fact]
        public void ExpensesMethod_Delete_InvalidIDDoesntCrash()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{folder}\\messyDB";
            System.IO.File.Copy(goodDB, messyDB, true);
            Database.existingDatabase(messyDB);
            SQLiteConnection conn = Database.dbConnection;
            Expenses expenses = new Expenses();
            int IdToDelete = 1006;
            int sizeOfList = expenses.List().Count;

            // Act
            try
            {
                expenses.Delete(IdToDelete);
                Assert.Equal(sizeOfList, expenses.List().Count);
            }

            // Assert
            catch
            {
                Assert.True(false, "Invalid ID causes Delete to break");
            }
        }


        // ========================================================================

        /*[Fact]
        public void ExpenseMethod_WriteToFile()
        {
            // Arrange
            String dir = TestConstants.GetSolutionDir();
            Expenses expenses = new Expenses();
            expenses.ReadFromFile(dir + "\\" + testInputFile);
            string fileName = TestConstants.ExpenseOutputTestFile;
            String outputFile = dir + "\\" + fileName;
            File.Delete(outputFile);

            // Act
            expenses.SaveToFile(outputFile);

            // Assert
            Assert.True(File.Exists(outputFile), "output file created");
            Assert.True(FileEquals(dir + "\\" + testInputFile, outputFile), "Input /output files are the same");
            String fileDir = Path.GetFullPath(Path.Combine(expenses.DirName, ".\\"));
            Assert.Equal(dir, fileDir);
            Assert.Equal(fileName, expenses.FileName);

            // Cleanup
            if (FileEquals(dir + "\\" + testInputFile, outputFile))
            {
                File.Delete(outputFile);
            }

        }
        */
        // ========================================================================

        /*[Fact]
        public void ExpenseMethod_WriteToFile_VerifyNewExpenseWrittenCorrectly()
        {
            // Arrange
            String dir = TestConstants.GetSolutionDir();
            Expenses expenses = new Expenses();
            expenses.ReadFromFile(dir + "\\" + testInputFile);
            string fileName = TestConstants.ExpenseOutputTestFile;
            String outputFile = dir + "\\" + fileName;
            File.Delete(outputFile);

            // Act
            expenses.Add(DateTime.Now, 14, 35.27, "McDonalds");
            List<Expense> listBeforeSaving = expenses.List();
            expenses.SaveToFile(outputFile);
            expenses.ReadFromFile(outputFile);
            List<Expense> listAfterSaving = expenses.List();

            Expense beforeSaving = listBeforeSaving[listBeforeSaving.Count - 1];
            Expense afterSaving = listAfterSaving.Find(e => e.Id == beforeSaving.Id);

            // Assert
            Assert.Equal(beforeSaving.Id, afterSaving.Id);
            Assert.Equal(beforeSaving.Category, afterSaving.Category);
            Assert.Equal(beforeSaving.Description, afterSaving.Description);
            Assert.Equal(beforeSaving.Amount, afterSaving.Amount);

        }*/

        // ========================================================================

        /*
        [Fact]
        public void ExpenseMethod_WriteToFile_WriteToLastFileWrittenToByDefault()
        {
            // Arrange
            String dir = TestConstants.GetSolutionDir();
            Expenses expenses = new Expenses();
            expenses.ReadFromFile(dir + "\\" + testInputFile);
            string fileName = TestConstants.ExpenseOutputTestFile;
            String outputFile = dir + "\\" + fileName;
            File.Delete(outputFile);
            expenses.SaveToFile(outputFile); // output file is now last file that was written to.
            File.Delete(outputFile);  // Delete the file

            // Act
            expenses.SaveToFile(); // should write to same file as before

            // Assert
            Assert.True(File.Exists(outputFile), "output file created");
            String fileDir = Path.GetFullPath(Path.Combine(expenses.DirName, ".\\"));
            Assert.Equal(dir, fileDir);
            Assert.Equal(fileName, expenses.FileName);

            // Cleanup
            if (FileEquals(dir + "\\" + testInputFile, outputFile))
            {
                File.Delete(outputFile);
            }

        }*/

        // ========================================================================



        // -------------------------------------------------------
        // helpful functions, ... they are not tests
        // -------------------------------------------------------

        // source taken from: https://www.dotnetperls.com/file-equals

        private bool FileEquals(string path1, string path2)
        {
            byte[] file1 = File.ReadAllBytes(path1);
            byte[] file2 = File.ReadAllBytes(path2);
            if (file1.Length == file2.Length)
            {
                for (int i = 0; i < file1.Length; i++)
                {
                    if (file1[i] != file2[i])
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }
    }
}
