using Demo.Services;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Controllers;

[Route("[controller]/[action]")]
public class TestRequestController : ControllerBase
{
    private TestRequestService _testRequestService;

    public TestRequestController(TestRequestService testRequestService)
    {
        _testRequestService = testRequestService;
    }

    [HttpPost]
    public async Task<IActionResult> Add()
    {
        await _testRequestService.Add();
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        return Ok(await _testRequestService.Get());
    }

    [HttpPut]
    public async Task<IActionResult> Approve(Guid id)
    {
        return Ok(await _testRequestService.Approve(id, Guid.NewGuid(), "wow!"));
    }
}
