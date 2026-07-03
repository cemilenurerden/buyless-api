using MarketplaceService.Application.Interfaces;
using MarketplaceService.Domain;
using MediatR;

namespace MarketplaceService.Application.Queries;

// Tüm aktif ilanları getir (herkes görebilir)
public class GetAllListingsQuery : IRequest<List<Listing>> { }

public class GetAllListingsQueryHandler : IRequestHandler<GetAllListingsQuery, List<Listing>>
{
    private readonly IListingRepository _listingRepository;

    public GetAllListingsQueryHandler(IListingRepository listingRepository)
    {
        _listingRepository = listingRepository;
    }

    public async Task<List<Listing>> Handle(GetAllListingsQuery request, CancellationToken cancellationToken)
        => await _listingRepository.GetAllActiveAsync();
}

// Tek bir ilanı getir
public class GetListingByIdQuery : IRequest<Listing?>
{
    public Guid Id { get; set; }
}

public class GetListingByIdQueryHandler : IRequestHandler<GetListingByIdQuery, Listing?>
{
    private readonly IListingRepository _listingRepository;

    public GetListingByIdQueryHandler(IListingRepository listingRepository)
    {
        _listingRepository = listingRepository;
    }

    public async Task<Listing?> Handle(GetListingByIdQuery request, CancellationToken cancellationToken)
    {
        var listing = await _listingRepository.GetByIdAsync(request.Id);
        if (listing is not null)
            listing.IncrementViewCount();
        return listing;
    }
}

// Kullanıcının kendi ilanlarını getir
public class GetMyListingsQuery : IRequest<List<Listing>>
{
    public Guid SellerId { get; set; }
}

public class GetMyListingsQueryHandler : IRequestHandler<GetMyListingsQuery, List<Listing>>
{
    private readonly IListingRepository _listingRepository;

    public GetMyListingsQueryHandler(IListingRepository listingRepository)
    {
        _listingRepository = listingRepository;
    }

    public async Task<List<Listing>> Handle(GetMyListingsQuery request, CancellationToken cancellationToken)
        => await _listingRepository.GetBySellerIdAsync(request.SellerId);
}