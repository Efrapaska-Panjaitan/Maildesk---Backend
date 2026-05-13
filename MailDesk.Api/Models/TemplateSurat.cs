using System;
using System.Collections.Generic;

namespace MailDesk.Api.Models;

public partial class TemplateSurat
{
    public int Id { get; set; }

    public string NamaTemplate { get; set; } = null!;

    public string IsiTemplate { get; set; } = null!;

    public int? DibuatOleh { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User? DibuatOlehNavigation { get; set; }
}
