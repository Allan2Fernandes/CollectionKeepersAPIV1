namespace CollectionKeepersAPIV1.DataTransferObjects;

public class GetAllAttributeValuesForACollectionDTO
{
    public int FldCollectionId { get; set; }
    public int FldUserId { get; set; }
    public string FldCollectionName { get; set; }
    public string FldCollectionDescription { get; set; }
    public string FldCollectionThumbnail { get; set; }
    public bool FldIsPrivate { get; set; }
    public int FldAttributeId { get; set; }
    public string FldAttributeName { get; set; }
    public int FldAttributeValueId { get; set; }
    public string FldValue { get; set; }
    public int FldCollectionEntryId { get; set; }
}