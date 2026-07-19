namespace RetailSphere.Domain.Finance;

public interface ICashRegisterSessionRepository
{
    Task<CashRegisterSession?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<CashRegisterSession?> GetOpenSessionAsync(long branchId, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<CashRegisterSession> Items, long TotalCount)> SearchAsync(
        int page,
        int pageSize,
        long? branchId,
        string? status,
        CancellationToken cancellationToken = default);

    void Add(CashRegisterSession session);

    void Update(CashRegisterSession session);
}
