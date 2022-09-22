namespace BankLoan.Model
{
    public class LoanApproval
    {
        public int lAprvId { get; set; }
        public int loanId { get; set; }
        public int bankId { get; set; }
        public int custId { get; set; }
        public string? loanstatus { get; set; }
    }
}
