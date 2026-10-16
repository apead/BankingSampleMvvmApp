using BankingApp.ViewModels;

namespace BankingApp.Views;

public class BaseView : ContentPage
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
