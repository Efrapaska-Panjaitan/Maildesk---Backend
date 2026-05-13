using MailDesk.API.DTOs.Surat;

namespace MailDesk.API.Services.Interfaces;

public interface ISuratService
{
    /// <summary>Catat surat masuk baru beserta file PDF-nya (wajib).</summary>
    Task<SuratResponse> CreateSuratMasukAsync(CreateSuratRequest request, IFormFile file);

    /// <summary>Ganti/upload ulang file PDF untuk surat yang sudah ada.</summary>
    Task<UploadPdfResponse> UploadPdfAsync(int suratId, IFormFile file);

    Task<NomorAgendaPreviewResponse> GetNomorAgendaPreviewAsync();
    Task<SuratResponse> GetSuratByIdAsync(int id);

    /// <summary>Get semua surat masuk tanpa filter — return seluruh data.</summary>
    Task<IEnumerable<SuratListResponse>> GetAllSuratMasukAsync();
}