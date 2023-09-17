using Demo.Services;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Controllers
{
    [Route("[controller]")]
    public class LeaveController : ControllerBase
    {
        private readonly CashRequestService _cashRequestService;

        public LeaveController(CashRequestService cashRequestService)
        {
            _cashRequestService = cashRequestService;
        }

        [HttpPut]
        public async Task<IActionResult> Approve(Guid id)
        {
            await _cashRequestService.Approve(id, "wow!");

            return Ok();
        }
    }
}
