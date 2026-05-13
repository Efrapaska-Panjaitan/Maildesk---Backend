using System;
using System.Collections.Generic;

namespace MailDesk.Api.Models;

public partial class User
{
    public int Id { get; set; }

    public string Nama { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int? RoleId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Disposisi> DisposisiPemberis { get; set; } = new List<Disposisi>();

    public virtual ICollection<Disposisi> DisposisiPenerimas { get; set; } = new List<Disposisi>();

    public virtual ICollection<Inbox> InboxPenerimas { get; set; } = new List<Inbox>();

    public virtual ICollection<Inbox> InboxPengirims { get; set; } = new List<Inbox>();

    public virtual Role? Role { get; set; }

    public virtual ICollection<Surat> Surats { get; set; } = new List<Surat>();

    public virtual ICollection<TemplateSurat> TemplateSurats { get; set; } = new List<TemplateSurat>();
}
