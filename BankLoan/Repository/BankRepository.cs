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

        public async Task<List<Combine>> GetCustomerLoanById(int? id)
        {
            List<Combine> Linf = new List<Combine>();
            var qry = @"select bankname,c.custname,c.custmobile from tblCustomer c
                        inner join tblBank b on c.BankId = b.bankId
                        inner join loanApproval la on c.custId = la.custId";
            using (var con = context.CreateConnection())
            {
                var ltype = new Combine();

                if (id == null)
                {
                    var loaninfo = await con.QueryAsync<Combine>(qry);
                  
                    foreach (var item in loaninfo)
                    {
                        ltype = await con.QuerySingleAsync<Combine>(@"select loantype from tblLoans where loanId=(select loanId from
                        loanApproval where custId=@id)", new { id=ltype.custId });
                        item.loanType = ltype.loanType;
                    }
                    return loaninfo.ToList();
                }
                else
                {
                    var qry1 = @"select bankname,c.custname,c.custmobile from tblCustomer c
                        inner join tblBank b on c.BankId = b.bankId
                        inner join loanApproval la on c.custId = la.custId where c.custId=@id";
                    var loaninfo = await con.QuerySingleAsync<Combine>(qry1, new { id });

                    ltype = await con.QuerySingleAsync<Combine>(@"select loantype from tblLoans where loanId=(select loanId from
                        loanApproval where custId=@id)", new { id });
                    loaninfo.loanType = ltype.loanType;
                    Linf.Add(loaninfo);
                    return Linf;

                }
               
            }
        }

        public async Task<int> LoanApproval(LoanApproval loanApproval)
        {
            var qry = "insert into loanApproval(loanId,BankId,custId,loanStatus)values" +
                     "(@loanId,@BankId,@custId,@loanStatus)";
            using(var con=context.CreateConnection())
            {
                var tblbank =await con.QuerySingleAsync<LoanApproval>("select bankId from tblCustomer where custId=@custId", new {loanApproval.custId});
                loanApproval.bankId = tblbank.bankId;
                loanApproval.loanstatus = "Pending";
                var lapprove=await con.ExecuteAsync(qry,loanApproval);
                return lapprove;

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
