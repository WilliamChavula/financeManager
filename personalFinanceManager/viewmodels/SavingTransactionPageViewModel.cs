using System;
using personalFinanceManager.interfaces;
using personalFinanceManager.models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;

namespace personalFinanceManager.viewmodels
{
    public class SavingTransactionPageViewModel : BindableBase, INavigationAware
    {
        private double totalSavings;
        private Saving selectedItem;
        private bool isBusy;
        private bool isVisible;
        private double savingAmount = 0.00;
        private DateTime savingDate = DateTime.Today;

        public bool IsBusy { get => isBusy; set => SetProperty(ref isBusy, value); }
        public bool IsVisible { get => isVisible; set => SetProperty(ref isVisible, value); }
        public double TotalSavingsAmount { get => totalSavings; set => SetProperty(ref totalSavings, value); }
        public Saving SelectedItem { get => selectedItem; set => SetProperty(ref selectedItem, value); }
        public double SavingAmount { get => savingAmount; set => SetProperty(ref savingAmount, value); }
        public DateTime SavingDate { get => savingDate; set => SetProperty(ref savingDate, value); }
        public DelegateCommand SaveCommand { get; set; }
        public DelegateCommand GoBack { get; set; }

        readonly INavigationService _navigationService;
        readonly IRepository<Saving> repository;


        public SavingTransactionPageViewModel(IRepository<Saving> repository, INavigationService navigationService)
        {
            _navigationService = navigationService;
            this.repository = repository;

            SaveCommand = new DelegateCommand(AddSaving);
            GoBack = new DelegateCommand(() => _navigationService.GoBackAsync());

            TotalSavings();
        }

        public async void TotalSavings()
        {
            double totalSavings = 0.0;
            try
            {
                var savings = await repository.GetItems();

                foreach (var saving in savings)
                {
                    totalSavings += saving.SavingAmount;
                }

                TotalSavingsAmount = totalSavings;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async void AddSaving()
        {
            IsBusy = true;
            if (SavingAmount == 0)
            {
                IsVisible = true;
                IsBusy = false;
                return;
            }

            Saving saving = new Saving()
            {
                ID = SelectedItem?.ID ?? null,
                SavingAmount = SavingAmount,
                SavingDate = SavingDate
            };

            try
            {
                await repository.AddOrUpdate(saving);

                await _navigationService.GoBackAsync();
                IsBusy = false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            IsBusy = false;
        }

        public void OnNavigatedFrom(INavigationParameters parameters)
        {

        }

        public void OnNavigatedTo(INavigationParameters parameters)
        {

        }

        public void OnNavigatingTo(INavigationParameters parameters)
        {
            Saving savingFromParm = parameters.GetValue<Saving>("selectedExpense");
            if (savingFromParm != null)
            {
                SavingAmount = savingFromParm.SavingAmount;
                SavingDate = savingFromParm.SavingDate;

                SelectedItem = savingFromParm;
            }
        }
    }
}
