using MailDesk.API.DTOs.Surat;

namespace MailDesk.API.Services.Interfaces;

public interface ISuratService
{
    Task<SuratResponse> CreateSuratMasukAsync(CreateSuratRequest request);
    Task<UploadPdfResponse> UploadPdfAsync(int suratId, IFormFile file);
    Task<NomorAgendaPreviewResponse> GetNomorAgendaPreviewAsync();
    Task<SuratResponse> GetSuratByIdAsync(int id); //endpoint GET /{id}
    Task<PaginatedResponse<SuratListResponse>> GetAllSuratAsync(SuratQueryParams query);
    Task<PaginatedResponse<SuratListResponse>> GetSuratMasukAsync(SuratQueryParams query);
    Task<PaginatedResponse<SuratListResponse>> GetSuratKeluarAsync(SuratQueryParams query);

}