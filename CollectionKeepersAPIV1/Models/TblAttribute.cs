using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CollectionKeepersAPIV1.Models;

public partial class TblAttribute
{
    public int FldAttributeId { get; set; }

    public int? FldCollectionId { get; set; }

    public string? FldAttributeName { get; set; }

    [JsonIgnore]
    public virtual TblCollection? FldCollection { get; set; }
}
