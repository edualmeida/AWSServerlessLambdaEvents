
using Microsoft.AspNetCore.Mvc;
using AWSServerless2.Application.Services;
using AWSServerless2.Domain.Models;

namespace AWSServerless2.Controllers;

[Route("api/[controller]")]
public class EventController : ControllerBase
{
    private readonly IEventService _eventService;

    public EventController(IEventService eventService)
    {
        _eventService = eventService ?? throw new ArgumentNullException(nameof(eventService));
    }

    // GET api/values
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _eventService.GetAllAsync());
    }

    // GET api/values/daterange/
    [HttpGet("daterange")]
    public async Task<IActionResult> GetByDateRange(DateTime start, DateTime end)
    {
        //return Ok(await _eventService.GetByDateRangeAsync(start, end));
        return Ok(await _eventService.GetAllAsync()); 
    }

    // POST api/values
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Event request)
    {
        await _eventService.CreateAsync(request);

        return Ok();
    }

    // PUT api/values/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] Event request)
    {
        await _eventService.UpdateAsync(id, request);

        return Ok(true);
    }

    // DELETE api/values/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        await _eventService.DeleteAsync(id);

        return Ok();
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetList(DateTime start, DateTime end)
    {
        var listResponse = await _eventService.GetAllAsync();//await _eventService.GetListByDateRangeAsync(start, end);

        return Ok(listResponse);
    }
}