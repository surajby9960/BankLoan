namespace BankLoan.Model
{
    public class Bank
    {
        public int bankId { get; set; }
        public string? bankName { get; set; }
        public string? bankAddress { get; set; }
        public string? bankMobile { get; set; }
        public int isDeleted { get; set; }
        public List<Customer>? customers { get; set; }
    }
}
