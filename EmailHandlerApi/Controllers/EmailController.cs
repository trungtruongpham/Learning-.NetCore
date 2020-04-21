using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace EmailHandlerApi.Controllers {
    [Route("/api/Email")]
    public class EmailController : ControllerBase {
        private readonly IEmailService _emailService;

        public EmailController(IEmailService emailService) => this._emailService = emailService;

        [HttpPost]
        public Task SendEmail(string toName, string toEmail,string subject, string message){
            return _emailService.SendEmailAsync( toName, toEmail,subject, message);
        }
    }
}