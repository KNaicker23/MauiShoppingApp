using ShoppingApp.ViewModels;

namespace ShoppingApp.Views;

public partial class ProductView : BaseView
{
    public ProductView(ProductViewModel viewModel)
    {
        InitializeComponent();

        BindingContext = viewModel;
    }
}