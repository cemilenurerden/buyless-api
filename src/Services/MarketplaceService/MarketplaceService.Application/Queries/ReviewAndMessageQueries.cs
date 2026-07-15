using MarketplaceService.Application.Interfaces;
using MarketplaceService.Domain;
using MediatR;

namespace MarketplaceService.Application.Queries;

// Satıcının tüm değerlendirmelerini getir
public class GetSellerReviewsQuery : IRequest<List<SellerReview>>
{
    public Guid SellerId { get; set; }
}

public class GetSellerReviewsQueryHandler : IRequestHandler<GetSellerReviewsQuery, List<SellerReview>>
{
    private readonly ISellerReviewRepository _reviewRepository;

    public GetSellerReviewsQueryHandler(ISellerReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    public async Task<List<SellerReview>> Handle(GetSellerReviewsQuery request, CancellationToken cancellationToken)
        => await _reviewRepository.GetBySellerIdAsync(request.SellerId);
}

// Bir ilana ait mesajları getir (sadece ilgili kullanıcı görebilir)
public class GetMessagesQuery : IRequest<List<Message>>
{
    public Guid ListingId { get; set; }
    public Guid UserId { get; set; }
}

public class GetMessagesQueryHandler : IRequestHandler<GetMessagesQuery, List<Message>>
{
    private readonly IMessageRepository _messageRepository;

    public GetMessagesQueryHandler(IMessageRepository messageRepository)
    {
        _messageRepository = messageRepository;
    }

    public async Task<List<Message>> Handle(GetMessagesQuery request, CancellationToken cancellationToken)
        => await _messageRepository.GetByListingIdAsync(request.ListingId, request.UserId);
}