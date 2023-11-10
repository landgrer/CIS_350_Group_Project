using System;
using System.Collections.Generic;
using TutorApp.Models;
using TutorApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TutorApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ScheduleMeetingPage : ContentPage
    {
        ScheduleMeetingViewModel _viewModel;

        public ScheduleMeetingPage(Meeting meeting)
        {
            InitializeComponent();
            BindingContext = _viewModel = new ScheduleMeetingViewModel(meeting);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing();
        }
    }
}