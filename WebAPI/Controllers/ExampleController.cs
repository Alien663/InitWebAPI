using Microsoft.AspNetCore.Mvc;
using Sample.Service1.Interfaces;
using Sample.Service2.Interfaces;
using WebAPI.Filter;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[Login] // this will call Login Filter, you can also use in fron of api instead of controller
public class ExampleController : ControllerBase
{
    private readonly ISampleServices1 _sampleService1;
    private readonly IMyHttpClient _myHttpClient;

    public ExampleController(ISampleServices1 sampleService1, IMyHttpClient myHttpClient)
    {
        _sampleService1 = sampleService1;
        _myHttpClient = myHttpClient;
    }

    [HttpGet]
    public IActionResult GetExampleData()
    {
        _sampleService1.UpdateData();
        return Ok();
    }

    [HttpPost]
    public IActionResult PostExampleData()
    {
        _myHttpClient.PostSample();
        return Ok();
    }

    [HttpPut]
    public IActionResult PutExampleData()
    {
        _sampleService1.UpdateData();
        _myHttpClient.GetSample(new Dictionary<string, string> { { "key", "value" } });
        return Ok();
    }
}
