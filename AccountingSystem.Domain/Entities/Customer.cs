using AccountingSystem.Domain.Enums;

namespace AccountingSystem.Domain.Entities
{
    public class Customer
    {
        public int Id { get; set; }
        public  string Name { get; set; } = string.Empty;
        public  string NIP { get; set; } = string.Empty;
        public  string Email { get; set; } = string.Empty;
        public  string Street { get; set; } = string.Empty;
        public  string City { get; set; } = string.Empty;
        public  string ZipCode { get; set; } = string.Empty;
        public bool IsCustomerArchived { get; set; }
        public bool InDebt { get; set; }
    }
}

