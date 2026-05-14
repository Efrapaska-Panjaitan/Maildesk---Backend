using System;
using System.Collections.Generic;

namespace MailDesk.Api.Models;

public partial class DisposisiRelation
{
    public int Id { get; set; }

    public int? ParentId { get; set; }

    public int? ChildId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Disposisi? Child { get; set; }

    public virtual Disposisi? Parent { get; set; }
}
