
using ShoppingApp.Models;
using ShoppingApp.Services;
using System.Collections.Generic;
using System.Windows.Input;

namespace ShoppingApp.ViewModels;

public class ProductDetailsViewModel : BaseViewModel, IQueryAttributable
{
    private readonly DatabaseService databaseService;

    private Product? selectedProduct;

    public Product? SelectedProduct
    {
        get { return selectedProduct; }
        set
        {
            selectedProduct = value;
            OnPropertyChanged();

            Quantity = selectedProduct == null
                ? 1
                : selectedProduct.StockAvailable > 0 ? 1 : 0;
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

    private int quantity = 1;

    public int Quantity
    {
        get { return quantity; }
        set
        {
            int maximum = SelectedProduct?.StockAvailable ?? 0;

            if (value < 0)
                value = 0;

            if (value > maximum)
                value = maximum;

            if (quantity != value)
            {
                quantity = value;
                OnPropertyChanged();
            }
        }
    }

    public ICommand IncreaseQuantityCommand { get; }
    public ICommand DecreaseQuantityCommand { get; }
    public ICommand AddToCartCommand { get; }//add to cart command

    public ProductDetailsViewModel(DatabaseService databaseService)
    {
        this.databaseService = databaseService;

        IncreaseQuantityCommand = new Command(IncreaseQuantity);
        DecreaseQuantityCommand = new Command(DecreaseQuantity);
        AddToCartCommand = new Command(async () => await AddToCartAsync());
    }

    private void IncreaseQuantity()
    {
        if (SelectedProduct == null)
            return;

        if (Quantity < SelectedProduct.StockAvailable)
        {
            Quantity++;
        }
    }

    private void DecreaseQuantity()
    {
        if (Quantity > 1)
        {
            Quantity--;
        }
    }

    private async Task AddToCartAsync()
    {
        if (SelectedProduct == null || SelectedCustomer == null)
        {
            await Shell.Current.DisplayAlertAsync(
                "Error",
                "Product or customer information is missing.",
                "OK");

            return;
        }

        if (Quantity <= 0)
        {
            await Shell.Current.DisplayAlertAsync(
                "Invalid Quantity",
                "Please select a quantity greater than zero.",
                "OK");

            return;
        }

        var result = await databaseService.AddToCartAsync(
            SelectedCustomer.CustomerID,
            SelectedProduct.ProductID,
            Quantity);

        await Shell.Current.DisplayAlertAsync(
            result.Success ? "Success" : "Unable to Add Product",
            result.Message,
            "OK");
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("SelectedProduct", out var product)
            && product is Product selected)
        {
            SelectedProduct = selected;
        }

        if (query.TryGetValue("SelectedCustomer", out var customer)
            && customer is Customer selectedCustomer)
        {
            SelectedCustomer = selectedCustomer;
        }
    }
}
