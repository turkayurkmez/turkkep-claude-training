using Microsoft.AspNetCore.Mvc;
using Api.Models;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class CustomersController : ControllerBase
{
    private static readonly List<Customer> _customers =
    [
        new Customer { Id = 1, Name = "Ahmet Yılmaz",  Email = "ahmet@example.com"  },
        new Customer { Id = 2, Name = "Ayşe Kaya",     Email = "ayse@example.com"   },
        new Customer { Id = 3, Name = "Mehmet Demir",  Email = "mehmet@example.com" },
    ];

    [HttpGet]
    public IEnumerable<Customer> Get() => _customers;
}
