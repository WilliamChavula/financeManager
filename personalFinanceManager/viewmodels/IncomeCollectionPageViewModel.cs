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
    public class IncomeCollectionPageViewModel : BindableBase
    {

        readonly IRepository<Income> _repository;
        readonly INavigationService _navigationService;
        readonly IPageDialogService _dialogService;
        private double totalIncomeAmount;
        private bool isBusy;

        public ObservableCollection<Income> Incomes { get; set; }
        public DelegateCommand<Income> DeleteIncomeItemCommand { get; set; }
        public DelegateCommand<Income> SelectIncomeItemCommand { get; set; }
        public DelegateCommand AddIncomeCommand { get; set; }
        public DelegateCommand GoBack { get; set; }

        public bool IsBusy { get => isBusy; set => SetProperty(ref isBusy, value); }
        public double TotalIncomeAmount { get => totalIncomeAmount; set => SetProperty(ref totalIncomeAmount, value); }

        public IncomeCollectionPageViewModel(IRepository<Income> repository, INavigationService navigationService, IPageDialogService pageDialogService)
        {
            _repository = repository;
            _navigationService = navigationService;
            _dialogService = pageDialogService;

            Incomes = new ObservableCollection<Income>();
            DeleteIncomeItemCommand = new DelegateCommand<Income>(DisplayActionSheet);
            SelectIncomeItemCommand = new DelegateCommand<Income>(GetSelectedIncome);
            AddIncomeCommand = new DelegateCommand(() => _navigationService.NavigateAsync("IncomeTransactionPage"));
            GoBack = new DelegateCommand(() => _navigationService.GoBackAsync());

            GetAllIncomes();
            TotalIncomeSum();

            _repository.OnItemAdded += _repository_OnItemAdded;
        }

        private void _repository_OnItemAdded(object sender, Income e)
        {
            GetAllIncomes();
            TotalIncomeSum();
        }

        private void GetSelectedIncome(Income obj)
        {
            IsBusy = true;
            try
            {
                var navParams = new NavigationParameters
                {
                    {"selectedExpense", obj }
                };

                // changed method from await _navigationService.NavigateAsync("ExpenseTransactionPage", navParams);

                Device.BeginInvokeOnMainThread(async () => await _navigationService.NavigateAsync("IncomeTransactionPage", navParams));

                IsBusy = false;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            IsBusy = false;
        }

        private async void DisplayActionSheet(Income obj)
        {
            var _confirmDelete = await _dialogService.DisplayActionSheetAsync("Would you like to delete this entry?", "Cancel", "Delete");
            if (_confirmDelete == "Delete")
            {
                DeleteIncomeItem(obj);
            }
        }

        private async void DeleteIncomeItem(Income obj)
        {
            IsBusy = true;

            try
            {
                await _repository.DeleteItem(obj);
                TotalIncomeSum();
                GetAllIncomes();


                IsBusy = false;
            }
            catch (Exception ex)
            {
                throw ex;
            }


            IsBusy = false;
        }

        private async void GetAllIncomes()
        {
            IsBusy = true;

            try
            {
                var incomes = await _repository.GetItems();
                Incomes.Clear();

                foreach (var income in incomes)
                {
                    Incomes.Add(income);
                }

                IsBusy = false;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            IsBusy = false;
        }

        private async void TotalIncomeSum()
        {
            var totalSum = 0.0;
            try
            {
                var incomes = await _repository.GetItems();

                foreach (var income in incomes)
                {
                    totalSum += income.IncomeAmount;
                }
                TotalIncomeAmount = totalSum;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
