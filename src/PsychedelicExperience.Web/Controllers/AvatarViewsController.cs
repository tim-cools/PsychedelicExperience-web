using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.UserProfiles;
using PsychedelicExperience.Web.Infrastructure.Resources;
using PsychedelicExperience.Common;

namespace PsychedelicExperience.Web.Controllers
{
    public class AvatarViewsController : Controller
    {    
        private readonly IMediator _messageDispatcher;
        private readonly IConfiguration _configuration;

        public AvatarViewsController(IMediator messageDispatcher, IConfiguration configuration)
        {
            _messageDispatcher = messageDispatcher ?? throw new ArgumentNullException(nameof(messageDispatcher));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        /// <summary>
        /// Get the avatar of a specific user.
        /// </summary>
        [HttpGet("~/avatar/{id}")]
        [ProducesResponseType(typeof(File), 200)]
        [ProducesResponseType(typeof(NotFoundResult), 404)]
        public async Task<FileStreamResult> Get(UserProfileId id, int? size = 32)
        {
            var query = new UserProfileAvatarQuery( id);
            var result = await _messageDispatcher.Send(query);
            if (result == null)
            {
                return DefaultAvatar();
            }

            var folder = _configuration.PhotosFolder();
            var fileName = Path.GetFileName(result.FileName);
            var fullPath = Path.Combine(folder, $"h{size}w{size}-{fileName}");

            if (!System.IO.File.Exists(fullPath))
            {
                fullPath = Path.Combine(folder, fileName);
            }

            if (!System.IO.File.Exists(fullPath))
            {
                return DefaultAvatar();
            }

            var stream = System.IO.File.OpenRead(fullPath);

            return new FileStreamResult(stream, "image/" + Path.GetExtension(result.FileName).TrimStart('.'));
        }

        private FileStreamResult DefaultAvatar() => Image(WebResources.DefaultAvatar);

        private static FileStreamResult Image(byte[] data)
        {
            var stream = new MemoryStream(data);
            return new FileStreamResult(stream, "image/png");
        }
    }
}