using BankLoan.Model;

namespace BankLoan.Repository.Interfaces
{
    public interface ICustomerRepository
    {
        public Task<int> AddCustomer(List<Customer> customer, int id);
        public Task<List<Combine>> GetCustomerLoanById(int? custid,int? bankid);
        public Task<int> Transcation(int id,double amount,string TranscType);
        public Task<BaseResponse> GetAllByPagination(int pageno,int pageSize);
        public Task<int> PayLoan(int custid, int loanid,int bankid);
         

    }
}
