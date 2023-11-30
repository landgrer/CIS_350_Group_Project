using System;
using System.Threading.Tasks;
using TutorApp.Services;
using TutorApp.Views;
using Xamarin.Forms;

namespace TutorApp.ViewModels
{
    public class ProfileViewModel : BaseViewModel
    {
        #region Properties
        public Command SubmitCommand { get; set; }
        public Command DeleteCommand { get; set; }

        private DatabaseClient database = DatabaseClient.GetInstance();

        bool _tabBar = false;
        public bool TabBar
        {
            get { return _tabBar; }
            set { SetProperty(ref _tabBar, value); }
        }

        string _deviceID = string.Empty;
        public string DeviceID
        {
            get { return _deviceID; }
            set { SetProperty(ref _deviceID, value); }
        }

        string _firstName = string.Empty;
        public string FirstName
        {
            get { return _firstName; }
            set { SetProperty(ref _firstName, value); }
        }

        string _lastName = string.Empty;
        public string LastName
        {
            get { return _lastName; }
            set { SetProperty(ref _lastName, value); }
        }
        #endregion

        #region Constructors
        public ProfileViewModel()
        {
            Title = "User Profile";
            SubmitCommand = new Command(OnSubmitClicked);
            DeleteCommand = new Command(OnDeleteClicked);
            DeviceID = database.DeviceID;
        }
        #endregion

        #region Events
        public async void OnAppearing()
        {
            // If first name is already filled out, the user navigated to this page to view or make a change.
            if (string.IsNullOrEmpty(FirstName) == false)
                return;

            await ValidateProfileInfo();
        }

        public void OnDisappearing()
        {
            TabBar = true;
        }

        private async void OnSubmitClicked(object obj)
        {
            if (await IsValidProfile())
            {
                await database.AddProfile(FirstName, LastName);
                await ValidateProfileInfo();
            }
        }

        private async void OnDeleteClicked(object obj)
        {
            if (await Application.Current.MainPage.DisplayAlert("Alert", "Are you sure you want to delete your profile?", "OK", "Cancel"))
            {
                await database.RemoveProfile();

                TabBar = false;

                // Clear profile values.
                FirstName = string.Empty;
                LastName = string.Empty;
            }
        }
        #endregion

        #region Methods
        private async Task<bool> IsValidProfile()
        {
            bool valid = true;
            string message = string.Empty;
            if (string.IsNullOrEmpty(FirstName) || string.IsNullOrWhiteSpace(FirstName))
                message += "No first name.\n";
            if (string.IsNullOrEmpty(LastName) || string.IsNullOrWhiteSpace(LastName))
                message += "No last name.\n";

            if (string.IsNullOrEmpty(message) == false)
            {
                await Application.Current.MainPage.DisplayAlert("Alert", message, "OK");
                valid = false;
            }

            return valid;
        }

        private async Task ValidateProfileInfo()
        {
            // If profile already exists, go to meeting page.
            if (await database.HasUserProfile())
            {
                FirstName = database.User.FirstName;
                LastName = database.User.LastName;

                // Prefixing with `//` switches to a different navigation stack instead of pushing to the active one
                await Shell.Current.GoToAsync($"//{nameof(MeetingPage)}?");
            }
        }
        #endregion
    }
}
