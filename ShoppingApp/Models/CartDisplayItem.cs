namespace ShoppingApp.Models;

public class CartDisplayItem
{
    public int CartItemID { get; set; }

    public int ProductID { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public string? ImageName { get; set; }

    public decimal ProductPrice { get; set; }

    public int Quantity { get; set; }

    public decimal LineTotal => ProductPrice * Quantity;
}