using System;
using System.Collections.Generic;

namespace CollectionKeepersAPIV1.Models;

public partial class TblCollectionEntry
{
    public int FldCollectionEntryId { get; set; }

    public virtual ICollection<TblAttributeValue> TblAttributeValues { get; set; } = new List<TblAttributeValue>();
}
