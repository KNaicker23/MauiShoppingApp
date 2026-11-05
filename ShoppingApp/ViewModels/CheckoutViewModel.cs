using ShoppingApp.Models;
using ShoppingApp.Services;
using ShoppingApp.Views;
using System.Collections.Generic;
using System.Windows.Input;

namespace ShoppingApp.ViewModels;

public class CheckoutViewModel : BaseViewModel, IQueryAttributable
{
    private readonly DatabaseService databaseService;

    private Customer? selectedCustomer;

    public Customer? SelectedCustomer
    {
        get { return selectedCustomer; }
        set
        {
            selectedCustomer = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CustomerDisplayName));
        }
    }

    public string CustomerDisplayName =>
        SelectedCustomer == null
            ? "Customer"
            : $"{SelectedCustomer.CustomerName} {SelectedCustomer.CustomerLastName}";

    public ICommand PlaceOrderCommand { get; }

    public CheckoutViewModel(DatabaseService databaseService)
    {
        this.databaseService = databaseService;

        PlaceOrderCommand = new Command(
            async () => await PlaceOrderAsync());
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("SelectedCustomer", out var customer)
            && customer is Customer selected)
        {
            SelectedCustomer = selected;
        }
    }

    private async Task PlaceOrderAsync()
    {
        if (SelectedCustomer == null)
        {
            await Shell.Current.DisplayAlertAsync(
                "Error",
                "Customer information is missing.",
                "OK");

            return;
        }

        var result = await databaseService.CheckoutAsync(
            SelectedCustomer.CustomerID);

        if (!result.Success)
        {
            await Shell.Current.DisplayAlertAsync(
                "Checkout Failed",
                result.Message,
                "OK");

            return;
        }

        await Shell.Current.GoToAsync(
            nameof(OrderConfirmationView),
            new Dictionary<string, object>
            {
                { "OrderID", result.OrderID },
                { "SelectedCustomer", SelectedCustomer }
            });
    }
}