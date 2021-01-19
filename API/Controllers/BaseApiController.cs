using API.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    // Appliqer me filter sur l'ensemble des controllers derivant de BaseApiController
    [ServiceFilter(typeof(LogUserActivity))]
    [ApiController]
    [Route("api/[controller]")]
    public class BaseApiController : ControllerBase
    {
        
    }
}