using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PsychedelicExperience.Common.Messages;

namespace PsychedelicExperience.Web.Controllers
{
    public class HomeViewsController : ViewController
    {
        public HomeViewsController(IMediator mediator, IConfiguration configuration) : base(mediator, configuration)
        {
        }

        [HttpGet("~/")]
        [HttpGet("~/admin")]
        public IActionResult RenderView() => ViewWithState();

        [HttpGet("~/error")]
        public ActionResult Error() => View();
    }
}