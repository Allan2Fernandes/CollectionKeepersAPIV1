using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CollectionKeepersAPIV1.Models;

public partial class TblCollection
{
    public int FldCollectionId { get; set; }

    public int? FldUserId { get; set; }

    public string? FldCollectionName { get; set; }

    public string? FldCollectionDescription { get; set; }

    public string? FldCollectionThumbnail { get; set; }

    public bool? FldIsPrivate { get; set; }

    [JsonIgnore]
    public virtual TblUser? FldUser { get; set; }

    [JsonIgnore]
    public virtual ICollection<TblAttribute> TblAttributes { get; set; } = new List<TblAttribute>();
}
