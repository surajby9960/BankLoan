namespace BankLoan.Model
{
    public class Customer
    {
        public int custId { get; set; }
        public string? custName { get; set; }
        public string? accountType { get; set; }
        public string? custAddress { get; set; }
        public string? custMobile { get; set; }
        public string? custEmail { get; set; }
        public double? balance { get; set; }
        public int BankId { get; set; }
        public int isDeleted { get; set; }
    }
}
