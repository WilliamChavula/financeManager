using System;
using System.Collections.ObjectModel;
using personalFinanceManager.interfaces;
using personalFinanceManager.models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;

namespace personalFinanceManager.viewmodels
{
    public class IncomeTransactionPageViewModel : BindableBase, INavigationAware
    {
        private double totalIncome;
        private bool isBusy;
        private bool isVisible;
        private Income selectedIncome;
        private double amount = 0.00;
        private string selectedSource;

        public bool IsVisible { get => isVisible; set => SetProperty(ref isVisible, value); }
        public bool IsBusy { get => isBusy; set => SetProperty(ref isBusy, value); }
        public double Amount { get => amount; set => SetProperty(ref amount, value); }
        public double TotalIncome { get => totalIncome; set => SetProperty(ref totalIncome, value); }
        public string SelectedSource { get => selectedSource; set => SetProperty(ref selectedSource, value); }
        public DateTime IncomeDateTime { get; set; } = DateTime.Today;
        public Income SelectedIncome { get => selectedIncome; set => SetProperty(ref selectedIncome, value); }

        public ObservableCollection<string> Source { get; set; }

        //Commands
        public DelegateCommand SaveIncomeCommand { get; set; }
        public DelegateCommand GoBack { get; set; }

        readonly IRepository<Income> incomeRepo;
        readonly INavigationService _navigationService;

        public IncomeTransactionPageViewModel(IRepository<Income> repository, INavigationService navigationService)
        {
            incomeRepo = repository;
            _navigationService = navigationService;
            Source = new ObservableCollection<string>()
            {
                "Salary",
                "Dividends",
                "Business",
                "Pension",
                "Insurance",
                "Bank Interest"
            };

            SaveIncomeCommand = new DelegateCommand(SaveIncomeToDB);
            GoBack = new DelegateCommand(() => _navigationService.GoBackAsync());

            TotalIncomeSum();
        }

        private async void SaveIncomeToDB()
        {
            IsBusy = true;

            if (Amount == 0)
            {
                IsVisible = true;
                IsBusy = false;
                return;
            }

            var income = new Income
            {
                ID = SelectedIncome?.ID ?? null,
                IncomeAmount = Amount,
                IncomeSource = SelectedSource,
                IncomeDate = IncomeDateTime
            };
            try
            {
                await incomeRepo.AddOrUpdate(income);
                await _navigationService.GoBackAsync();
                IsBusy = false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            IsBusy = false;
        }

        public async void TotalIncomeSum()
        {
            var totalSum = 0.0;
            try
            {
                var entries = await incomeRepo.GetItems();

                foreach (var entry in entries)
                {
                    totalSum += entry.IncomeAmount;
                }
                TotalIncome = totalSum;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public void OnNavigatedFrom(INavigationParameters parameters)
        {

        }

        public void OnNavigatedTo(INavigationParameters parameters)
        {

        }

        public void OnNavigatingTo(INavigationParameters parameters)
        {
            Income incomeFromParm = parameters.GetValue<Income>("selectedExpense");
            if (incomeFromParm != null)
            {
                Amount = incomeFromParm.IncomeAmount;
                SelectedSource = incomeFromParm.IncomeSource;
                IncomeDateTime = incomeFromParm.IncomeDate;

                SelectedIncome = incomeFromParm;


            }
        }

    }
}
