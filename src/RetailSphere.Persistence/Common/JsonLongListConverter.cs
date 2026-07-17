using System.Text.Json;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace RetailSphere.Persistence.Common;

/// <summary>
/// Maps a List&lt;long&gt; (e.g. User.RoleIds, Role.PermissionIds) to a JSON column.
///
/// Phase-0 simplification: the architecture doc (§4.2) specifies UserRoles/RolePermissions
/// as proper join tables. This JSON-column approach is a pragmatic stand-in that keeps the
/// aggregate boundary intact (User/Role never reference each other's objects directly) without
/// requiring EF Core's more elaborate shadow-join-entity configuration for a first pass. If
/// per-assignment metadata is ever needed (who assigned a role and when, beyond what the
/// AuditLogs table already captures), promote this to real join tables in a Phase 1 migration.
/// </summary>
public static class JsonLongListConverter
{
    public static readonly ValueConverter<List<long>, string> Converter = new(
        list => JsonSerializer.Serialize(list, JsonOptions),
        json => JsonSerializer.Deserialize<List<long>>(json, JsonOptions) ?? new List<long>());

    public static readonly ValueComparer<List<long>> Comparer = new(
        (a, b) => (a ?? new()).SequenceEqual(b ?? new()),
        list => list.Aggregate(0, (hash, id) => HashCode.Combine(hash, id)),
        list => list.ToList());

    private static readonly JsonSerializerOptions JsonOptions = new();
}
