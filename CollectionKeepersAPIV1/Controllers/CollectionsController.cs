using CollectionKeepersAPIV1.DataTransferObjects;
using CollectionKeepersAPIV1.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CollectionKeepersAPIV1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CollectionsController : ControllerBase
    {
        CollectionsDbContext ctx;
        public CollectionsController(CollectionsDbContext context) 
        {
            ctx = context;
        }

        [HttpPost(nameof(CreateCollection))]
        public async Task<ActionResult<string>> CreateCollection(TblCollection Collection)
        {
            //Get the user with that ID. Make sure the user exists
            List<TblUser> ListOfQueriedUsers = await ctx.TblUsers.Where(User => User.FldUserId == Collection.FldUserId).ToListAsync();

            if(ListOfQueriedUsers.Count == 0)
            {
                return NotFound($"User With ID {Collection.FldUserId} not found");
            }

            TblUser QueriedUser = ListOfQueriedUsers.First();

            //Insert into the collections table
            await ctx.TblCollections.AddAsync(Collection);
            await ctx.SaveChangesAsync();

            return Ok("Collection added");
        }

        [HttpPost(nameof(GetRandomSetOfCollections))]
        public async Task<ActionResult<List<TblCollection>>> GetRandomSetOfCollections(GetRandomItemCollectionsDTO InputDetails)
        {
            Random rand = new Random();

            //Query the DB for all the collections for that user
            List<TblCollection> Collections = await ctx.TblCollections.Where(Collection => Collection.FldUserId == InputDetails.FldUserId).ToListAsync();

            List<TblCollection> RandomisedList= new List<TblCollection>();

            for(int i=0; i<InputDetails.NumberRandomCollections; i++)
            {
                TblCollection RandomElement = Collections[rand.Next(Collections.Count)];
                RandomisedList.Add(RandomElement);
                Collections.Remove(RandomElement);
            }

            return Ok(RandomisedList); 
        }
    }
}
