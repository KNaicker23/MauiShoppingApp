using ShoppingApp.ViewModels;

namespace ShoppingApp.Views;

public partial class OrderConfirmationView : BaseView
{
    public OrderConfirmationView(
        OrderConfirmationViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}