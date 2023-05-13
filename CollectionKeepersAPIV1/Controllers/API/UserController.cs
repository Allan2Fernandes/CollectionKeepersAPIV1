using CollectionKeepersAPIV1.Controllers.ControllerLogic;
using CollectionKeepersAPIV1.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text.RegularExpressions;

namespace CollectionKeepersAPIV1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        CollectionsDbContext ctx;
        UserServices UserServices;

        public UserController(CollectionsDbContext context) 
        { 
            ctx = context;
            UserServices = new UserServices(ctx);
        }

        [HttpGet(nameof(GetAllUsers))]
        public async Task<ActionResult<List<TblUser>>> GetAllUsers()
        {
            var ListOfUsers = UserServices.GetAllUsers();
            return Ok(ListOfUsers);
        }

        [HttpPost(nameof(CreateNewUser))]
        public async Task<ActionResult<string>> CreateNewUser(TblUser NewUser)
        {

            if (!Functions.Functions.CheckIfValidEmail(NewUser.FldEmail))
            {
                return Ok("Invalid email");
            }
            string Email = NewUser.FldEmail;

            //TODO: Check that the email id doesn't already exist in the db before adding a new user with that email
            List<TblUser> QueriedUsers = UserServices.GetUsersOnEmail(Email);

            if (QueriedUsers.Count != 0)
            {
                return Ok("Email ID already in DB");
            }

            UserServices.AddUserToDB(NewUser);
            return Ok("NewUser Posted");
        }

        [HttpPost(nameof(Login))]
        public async Task<ActionResult<string>> Login(TblUser User)
        {
            string Email = User.FldEmail;
            string Password = User.FldPassword;
            string ErrorMessage = "Invalid credentials entered";

            List<TblUser> QueriedUsers = UserServices.GetUsersOnEmail(Email);

            if (QueriedUsers.Count == 0)
            {
                return Ok(ErrorMessage);
            }

            TblUser QueriedUser = QueriedUsers.First();

            if (Password == QueriedUser.FldPassword)
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
            List<TblUser> QueriedAccounts = UserServices.GetUsersOnID(NewUserDetails.FldUserId);

            if(QueriedAccounts.Count == 0)
            {
                return Ok("Account not found");
            }
            TblUser FoundUser = QueriedAccounts.First();
            
            if (!Functions.Functions.CheckIfValidEmail(NewUserDetails.FldEmail))
            {
                return Ok("New Email is invalid");
            }

            UserServices.UpdateUserDetails(FoundUser, NewUserDetails);

            return Ok("User Updated");
        }

        [HttpDelete(nameof(DeleteUserOnID) + "/{UserID}")]
        public async Task<ActionResult<string>> DeleteUserOnID(int UserID)
        {
            //Find the user
            List<TblUser> QueriedAccounts = UserServices.GetUsersOnID(UserID);

            if (QueriedAccounts.Count == 0)
            {
                return Ok("Account not found");
            }

            //Find all the Collections
            List<TblCollection> ConnectedCollections = await ctx.TblCollections.Where(row => row.FldUserId == UserID).ToListAsync();
            List<int> ListOfCollectionIDs = ConnectedCollections.Select(row => row.FldCollectionId).ToList();
            
            
            //Find all the attributes
            List<TblAttribute> ConnectedAttributes = await ctx.TblAttributes.Where(row => ListOfCollectionIDs.Contains((int)row.FldCollectionId)).ToListAsync();
            List<int> ListOfAttributeIDs = ConnectedAttributes.Select(row => row.FldAttributeId).ToList();
            
            
            //Find all the attribute values
            List<TblAttributeValue> ConnectedAttributeValues = await ctx.TblAttributeValues.Where(row => ListOfAttributeIDs.Contains((int)row.FldAttributeId)).ToListAsync();  
            List<int> ListOfDistinctCollectionEntryIDs = ConnectedAttributeValues.Select(row => (int)row.FldCollectionEntryId).Distinct().ToList();
            
            
            //Find all the collection entries
            List<TblCollectionEntry> ConnectedCollectionEntries = await ctx.TblCollectionEntries.Where(row => ListOfDistinctCollectionEntryIDs.Contains((int)row.FldCollectionEntryId)).ToListAsync();
                        

            ctx.TblAttributeValues.RemoveRange(ConnectedAttributeValues);
            await ctx.SaveChangesAsync();

            ctx.TblCollectionEntries.RemoveRange(ConnectedCollectionEntries);
            await ctx.SaveChangesAsync();

            ctx.TblAttributes.RemoveRange(ConnectedAttributes);
            await ctx.SaveChangesAsync();

            ctx.TblCollections.RemoveRange(ConnectedCollections);
            await ctx.SaveChangesAsync();

            TblUser FoundUser = QueriedAccounts.First();

            ctx.TblUsers.Remove(FoundUser);
            await ctx.SaveChangesAsync();

            return Ok("User successfully deleted");
        }
    }


}
