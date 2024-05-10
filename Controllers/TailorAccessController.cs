using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tailors_fitv0._2.Models;

namespace Tailors_fitv0._2.Controllers
{
    [Authorize(Roles="Tailor")]
    [Route("api/[controller]")]
    [ApiController]
    public class TailorAccessController : ControllerBase
    {
        private readonly UserManager<AllApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        public TailorAccessController(UserManager<AllApplicationUser> userManager,  ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpGet]
        [Route("GetTailorDetails/{username}")]
        public async Task<ActionResult<tailorModel>> GetTailorDetails(string username)
        {
            return await _context.tailors.FindAsync(username);
        }
        [HttpGet]
        [Route("GetMyFeedback/{username}")]
        public async Task<ActionResult<Tailors_Feedbacks>> GetMyFeedback(string username)
        {
            /*var res = new List<Tailors_Feedbacks>();
            foreach (var comment in _context.comments)
            {
                if (comment.username == username) { res.Add(comment); }
            }
            return Ok(new { Result = "1", CommentList = res });*/
            var res = new List<Tailors_Feedbacks>();
            var comms = await _context.comments.ToListAsync();
            foreach (var com in comms)
            {
                if (com.username == username)
                    res.Add(com);
            }
            return Ok(new { Result = "1", CommentList = res });
        }
        [HttpPut]
        [Route("UpdateMyDetails")]
        public async Task<ActionResult> UpdateMyDetail(tailorModel user)
        {
            var _user = await _userManager.FindByNameAsync(user.UserName);
            var __user = await _context.tailors.FindAsync(user.UserName);
            if (_user == null || __user == null) { return BadRequest(new { Result = "0", Messsage = "Bad Request!" }); }
            _user.Email = user.email;
            __user.email = user.email;
            _user.PhoneNumber = user.phone_No;
            __user.phone_No = user.phone_No;
            __user.address = user.address;
            __user.Age = user.Age;
            __user.Name = user.Name;
            __user.Services_type = user.Services_type;
            try
            {
                await _userManager.UpdateAsync(_user);
                await _context.SaveChangesAsync();
                return Ok(new { Result = "1", Message = "Your Details have been updated!" });
            }
            catch
            {
                return BadRequest(new { Result = "0", Message = "Bad Request!" });
            }
        }
    }
}
