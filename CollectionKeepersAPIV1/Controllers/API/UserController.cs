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
        UserControllerLogic UserControllerLogic;

        public UserController(CollectionsDbContext context) 
        { 
            ctx = context;
            UserControllerLogic = new UserControllerLogic(ctx);
        }

        [HttpGet(nameof(GetAllUsers))]
        public async Task<ActionResult<List<TblUser>>> GetAllUsers()
        {            
            return Ok(UserControllerLogic.GetAllUsers());
        }

        [HttpPost(nameof(CreateNewUser))]
        public async Task<ActionResult<string>> CreateNewUser(TblUser NewUser)
        {
            
            return Ok(UserControllerLogic.CreateNewUser(NewUser));
        }

        [HttpPost(nameof(Login))]
        public async Task<ActionResult<string>> Login(TblUser User)
        {
            return Ok(UserControllerLogic.Login(User));
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

            //Find all the Collections
            List<TblCollection> ConnectedCollections = await ctx.TblCollections.Where(row => row.FldUserId == UserID).ToListAsync();
            List<int> ListOfCollectionIDs = ConnectedCollections.Select(row => row.FldCollectionId).ToList();
            //Find all the attributes
            List<TblAttribute> ConnectedAttributes = await ctx.TblAttributes.Where(row => ListOfCollectionIDs.Contains((int)row.FldCollectionId)).ToListAsync();
            List<int> ListOfAttributeIDs = ConnectedAttributes.Select(row => row.FldAttributeId).ToList();
            //Find all the attribute values
            List<TblAttributeValue> ConnectedAttributeValues = await ctx.TblAttributeValues.Where(row => ListOfAttributeIDs.Contains((int)row.FldAttributeValueId)).ToListAsync();  
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
