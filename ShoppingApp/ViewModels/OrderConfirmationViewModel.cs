using ShoppingApp.Models;
using System.Collections.Generic;

namespace ShoppingApp.ViewModels;

public class OrderConfirmationViewModel : BaseViewModel, IQueryAttributable
{
    private int orderID;

    public int OrderID
    {
        get { return orderID; }
        set
        {
            orderID = value;
            OnPropertyChanged();
        }
    }

    private Customer? selectedCustomer;

    public Customer? SelectedCustomer
    {
        get { return selectedCustomer; }
        set
        {
            selectedCustomer = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CustomerDisplayName));
        }
    }

    public string CustomerDisplayName =>
        SelectedCustomer == null
            ? string.Empty
            : $"{SelectedCustomer.CustomerName} {SelectedCustomer.CustomerLastName}";

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("OrderID", out var id))
            OrderID = Convert.ToInt32(id);

        if (query.TryGetValue("SelectedCustomer", out var customer)
            && customer is Customer selected)
        {
            SelectedCustomer = selected;
        }
    }
}