using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tailors_fitv0._2.Models;

namespace Tailors_fitv0._2.Controllers
{
    [Authorize(Roles = "User")]
    [Route("api/[controller]")]
    [ApiController]
    public class UserAccessController : ControllerBase
    {

        private readonly UserManager<AllApplicationUser> _userManager; 
        private readonly ApplicationDbContext _context;
        public UserAccessController(UserManager<AllApplicationUser> userManager, ApplicationDbContext context) 
        {
            _userManager = userManager;
            _context = context;
        }
        [HttpGet]
        [Route("GetAlltailorslist")]
        public async Task<ActionResult<IEnumerable<tailorModel>>> GetTailors()
        {
            var tailors = await _context.tailors.ToListAsync();
            var Tailors_Details = new List<tailorModel>();
            foreach(var user in tailors)
            {
                Tailors_Details.Add(user);
            }
            if ((Tailors_Details.Count == 0)) return Ok(new { Result = "0" }); 
            return  Ok(new {Tailors = Tailors_Details, Result = "1" });
        }
        [HttpPut]
        [Route("UserUpdateDetails")]
        public async Task<ActionResult<UserModel>> UserUpdateDetails(UserModel user)
        {
            var _user = await _userManager.FindByNameAsync(user.UserName);
            var __user = await _context.users.FindAsync(user.UserName);
            if (_user == null || __user == null) { return BadRequest(new {Result = "0",Messsage="Bad Request!" }); }
            _user.Email = user.email;
            __user.email = user.email;
            _user.PhoneNumber = user.phone_No;
            __user.phone_No = user.phone_No;
            __user.address = user.address;
            __user.Age = user.Age;
            __user.Name = user.Name;
            try
            {
                await _userManager.UpdateAsync(_user);
                await _context.SaveChangesAsync();
                return Ok(new {Result="1", Message="Your Details have been updated!"});
            }
            catch
            {
                return BadRequest(new { Result="0", Message="Bad Request!"});
            }
        }
        [HttpPost]
        [Route("PostComment")]
        public async Task<ActionResult<Tailors_Feedbacks>> PostTailorComment(Tailors_Feedbacks comment_)
        {
            _ = await _context.comments.AddAsync(comment_);
            await _context.SaveChangesAsync();
            return comment_;
        }
        [HttpGet]
        [Route("Get/{username}/comments")]
        public async Task<ActionResult<Tailors_Feedbacks>> getTailorComments(string username)
        {
            var res = new List<Tailors_Feedbacks>();
            var comms = await _context.comments.ToListAsync();
            foreach(var com in comms)
            {
                if(com.username==username)
                res.Add(com);
            }
            return Ok(new {Result="1",CommentList=res});
        }

    }
}
