using CollectionKeepersAPIV1.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace CollectionKeepersAPIV1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        CollectionsDbContext ctx; 


        public UserController(CollectionsDbContext context) 
        { 
            ctx = context;
        }

        [HttpGet(nameof(GetAllUsers))]
        public async Task<ActionResult<List<TblUser>>> GetAllUsers()
        {
            List<TblUser> users = ctx.TblUsers.ToList();    
            return Ok(users);
        }

        [HttpPost(nameof(CreateNewUser))]
        public async Task<ActionResult<string>> CreateNewUser(TblUser NewUser)
        {
            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            if(!Regex.IsMatch(NewUser.FldEmail, emailPattern))
            {
                return Ok("Invalid email");
            }
            string Email = NewUser.FldEmail;

            //TODO: Check that the email id doesn't already exist in the db before adding a new user with that email
            List<TblUser> QueriedUsers = await ctx.TblUsers.Where(User => User.FldEmail == Email).ToListAsync();

            if(QueriedUsers.Count != 0)
            {
                return Ok("Email ID already in DB");
            }

            await ctx.TblUsers.AddAsync(NewUser);
            await ctx.SaveChangesAsync();
            return Ok("NewUser Posted");
        }

        [HttpPost(nameof(Login))]
        public async Task<ActionResult<string>> Login(TblUser User)
        {
            string Email = User.FldEmail;
            string Password = User.FldPassword;
            string ErrorMessage = "Invalid credentials entered";

            List<TblUser> QueriedUsers = await ctx.TblUsers.Where(User => User.FldEmail == Email).ToListAsync();

            if(QueriedUsers.Count == 0)
            {
                return Ok(ErrorMessage);
            }

            TblUser QueriedUser = QueriedUsers.First();

            if(Password == QueriedUser.FldPassword)
            {
                return Ok(QueriedUser);
            }
            else
            {
                return Ok(ErrorMessage);
            }
        }

        [HttpPut(nameof(UpdateUser))]
        public async Task<ActionResult<string>> UpdateUser(TblUser NewUserDetails)
        {
            List<TblUser> QueriedAccounts = await ctx.TblUsers.Where(User => User.FldUserId == NewUserDetails.FldUserId).ToListAsync();

            if(QueriedAccounts.Count == 0)
            {
                return Ok("Account not found");
            }

            TblUser FoundUser = QueriedAccounts.First();

            //Update all details for this account
            FoundUser.FldUsername = NewUserDetails.FldUsername;
            FoundUser.FldPassword = NewUserDetails.FldPassword;
            FoundUser.FldEmail = NewUserDetails.FldEmail;

            //Save the changes
            await ctx.SaveChangesAsync();

            return Ok("User Updated");
        }

        [HttpDelete(nameof(DeleteUserOnID) + "/{UserID}")]
        public async Task<ActionResult<string>> DeleteUserOnID(int UserID)
        {
            //Find the user
            List<TblUser> QueriedAccounts = await ctx.TblUsers.Where(User => User.FldUserId == UserID).ToListAsync();

            if (QueriedAccounts.Count == 0)
            {
                return Ok("Account not found");
            }

            TblUser FoundUser = QueriedAccounts.First();

            ctx.TblUsers.Remove(FoundUser);
            await ctx.SaveChangesAsync();

            return Ok("User successfully deleted");
        }
    }


}
