using BankLoan.Context;
using BankLoan.Model;
using BankLoan.Repository.Interfaces;
using Dapper;

namespace BankLoan.Repository
{
    public class BankRepository : IBankRepository,ICustomerRepository
    {
        private readonly DapperContext context;
      
        public BankRepository(DapperContext context )
        {
            this.context = context;
            
        }

        public async Task<int> AddCustomer(List<Customer> customer, int id)
        {
            if (customer.Count() > 0)
            {
                using (var conn = context.CreateConnection())
                {
                    var qry = @"insert into tblCustomer(custName,accountType,custAddress,custMobile,custEmail,bankId,isDeleted)values
                            (@custName,@accountType,@custAddress,@custMobile,@custEmail,@bankId,0)";
                    foreach (var customerItem in customer)
                    {
                        customerItem.BankId = id;
                        var res = await conn.ExecuteAsync(qry,customerItem);
                       
                     
                    }


                }
            }
            return 0;
        }

        public async Task<int> AddNewBank(Bank bank)
        {
            var qry = "insert into tblBank(bankName,bankAddress,bankMobile,isDeleted)values(@bankName,@bankAddress,@bankMobile,0);" +
                "SELECT CAST(SCOPE_IDENTITY() as int)";
            using(var con=context.CreateConnection())
            {
                var res=await con.QuerySingleAsync<int>(qry,bank);
                 await AddCustomer(bank.customers, res);  
                return res;
            }
            
        }

        public async Task<int> DeleteBank(int bankid, int? custid)
        {
            var qry = "update tblBank set isDeleted=1 where bankId=@bankid";
            using (var conn = context.CreateConnection())
            {
                if (custid.HasValue)
                {

                    var res= await conn.ExecuteAsync(@"update tblCustomer set isDeleted=1 where
                                                       custId=@custId and bankId=@bankid", new {bankid,custid});
                    return res;
                }
                else
                {

                    var res = await conn.ExecuteAsync(qry, new { bankid });
                    await conn.ExecuteAsync("Update tblCustomer set isDeleted=1 where BankId=@bankid ", new { bankid });
                    return res;
                }
            }
        }

        public async Task<IEnumerable<Bank>> GetAllBank()
        {
            List<Bank> banks = new List<Bank>();
            var qry = "select * from tblBank";
            using(var con=context.CreateConnection())
            {
                var bank=await con.QueryAsync<Bank>(qry);
                banks= bank.ToList();
                var qry1 = "select * from tblcustomer where bankid=@id";
                foreach(Bank bank1 in banks)
                {
                     var customer = await con.QueryAsync<Customer>(qry1, new { id = bank1.bankId });
                    bank1.customers = customer.ToList();
                }
                  return banks;
            }
        }

        public async Task<Bank> GetBankById(int id)
        { 
          
            var qry = "select * from tblbank where bankid=@id";
            using (var con = context.CreateConnection())
            {
                var bank = await con.QuerySingleAsync<Bank>(qry, new {id});
               
                if(bank!=null)
                {
                    var qry1 = "select * from tblcustomer where bankid=@id ";
                    var customer=await con.QueryAsync<Customer>(qry1, new {id = bank.bankId});
                    bank.customers=customer.ToList();
                }
                return bank;
            }
        }

        public async Task<int> UpdateBank(Bank bank)
        {
            var qry = "Update tblBank set bankName=@bankName,bankAddress=@bankAddress,bankMobile=@bankMobile where bankid=@bankid";
            using (var con = context.CreateConnection())
            { 
                  var res = await con.ExecuteAsync(qry, bank);
                  return res;
             }
        }

        
    }
}
