using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PsychedelicExperience.Common.Messages;

namespace PsychedelicExperience.Web.Controllers
{
    public class UserViewsController : ViewController
    {
        public UserViewsController(IMediator messageDispatcher, IConfiguration configuration) : base(messageDispatcher, configuration)
        {
        }

        [HttpGet("~/user/")]
        public Task<IActionResult> Get()
        {
            return Task.FromResult(ViewWithState());
        }

        [HttpGet("~/user/invite")]
        public Task<IActionResult> Invite()
        {
            return Task.FromResult(ViewWithState());
        }

        [HttpGet("~/user/{id}")]
        public Task<IActionResult> GetById(Guid id)
        {
            return Task.FromResult(ViewWithState());
        }
    }
}
