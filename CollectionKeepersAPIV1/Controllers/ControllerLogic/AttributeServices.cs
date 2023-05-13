using CollectionKeepersAPIV1.Models;
using Microsoft.EntityFrameworkCore;

namespace CollectionKeepersAPIV1.Controllers.ControllerLogic
{
    public class AttributeServices
    {
        CollectionsDbContext ctx;

        public AttributeServices(CollectionsDbContext ctx)
        {
            this.ctx = ctx;
        }   

        public void AddNewAttribute(TblAttribute Entry)
        {
            ctx.TblAttributes.Add(Entry);
            ctx.SaveChanges();
        }

        public bool CheckIfAttributeExistsInCollection(TblAttribute Entry)
        {
            List<TblAttribute> queriedAttributes = ctx.TblAttributes.Where(row => row.FldCollectionId == Entry.FldCollectionId && row.FldAttributeName == Entry.FldAttributeName).ToList();
            return queriedAttributes.Count > 0;          
        }

        public List<TblAttribute> GetAttributesOnAttributeID(int AttributeID)
        {
            List<TblAttribute> QueriedList = ctx.TblAttributes.Where(row => row.FldAttributeId == AttributeID).ToList();
            return QueriedList;
        }

        public List<TblAttribute> GetAllAttributesOnCollectionID(int CollectionID)
        {
            List<TblAttribute> QueriedList = ctx.TblAttributes.Where(row => row.FldCollectionId == CollectionID).ToList();
            return QueriedList;
        }

        public void UpdateAttribute(TblAttribute OriginalAttribute, TblAttribute NewAttribute)
        {
            OriginalAttribute.FldAttributeName = NewAttribute.FldAttributeName;
            OriginalAttribute.FldCollectionId = NewAttribute.FldCollectionId;
            ctx.SaveChanges();
        }
    }
}
