using ShoppingApp.ViewModels;

namespace ShoppingApp.Views;

public partial class ProductDetailsView : BaseView
{
    public ProductDetailsView(ProductDetailsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}