using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CollectionKeepersAPIV1.Models;

public partial class TblCollectionEntry
{
    public int FldCollectionEntryId { get; set; }

    [JsonIgnore]
    public virtual ICollection<TblAttributeValue> TblAttributeValues { get; set; } = new List<TblAttributeValue>();
}
