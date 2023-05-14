using CollectionKeepersAPIV1.Controllers.ControllerLogic;
using CollectionKeepersAPIV1.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;

namespace CollectionKeepersAPIV1.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttributeController : ControllerBase
    {
        private readonly Serilog.ILogger log; 
        CollectionsDbContext ctx;
        AttributeServices AttributeService;
        CollectionsServices CollectionService;

        public AttributeController(CollectionsDbContext ctx) 
        {
            this.ctx = ctx;
            AttributeService = new AttributeServices(this.ctx);
            CollectionService = new CollectionsServices(this.ctx); 
            log = Log.Logger.ForContext<AttributeController>();
        }

        [HttpPost(nameof(CreateAttribute))]
        public async Task<ActionResult<string>> CreateAttribute(TblAttribute Entry)
        {
            //Check if there exists an attribute with that name
            
            if(AttributeService.CheckIfAttributeExistsInCollection(Entry))
            {
                return Ok($"Attribute with the name {Entry.FldAttributeName} already exists for the collection with ID: {Entry.FldCollectionId}");
            }
            else
            {
                AttributeService.AddNewAttribute(Entry);
                return Ok("Created Attribute value");
            }            
        }

        [HttpGet(nameof(GetAttributeOnID) + "/{AttributeID}")]
        public async Task<ActionResult<TblAttribute>> GetAttributeOnID(int AttributeID)
        {
            List<TblAttribute> QueriedList = AttributeService.GetAttributesOnAttributeID(AttributeID);
            if (Functions.Functions.IsListEmpty(QueriedList))
            {
                return Ok("Attribute not found");
            }
            else
            {
                TblAttribute QueriedAttribute = QueriedList.First();
                return Ok(QueriedAttribute);
            }            
        }

        [HttpPut(nameof(ModifyAttribute))]
        public async Task<ActionResult<string>> ModifyAttribute(TblAttribute NewAttribute)
        {
            //Get the attribute to be modified
            List<TblAttribute> QueriedList = AttributeService.GetAttributesOnAttributeID(NewAttribute.FldAttributeId);
            if(Functions.Functions.IsListEmpty(QueriedList))
            {
                return Ok("Couldn't find the attribute to be modified");
            }         
            //Check if the CollectionID is valid
            List<TblCollection> QueriedCollectionsList = CollectionService.GetCollectionsOnCollectionID((int)NewAttribute.FldCollectionId);
            if(Functions.Functions.IsListEmpty(QueriedCollectionsList))
            {
                return Ok($"The collection with ID {NewAttribute.FldCollectionId} doesn't exist");
            }

            TblAttribute QueriedAttribute = QueriedList.First();
            AttributeService.UpdateAttribute(QueriedAttribute, NewAttribute);
            return Ok("Attribute has been modified");
        }

        [HttpGet(nameof(GetAllAttributesInCollection) + "/{CollectionID}")]
        public async Task<ActionResult<List<TblAttribute>>> GetAllAttributesInCollection(int CollectionID)
        {
            List<TblAttribute> QueriedList = AttributeService.GetAllAttributesOnCollectionID(CollectionID);
            return Ok(QueriedList);
        }

        [HttpDelete(nameof(DeleteAttributeOnID) + "/{AttributeID}")]
        public async Task<ActionResult<string>> DeleteAttributeOnID(int AttributeID)
        {
            //Find the Attribute with that ID
            List<TblAttribute> QueriedList = AttributeService.GetAttributesOnAttributeID(AttributeID);
            if(Functions.Functions.IsListEmpty(QueriedList))
            {
                return Ok($"Attribute with the ID {AttributeID} not found");
            }           

            TblAttribute AttributeToDelete = QueriedList.First();

            //Find all the attribute values and with that attribute ID and remove them
            List<TblAttributeValue> QueriedListOfAttributeValues = await ctx.TblAttributeValues
                .Where(row => row.FldAttributeId == AttributeToDelete.FldAttributeId).ToListAsync();
            
            List<int> ListOfDistinctCollectionEntryIDs = QueriedListOfAttributeValues
                .Select(row => (int)row.FldCollectionEntryId).Distinct().ToList();
            
            List<TblCollectionEntry> ConnectedCollectionEntries = ctx.TblCollectionEntries.
                Where(row => ListOfDistinctCollectionEntryIDs.Contains((int)row.FldCollectionEntryId)).ToList();
            
            ctx.TblAttributeValues.RemoveRange(QueriedListOfAttributeValues);
            await ctx.SaveChangesAsync();
            //Delete the attributes
            ctx.TblAttributes.Remove(AttributeToDelete);
            await ctx.SaveChangesAsync();          
            //Delete the associated collection entries
            ctx.TblCollectionEntries.RemoveRange(ConnectedCollectionEntries);
            await ctx.SaveChangesAsync();
            return Ok($"Attribute with the ID {AttributeID} was deleted");
        }

    }
}
