using System;
using System.Collections.Generic;

namespace CollectionKeepersAPIV1.Models;

public partial class TblAttribute
{
    public int FldAttributeId { get; set; }

    public int? FldCollectionId { get; set; }

    public string? FldAttributeName { get; set; }

    public virtual TblCollection? FldCollection { get; set; }
}
