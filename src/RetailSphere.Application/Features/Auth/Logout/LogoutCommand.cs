using MediatR;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Auth.Logout;

public sealed record LogoutCommand(string RefreshToken) : IRequest<Result>;
