using CollectionKeepersAPIV1.DataTransferObjects;
using CollectionKeepersAPIV1.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

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

            if (ListOfQueriedUsers.Count == 0)
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
            RandomisedList = Collections.Take(InputDetails.NumberRandomCollections).ToList();

            return Ok(RandomisedList);
        }


        [HttpPost(nameof(GetUsersCollection))]
        public async Task<ActionResult<List<TblCollection>>> GetUsersCollection(GetRandomItemCollectionsDTO InputDetails)
        {
            Random rand = new Random();

            //Query the DB for all the collections for that user
            List<TblCollection> Collections = await ctx.TblCollections.Where(Collection => Collection.FldUserId == InputDetails.FldUserId).ToListAsync();

            List<TblCollection> RandomisedList = new List<TblCollection>();
    
            RandomisedList = Collections.Take(InputDetails.NumberRandomCollections).ToList();
            return Ok(RandomisedList);
        }


        [HttpGet(nameof(GetAnotherUsersPublicCollection) + "/UserID")]
        public async Task<ActionResult<List<TblCollection>>> GetAnotherUsersPublicCollection(int UserID)
        {
            List<TblCollection> Collections = await ctx.TblCollections.Where(Collection => Collection.FldUserId == UserID && (Collection.FldIsPrivate == false)).ToListAsync();

            return Ok(Collections); 
        }

        //TODO Search all public collections and return a list of collections which match the keyword
        [HttpGet(nameof(SearchAllPublicCollectionsOnKeyboard) + "/Keyword")]
        public async Task<ActionResult<List<TblCollection>>> SearchAllPublicCollectionsOnKeyboard(string Keyword)
        {
            List<TblCollection> Collections = await ctx.TblCollections.Where(Collection => Collection.FldCollectionName.Contains(Keyword) && (Collection.FldIsPrivate == false)).ToListAsync();

            return Ok(Collections);
        }

        //Update collection
        [HttpPut(nameof(UpdateCollection))]
        public async Task<ActionResult<string>> UpdateCollection(TblCollection CollectionToUpdate)
        {
            //Find the correct collection
            List<TblCollection> ListOfQueriedCollections = await ctx.TblCollections.Where(Collection => CollectionToUpdate.FldCollectionId == Collection.FldCollectionId).ToListAsync();

            if(ListOfQueriedCollections.Count == 0)
            {
                return Ok("Collection not found");
            }

            TblCollection SearchedCollection = ListOfQueriedCollections.First();

            //Update every property of the collection
            SearchedCollection.FldCollectionName = CollectionToUpdate.FldCollectionName;
            SearchedCollection.FldCollectionDescription = CollectionToUpdate.FldCollectionDescription;
            SearchedCollection.FldCollectionThumbnail = CollectionToUpdate.FldCollectionThumbnail;
            SearchedCollection.FldIsPrivate= CollectionToUpdate.FldIsPrivate;
            
            await ctx.SaveChangesAsync();

            return Ok("Collection was updated");
        }

        //Delete collection
        [HttpDelete(nameof(DeleteCollection) + "/CollectionID")]
        public async Task<ActionResult<string>> DeleteCollection(int CollectionID)
        {
            List<TblCollection> ListOfQueriedCollections = await ctx.TblCollections.Where(Collection => CollectionID == Collection.FldCollectionId).ToListAsync();

            if(ListOfQueriedCollections.Count == 0)
            {
                return Ok("Collection not found");
            }

            TblCollection FoundCollection = ListOfQueriedCollections.First();

            ctx.TblCollections.Remove(FoundCollection);
            await ctx.SaveChangesAsync();

            return Ok("Collection successfully deleted");
        }

    }
}
