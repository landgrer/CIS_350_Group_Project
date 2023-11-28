using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Threading.Tasks;
using TutorApp.Models;
using TutorApp.Services;

namespace Tests
{
    [TestClass]
    public class DatabaseTests
    {
        [TestMethod]
        public async Task Add_Profile_ReturnsTrue()
        {
            // Arrange
            DatabaseClient database = DatabaseClient.GetInstance();

            // Act
            await database.AddProfile("FirstName", "LastName");
            bool hasProfile = await database.HasUserProfile();

            // Assert
            Assert.IsTrue(hasProfile);
        }

        [TestMethod]
        public async Task Get_Profile_ReturnsTrue()
        {
            // Arrange
            DatabaseClient database = DatabaseClient.GetInstance();
            Profile expected = new Profile() { FirstName = "FirstName", LastName = "LastName", ID = database.ProfileID };
            Profile actual = new Profile();

            // Act
            if (await database.HasUserProfile())
                actual = database.User;

            // Assert
            Assert.AreEqual(expected.FirstName, actual.FirstName);
            Assert.AreEqual(expected.LastName, actual.LastName);
            Assert.AreEqual(expected.ID, actual.ID);
        }

        //[TestMethod]
        //public async Task Remove_Profile_ReturnsFalse()
        //{
        //    // Arrange
        //    DatabaseClient database = DatabaseClient.GetInstance();
        //    Profile notExpected = new Profile() { FirstName = "FirstName", LastName = "LastName", ID = database.ProfileID };
        //    Profile actual = new Profile();

        //    // Act
        //    await database.RemoveProfile();
        //    Thread.Sleep(4000);
        //    if (await database.HasUserProfile())
        //        actual = database.User;

        //    // Assert
        //    Assert.AreNotEqual(notExpected.FirstName, actual.FirstName);
        //    Assert.AreNotEqual(notExpected.LastName, actual.LastName);
        //}
    }
}
