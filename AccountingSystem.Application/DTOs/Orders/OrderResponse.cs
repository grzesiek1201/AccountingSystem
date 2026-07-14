using AccountingSystem.Application.DTOs.Customers;

namespace AccountingSystem.Application.DTOs.Orders
{
    public class OrderResponse
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime DateCreated { get; set; }

        public CustomerResponse Customer { get; set; } = new();

        public List<OrderItemResponse> Items { get; set; } = new();
    }
}
