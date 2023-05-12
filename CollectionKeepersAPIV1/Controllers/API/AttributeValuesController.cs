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

        public AttributeValuesController(CollectionsDbContext ctx)
        {
            this.ctx = ctx;
        }

        [HttpGet(nameof(GetCollectionEntry) + "/{CollectionEntryID}")]
        public async Task<ActionResult<List<object>>> GetCollectionEntry(int CollectionEntryID)
        {
            //Set up query
            var query = from AttributeValuesTable in ctx.TblAttributeValues
                        join
                        AttributesTable in ctx.TblAttributes
                        on
                        AttributeValuesTable.FldAttributeId equals AttributesTable.FldAttributeId
                        join
                        CollectionEntriestable in ctx.TblCollectionEntries
                        on
                        AttributeValuesTable.FldCollectionEntryId equals CollectionEntriestable.FldCollectionEntryId
                        where
                        AttributeValuesTable.FldCollectionEntryId == CollectionEntryID
                        select new
                        {
                            AttributesTable.FldAttributeId,
                            AttributesTable.FldCollectionId,
                            AttributesTable.FldAttributeName,
                            AttributeValuesTable.FldAttributeValueId,
                            AttributeValuesTable.FldValue,
                            AttributeValuesTable.FldCollectionEntryId
                        };

            //Execute query
            List<object> QueriedList = new List<object>();
            foreach(var row in query)
            {
                QueriedList.Add(new
                {
                    FldAttributeId = row.FldAttributeId,
                    FldCollectionId = row.FldCollectionId,
                    FldAttributeName = row.FldAttributeName,
                    FldAttributeValueId = row.FldAttributeValueId,
                    FldValue = row.FldValue,
                    FldCollectionEntryId = row.FldCollectionEntryId
                });
            }
            return QueriedList;
        }

        [HttpPost(nameof(PostAttributeValue))]
        public async Task<ActionResult<string>> PostAttributeValue(InsertAttributeValueDTO[] ListOfValues)
        {
            //Enter a value into CollectionEntry Table
            TblCollectionEntry NewCollectionEntry = new TblCollectionEntry();
            await ctx.TblCollectionEntries.AddAsync(NewCollectionEntry);
            await ctx.SaveChangesAsync();

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
            
            await ctx.TblAttributeValues.AddRangeAsync(AttributeValuesToInsert);
            await ctx.SaveChangesAsync();
            return Ok("Okay");
        }

        [HttpGet(nameof(GetAllAttributeValuesForCollection) + "/{CollectionID}")]
        public async Task<ActionResult<List<object>>> GetAllAttributeValuesForCollection(int CollectionID)
        {
            var query = from Collections in ctx.TblCollections
                        join Attributes in ctx.TblAttributes on Collections.FldCollectionId equals Attributes.FldCollectionId
                        join AttributeValues in ctx.TblAttributeValues on Attributes.FldAttributeId equals AttributeValues.FldAttributeId
                        join CollectionEntry in ctx.TblCollectionEntries on AttributeValues.FldCollectionEntryId equals CollectionEntry.FldCollectionEntryId
                        where Collections.FldCollectionId == CollectionID
                        select new
                        {
                            Collections.FldCollectionId,
                            Collections.FldUserId,
                            Collections.FldCollectionName,
                            Collections.FldCollectionDescription,
                            Collections.FldCollectionThumbnail,
                            Collections.FldIsPrivate,
                            Attributes.FldAttributeId,
                            Attributes.FldAttributeName,
                            AttributeValues.FldAttributeValueId,
                            AttributeValues.FldValue,
                            AttributeValues.FldCollectionEntryId
                        };
            List<object> ReturnedData = new List<object>(); 
            foreach(var row in query)
            {
                var MyData = new
                {
                    FldCollectionId = row.FldCollectionId,
                    FldUserId = row.FldUserId,
                    FldCollectionName = row.FldCollectionName,
                    FldCollectionDescription = row.FldCollectionDescription,
                    FldCollectionThumbnail = row.FldCollectionThumbnail,
                    FldIsPrivate = row.FldIsPrivate,
                    FldAttributeId = row.FldAttributeId,
                    FldAttributeName = row.FldAttributeName,
                    FldAttributeValueId = row.FldAttributeValueId,
                    FldValue = row.FldValue,
                    FldCollectionEntryId = row.FldCollectionEntryId 
                };
                ReturnedData.Add(MyData);
            }

            return Ok(ReturnedData);                
        }

        [HttpPut(nameof(ModifyAttributeValue))]
        public async Task<ActionResult<string>> ModifyAttributeValue(TblAttributeValue NewAttributeValue)
        {
            // Get the new attribute value
            List<TblAttributeValue> QueriedList = await ctx.TblAttributeValues
                .Where(row => row.FldAttributeValueId == NewAttributeValue.FldAttributeValueId).ToListAsync();

            if(QueriedList.Count == 0)
            {
                return Ok($"The row couldn't be found with the ID {NewAttributeValue.FldAttributeValueId}");
            }

            TblAttributeValue QueriedRow = QueriedList.First();

            //Modify the row
            QueriedRow.FldAttributeId = NewAttributeValue.FldAttributeId;
            QueriedRow.FldValue = NewAttributeValue.FldValue;
            QueriedRow.FldCollectionEntryId= NewAttributeValue.FldCollectionEntryId;
            await ctx.SaveChangesAsync();

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
