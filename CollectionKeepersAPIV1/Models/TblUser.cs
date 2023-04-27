using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CollectionKeepersAPIV1.Models;

public partial class TblUser
{
    public int FldUserId { get; set; }

    public string? FldUsername { get; set; }

    public string? FldPassword { get; set; }

    public string? FldEmail { get; set; }

    [JsonIgnore]
    public virtual ICollection<TblCollection> TblCollections { get; set; } = new List<TblCollection>();
}
