using BankLoan.Model;
using BankLoan.Repository.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BankLoan.Controllers
{
    [Route("BankLoan/[controller]")]
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
        [HttpPut("Transcation")]
        public async Task<IActionResult> Transcation(int id, double amount, string TranscType)
        {
            BaseResponse baseResponse = new BaseResponse();
            try
            {
                var res = await customerRepository.Transcation(id,amount, TranscType);
                if(res==2)
                {
                    baseResponse.StatusCode = StatusCodes.Status404NotFound.ToString();
                    baseResponse.StatusMessage = "Something went wronng";
                }else if(res==1)
                {
                    baseResponse.StatusMessage = $"{amount} Deposite Succesfully";
                }
                else 
                {
                    baseResponse.StatusMessage = $"Withdraw {amount} rupees";
                }
                return Ok(baseResponse);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }
        [HttpGet("Pagination")]
        public async Task<IActionResult> GetAllByPagination(int pageno, int pageSize)
        {
            var res=await customerRepository.GetAllByPagination(pageno,pageSize);
            if(res.ResponseData1==null)
           return BadRequest(res.ResponseData1);
            return Ok(res);
        }
        [HttpPost("PayLoan")]
        public async Task<IActionResult> PayLoan(int custid, int loanid, int bankid)
        {
            BaseResponse baseResponse = new BaseResponse();
            var res = await customerRepository.PayLoan(custid, loanid, bankid);
            if (res != 1)
            {
                baseResponse.StatusCode = StatusCodes.Status400BadRequest.ToString();
                baseResponse.StatusMessage = "Error";
            }
            else
            {
                baseResponse.StatusMessage = "Succesful";
            }
            return Ok(baseResponse);
        }
    }

}
