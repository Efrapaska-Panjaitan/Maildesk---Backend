using MailDesk.API.DTOs.User;

namespace MailDesk.API.Services.Interfaces;

public interface IUserService
{
    /// <summary>
    /// Ambil daftar user yang memiliki role tertentu untuk kebutuhan dropdown UI.
    /// Digunakan TU/Sekretaris saat memilih Pimpinan tujuan disposisi.
    /// </summary>
    /// <param name="namaRole">
    /// Nama role yang difilter (contoh: "Pimpinan").
    /// Jika null, kembalikan semua user.
    /// </param>
    Task<IEnumerable<UserDropdownResponse>> GetUserDropdownAsync(string? namaRole);
}
