public class OrderDTO
{
    public int OrderId { get; set; }
    public int UserId { get; set; }
    public int AddressId { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; }
    public DateTime OrderDate { get; set; }
    public List<OrderItemDTO> OrderItems { get; set; } = new List<OrderItemDTO>();
}
