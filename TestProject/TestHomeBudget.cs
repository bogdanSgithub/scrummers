using System;
using Xunit;
using System.IO;
using System.Collections.Generic;
using Budget;

namespace BudgetCodeTests
{
    [Collection("Sequential")]
    public class TestHomeBudget
    {
        [Fact]
        public void TestNewDBIsCreated()
        {
            //Arrange
            string invalidInput = "testingInvalidDB" + new Random().Next() + ".db";
            while (File.Exists(invalidInput))
            {
                invalidInput = "testingInvalidDB" + new Random().Next();
            }

            //Act
            HomeBudget homeBudget = new HomeBudget(invalidInput, true);

            //Assert
            Assert.True(File.Exists(invalidInput));

            //Clean up
            File.Delete(invalidInput);
        }
    }
}

