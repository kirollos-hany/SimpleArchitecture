using MediatR;
using SimpleArchitecture.Authentication.Enums;
using SimpleArchitecture.Authentication.Types;

namespace SimpleArchitecture.Auditing.Events;

public record LoginEvent(User User, LoginStatus Status) : INotification;