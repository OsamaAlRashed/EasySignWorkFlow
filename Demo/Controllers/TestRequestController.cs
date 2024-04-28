using Demo.Services;
using EasySignWorkFlow.Extensions;
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
    public async Task<IActionResult> Add() => Ok(await _testRequestService.Add());

    [HttpGet]
    public async Task<IActionResult> Get() => Ok(await _testRequestService.Get());

    [HttpGet]
    public async Task<IActionResult> GetByStatus() => Ok(await _testRequestService.GetByStatus());

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id) => Ok(await _testRequestService.Get(id));

    [HttpPut]
    public async Task<IActionResult> Approve(Guid id) => Ok(await _testRequestService.Approve(id, Guid.NewGuid(), "wow!"));

    [HttpPut]
    public async Task<IActionResult> Refuse(Guid id) => Ok(await _testRequestService.Refuse(id, Guid.NewGuid(), "wow!"));

    [HttpPut]
    public async Task<IActionResult> Undo(Guid id, Guid undoBy) => Ok(await _testRequestService.Undo(id, undoBy));

    [HttpPut]
    public async Task<IActionResult> Cancel(Guid id) => Ok(await _testRequestService.Cancel(id, Guid.NewGuid(), "wow!"));

    [HttpPut]
    public async Task<IActionResult> Reset(Guid id) => Ok(await _testRequestService.Reset(id, Guid.NewGuid(), "wow!"));

    [HttpDelete]
    public async Task<IActionResult> Clear()
    {
        await _testRequestService.Clear();
        return Ok();
    }

    [HttpGet]
    public IActionResult Print() => Ok(_testRequestService.Print());

    [HttpGet("{id}")]
    public async Task<IActionResult> Print(Guid id) => Ok(await _testRequestService.Print(id));

    [HttpGet]
    public async Task<IActionResult> GetNextUsers(Guid id)
    {
        var result = await _testRequestService.GetNextUsers(id);

        return Ok(result.ToList());
    }
}
