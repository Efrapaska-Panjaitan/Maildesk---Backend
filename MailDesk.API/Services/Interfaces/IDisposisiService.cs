using MailDesk.API.DTOs.Disposisi;

namespace MailDesk.API.Services.Interfaces;

public interface IDisposisiService
{
    // Task 4
    Task<DisposisiDetailResponse> CreateDisposisiAsync(CreateDisposisiRequest request);
    Task<IEnumerable<DisposisiListResponse>> GetDisposisiListAsync(int? userId);
    Task<DisposisiDetailResponse> GetDisposisiByIdAsync(int id);
    Task<DisposisiTrackingResponse> GetTrackingBySuratIdAsync(int suratId);
}