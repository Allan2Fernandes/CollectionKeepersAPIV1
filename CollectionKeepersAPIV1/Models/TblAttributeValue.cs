using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CollectionKeepersAPIV1.Models;

public partial class TblAttributeValue
{
    public int FldAttributeValueId { get; set; }

    public int? FldAttributeId { get; set; }

    public string? FldValue { get; set; }

    public int? FldCollectionEntryId { get; set; }

    [JsonIgnore]
    public virtual TblCollectionEntry? FldCollectionEntry { get; set; }
}
