using MediatR;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Users.ResetPassword;

public sealed record ResetPasswordCommand(long Id, string NewPassword) : IRequest<Result>;
