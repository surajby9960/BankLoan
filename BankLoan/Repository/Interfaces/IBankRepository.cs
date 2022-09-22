using BankLoan.Model;

namespace BankLoan.Repository.Interfaces
{
    public interface IBankRepository
    {

        public Task<IEnumerable<Bank>> GetAllBank();
        public Task<Bank> GetBankById(int id);
        public Task<int> AddNewBank(Bank bank);
        public Task<int > UpdateBank(Bank bank);
        public Task<int> DeleteBank(int bankid,int? custid); 
       
    }
}
