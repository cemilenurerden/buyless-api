using FluentValidation;
using MarketplaceService.Application.Interfaces;
using MarketplaceService.Domain;
using MediatR;

namespace MarketplaceService.Application.Commands;

public class CreateSellerReviewCommand : IRequest<Guid>
{
    public Guid OrderId { get; set; }
    public Guid ReviewerId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
}

public class CreateSellerReviewCommandValidator : AbstractValidator<CreateSellerReviewCommand>
{
    public CreateSellerReviewCommandValidator()
    {
        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 5).WithMessage("Puan 1 ile 5 arasında olmalıdır.");
    }
}

public class CreateSellerReviewCommandHandler : IRequestHandler<CreateSellerReviewCommand, Guid>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ISellerReviewRepository _reviewRepository;

    public CreateSellerReviewCommandHandler(
        IOrderRepository orderRepository,
        ISellerReviewRepository reviewRepository)
    {
        _orderRepository = orderRepository;
        _reviewRepository = reviewRepository;
    }

    public async Task<Guid> Handle(CreateSellerReviewCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId);

        if (order is null)
            throw new InvalidOperationException("Belirtilen sipariş bulunamadı.");

        // Sadece alıcı değerlendirme yapabilir
        if (order.BuyerId != request.ReviewerId)
            throw new InvalidOperationException("Bu siparişi değerlendirme yetkiniz yok.");

        // Sadece teslim edilmiş siparişler değerlendirilebilir
        if (order.Status != OrderStatus.Delivered)
            throw new InvalidOperationException("Sadece teslim edilmiş siparişler değerlendirilebilir.");

        // Daha önce değerlendirme yapılmış mı?
        var existing = await _reviewRepository.GetByOrderIdAsync(request.OrderId);
        if (existing is not null)
            throw new InvalidOperationException("Bu sipariş için zaten bir değerlendirme yapılmış.");

        var review = new SellerReview(
            request.OrderId,
            request.ReviewerId,
            order.SellerId,
            request.Rating,
            request.Comment);

        await _reviewRepository.AddAsync(review);

        return review.Id;
    }
}