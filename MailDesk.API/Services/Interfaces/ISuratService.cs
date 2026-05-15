using MailDesk.API.DTOs.Surat;
using MailDesk.API.DTOs.Statistik;

namespace MailDesk.API.Services.Interfaces;

public interface ISuratService
{
    /// <summary>Catat surat masuk baru beserta file PDF-nya (wajib).</summary>
    Task<SuratResponse> CreateSuratMasukAsync(CreateSuratRequest request, IFormFile file);

    /// <summary>Ganti/upload ulang file PDF untuk surat yang sudah ada.</summary>
    Task<UploadPdfResponse> UploadPdfAsync(int suratId, IFormFile file);

    Task<NomorAgendaPreviewResponse> GetNomorAgendaPreviewAsync();
    Task<SuratResponse> GetSuratByIdAsync(int id);

    /// <summary>Get semua surat (masuk) dengan filter & pagination — untuk Dashboard.</summary>
    Task<PaginatedResponse<SuratListResponse>> GetAllSuratAsync(SuratQueryParams query);

    /// <summary>Get surat masuk dengan filter & pagination — task Akmal.</summary>
    Task<PaginatedResponse<SuratListResponse>> GetSuratMasukAsync(SuratQueryParams query);

    /// <summary>Statistik surat dan disposisi.</summary>
    Task<StatistikResponse> GetStatistikAsync();
}