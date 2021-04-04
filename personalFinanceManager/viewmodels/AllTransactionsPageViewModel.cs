using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using personalFinanceManager.interfaces;
using personalFinanceManager.models;
using Prism.AppModel;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using Xamarin.Forms;

namespace personalFinanceManager.viewmodels
{
    public class AllTransactionsPageViewModel : BindableBase
    {
        readonly IRepository<Expense> repository;
        readonly INavigationService _navigationService;
        readonly IPageDialogService _dialogService;
        private bool isBusy = false;
        private double totalExpenseAmount;

        public ObservableCollection<Expense> Expenses { get; set; }
        public DelegateCommand<Expense> DeleteItemCommand { get; set; }
        public DelegateCommand<Expense> SelectItemCommand { get; set; }
        public DelegateCommand AddExpenseCommand { get; set; }
        public DelegateCommand GoBack { get; set; }

        public bool IsBusy { get => isBusy; set => SetProperty(ref isBusy, value); }
        public double TotalExpenseAmount { get => totalExpenseAmount; private set => SetProperty(ref totalExpenseAmount, value); }

        public AllTransactionsPageViewModel(IRepository<Expense> repository, INavigationService navigationService, IPageDialogService dialogService)
        {

            this.repository = repository;
            _navigationService = navigationService;
            _dialogService = dialogService;

            Expenses = new ObservableCollection<Expense>();
            DeleteItemCommand = new DelegateCommand<Expense>(DisplayActionSheet);
            SelectItemCommand = new DelegateCommand<Expense>(GetSelectedExpense);
            AddExpenseCommand = new DelegateCommand(() => _navigationService.NavigateAsync("ExpenseTransactionPage"));
            GoBack = new DelegateCommand(() => _navigationService.GoBackAsync());

            GetAllExpenses();
            TotalExpenseSum();

            this.repository.OnItemAdded += Repository_OnItemAdded;
        }

        private void Repository_OnItemAdded(object sender, Expense e)
        {
            GetAllExpenses();
            TotalExpenseSum();
        }

        private async void DisplayActionSheet(Expense arg)
        {
            var _confirmDelete = await _dialogService.DisplayActionSheetAsync("Would you like to delete this entry?", "Cancel", "Delete");
            if (_confirmDelete == "Delete")
            {
                DeleteExpenseItem(arg);
            }
        }

        private void GetSelectedExpense(Expense selectedExpense)
        {
            IsBusy = true;
            try
            {
                //var item = await repository.GetItemByID(selectedExpense.ID);
                var navParams = new NavigationParameters
                {
                    {"selectedExpense", selectedExpense }
                };

                // changed method from await _navigationService.NavigateAsync("ExpenseTransactionPage", navParams);

                Device.BeginInvokeOnMainThread(async () => await _navigationService.NavigateAsync("ExpenseTransactionPage", navParams));

                IsBusy = false;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            IsBusy = false;
        }

        private async void DeleteExpenseItem(Expense expense)
        {
            IsBusy = true;

            try
            {
                await repository.DeleteItem(expense);
                TotalExpenseSum();
                GetAllExpenses();


                IsBusy = false;
            }
            catch (Exception ex)
            {
                throw ex;
            }


            IsBusy = false;
        }

        public async void GetAllExpenses()
        {
            IsBusy = true;

            try
            {
                var expenses = await repository.GetItems();
                Expenses.Clear();

                for (var i = expenses.Count - 1; i >= 0; i--)
                {
                    Expenses.Add(expenses[i]);
                }

                IsBusy = false;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            IsBusy = false;
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
    }
}
