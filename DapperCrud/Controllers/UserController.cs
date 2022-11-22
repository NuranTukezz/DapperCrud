using Dapper;
using DapperCrud;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Xml.Linq;

namespace DapperCrud.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _config;//Sql bağlantısını bu şekilde çağırıyorsun

        public UserController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        public async Task<ActionResult<List<Model>>> GetAllModels()
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            IEnumerable<Model> builder = await SelectAllModels(connection);
            return Ok(builder);
        }

        [HttpGet("{Id}")]
        public async Task<ActionResult<List<Model>>> GetAllId(int Id)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            var build = await connection.QueryAsync<Model>("select * from tbl_Users where Id = @Id",
            new {Id=Id});
            return Ok(build);
        }

        [HttpPost]
        public async Task<ActionResult<List<Model>>> CreatelModels(Model m)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

            await connection.ExecuteAsync(" insert into tbl_Users ( Name , FirstName , LastName , City ) values( @Name , @FirstName , @LastName , @City  )", m);//sql'de tabloda olmayan bir column olmamalı
            

            return Ok(await SelectAllModels(connection));
        }

        [HttpPut]
        public async Task<ActionResult<List<Model>>> UpdateModels(Model m)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

            await connection.ExecuteAsync(" update tbl_Users  set Name = @Name ,FirstName =@FirstName, LastName = @LastName , City = @City where Id=@Id ", m);//new  

            return Ok(await SelectAllModels(connection));
        }

        [HttpDelete("{Id}")]
        public async Task<ActionResult<List<Model>>> DeleteModels(int Id)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

            await connection.ExecuteAsync("delete from tbl_Users Where Id = @Id", new {Id = Id} ); 

            return Ok(await SelectAllModels(connection));
        }

        private static async Task<IEnumerable<Model>> SelectAllModels(SqlConnection connection)
        {
            return await connection.QueryAsync<Model>("select * from tbl_Users");
        }


    }
}



//using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));  
//var builder = await connection.QueryAsync<Model>("select * from tbl_Users");
//return Ok(builder);