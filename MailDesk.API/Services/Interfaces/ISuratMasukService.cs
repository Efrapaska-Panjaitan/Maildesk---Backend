using MailDesk.API.DTOs.SuratMasuk;

namespace MailDesk.API.Services.Interfaces;

public interface ISuratMasukService
{
    Task<SuratMasukResponse> CreateSuratMasukAsync(CreateSuratMasukRequest request);
    Task<UploadPdfResponse> UploadPdfAsync(int suratMasukId, IFormFile file);
}