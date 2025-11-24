using RecruitmentSystem.API.DTOs;

namespace RecruitmentSystem.API.Services;

public interface IOfferService
{
    Task<PagedResultOfferDto> GetOffersAsync(OfferFilterDto filter);
    Task<OfferLetterDto?> GetOfferAsync(int id);
    Task<OfferLetterDto> CreateOfferAsync(CreateOfferLetterDto dto);
    Task<OfferLetterDto?> UpdateOfferAsync(int id, UpdateOfferLetterDto dto);
    Task<bool> DeleteOfferAsync(int id);
}
