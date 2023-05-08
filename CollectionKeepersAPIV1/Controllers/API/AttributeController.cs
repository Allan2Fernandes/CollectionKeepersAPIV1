using CollectionKeepersAPIV1.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace CollectionKeepersAPIV1.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttributeController : ControllerBase
    {

        CollectionsDbContext ctx = new CollectionsDbContext();

        public AttributeController(CollectionsDbContext ctx) 
        {
            this.ctx = ctx;
        }

        [HttpPost(nameof(CreateAttribute))]
        public async Task<ActionResult<string>> CreateAttribute(TblAttribute Entry)
        {
            //Check if there exists an attribute with that name
            List<TblAttribute> queriedAttributes = await ctx.TblAttributes.Where(row => row.FldCollectionId == Entry.FldCollectionId && row.FldAttributeName == Entry.FldAttributeName).ToListAsync();
            if(queriedAttributes.Count > 0)
            {
                return Ok($"Attribute with the name {Entry.FldAttributeName} already exists for the collection with ID: {Entry.FldCollectionId}");
            }
            else
            {
                await ctx.TblAttributes.AddAsync(Entry);
                await ctx.SaveChangesAsync();
                return Ok("Created Attribute value");
            }            
        }

        [HttpGet(nameof(GetAttributeOnID) + "/{AttributeID}")]
        public async Task<ActionResult<TblAttribute>> GetAttributeOnID(int AttributeID)
        {
            List<TblAttribute> QueriedList = await ctx.TblAttributes.Where(row => row.FldAttributeId == AttributeID).ToListAsync();
            if(QueriedList.Count == 0)
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
            List<TblAttribute> QueriedList = await ctx.TblAttributes.Where(row => row.FldAttributeId == NewAttribute.FldAttributeId).ToListAsync();
            if(QueriedList.Count == 0)
            {
                return Ok("Couldn't find the attribute to be modified");
            }         
            //Check if the CollectionID is valid
            List<TblCollection> QueriedCollectionsList = await ctx.TblCollections.Where(row => row.FldCollectionId == NewAttribute.FldCollectionId).ToListAsync();
            if(QueriedCollectionsList.Count == 0)
            {
                return Ok($"The collection with ID {NewAttribute.FldCollectionId} doesn't exist");
            }

            TblAttribute QueriedAttribute = QueriedList.First();    
            QueriedAttribute.FldAttributeName = NewAttribute.FldAttributeName;
            QueriedAttribute.FldCollectionId = NewAttribute.FldCollectionId;
            await ctx.SaveChangesAsync();
            return Ok("Attribute has been modified");
        }

        [HttpGet(nameof(GetAllAttributesInCollection) + "/{CollectionID}")]
        public async Task<ActionResult<List<TblAttribute>>> GetAllAttributesInCollection(int CollectionID)
        {
            List<TblAttribute> QueriedList = await ctx.TblAttributes.Where(row => row.FldCollectionId == CollectionID).ToListAsync();
            return Ok(QueriedList);
        }

        [HttpDelete(nameof(DeleteAttributeOnID) + "/{AttributeID}")]
        public async Task<ActionResult<string>> DeleteAttributeOnID(int AttributeID)
        {
            //Find the Attribute with that ID
            List<TblAttribute> QueriedList = await ctx.TblAttributes.Where(row => row.FldAttributeId == AttributeID).ToListAsync();
            if(QueriedList.Count == 0)
            {
                return Ok($"Attribute with the ID {AttributeID} not found");
            }           

            TblAttribute AttributeToDelete = QueriedList.First();

            //Find all the attribute values and with that attribute ID and remove them
            List<TblAttributeValue> QueriedListOfAttributeValues = await ctx.TblAttributeValues.Where(row => row.FldAttributeId == AttributeToDelete.FldAttributeId).ToListAsync();
            ctx.TblAttributeValues.RemoveRange(QueriedListOfAttributeValues);
            await ctx.SaveChangesAsync();
            ctx.TblAttributes.Remove(AttributeToDelete);
            await ctx.SaveChangesAsync();          
            return Ok($"Attribute with the ID {AttributeID} was deleted");
        }

    }
}
