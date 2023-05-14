﻿using CollectionKeepersAPIV1.Models;
using Microsoft.EntityFrameworkCore;

namespace CollectionKeepersAPIV1.Controllers.ControllerLogic
{
    public class AttributeValueServices
    {
        CollectionsDbContext ctx;
        public AttributeValueServices(CollectionsDbContext ctx) 
        { 
            this.ctx = ctx;
        }

        public TblCollectionEntry CreatenewCollectionEntry()
        {
            //Enter a value into CollectionEntry Table
            TblCollectionEntry NewCollectionEntry = new TblCollectionEntry();
            
            ctx.TblCollectionEntries.Add(NewCollectionEntry);
            ctx.SaveChanges();
            return NewCollectionEntry;
        }

        public void AddListofAttributeValues(TblAttributeValue[] AttributeValuesToInsert)
        {
            ctx.TblAttributeValues.AddRange(AttributeValuesToInsert);
            ctx.SaveChanges();
        }

        public void UpdateAttributeValue(TblAttributeValue OriginalAttributeValue, TblAttributeValue NewAttributeValueDetails)
        {
            //Modify the row
            OriginalAttributeValue.FldAttributeId = NewAttributeValueDetails.FldAttributeId;
            OriginalAttributeValue.FldValue = NewAttributeValueDetails.FldValue;
            OriginalAttributeValue.FldCollectionEntryId= NewAttributeValueDetails.FldCollectionEntryId;
            ctx.SaveChanges();
        }

        public List<TblAttributeValue> GetAttributeValueOnID(int AttributeValueID)
        {
            // Get the new attribute value
            List<TblAttributeValue> QueriedList = ctx.TblAttributeValues
                .Where(row => row.FldAttributeValueId == AttributeValueID).ToList();
            return QueriedList;
        }

        public List<object> GetCollectionEntryOnID(int CollectionEntryID)
        {
            //Set up query
            var query = from AttributeValuesTable in ctx.TblAttributeValues
                join AttributesTable in ctx.TblAttributes
                    on AttributeValuesTable.FldAttributeId equals AttributesTable.FldAttributeId
                join CollectionEntriestable in ctx.TblCollectionEntries
                    on AttributeValuesTable.FldCollectionEntryId equals CollectionEntriestable.FldCollectionEntryId
                where AttributeValuesTable.FldCollectionEntryId == CollectionEntryID
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

        public List<int> GetDisctinctCollectionEntryIDsOnCollectionID(int CollectionID)
        {
            var query = from CollectionsTable in ctx.TblCollections
                join AttributesTable in ctx.TblAttributes
                    on CollectionsTable.FldCollectionId equals AttributesTable.FldCollectionId
                join AttributeValuesTable in ctx.TblAttributeValues
                    on AttributesTable.FldAttributeId equals AttributeValuesTable.FldAttributeId
                join CollectionEntriesTable in ctx.TblCollectionEntries
                    on AttributeValuesTable.FldCollectionEntryId equals CollectionEntriesTable.FldCollectionEntryId
                where CollectionsTable.FldCollectionId == CollectionID
                select new
                {
                    columnName = CollectionEntriesTable.FldCollectionEntryId
                };
            
            List<int> ListOfCollectionEntryIDs = new List<int>();
            foreach (var row in query)
            {
                ListOfCollectionEntryIDs.Add(row.columnName);
            }
            return ListOfCollectionEntryIDs.Distinct().ToList();
        }

        public List<object> GetAllAttributeValuesForACollection(int CollectionID)
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
            return ReturnedData;
        }
    }
}
