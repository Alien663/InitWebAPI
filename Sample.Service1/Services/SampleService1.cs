using Sample.Service1.Entities;
using Sample.Service1.Interfaces;

namespace Sample.Service1.Services;

public class SampleService1 : ISampleServices1
{
    private readonly SampleContext _db;

    public SampleService1(SampleContext db)
    {
        _db = db;
    }

    public void UpdateData()
    {

    }
}
