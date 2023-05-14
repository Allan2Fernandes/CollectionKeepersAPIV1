using CollectionKeepersAPIV1.Controllers.ControllerLogic;
using CollectionKeepersAPIV1.DataTransferObjects;
using CollectionKeepersAPIV1.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Serilog;

namespace CollectionKeepersAPIV1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CollectionsController : ControllerBase
    {
        CollectionsDbContext ctx;
        CollectionsServices CollectionsService;
        UserServices UserService;
        private readonly Serilog.ILogger log; 
        public CollectionsController(CollectionsDbContext context)
        {
            this.ctx = context;
            CollectionsService = new CollectionsServices(ctx);
            UserService = new UserServices(ctx);
            log = Log.Logger.ForContext<CollectionsController>();
        }

        [HttpPost(nameof(CreateCollection))]
        public async Task<ActionResult<string>> CreateCollection(TblCollection Collection)
        {
            //Get the user with that ID. Make sure the user exists
            List<TblUser> ListOfQueriedUsers = UserService.GetUsersOnID((int)Collection.FldUserId);

            if (Functions.Functions.IsListEmpty(ListOfQueriedUsers))
            {
                return NotFound($"User With ID {Collection.FldUserId} not found");
            }

            TblUser QueriedUser = ListOfQueriedUsers.First();

            CollectionsService.AddCollectionToDB(Collection);

            return Ok(Collection.FldCollectionId);
        }

        [HttpPost(nameof(GetRandomSetOfCollections))]
        public async Task<ActionResult<List<TblCollection>>> GetRandomSetOfCollections(GetRandomItemCollectionsDTO InputDetails)
        {
            Random rand = new Random();

            //Query the DB for all the collections for that user
            List<TblCollection> Collections = CollectionsService.GetAllCollectionsOnUserID(InputDetails.FldUserId);
            
            List<TblCollection> RandomisedList = new List<TblCollection>();
            /*
            for (int i = 0; i < InputDetails.NumberRandomCollections; i++)
            {
                int randomIndex = rand.Next(Collections.Count);
                Debug.WriteLine(randomIndex);
                TblCollection RandomElement = Collections[i];
                RandomisedList.Add(RandomElement);
                Collections.Remove(RandomElement);
            }
            */
            //Take The specified number of collections
            RandomisedList = Collections.Take(InputDetails.NumberRandomCollections).ToList();
            return Ok(RandomisedList);
        }


        [HttpPost(nameof(GetUsersCollection))]
        public async Task<ActionResult<List<TblCollection>>> GetUsersCollection(GetRandomItemCollectionsDTO InputDetails)
        {
            //Query the DB for all the collections for that user
            List<TblCollection> Collections = CollectionsService.GetAllCollectionsOnUserID(InputDetails.FldUserId);
            return Ok(Collections);
        }


        [HttpGet(nameof(GetAnotherUsersPublicCollection) + "/UserID")]
        public async Task<ActionResult<List<TblCollection>>> GetAnotherUsersPublicCollection(int UserID)
        {
            List<TblCollection> Collections = CollectionsService.GetAllUsersPublicCollections(UserID);

            return Ok(Collections); 
        }

        //TODO Search all public collections and return a list of collections which match the keyword
        [HttpGet(nameof(SearchAllPublicCollectionsOnKeyboard) + "/Keyword")]
        public async Task<ActionResult<List<TblCollection>>> SearchAllPublicCollectionsOnKeyboard(string Keyword)
        {
            List<TblCollection> Collections = CollectionsService.GetAllCollectionsContainingKeyword(Keyword);

            return Ok(Collections);
        }

        //Update collection
        [HttpPut(nameof(UpdateCollection))]
        public async Task<ActionResult<string>> UpdateCollection(TblCollection NewCollectionDetails)
        {
            //Find the correct collection
            List<TblCollection> ListOfQueriedCollections = CollectionsService.GetCollectionsOnCollectionID(NewCollectionDetails.FldCollectionId);

            if(Functions.Functions.IsListEmpty(ListOfQueriedCollections))
            {
                return Ok("Collection not found");
            }

            TblCollection SearchedCollection = ListOfQueriedCollections.First();           

            CollectionsService.UpdateCollection(SearchedCollection, NewCollectionDetails);

            return Ok("Collection was updated");
        }

        //Delete collection
        [HttpDelete(nameof(DeleteCollection) + "/CollectionID")]
        public async Task<ActionResult<string>> DeleteCollection(int CollectionID)
        {
            List<TblCollection> ListOfQueriedCollections = await ctx.TblCollections.Where(Collection => CollectionID == Collection.FldCollectionId).ToListAsync();

            if(Functions.Functions.IsListEmpty(ListOfQueriedCollections))
            {
                return Ok("Collection not found");
            }

            TblCollection FoundCollection = ListOfQueriedCollections.First();

            //Find all the Attributes in this collection and delete it
            List<TblAttribute> ListOfConnectedAttributes = await ctx.TblAttributes.Where(row => row.FldCollectionId == CollectionID).ToListAsync();
            List<int> ListOfConnectedAttributeIDs = ListOfConnectedAttributes.Select(row => row.FldAttributeId).Distinct().ToList();

            //Find all Attribute values
            List<TblAttributeValue> ConnectedAttributeValues = await ctx.TblAttributeValues.Where(row => ListOfConnectedAttributeIDs.Contains((int)row.FldAttributeId)).ToListAsync();

            List<int> ListOfConnectedCollectionEntryIDs =
                ConnectedAttributeValues.Select(row => (int)row.FldCollectionEntryId).Distinct().ToList();
            
            //Find all CollectionEntries
            List<TblCollectionEntry> ConnectedCollectionEntries = await ctx.TblCollectionEntries
                .Where(row => ListOfConnectedCollectionEntryIDs.Contains((int)row.FldCollectionEntryId)).ToListAsync();
            
            log.Information("List of Connected Collection entries are: ");
            ConnectedCollectionEntries.ForEach(row => log.Information(row.ToString()));
            
            //Remove the attribute values
            ctx.TblAttributeValues.RemoveRange(ConnectedAttributeValues);
            await ctx.SaveChangesAsync();

            //Remove the collection entries
            ctx.TblCollectionEntries.RemoveRange(ConnectedCollectionEntries);
            await ctx.SaveChangesAsync();

            //Remove the attributes
            ctx.TblAttributes.RemoveRange(ListOfConnectedAttributes);
            await ctx.SaveChangesAsync();

            //Finally remove the collection
            ctx.TblCollections.Remove(FoundCollection);
            await ctx.SaveChangesAsync();
            
            return Ok("Collection successfully deleted");
        }

    }
}
