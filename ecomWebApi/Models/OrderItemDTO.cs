public class OrderItemDTO
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } // Optional: If needed in response
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal TotalPrice => Quantity * Price;
}
