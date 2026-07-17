using MediatR;
using RetailSphere.Contracts.Auth;
using RetailSphere.SharedKernel;

// Note: deliberately NOT namespaced as "...Auth.RefreshToken" — that would collide
// with RetailSphere.Domain.IdentityAccess.RefreshToken (a namespace segment can't
// share a name with a type you need to reference unqualified inside it).
namespace RetailSphere.Application.Features.Auth;

public sealed record RefreshTokenCommand(string RefreshToken, string? ClientIp) : IRequest<Result<LoginResponse>>;
