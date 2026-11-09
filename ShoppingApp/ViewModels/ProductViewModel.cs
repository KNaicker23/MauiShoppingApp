using Microsoft.Maui.Controls;
using ShoppingApp.Models;
using ShoppingApp.Services;
using ShoppingApp.Views;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Linq;

namespace ShoppingApp.ViewModels;

public class ProductViewModel : BaseViewModel, IQueryAttributable//allows ViewModel to receive info passed during Shell nav
{
    public ICommand OpenProductDetailsCommand { get; }
    public ICommand OpenCartCommand { get; }
    private readonly DatabaseService databaseService;

    public ObservableCollection<Product> Products { get; set; } = new();

    public ObservableCollection<Category> Categories { get; set; } = new();
    private Customer? selectedCustomer;

    public Customer? SelectedCustomer
    {
        get { return selectedCustomer; }
        set
        {
            selectedCustomer = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CustomerDisplayName));//welcome message updates when a customer is selected
        }
    }
    public string CustomerDisplayName =>
    SelectedCustomer == null
        ? "Welcome to Tadka!"
        : $"Welcome, {SelectedCustomer.CustomerName}!";

    public ProductViewModel(DatabaseService databaseService)
    {
        this.databaseService = databaseService;

        OpenProductDetailsCommand = new Command<Product>(
    async (product) => await OpenProductDetailsAsync(product));
        OpenCartCommand = new Command(async () => await OpenCartAsync());
    }
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("SelectedCustomer", out var customer)
            && customer is Customer selected)
        {
            SelectedCustomer = selected;
        }
    }
    public async Task LoadProductsAsync()
    {
        try
        {
            var productList = await databaseService.GetProductsAsync();
            var categoryList = await databaseService.GetCategoriesAsync();

            foreach (var product in productList)
            {
                var category = categoryList.FirstOrDefault(c => c.CategoryID == product.CategoryID);


                product.CategoryName = category?.CategoryName ?? "Other";
            }

            Products = new ObservableCollection<Product>(productList);
            Categories = new ObservableCollection<Category>(categoryList);

            OnPropertyChanged(nameof(Products));
            OnPropertyChanged(nameof(Categories));
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync(
                "Error",
                $"Unable to load products: {ex.Message}",
                "OK");
        }
    }
    private async Task OpenProductDetailsAsync(Product? product)
    {

        if (product == null || SelectedCustomer == null)
            return;

        await Shell.Current.GoToAsync(
            nameof(ProductDetailsView),
            new Dictionary<string, object>
            {
            { "SelectedProduct", product },
            { "SelectedCustomer", SelectedCustomer }
            });
    }
    private async Task OpenCartAsync()
    {
        if (SelectedCustomer == null)
            return;

        await Shell.Current.GoToAsync(
            nameof(CartView),
            new Dictionary<string, object>
            {
            { "SelectedCustomer", SelectedCustomer }
            });
    }
    public override async Task OnAppearingAsync()
    {
        await LoadProductsAsync();
    }
}