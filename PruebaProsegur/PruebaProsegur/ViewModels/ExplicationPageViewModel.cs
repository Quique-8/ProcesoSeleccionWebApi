using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using PruebaProsegur.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace PruebaProsegur.ViewModels
{
    public class ExplicationPageViewModel : ViewModelBase
    {
        private UserResponse _user;
        private ObservableCollection<ExplicationResponse> _explications;

        public ExplicationPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            Title = "Logs de la prueba";
        }

        public ObservableCollection<ExplicationResponse> Explications
        {
            get => _explications;
            set => SetProperty(ref _explications, value);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            if (parameters.ContainsKey("user"))
            {
                _user = parameters.GetValue<UserResponse>("user");
                Explications = new ObservableCollection<ExplicationResponse>(_user.Explications);
            }
        }
    }
}
