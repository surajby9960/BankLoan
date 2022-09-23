using BankLoan.Model;

namespace BankLoan.Repository.Interfaces
{
    public interface ICustomerRepository
    {
        public Task<int> AddCustomer(List<Customer> customer, int id);
        public Task<List<Combine>> GetCustomerLoanById(int? id);

    }
}
