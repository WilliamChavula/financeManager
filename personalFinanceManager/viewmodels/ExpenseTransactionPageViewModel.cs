using System;
using System.Collections.ObjectModel;
using personalFinanceManager.interfaces;
using personalFinanceManager.models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;

namespace personalFinanceManager.viewmodels
{
    public class ExpenseTransactionPageViewModel : BindableBase, INavigationAware
    {
        private Expense selectedExpense;
        private bool isBusy = false;
        private double totalExpenseAmount;
        private string name;
        private string category;
        private double amount = 0.00;
        private DateTime expenseDateTime = DateTime.Today;
        private bool isVisible = false;

        public ObservableCollection<string> ExpenseCategory { get; private set; }


        public DelegateCommand SaveExpenseCommand { get; private set; }
        public DelegateCommand GoBack { get; set; }

        readonly INavigationService _navigationService;
        readonly IRepository<Expense> repository;

        public bool IsVisible { get => isVisible; set => SetProperty(ref isVisible, value); }
        public bool IsBusy { get => isBusy; set => SetProperty(ref isBusy, value); }
        public string Name { get => name; set => SetProperty(ref name, value); }
        public string Category { get => category; set => SetProperty(ref category, value); }
        public double Amount { get => amount; set => SetProperty(ref amount, value); }
        public double TotalExpenseAmount { get => totalExpenseAmount; set => SetProperty(ref totalExpenseAmount, value); }
        public DateTime ExpenseDateTime { get => expenseDateTime; set => SetProperty(ref expenseDateTime, value); }
        public Expense SelectedExpense { get => selectedExpense; set => SetProperty(ref selectedExpense, value); }

        public ExpenseTransactionPageViewModel(IRepository<Expense> repository, INavigationService navigationService)
        {
            this.repository = repository;
            _navigationService = navigationService;

            ExpenseCategory = new ObservableCollection<string>()
            {
                "Personal",
                "Rent",
                "Utilities",
                "Food",
                "Dining out",
                "Groceries",
                "Transport",
                "Internet",
                "Subscriptions",
                "Charity",
                "Electronics",
                "Clothes",
                "Shoes",
                "Furniture",
                "Other"
            };

            GoBack = new DelegateCommand(() => _navigationService.GoBackAsync());
            SaveExpenseCommand = new DelegateCommand(AddExpense);

            TotalExpenseSum();
        }

        public async void TotalExpenseSum()
        {
            var totalSum = 0.0;
            try
            {
                var expenses = await repository.GetItems();

                foreach (var expense in expenses)
                {
                    totalSum += expense.ExpenseAmount;
                }
                TotalExpenseAmount = totalSum;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        public async void AddExpense()
        {
            IsBusy = true;

            if (Amount == 0)
            {
                IsVisible = true;
                IsBusy = false;
                return;
            }

            Expense expense = new Expense()
            {
                ID = SelectedExpense?.ID ?? null,
                ExpenseName = Name,
                ExpenseAmount = Amount,
                ExpenseCategory = Category,
                ExpenseDate = ExpenseDateTime
            };

            try
            {
                await repository.AddOrUpdate(expense);

                await _navigationService.GoBackAsync();
                IsBusy = false;

            }
            catch (Exception ex)
            {
                throw ex;
            }

            IsBusy = false;

        }

        public void OnNavigatedTo(INavigationParameters parameters)
        {

        }

        public void OnNavigatedFrom(INavigationParameters parameters)
        {

        }

        public void OnNavigatingTo(INavigationParameters parameters)
        {
            Expense expenseFromParm = parameters.GetValue<Expense>("selectedExpense");
            if (expenseFromParm != null)
            {
                Name = expenseFromParm.ExpenseName;
                Amount = expenseFromParm.ExpenseAmount;
                Category = expenseFromParm.ExpenseCategory;
                ExpenseDateTime = expenseFromParm.ExpenseDate;

                SelectedExpense = expenseFromParm;


            }
        }
    }
}
