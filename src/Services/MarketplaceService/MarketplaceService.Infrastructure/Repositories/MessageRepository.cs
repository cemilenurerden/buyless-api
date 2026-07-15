using MarketplaceService.Application.Interfaces;
using MarketplaceService.Domain;
using MarketplaceService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MarketplaceService.Infrastructure.Repositories;

public class MessageRepository : IMessageRepository
{
    private readonly AppDbContext _context;

    public MessageRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Message>> GetByListingIdAsync(Guid listingId, Guid userId)
        => await _context.Messages
            .Where(m => m.ListingId == listingId &&
                        (m.SenderId == userId || m.ReceiverId == userId))
            .OrderBy(m => m.CreatedAt)
            .ToListAsync();

    public async Task AddAsync(Message message)
    {
        await _context.Messages.AddAsync(message);
        await _context.SaveChangesAsync();
    }

    public async Task MarkAsReadAsync(Guid listingId, Guid receiverId)
    {
        var unread = await _context.Messages
            .Where(m => m.ListingId == listingId && m.ReceiverId == receiverId && !m.IsRead)
            .ToListAsync();

        foreach (var message in unread)
            message.MarkAsRead();

        await _context.SaveChangesAsync();
    }
}