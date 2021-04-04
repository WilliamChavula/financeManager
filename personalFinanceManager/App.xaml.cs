using personalFinanceManager.interfaces;
using personalFinanceManager.models;
using personalFinanceManager.services;
using personalFinanceManager.viewmodels;
using personalFinanceManager.views;
using Prism;
using Prism.Autofac;
using Prism.Ioc;
using Xamarin.Forms;

namespace personalFinanceManager
{
    public partial class App : PrismApplication
    {
        public App(IPlatformInitializer initializer = null) : base(initializer)
        {

        }

        protected override void OnInitialized()
        {
            InitializeComponent();

            NavigationService.NavigateAsync("NavigationPage/HomePage");
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // Register Pages and ViewModels for Navigation
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<HomePage, HomePageViewModel>();
            containerRegistry.RegisterForNavigation<IncomeTransactionPage, IncomeTransactionPageViewModel>();
            containerRegistry.RegisterForNavigation<SavingTransactionPage, SavingTransactionPageViewModel>();
            containerRegistry.RegisterForNavigation<ExpenseTransactionPage, ExpenseTransactionPageViewModel>();
            containerRegistry.RegisterForNavigation<AllTransactionsPage, AllTransactionsPageViewModel>();
            containerRegistry.RegisterForNavigation<SavingsCollectionPage, SavingsCollectionPageViewModel>();
            containerRegistry.RegisterForNavigation<IncomeCollectionPage, IncomeCollectionPageViewModel>();

            // Register Services in Prism for Dependency Injection
            containerRegistry.RegisterSingleton<IRepository<Income>, IncomeRepository>();
            containerRegistry.RegisterSingleton<IRepository<Saving>, SavingRepository>();
            containerRegistry.RegisterSingleton<IRepository<Expense>, ExpenseRepository>();
        }
    }
}
