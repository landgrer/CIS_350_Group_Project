using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;
using TutorApp.Models;
using TutorApp.Services;
using TutorApp.ViewModels;

namespace Tests
{
    [TestClass]
    public class FrontendTests
    {
        #region Profile View Model Tests
        [TestMethod]
        public async Task Profile_Add_ReturnsTrue()
        {
            // Arrange
            bool result = false;
            DatabaseClient database = DatabaseClient.GetInstance();
            ProfileViewModel profileViewModel = new ProfileViewModel();
            await Task.Delay(2000);
            profileViewModel.FirstName = "TesingFirstName";
            profileViewModel.LastName = "TestingLastName";

            // Act
            profileViewModel.SubmitCommand.Execute(this);
            await Task.Delay(2000);
            Dictionary<string, Profile> profiles = await database.GetProfiles();
            foreach (var profile in profiles)
                if (profile.Value.FirstName.Equals("TesingFirstName") && profile.Value.LastName.Equals("TestingLastName"))
                    result = true;

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task Profile_Remove_ReturnsTrue()
        {
            // Arrange
            bool result = true;
            DatabaseClient database = DatabaseClient.GetInstance();
            ProfileViewModel profileViewModel = new ProfileViewModel();
            await Task.Delay(4000);
            profileViewModel.FirstName = "TesingFirstName";
            profileViewModel.LastName = "TestingLastName";

            // Act
            profileViewModel.DeleteCommand.Execute(this);
            await Task.Delay(2000);
            Dictionary<string, Profile> profiles = await database.GetProfiles();
            foreach (var profile in profiles)
                if (profile.Value.FirstName.Equals("TesingFirstName") && profile.Value.LastName.Equals("TestingLastName"))
                    result = false;

            // Assert
            Assert.IsTrue(result);
        }
        #endregion
    }
}
