using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microcharts;
using personalFinanceManager.interfaces;
using personalFinanceManager.models;
using Prism.AppModel;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using SkiaSharp;

namespace personalFinanceManager.viewmodels
{
    public class HomePageViewModel : BindableBase, IApplicationLifecycleAware, IPageLifecycleAware
    {
        private double totalIncome;
        private double totalExpenses;
        private double totalSavings;
        private double balance;
        private DonutChart chart;

        public DelegateCommand AddSaving { get; set; }
        public DelegateCommand AddIncome { get; set; }
        public DelegateCommand AddExpense { get; set; }
        public DelegateCommand ListExpensesCommand { get; set; }
        public DelegateCommand ListSavingsCommand { get; set; }
        public DelegateCommand ListIncomeCommand { get; set; }

        private readonly IRepository<Saving> savingRepo;
        private readonly IRepository<Expense> expenseRepo;
        private readonly IRepository<Income> incomeRepo;
        private readonly INavigationService _navigationService;

        public double TotalIncome { get => totalIncome; set => SetProperty(ref totalIncome, value); }
        public double TotalExpenses { get => totalExpenses; set => SetProperty(ref totalExpenses, value); }
        public double TotalSavings { get => totalSavings; set => SetProperty(ref totalSavings, value); }
        public double Balance { get => balance; set => SetProperty(ref balance, value); }
        public DonutChart Chart { get => chart; set => SetProperty(ref chart, value); }

        public HomePageViewModel(IRepository<Saving> savingRepository, IRepository<Expense> expenseRepository, IRepository<Income> incomeRepository, INavigationService navigationService)
        {
            _navigationService = navigationService;
            savingRepo = savingRepository;
            expenseRepo = expenseRepository;
            incomeRepo = incomeRepository;

            AddIncome = new DelegateCommand(() => _navigationService.NavigateAsync("IncomeTransactionPage"));
            AddExpense = new DelegateCommand(() => _navigationService.NavigateAsync("ExpenseTransactionPage"));
            AddSaving = new DelegateCommand(() => _navigationService.NavigateAsync("SavingTransactionPage"));
            ListExpensesCommand = new DelegateCommand(() => _navigationService.NavigateAsync("AllTransactionsPage"));
            ListSavingsCommand = new DelegateCommand(() => _navigationService.NavigateAsync("SavingsCollectionPage"));
            ListIncomeCommand = new DelegateCommand(() => _navigationService.NavigateAsync("IncomeCollectionPage"));



            expenseRepo.OnItemAdded += (sender, item) =>
            {
                GetTotalExpenses();
            };

            incomeRepo.OnItemAdded += (sender, item) =>
            {
                GetTotalIncome();
            };

            savingRepo.OnItemAdded += (sender, item) =>
            {
                GetTotalSavings();
            };
        }

        public async void GetTotalIncome()
        {
            var incomeTotal = 0.0;
            try
            {
                var income = await incomeRepo.GetItems();
                foreach (var inc in income)
                {
                    incomeTotal += inc.IncomeAmount;
                }

                TotalIncome = incomeTotal;
                Balance = GetIncomeBalance();
                await Task.Delay(50);

                DrawChart();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async void GetTotalSavings()
        {
            var savingsTotal = 0.0;

            try
            {
                var savings = await savingRepo.GetItems();
                foreach (var saving in savings)
                {
                    savingsTotal += saving.SavingAmount;
                }

                TotalSavings = savingsTotal;
                Balance = GetIncomeBalance();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async void GetTotalExpenses()
        {
            var expensesTotal = 0.0;

            try
            {
                var expenses = await expenseRepo.GetItems();
                foreach (var expense in expenses)
                {
                    expensesTotal += expense.ExpenseAmount;
                }

                TotalExpenses = expensesTotal;
                Balance = GetIncomeBalance();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public double GetIncomeBalance()
        {
            return ((TotalIncome + TotalSavings) - TotalExpenses);
        }

        private void DrawChart()
        {
            var chartEntries = new[]
            {
                new ChartEntry((float)totalIncome)
                {
                    Label = "Income",
                    ValueLabel =totalIncome.ToString(),
                    Color = SKColor.Parse("#CD8944"),
                    ValueLabelColor = SKColor.Parse("#CD8944")
                },

                new ChartEntry((float)totalExpenses)
                {
                    Label = "Expenses",
                    ValueLabel = totalExpenses.ToString(),
                    Color = SKColor.Parse("#D44A6E"),
                    ValueLabelColor = SKColor.Parse("#D44A6E")
                },

                new ChartEntry((float)totalSavings)
                {
                    Label = "Savings",
                    ValueLabel = totalSavings.ToString(),
                    Color = SKColor.Parse("#7C63A5"),
                    ValueLabelColor = SKColor.Parse("#7C63A5")
                }
            };

            Chart = new DonutChart { Entries = chartEntries, BackgroundColor = SKColors.Transparent, HoleRadius = 0.6f, LabelMode = LabelMode.None };
        }

        public void OnResume()
        {
            GetTotalSavings();
            GetTotalExpenses();
            GetTotalIncome();
        }

        public void OnSleep()
        {
            //GetTotalSavings();
        }

        public void OnAppearing()
        {
            GetTotalSavings();
            GetTotalExpenses();
            GetTotalIncome();
        }

        public void OnDisappearing()
        {
        }
    }
}
