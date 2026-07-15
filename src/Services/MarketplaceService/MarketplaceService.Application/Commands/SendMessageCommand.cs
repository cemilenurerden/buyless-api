using MarketplaceService.Application.Interfaces;
using MarketplaceService.Domain;
using MediatR;

namespace MarketplaceService.Application.Commands;

public class SendMessageCommand : IRequest<Guid>
{
    public Guid ListingId { get; set; }
    public Guid SenderId { get; set; }
    public Guid ReceiverId { get; set; }
    public string Content { get; set; } = null!;
}

public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, Guid>
{
    private readonly IListingRepository _listingRepository;
    private readonly IMessageRepository _messageRepository;

    public SendMessageCommandHandler(
        IListingRepository listingRepository,
        IMessageRepository messageRepository)
    {
        _listingRepository = listingRepository;
        _messageRepository = messageRepository;
    }

    public async Task<Guid> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        var listing = await _listingRepository.GetByIdAsync(request.ListingId);

        if (listing is null)
            throw new InvalidOperationException("Belirtilen ilan bulunamadı.");

        var message = new Message(
            request.ListingId,
            request.SenderId,
            request.ReceiverId,
            request.Content);

        await _messageRepository.AddAsync(message);

        return message.Id;
    }
}