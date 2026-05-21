using Microsoft.AspNetCore.Mvc;
using Api.Models;
using Api.Services;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class CustomersController(ICustomerService customerService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<Customer>>> Get(
        [FromQuery] PagedQuery query, CancellationToken cancellationToken)
        => Ok(await customerService.GetPagedAsync(query, cancellationToken));

    [HttpGet("{id}")]
    public async Task<ActionResult<Customer>> GetById(
        int id, CancellationToken cancellationToken)
    {
        var customer = await customerService.GetByIdAsync(id, cancellationToken);
        return customer is null ? NotFound() : Ok(customer);
    }
}
