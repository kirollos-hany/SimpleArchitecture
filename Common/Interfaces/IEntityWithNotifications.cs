using MediatR;

namespace SimpleArchitecture.Common.Interfaces;

public interface IEntityWithNotifications
{ 
    IReadOnlyList<INotification> Notifications { get; }
}