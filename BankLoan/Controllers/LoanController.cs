using BankLoan.Model;
using BankLoan.Repository.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BankLoan.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoanController : ControllerBase
    {
        private readonly IBankRepository bankRepository;
        private readonly ICustomerRepository customerRepository;
        public LoanController(IBankRepository bankRepository, ICustomerRepository customerRepository)
        {
            this.bankRepository = bankRepository;
            this.customerRepository = customerRepository;
        }
        [HttpPost]
        public async Task<IActionResult> ApplyForLoan(LoanApproval loanApproval)
        {
            try
            {
                var res=await bankRepository.LoanApproval(loanApproval);
                return Ok(res);
            }
            catch(Exception ex)
            {
                return StatusCode(500,ex.Message);
            }
        }
        [HttpGet("custid")]
        public async Task<IActionResult> GetCustomerLoanInfo(int custid,int bankid)
        {
            try
            {
                var loaninfo = await customerRepository.GetCustomerLoanById(custid, bankid);
                return Ok(loaninfo);
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
