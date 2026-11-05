using ShoppingApp.Views;

namespace ShoppingApp
{
    public partial class AppShell : Shell
    {
        public AppShell(CustomerView customerView)
        {
            InitializeComponent();

            var shellContent = new ShellContent
            {
                Title = "Select Customer",
                Route = "CustomerView",
                Content = customerView
            };

            var shellItem = new ShellItem
            {
                Items =
                {
                    new ShellSection
                    {
                        Items = { shellContent }
                    }
                }
            };

            Items.Add(shellItem);

            Routing.RegisterRoute(nameof(ProductView), typeof(ProductView));

            Routing.RegisterRoute(nameof(ProductDetailsView), typeof(ProductDetailsView));

            Routing.RegisterRoute(nameof(CartView), typeof(CartView));
            Routing.RegisterRoute(nameof(OrderConfirmationView), typeof(OrderConfirmationView));
            Routing.RegisterRoute(nameof(CheckoutView), typeof(CheckoutView));




        }
    }
} 