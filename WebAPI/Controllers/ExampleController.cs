using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using WebAPI.Lib;
using WebAPI.Models;
using WebAPI.Filter;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [Login] // this will call Login Filter, you can also use in fron of api instead of controller
    public class ExampleController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetExampleData()
        {
            using(var db = new AppDb())
            {
                string sql = "select * from TT";
                List<TTModel> data = db.Connection.Query<TTModel>(sql).ToList();
                return Ok(data);
            }
        }

        [HttpPost]
        public IActionResult PostExampleData([FromBody] TTModel payload)
        {
            using (var db = new AppDb())
            {
                string sql = "xp_TTupdate";
                DynamicParameters p = new DynamicParameters();
                p.Add("@TID", payload.TID);
                p.Add("@TName", payload.TName);
                p.Add("@TDes", payload.TDes);
                db.Connection.Execute(sql, p, commandType: CommandType.StoredProcedure);
            }
            return Ok();
        }

        [HttpPut]
        public IActionResult PutExampleData([FromBody] TTModel payload)
        {
            using (var db = new AppDb())
            {
                string sql = "xp_TTinsert";
                DynamicParameters p = new DynamicParameters();
                p.Add("@TID", payload.TID, direction: ParameterDirection.Output);
                p.Add("@TName", payload.TName);
                p.Add("@TDes", payload.TDes);
                db.Connection.Execute(sql, p, commandType: CommandType.StoredProcedure);
            }
            return Ok(new { payload.TID });
        }

        [HttpDelete]
        public IActionResult DeleteExampleData([FromBody] TTModel payload)
        {
            using (var db = new AppDb())
            {
                string sql = "xp_TTdelete";
                DynamicParameters p = new DynamicParameters();
                p.Add("@TID", payload.TID);
                db.Connection.Execute(sql, p);
            }
            return Ok();
        }
    }
}
