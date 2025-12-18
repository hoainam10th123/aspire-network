using Microsoft.AspNetCore.Mvc;
using PostService.Service;


namespace PostService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ErrorsController(IKeycloakService keycloak) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetDetailUser()
        {
            var data = await keycloak.GetUserByUserIdAsync("202e2c23-8e27-432c-becc-2f03fe84e229");

            return Ok(data);
        }
    }
}
