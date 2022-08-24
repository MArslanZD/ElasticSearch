using Microsoft.AspNetCore.Mvc;
using Nest;

namespace ElasticSearch_API.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IElasticClient _elasticClient;

        public UserController(IElasticClient elasticClient) => _elasticClient = elasticClient;

        [HttpGet("allusers")]
        public async Task<IEnumerable<User>?> FetchAllUsers()
        {
            var response = await _elasticClient.SearchAsync<User>(s => s.Index("users").Query(q => q.MatchAll()));
            return response?.Documents;
        }

        [HttpGet("userbyid")]
        public async Task<User?> FetchUserById([FromQuery] int id)
        {

            //var response = await _elasticClient.GetAsync<User>(new DocumentPath<User>(new Id(id)), s => s.Index("users"));
            //return response?.Source;

            var response = await _elasticClient.SearchAsync<User>(s => s.Index("users").Query(q => q.MatchAll()));
            return response?.Documents.Where(s => s.Id == id).FirstOrDefault();

        }

        [HttpPost("adduser")]
        public async Task<string> AddUsers([FromBody] User user)
        {
            var response = await _elasticClient.IndexAsync(user, s => s.Index("users"));
            return response.Id;
        }

        [HttpPut("updateuser")]
        public async Task<string> UpdateUser([FromBody] User user, [FromQuery] string id)
        {
            var response = await _elasticClient.UpdateAsync<User>(id, s => s.Index("users").Doc(user));

            // Update Through Index
            // var response = await _elasticClient.IndexAsync(user, s => s.Index("users").Id(id)); 

            return response.Id;
        }

        [HttpDelete("deleteuser")]
        public async Task<string> DeleteUser([FromQuery] string id)
        {
            var response = await _elasticClient.DeleteAsync<User>(id, d => d.Index("users"));
            return response.Id;
        }

    }
}
