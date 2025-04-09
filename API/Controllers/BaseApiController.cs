
using API.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
	[ApiController]
	[ServiceFilter(typeof(LogUserActivity))]
	[Route("api/[controller]")]
	public class BaseApiController : ControllerBase
	{
	}
}