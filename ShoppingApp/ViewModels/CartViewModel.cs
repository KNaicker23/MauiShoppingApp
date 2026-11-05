using ShoppingApp.Models;
using ShoppingApp.Services;
using ShoppingApp.Views;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace ShoppingApp.ViewModels;

public class CartViewModel : BaseViewModel, IQueryAttributable
{
    private readonly DatabaseService databaseService;

    private ObservableCollection<CartDisplayItem> cartItems = new();

    public ObservableCollection<CartDisplayItem> CartItems
    {
        get { return cartItems; }
        set
        {
            cartItems = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CartTotal));
            OnPropertyChanged(nameof(TotalItems));
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
            OnPropertyChanged(nameof(CustomerDisplayName));
        }
    }

    public string CustomerDisplayName =>
        SelectedCustomer == null
            ? "Your Shopping Cart"
            : $"{SelectedCustomer.CustomerName}'s Shopping Cart";

    public decimal CartTotal =>
        CartItems.Sum(item => item.LineTotal);

    public int TotalItems =>
        CartItems.Sum(item => item.Quantity);

    public ICommand RemoveItemCommand { get; }
    public ICommand CheckoutCommand { get; }

    public CartViewModel(DatabaseService databaseService)
    {
        this.databaseService = databaseService;

        RemoveItemCommand = new Command<CartDisplayItem>(
            async (item) => await RemoveItemAsync(item));
        CheckoutCommand = new Command(
            async () => await OpenCheckoutAsync());
    }

    public async Task LoadCartAsync()
    {
        if (SelectedCustomer == null)
            return;

        try
        {
            var items = await databaseService.GetCartDisplayItemsAsync(
                SelectedCustomer.CustomerID);

            CartItems = new ObservableCollection<CartDisplayItem>(items);
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync(
                "Error",
                $"Unable to load your cart: {ex.Message}",
                "OK");
        }
    }

    private async Task RemoveItemAsync(CartDisplayItem? item)
    {
        if (item == null)
            return;

        bool removed = await databaseService.RemoveCartItemAsync(
            item.CartItemID);

        if (removed)
        {
            CartItems.Remove(item);

            OnPropertyChanged(nameof(CartTotal));
            OnPropertyChanged(nameof(TotalItems));

            await Shell.Current.DisplayAlertAsync(
                "Removed",
                $"{item.ProductName} was removed from your cart.",
                "OK");
        }
        else
        {
            await Shell.Current.DisplayAlertAsync(
                "Error",
                "Unable to remove this item from your cart.",
                "OK");
        }
    }
    private async Task OpenCheckoutAsync()
    {
        if (SelectedCustomer == null)
            return;

        if (CartItems.Count == 0)
        {
            await Shell.Current.DisplayAlertAsync(
                "Empty Cart",
                "Please add products before checking out.",
                "OK");

            return;
        }

        await Shell.Current.GoToAsync(
            nameof(CheckoutView),
            new Dictionary<string, object>
            {
            { "SelectedCustomer", SelectedCustomer }
            });
    }

    public override async Task OnAppearingAsync()
    {
        await LoadCartAsync();
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("SelectedCustomer", out var customer)
            && customer is Customer selected)
        {
            SelectedCustomer = selected;
        }
    }
}
