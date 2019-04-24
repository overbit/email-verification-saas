using System;
using System.Diagnostics;
using System.Net;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using EmailVerificationService.Extensions;
using EmailVerificationService.Models;
using EmailVerificationService.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters.Internal;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;

namespace EmailVerificationService.Controllers
{
    [Route("api/verification")]
    [ApiController]
    public class VerificationController : ControllerBase
    {

        private readonly UserManager<ApplicationUser> userManager;
        private IIdentityMessageService messageService;

        public VerificationController(UserManager<ApplicationUser> userManager, IIdentityMessageService messageService)
        {
            this.userManager = userManager;
            this.messageService = messageService;
        }

        // GET api/verification/
        [HttpGet("{email}")]
        public async Task<ActionResult<bool>> Get(string email)
        {
            var existingEntry = await userManager.FindByEmailAsync(email);
            var res = new VerificationResponseDto
            {
                IsVerified = existingEntry != null && existingEntry.EmailConfirmed
            };
            return new JsonResult(res);
        }

        // POST api/verification
        [HttpPost]
        public async Task<ActionResult> Post(VerificationRequestDto dto)
        {
            if (dto?.Email == null)
            {
                return new StatusCodeResult((int)HttpStatusCode.BadRequest);
            }

            var code = string.Empty;
            var existingEntry = await userManager.FindByEmailAsync(dto.Email);
            if (existingEntry != null)
            {
                code = await userManager.GenerateEmailConfirmationTokenAsync(existingEntry);
            }
            else
            {
                var user = new ApplicationUser { UserName = dto.Email, Email = dto.Email};
                var res = await userManager.CreateAsync(user);
                if (res.Succeeded)
                {
                    code = await userManager.GenerateEmailConfirmationTokenAsync(user);
                }
            }

            var confirmEmailUrl = Url.EmailConfirmationLink(dto.Email, code, HttpScheme.Https.ToString());
            var message = new IdentityMessage
            {
                Destination = dto.Email,
                Subject = "Confirm your email",
                Body = $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(confirmEmailUrl)}'>clicking here</a>."
            };

            await messageService.SendAsync(message);

            return new StatusCodeResult((int)HttpStatusCode.OK);
        }

        // POST api/verification
        [HttpGet("confirm")]
        public async Task<ActionResult> ConfirmEmail(string email, string code)
        {
            if (email == null || code == null)
            {
                return new StatusCodeResult((int)HttpStatusCode.BadRequest);
            }

            var entry = await userManager.FindByEmailAsync(email);
            if (entry == null)
            {
                return new StatusCodeResult((int)HttpStatusCode.NotFound);
            }

            var result = await userManager.ConfirmEmailAsync(entry, code);
            Debug.WriteLine($"id = {email}, code={code}");

            return new StatusCodeResult((int)HttpStatusCode.OK);
        }

        // DELETE api/values/5
        //        [HttpDelete("{id}")]
        //        public void Delete(int id)
        //        {
        //        }
    }
}