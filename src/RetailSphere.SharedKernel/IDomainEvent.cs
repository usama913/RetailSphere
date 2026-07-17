using MediatR;

namespace RetailSphere.SharedKernel;

/// <summary>
/// Marker for something that happened in the domain that other parts of the system
/// (other bounded contexts, notifications, audit, future analytics) may care about.
/// Implemented as a MediatR notification so in-process dispatch is free; cross-service
/// dispatch (outbox + broker) can be layered on later without changing this contract.
/// </summary>
public interface IDomainEvent : INotification
{
    Guid EventId => Guid.NewGuid();

    DateTimeOffset OccurredOnUtc => DateTimeOffset.UtcNow;
}
