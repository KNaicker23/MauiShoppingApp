using ShoppingApp.ViewModels;

namespace ShoppingApp.Views;

public class BaseView : ContentPage//reused code from banking app
{
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is BaseViewModel viewModel)
        {
            await viewModel.OnAppearingAsync();
        }
    }

    protected override async void OnDisappearing()
    {
        base.OnDisappearing();

        if (BindingContext is BaseViewModel viewModel)
        {
            await viewModel.OnDisappearingAsync();
        }
    }
}