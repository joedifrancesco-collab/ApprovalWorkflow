using ApprovalWorkflow.Application.DTOs;
using ApprovalWorkflow.Application.Interfaces;
using ApprovalWorkflow.Domain;
using ApprovalWorkflow.Infrastructure.Data;
using Dapper;

namespace ApprovalWorkflow.Infrastructure.Repositories;

public class CardRequestRepository : ICardRequestRepository
{
    private readonly DbConnectionFactory _factory;

    public CardRequestRepository(DbConnectionFactory factory) => _factory = factory;

    public async Task<IEnumerable<RequestSummaryDto>> GetAllAsync()
    {
        using var conn = _factory.Create();
        return await conn.QueryAsync<RequestSummaryDto>(@"
            SELECT
                r.RequestID,
                r.FirstName,
                r.LastName,
                r.CompanyName,
                t.TierName,
                s.StatusName,
                u.FirstName + ' ' + u.LastName AS SubmittedBy,
                r.Location,
                r.CreatedAt
            FROM CardRequests r
            JOIN Tier t   ON r.TierID           = t.TierID
            JOIN Status s ON r.StatusID          = s.StatusID
            JOIN Users u  ON r.SubmittedByUserID = u.Id
            ORDER BY r.CreatedAt DESC");
    }

    public async Task<RequestDetailDto?> GetByIdAsync(int id)
    {
        using var conn = _factory.Create();
        return await conn.QueryFirstOrDefaultAsync<RequestDetailDto>(@"
            SELECT
                r.RequestID,
                r.CompanyName, r.JobTitle,
                r.FirstName,  r.LastName,
                r.Address1,   r.Address2,
                r.City,       r.StateProvince, r.Zip, r.Country,
                t.TierName,   r.ExpirationDate, r.CardStatus,
                r.Location,   r.Manager,
                appr.FirstName + ' ' + appr.LastName AS ApproverName,
                r.Notes,
                s.StatusName,
                sub.FirstName + ' ' + sub.LastName AS SubmittedBy,
                r.CreatedAt
            FROM CardRequests r
            JOIN Tier   t    ON r.TierID           = t.TierID
            JOIN Status s    ON r.StatusID          = s.StatusID
            JOIN Users  sub  ON r.SubmittedByUserID = sub.Id
            LEFT JOIN Users  appr ON r.ApproverUserID = appr.Id
            WHERE r.RequestID = @id",
            new { id });
    }

    public async Task<int> CreateAsync(CardRequest request)
    {
        using var conn = _factory.Create();
        return await conn.ExecuteScalarAsync<int>(@"
            INSERT INTO CardRequests
                (CompanyName, JobTitle, FirstName, LastName, Address1, Address2,
                 City, StateProvince, Zip, Country, TierID, ExpirationDate,
                 CardStatus, Location, Manager, ApproverUserID, Notes, StatusID, SubmittedByUserID)
            OUTPUT INSERTED.RequestID
            VALUES
                (@CompanyName, @JobTitle, @FirstName, @LastName, @Address1, @Address2,
                 @City, @StateProvince, @Zip, @Country, @TierID, @ExpirationDate,
                 @CardStatus, @Location, @Manager, @ApproverUserID, @Notes, @StatusID, @SubmittedByUserID)",
            request);
    }

    public async Task UpdateStatusAsync(int requestId, int statusId)
    {
        using var conn = _factory.Create();
        await conn.ExecuteAsync(
            "UPDATE CardRequests SET StatusID = @StatusID WHERE RequestID = @RequestID",
            new { RequestID = requestId, StatusID = statusId });
    }
}