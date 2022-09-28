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

        public async Task<int> AccountOpen(Customer customer)
        {
            var qry= @"insert into tblCustomer(custName,accountType,custAddress,custMobile,custEmail,bankId,balance,isDeleted)values
                            (@custName,@accountType,@custAddress,@custMobile,@custEmail,@bankId,@balance,0)";

            using(var con=context.CreateConnection())
            {
                var res = await con.ExecuteAsync(qry, customer);
                return res;
            }
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

        public async Task<BaseResponse> GetAllByPagination(int pageno, int pageSize)
        {
            BaseResponse baseResponse=new BaseResponse();
            PaginationModel paginationModel = new PaginationModel();
            List<Customer> customerList= new List<Customer>();
            if (pageno == 0)
            {
                pageno = 1;
            }
            if (pageSize == 0)
            {
                pageSize = 10;
            }
            var val = (pageno - 1) * pageSize;
            var qry = @"select bankname,c.custname,c.custmobile,l.loanType from tblCustomer c 
                                inner join tblBank b on c.BankId=b.bankId
                                inner join loanApproval la on c.custId=la.custId 
                                inner join tblLoans l on la.loanId=l.loanId order by c.custid
                                 offset @val  rows fetch next @pageSize rows only;
                         select @pageno as PageNumber,count(distinct c.custid) as totalpages from tblCustomer c ";
            using (var con = context.CreateConnection())
            {


                var values = new { pageno = pageno, pagesize = pageSize, val = val };
                var result = await con.QueryMultipleAsync(qry, values);
                var list = await result.ReadAsync<Customer>();
                customerList = list.ToList();

                var pagination = await result.ReadAsync<PaginationModel>();
                paginationModel = pagination.FirstOrDefault();

                int pagecount = 0;
                int last = 0;
                int cnt = 0;

                cnt = paginationModel.totalpages;
                last = paginationModel.totalpages % pageSize;
                pagecount = paginationModel.totalpages / pageSize;
                paginationModel.pageno = pageno;

                if (last > 0)
                {
                    paginationModel.totalpages = pagecount + 1;
                }

                baseResponse.ResponseData1 = customerList;
                baseResponse.ResponseData2 = pagination;
            }
            return baseResponse;



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

        public async Task<List<Combine>> GetCustomerLoanById(int? custid,int? bankid)
        {
            List<Combine> Linf = new List<Combine>();
            var qry = @"select bankname,c.custname,c.custmobile,l.loanType from tblCustomer c 
                                inner join tblBank b on c.BankId=b.bankId
                                inner join loanApproval la on c.custId=la.custId 
                                inner join tblLoans l on la.loanId=l.loanId";

            var qry1 = @"select bankname,c.custname,c.custId,c.custmobile from tblCustomer c
                        inner join tblBank b on c.BankId = b.bankId
                        inner join loanApproval la on c.custId = la.custId where c.custId=@custid and b.bankId=@bankid";

            var qry2 = @"select bankname,c.custname,c.custmobile,l.loanType from tblCustomer c 
                                inner join tblBank b on c.BankId=b.bankId
                                inner join loanApproval la on c.custId=la.custId 
                                inner join tblLoans l on la.loanId=l.loanId where b.bankId=@bankid";

            using (var con = context.CreateConnection())
            {
                var ltype = new Combine();

                if (custid == 0&& bankid==0)
                {
                    var loaninfo = await con.QueryAsync<Combine>(qry);
                  
                    return loaninfo.ToList();
                }
                else if(custid==0)
                {
                    var loaninfo = await con.QueryAsync<Combine>(qry2, new {bankid});
                        return loaninfo.ToList();
                }
                else
                {
                     var loaninfo = await con.QuerySingleAsync<Combine>(qry1, new { custid,bankid });
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

        public async Task<int> PayLoan(int custid, int loanid, int bankid)
        {
            Loans loans = new Loans();
            var qry = @"Update loanapproval set loanstatus='Paid' 
                        where custId=@custId AND loanid=@loanid AND bankid=@bankid ;
                        select l.loanamount 
                        from tblLoans l join loanApproval la 
                        on l.loanId=la.loanId where bankid=@bankid
                        and custId=@custId and l.loanId=@loanid ";

            using (var con = context.CreateConnection())
            {
                var val = new { custId = custid, loanid = loanid, bankid = bankid };
                var res = await con.QueryMultipleAsync(qry, val);

                var amount = await res.ReadSingleAsync<double>();
                //var amnt = await res.ReadAsync<LoanApproval>();
                

             var r  =await Transcation(custid, amount, "wit");

                return 1;
            }
        }

        public async Task<int> Transcation(int id,double amount, string TranscType)
        {
            var qry = "update tblcustomer set balance=@balance where custId=@custid";
            using (var con = context.CreateConnection())
            {
                var ress = await con.QuerySingleAsync<Customer>("select balance from tblCustomer where custId=@custid", new { custId=id});
                 var bal = ress.balance;
                if (TranscType == "deposite")
                {
                    bal += amount;
                    var res = await con.ExecuteAsync(qry, new { custid = id, balance = bal });
                    return 1;

                }
                else
                {
                    if (bal > amount)
                    {
                        bal-= amount;
                        var res = await con.ExecuteAsync(qry, new { custid = id, balance = bal });
                        return 0;
                    }
                    else
                    {
                        return 2;
                    }
                }
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
