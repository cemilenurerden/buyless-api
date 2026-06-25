using MediatR;

namespace InventoryService.Application.Commands;

public class RecordWearCommand : IRequest<RecordWearResult>
{
    public Guid ItemId { get; set; }
    public Guid UserId { get; set; }
    public DateTime WornDate { get; set; }
}

public class RecordWearResult
{
    public Guid WearLogId { get; set; }
    public int NewWornCount { get; set; }
}