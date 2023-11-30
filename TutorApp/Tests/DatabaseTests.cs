using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using TutorApp.Models;
using TutorApp.Services;

namespace Tests
{
    [TestClass]
    public class DatabaseTests
    {
        #region Meeting Tests
        [TestMethod]
        public async Task Metting_Add_ReturnsTrue()
        {
            // Arrange
            DatabaseClient database = DatabaseClient.GetInstance();
            Meeting meeting = new Meeting()
            {
                TutorProfileID = database.DeviceID,
                StudentProfileID = database.DeviceID,
                Name = "UnitTest",
                Role = "Student",
                Availability = "Scheduled",
                Subject = "Math",
                StudentName = "UnitTest",
                StartTime = DateTime.Now.ToString(),
                EndTime = DateTime.Now.AddHours(1).ToString()
            };

            // Act
            await database.AddMeeting(meeting);
            var response = await database.GetMeetings();
            bool result = response.ContainsKey(meeting.ID);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task Metting_Remove_ReturnsTrue()
        {
            // Arrange
            DatabaseClient database = DatabaseClient.GetInstance();
            Meeting toRemove = new Meeting();
            Meeting meeting = new Meeting()
            {
                TutorProfileID = database.DeviceID,
                StudentProfileID = database.DeviceID,
                Name = "UnitTest",
                Role = "Student",
                Availability = "Scheduled",
                Subject = "Math",
                StudentName = "UnitTest",
                StartTime = DateTime.Now.ToString(),
                EndTime = DateTime.Now.AddHours(1).ToString()
            };

            // Act
            var response = await database.GetMeetings();
            foreach (var meetingResonse in response)
            {
                if (meetingResonse.Value.Name.Equals(meeting.Name))
                {
                    toRemove = meetingResonse.Value;
                    break;
                }
            }

            bool result = false;
            if (string.IsNullOrEmpty(toRemove.Name) == false)
            {
                await database.RemoveMeeting(toRemove);
                response = await database.GetMeetings();
                result = response.ContainsKey(toRemove.ID) == false;
            }

            // Assert
            Assert.IsTrue(result);
        }
        #endregion

        #region Profile Tests
        [TestMethod]
        public async Task Profile_Add_ReturnsTrue()
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
        public async Task Profile_Get_ReturnsTrue()
        {
            // Arrange
            DatabaseClient database = DatabaseClient.GetInstance();
            Profile expected = new Profile() { FirstName = "FirstName", LastName = "LastName", ID = database.DeviceID };
            Profile actual = new Profile();

            // Act
            if (await database.HasUserProfile())
                actual = database.User;

            // Assert
            Assert.AreEqual(expected.FirstName, actual.FirstName);
            Assert.AreEqual(expected.LastName, actual.LastName);
            Assert.AreEqual(expected.ID, actual.ID);
        }

        [TestMethod]
        public async Task Profile_Remove_ReturnsFalse()
        {
            // Arrange
            DatabaseClient database = DatabaseClient.GetInstance();

            // Act
            await database.RemoveProfile();
            bool result = await database.HasUserProfile();

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Profile_Removed_ReturnsAreNotEqual()
        {
            // This test must run after Profile_Remove_ReturnsFalse().
            // Otherwise it will not have been removed.

            // Arrange
            DatabaseClient database = DatabaseClient.GetInstance();
            Profile notExpected = new Profile() { FirstName = "FirstName", LastName = "LastName", ID = database.DeviceID };
            Profile actual = database.User;

            // Assert
            Assert.AreNotEqual(notExpected.FirstName, actual.FirstName);
            Assert.AreNotEqual(notExpected.LastName, actual.LastName);
        }
        #endregion

        #region Rating Tests
        [TestMethod]
        public async Task Rating_Add_ReturnsTrue()
        {
            // Arrange
            DatabaseClient database = DatabaseClient.GetInstance();
            MeetingRating rating = new MeetingRating()
            {
                TutorProfileID = database.DeviceID,
                StudentProfileID = database.DeviceID,
                Stars = 1,
                Comment = "Unit testing the add rating method in DatabaseClient",
                TutorName = "FirstName LastName"
            };

            // Act
            await database.AddRating(rating);
            var response = await database.GetRatings();
            bool result = response.ContainsKey(rating.ID);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task Rating_Remove_ReturnsTrue()
        {
            // Arrange
            DatabaseClient database = DatabaseClient.GetInstance();
            MeetingRating toRemove = new MeetingRating();
            MeetingRating rating = new MeetingRating()
            {
                TutorProfileID = database.DeviceID,
                StudentProfileID = database.DeviceID,
                Stars = 1,
                Comment = "Unit testing the add rating method in DatabaseClient",
                TutorName = "FirstName LastName"
            };

            // Act
            var response = await database.GetRatings();
            foreach (var ratingResponse in response)
            {
                if (ratingResponse.Value.TutorName.Equals(rating.TutorName))
                {
                    toRemove = ratingResponse.Value;
                    break;
                }
            }

            bool result = false;
            if (string.IsNullOrEmpty(toRemove.ID) == false)
            {
                await database.RemoveRating(toRemove);
                response = await database.GetRatings();
                result = response.ContainsKey(toRemove.ID) == false;
            }

            // Assert
            Assert.IsTrue(result);
        }
        #endregion
    }
}
