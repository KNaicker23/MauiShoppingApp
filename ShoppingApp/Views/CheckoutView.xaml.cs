using ShoppingApp.ViewModels;

namespace ShoppingApp.Views;

public partial class CheckoutView : BaseView
{
    public CheckoutView(CheckoutViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}