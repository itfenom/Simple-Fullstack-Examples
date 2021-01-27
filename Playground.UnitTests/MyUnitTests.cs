using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text.RegularExpressions;

namespace Playground.UnitTests
{
    [TestClass]
    public class MyUnitTests
    {
        [TestMethod]
        public void IsNumericTest()
        {
            //Arrange
            var inputVal = "32165498.99";

            //Act
            bool result = double.TryParse(inputVal, out _);

            //Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void AnonymousMethodExample()
        {
            //arrange
            string inputStr = "Dallas";

            //Func<string, string> convertToUpperCase = s => s.ToUpper();
            string ConvertToUpperCase(string s)
            {
                return s.ToUpper();
            }

            //act
            string outputStr = ConvertToUpperCase(inputStr);

            //assert
            Assert.AreEqual(outputStr, "DALLAS", "Failed to uppercase inputString");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ExceptionThrownTest()
        {
            /*This can be done in two ways:
             1:
            //arrange
            //act
            MethodThatThrowsException();
            //assert
            */

            //2:
            try
            {
                MethodThatThrowsException();
            }
            catch (Exception e)
            {
                Assert.AreEqual("Something bad has happened!", e.Message);
                throw;//We still need this so that it matches with the attribute of the method.
            }
        }

        private void MethodThatThrowsException()
        {
            throw new ArgumentException("Something bad has happened!");
        }

        [TestMethod]
        public void ValidateEmailTest()
        {
            var emailAddressToValidate = "this@test.com";
            var regExp = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                           @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                           @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";

            var result = Regex.IsMatch(emailAddressToValidate, regExp);

            Assert.IsTrue(result);
        }
    }
}
