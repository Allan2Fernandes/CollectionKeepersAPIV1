using CollectionKeepersAPIV1.Controllers.ControllerLogic;
using CollectionKeepersAPIV1.DataTransferObjects;
using CollectionKeepersAPIV1.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CollectionKeepersAPIV1.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttributeValuesController : ControllerBase
    {
        CollectionsDbContext ctx;
        private AttributeValueServices services;

        public AttributeValuesController(CollectionsDbContext ctx)
        {
            this.ctx = ctx;
            services = new AttributeValueServices(this.ctx);
        }

        [HttpGet(nameof(GetCollectionEntry) + "/{CollectionEntryID}")]
        public async Task<ActionResult<List<object>>> GetCollectionEntry(int CollectionEntryID)
        {
            List<object> QueriedList = services.GetCollectionEntryOnID(CollectionEntryID);
            return Ok(QueriedList);
        }

        [HttpGet(nameof(GetDisctinctCollectionEntryIDsOnCollectionID) + "/{CollectionID}")]
        public async Task<ActionResult<List<int>>> GetDisctinctCollectionEntryIDsOnCollectionID(int CollectionID)
        {
            return Ok(services.GetDisctinctCollectionEntryIDsOnCollectionID(CollectionID));
        }

        [HttpPost(nameof(PostAttributeValue))]
        public async Task<ActionResult<string>> PostAttributeValue(InsertAttributeValueDTO[] ListOfValues)
        {
            TblCollectionEntry NewCollectionEntry = services.CreatenewCollectionEntry();

            TblAttributeValue[] AttributeValuesToInsert = new TblAttributeValue[ListOfValues.Length];
            //Modify input value's collectionentryid
            for(int i = 0; i<ListOfValues.Length; i++)
            {
                AttributeValuesToInsert[i] = new TblAttributeValue
                {
                    FldAttributeId = ListOfValues[i].FldAttributeId,
                    FldValue = ListOfValues[i].FldValue,
                    FldCollectionEntryId = NewCollectionEntry.FldCollectionEntryId
                };
            };

            services.AddListofAttributeValues(AttributeValuesToInsert);
            return Ok(NewCollectionEntry);
        }

        [HttpGet(nameof(GetAllAttributeValuesForCollection) + "/{CollectionID}")]
        public async Task<ActionResult<List<object>>> GetAllAttributeValuesForCollection(int CollectionID)
        {
            List<object> ReturnedData = services.GetAllAttributeValuesForACollection(CollectionID);
            return Ok(ReturnedData);                
        }

        [HttpPut(nameof(ModifyAttributeValue))]
        public async Task<ActionResult<string>> ModifyAttributeValue(TblAttributeValue NewAttributeValue)
        {
            // Get the new attribute value
            List<TblAttributeValue> QueriedList = services.GetAttributeValueOnID(NewAttributeValue.FldAttributeValueId);

            if(QueriedList.Count == 0)
            {
                return Ok($"The row couldn't be found with the ID {NewAttributeValue.FldAttributeValueId}");
            }

            TblAttributeValue QueriedRow = QueriedList.First();

            services.UpdateAttributeValue(QueriedRow, NewAttributeValue);

            return Ok("Entry has been modified");
        }

        [HttpDelete(nameof(DeleteCollectionEntry) + "/{CollectionEntryID}")]
        public async Task<ActionResult<string>> DeleteCollectionEntry(int CollectionEntryID)
        {
            //First find the collection entry
            List<TblCollectionEntry> QueriedListCollectionEntries = await ctx.TblCollectionEntries
                .Where(row => row.FldCollectionEntryId == CollectionEntryID).ToListAsync();
            //If the list is empty
            if(QueriedListCollectionEntries.Count == 0)
            {
                return Ok($"There is no collection entry with the ID {CollectionEntryID}");
            }
            TblCollectionEntry FoundCollectionEntry = QueriedListCollectionEntries.First();

            //Second, find all the attribute values with that collection entry id
            List<TblAttributeValue> QueriedListOfAttributeValues = await ctx.TblAttributeValues
                .Where(row => row.FldCollectionEntryId == FoundCollectionEntry.FldCollectionEntryId).ToListAsync();

            //Third, delete all these entries
            ctx.TblAttributeValues.RemoveRange(QueriedListOfAttributeValues);
            await ctx.SaveChangesAsync();

            //Fourth, delete the collecton entry
            ctx.TblCollectionEntries.Remove(FoundCollectionEntry);
            await ctx.SaveChangesAsync();

            return Ok("The collection entry has been deleted");
        }
    }
}
