using System;
using System.Collections.ObjectModel;
using personalFinanceManager.interfaces;
using personalFinanceManager.models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using Xamarin.Forms;

namespace personalFinanceManager.viewmodels
{
    public class SavingsCollectionPageViewModel : BindableBase
    {

        readonly IRepository<Saving> _repository;
        readonly INavigationService _navigationService;
        readonly IPageDialogService _dialogService;
        private double totalSavingsAmount;
        private bool isBusy;

        public ObservableCollection<Saving> Savings { get; set; }
        public DelegateCommand<Saving> DeleteSavingItemCommand { get; set; }
        public DelegateCommand<Saving> SelectSavingItemCommand { get; set; }
        public DelegateCommand AddSavingCommand { get; set; }
        public DelegateCommand GoBack { get; set; }

        public bool IsBusy { get => isBusy; set => SetProperty(ref isBusy, value); }
        public double TotalSavingsAmount { get => totalSavingsAmount; set => SetProperty(ref totalSavingsAmount, value); }

        public SavingsCollectionPageViewModel(IRepository<Saving> repository, INavigationService navigationService, IPageDialogService pageDialogService)
        {
            _repository = repository;
            _navigationService = navigationService;
            _dialogService = pageDialogService;

            Savings = new ObservableCollection<Saving>();
            DeleteSavingItemCommand = new DelegateCommand<Saving>(DisplayActionSheet);
            SelectSavingItemCommand = new DelegateCommand<Saving>(GetSelectedSaving);
            AddSavingCommand = new DelegateCommand(() => _navigationService.NavigateAsync("SavingTransactionPage"));
            GoBack = new DelegateCommand(() => _navigationService.GoBackAsync());

            GetAllSavings();
            TotalSavingsSum();

            _repository.OnItemAdded += _repository_OnItemAdded;

        }

        private void _repository_OnItemAdded(object sender, Saving e)
        {
            GetAllSavings();
            TotalSavingsSum();
        }

        private void GetSelectedSaving(Saving obj)
        {
            IsBusy = true;
            try
            {
                var navParams = new NavigationParameters
                {
                    {"selectedExpense", obj }
                };

                // changed method from await _navigationService.NavigateAsync("ExpenseTransactionPage", navParams);

                Device.BeginInvokeOnMainThread(async () => await _navigationService.NavigateAsync("SavingTransactionPage", navParams));

                IsBusy = false;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            IsBusy = false;
        }

        private async void DisplayActionSheet(Saving obj)
        {
            var _confirmDelete = await _dialogService.DisplayActionSheetAsync("Would you like to delete this entry?", "Cancel", "Delete");
            if (_confirmDelete == "Delete")
            {
                DeleteSavingItem(obj);
            }
        }

        private async void DeleteSavingItem(Saving obj)
        {
            IsBusy = true;

            try
            {
                await _repository.DeleteItem(obj);
                TotalSavingsSum();
                GetAllSavings();


                IsBusy = false;
            }
            catch (Exception ex)
            {
                throw ex;
            }


            IsBusy = false;
        }

        private async void GetAllSavings()
        {
            IsBusy = true;

            try
            {
                var savings = await _repository.GetItems();
                Savings.Clear();

                for (var i = savings.Count - 1; i >= 0; i--)
                {
                    Savings.Add(savings[i]);
                }

                IsBusy = false;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            IsBusy = false;
        }

        private async void TotalSavingsSum()
        {
            var totalSum = 0.0;
            try
            {
                var savings = await _repository.GetItems();

                foreach (var saving in savings)
                {
                    totalSum += saving.SavingAmount;
                }
                TotalSavingsAmount = totalSum;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
