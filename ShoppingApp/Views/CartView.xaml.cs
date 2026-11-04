using ShoppingApp.ViewModels;

namespace ShoppingApp.Views;

public partial class CartView : BaseView
{
    public CartView(CartViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}