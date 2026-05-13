using System;
using System.Collections.Generic;

namespace MailDesk.Api.Models;

public partial class Disposisi
{
    public int Id { get; set; }

    public int? SuratId { get; set; }

    public int? PemberiId { get; set; }

    public int? PenerimaId { get; set; }

    public DateOnly TanggalDisposisi { get; set; }

    public string? SifatDisposisi { get; set; }

    public string Instruksi { get; set; } = null!;

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public virtual ICollection<DisposisiRelation> DisposisiRelationChildren { get; set; } = new List<DisposisiRelation>();

    public virtual ICollection<DisposisiRelation> DisposisiRelationParents { get; set; } = new List<DisposisiRelation>();

    public virtual User? Pemberi { get; set; }

    public virtual User? Penerima { get; set; }

    public virtual Surat? Surat { get; set; }
}
