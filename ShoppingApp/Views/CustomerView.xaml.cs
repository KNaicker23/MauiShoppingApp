using ShoppingApp.ViewModels;
using ShoppingApp.Views;
using ShoppingApp.Services;


namespace ShoppingApp.Views;

public partial class CustomerView : BaseView
{
    public CustomerView(CustomerViewModel viewModel)
    {
        InitializeComponent();

        BindingContext = viewModel;
    }
}