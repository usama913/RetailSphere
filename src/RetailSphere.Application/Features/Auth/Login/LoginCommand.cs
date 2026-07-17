using MediatR;
using RetailSphere.Contracts.Auth;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Auth.Login;

public sealed record LoginCommand(string Email, string Password, string? ClientIp) : IRequest<Result<LoginResponse>>;
