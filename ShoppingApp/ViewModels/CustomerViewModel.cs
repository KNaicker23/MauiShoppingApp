using ShoppingApp.Models;
using ShoppingApp.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;
using ShoppingApp.Views;

namespace ShoppingApp.ViewModels;

public class CustomerViewModel : BaseViewModel
{
    public ICommand ContinueCommand { get; }
    private readonly DatabaseService databaseService;

    private ObservableCollection<Customer> customers = new();

    public ObservableCollection<Customer> Customers
    {
        get { return customers; }
        set
        {
            customers = value;
            OnPropertyChanged();
        }
    }
    private Customer? selectedCustomer;

    public Customer? SelectedCustomer
    {
        get { return selectedCustomer; }
        set
        {
            selectedCustomer = value;
            OnPropertyChanged();
        }
    }

    public CustomerViewModel(DatabaseService databaseService)
    {
        this.databaseService = databaseService;

        ContinueCommand = new Command(async () => await ContinueAsync());
    }
    private async Task ContinueAsync()
    {
        if (SelectedCustomer == null)
        {
            await Shell.Current.DisplayAlertAsync(
                "No Customer Selected",
                "Please select a customer before continuing.",
                "OK");

            return;
        }

        await Shell.Current.GoToAsync(
            nameof(ProductView),
            new Dictionary<string, object>
            {
            { "SelectedCustomer", SelectedCustomer }
            });
    }

    public async Task LoadCustomersAsync()
    {
        try
        {
            var customerList = await databaseService.GetCustomersAsync();

            Customers = new ObservableCollection<Customer>(customerList);
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync(
                "Error",
                $"Unable to load customers: {ex.Message}",
                "OK");
        }
    }

    public override async Task OnAppearingAsync()
    {
        await LoadCustomersAsync();
    }
}