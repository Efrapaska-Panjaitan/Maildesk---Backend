using System;
using System.Collections.Generic;

namespace MailDesk.Api.Models;

public partial class Inbox
{
    public int Id { get; set; }

    public int? SuratId { get; set; }

    public int? PengirimId { get; set; }

    public int? PenerimaId { get; set; }

    public string? Status { get; set; }

    public string? CatatanPengantar { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User? Penerima { get; set; }

    public virtual User? Pengirim { get; set; }

    public virtual Surat? Surat { get; set; }
}
