using CollectionKeepersAPIV1.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace CollectionKeepersAPIV1.Controllers.ControllerLogic
{
    public class CollectionsServices
    {
        CollectionsDbContext ctx;
        public CollectionsServices(CollectionsDbContext ctx) 
        { 
            this.ctx = ctx;
        }

        public void AddCollectionToDB(TblCollection Collection)
        {
            //Insert into the collections table
            ctx.TblCollections.Add(Collection);
            ctx.SaveChanges();
        }

        public List<TblCollection> GetAllCollectionsOnUserID(int UserID) 
        { 
            List<TblCollection> ListOfCollections = ctx.TblCollections.Where(Collection => Collection.FldUserId == UserID).ToList();
            return ListOfCollections;
        }

        public List<TblCollection> GetAllUsersPublicCollections(int UserID)
        {
            List<TblCollection> ListOfCollections =  ctx.TblCollections.Where(Collection => (Collection.FldUserId == UserID) && (Collection.FldIsPrivate == false)).ToList();
            return ListOfCollections;
        }

        public List<TblCollection> GetAllCollectionsContainingKeyword(string Keyword)
        {
            List<TblCollection> Collections = ctx.TblCollections.Where(Collection => Collection.FldCollectionName.Contains(Keyword) && (Collection.FldIsPrivate == false)).ToList();
            return Collections;
        }

        public List<TblCollection> GetCollectionsOnCollectionID(int CollectionID)
        {
            List<TblCollection> ListOfQueriedCollections = ctx.TblCollections.Where(Collection => Collection.FldCollectionId == CollectionID).ToList();
            return ListOfQueriedCollections;
        }

        public void UpdateCollection(TblCollection DBCollection, TblCollection NewCollectionDetails)
        {
            //Update every property of the collection
            DBCollection.FldCollectionName = NewCollectionDetails.FldCollectionName;
            DBCollection.FldCollectionDescription = NewCollectionDetails.FldCollectionDescription;
            DBCollection.FldCollectionThumbnail = NewCollectionDetails.FldCollectionThumbnail;
            DBCollection.FldIsPrivate = NewCollectionDetails.FldIsPrivate;

            ctx.SaveChanges();
        }
    }
}
