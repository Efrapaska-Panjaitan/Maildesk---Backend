using System;
using System.Collections.Generic;

namespace MailDesk.Api.Models;

public partial class Surat
{
    public int Id { get; set; }

    public string? NoSurat { get; set; }

    public string? NomorAgenda { get; set; }

    public string? JenisSurat { get; set; }

    public string? KategoriSurat { get; set; }

    public DateOnly TanggalSurat { get; set; }

    public string Pengirim { get; set; } = null!;

    public string Penerima { get; set; } = null!;

    public string Perihal { get; set; } = null!;

    public string? IsiTeksOcr { get; set; }

    public byte[]? FileLampiran { get; set; }

    public string? NamaFile { get; set; }

    public string? Status { get; set; }

    public bool? IsArchived { get; set; }

    public int? UserId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Disposisi> Disposisis { get; set; } = new List<Disposisi>();

    public virtual ICollection<Inbox> Inboxes { get; set; } = new List<Inbox>();

    public virtual User? User { get; set; }
}
