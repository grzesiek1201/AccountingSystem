namespace AccountingSystem.Application.DTOs.Orders
{
    public class CreateOrderRequest
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public List<CreateOrderItemRequest> Items { get; set; } = new();
    }
}
