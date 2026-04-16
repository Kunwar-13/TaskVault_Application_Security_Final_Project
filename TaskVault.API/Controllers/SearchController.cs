using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TaskVault.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class SearchController : ControllerBase
{

}
