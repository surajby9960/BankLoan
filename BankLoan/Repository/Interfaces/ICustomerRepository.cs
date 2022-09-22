using BankLoan.Model;

namespace BankLoan.Repository.Interfaces
{
    public interface ICustomerRepository
    {
        public Task<int> AddCustomer(List<Customer> customer, int id);
        //public Task<int> UpdateCustomer(Customer customer, int id);
    }
}
