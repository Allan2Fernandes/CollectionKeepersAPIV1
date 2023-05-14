namespace CollectionKeepersAPIV1.DataTransferObjects;

public class GetCollectionEntryOnIDDTO
{
    public int FldAttributeId { get; set; }
    public int FldCollectionId { get; set; }
    public string FldAttributeName { get; set; }
    public int FldAttributeValueId { get; set; }
    public string FldValue { get; set; }
    public int FldCollectionEntryId { get; set; }
}