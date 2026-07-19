using MediatR;
using RetailSphere.Contracts.Reporting;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Reporting.GetFinancialSummary;

public sealed record GetFinancialSummaryQuery : IRequest<Result<FinancialSummaryDto>>;
