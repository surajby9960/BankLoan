using BankLoan.Model;
using BankLoan.Repository.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BankLoan.Controllers
{
    [Route("BankLoan/[controller]")]
    [ApiController]
    public class BankController : ControllerBase
    {
        private readonly IBankRepository bankRepository;
        public BankController(IBankRepository bankRepository)
        {
            this.bankRepository = bankRepository;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllBank()
        {

            try
            {
                BaseResponse baseResponse = new BaseResponse();
                var bank = await bankRepository.GetAllBank();

                if (bank == null)
                {
                    baseResponse.StatusCode = StatusCodes.Status404NotFound.ToString();
                    baseResponse.StatusMessage = "No Record Found";
                   
                    return Ok(baseResponse);
                }
                else
                {
                    baseResponse.StatusCode = StatusCodes.Status200OK.ToString();
                    baseResponse.StatusMessage = "All data fetch Succesfully";
                    baseResponse.ResponseData1 = bank;
                    return Ok(baseResponse);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("id")]
        public async Task<IActionResult> GetBankById(int id)
        {
            try
            {
                var bank=await bankRepository.GetBankById(id);
                if (bank == null)
                    return BadRequest("No data found");
                return Ok(bank);
            }catch(Exception ex)
            {
                return StatusCode(500,ex.Message);
            }
        }
        [HttpPost]
        public async Task<IActionResult> AddNewBank(Bank bank)
        {
            try
            {
                BaseResponse baseResponse = new BaseResponse();
                var res=await bankRepository.AddNewBank(bank);
                if(res<0)
                {
                    baseResponse.StatusCode=StatusCodes.Status404NotFound.ToString();
                    baseResponse.StatusMessage = "Error While Inserting";
                    return BadRequest(baseResponse);
                }
                else
                {
                    baseResponse.StatusCode=StatusCodes.Status200OK.ToString();
                    baseResponse.StatusMessage = " Data Inserted Succesfully";
                    return Ok(baseResponse);
                }
            }
            catch(Exception ex)
            {
                return StatusCode(404, ex.Message);
            }
        
        }
        [HttpDelete("id")]
        public async Task<IActionResult> DeleteBank(int bankid,int? custid)
        {
            BaseResponse baseResponse=new BaseResponse();
            var res=await bankRepository.DeleteBank(bankid, custid);
            if(res==0)
            {
                baseResponse.StatusCode = StatusCodes.Status404NotFound.ToString();
                baseResponse.StatusMessage = "Error while deleting";
                return Ok(baseResponse);
            }
            else
            {
                baseResponse.StatusCode = StatusCodes.Status200OK.ToString();
                baseResponse.StatusMessage = "Deleting Succesfully";
                return Ok(baseResponse);
            }
           
        }
        [HttpPut]
        public async Task<IActionResult> UpdateBank(Bank bank)
        {

            BaseResponse baseResponse=new BaseResponse();
            var res=await bankRepository.UpdateBank(bank);
            if(res==0)
            {
                baseResponse.StatusCode = StatusCodes.Status404NotFound.ToString();
                baseResponse.StatusMessage="Error while updating data";
                return Ok(baseResponse);
            }
            else
            baseResponse.StatusCode = StatusCodes.Status200OK.ToString();
            baseResponse.StatusMessage = "Data updated succesfullly";
            return Ok(baseResponse);
        }
        [HttpPost("/AccountOpen")]
        public async Task<IActionResult> AccountOpen(Customer customer)
        {
            try
            {
                var res = await bankRepository.AccountOpen(customer);
                if (res == 0)
                    return StatusCode(500, "Error in Opening Account");
                return StatusCode(200, "Account Open Succesfully");
            }catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
       
    }
}
