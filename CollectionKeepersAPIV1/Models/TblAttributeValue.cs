using System;
using System.Collections.Generic;

namespace CollectionKeepersAPIV1.Models;

public partial class TblAttributeValue
{
    public int FldAttributeValueId { get; set; }

    public int? FldAttributeId { get; set; }

    public string? FldValue { get; set; }

    public int? FldCollectionEntryId { get; set; }

    public virtual TblCollectionEntry? FldCollectionEntry { get; set; }
}
