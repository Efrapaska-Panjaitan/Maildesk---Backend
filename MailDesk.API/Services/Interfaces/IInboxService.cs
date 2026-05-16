using MailDesk.API.DTOs.Inbox;
using MailDesk.API.DTOs.Surat;

namespace MailDesk.API.Services.Interfaces;

public interface IInboxService
{
    Task<PaginatedResponse<InboxListResponse>> GetAllInboxAsync(InboxQueryParams query);
    Task<InboxDetailResponse> GetInboxByIdAsync(int id);
    Task<List<InboxDetailResponse>> CreateInboxFromSuratAsync(CreateInboxFromSuratRequest request);
}
