using MailDesk.API.DTOs.Disposisi;

namespace MailDesk.API.Services.Interfaces;

public interface IDisposisiService
{
    // Task 4
    Task<DisposisiDetailResponse> CreateDisposisiAsync(CreateDisposisiRequest request);
    Task<IEnumerable<DisposisiListResponse>> GetDisposisiListAsync(int? pemberiId, int? penerimaId);
    Task<DisposisiDetailResponse> GetDisposisiByIdAsync(int id);
    Task<DisposisiTrackingResponse> GetTrackingBySuratIdAsync(int suratId);

    // Task 5 — History Log
    Task<DisposisiDetailResponse> TerimaDisposisiAsync(int disposisiId, UpdateDisposisiStatusRequest request);
    Task<DisposisiDetailResponse> SelesaikanDisposisiAsync(int disposisiId, UpdateDisposisiStatusRequest request);
    Task<IEnumerable<DisposisiLogResponse>> GetLogByDisposisiIdAsync(int disposisiId);
    Task<IEnumerable<DisposisiLogResponse>> GetLogBySuratIdAsync(int suratId);
}