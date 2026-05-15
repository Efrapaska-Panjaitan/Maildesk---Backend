using MailDesk.API.DTOs.Tracking;

namespace MailDesk.API.Services.Interfaces;

public interface ITrackingService
{
    Task<TrackingResponseDto> GetTrackingSuratAsync(int suratId);
}
