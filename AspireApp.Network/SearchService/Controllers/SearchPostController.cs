using Microsoft.AspNetCore.Mvc;
using SearchService.Models;
using Typesense;

namespace SearchService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SearchPostController(ITypesenseClient client) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Search(string query)
        {
            var searchParams = new SearchParameters(query, "title,content");

            try
            {
                var result = await client.Search<SearchPost>("posts", searchParams);
                return Ok(result.Hits.Select(hit => hit.Document));
            }
            catch (Exception e)
            {
                return BadRequest(new { msg = e.Message });
            }
        }
    }
}
