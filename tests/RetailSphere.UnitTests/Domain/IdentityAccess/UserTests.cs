using FluentAssertions;
using RetailSphere.Domain.IdentityAccess;
using RetailSphere.Domain.IdentityAccess.ValueObjects;
using Xunit;

namespace RetailSphere.UnitTests.Domain.IdentityAccess;

public sealed class UserTests
{
    private static User CreateUser()
    {
        var email = Email.Create("cashier@retailsphere.pk").Value;
        var password = HashedPassword.FromHash("irrelevant-hash-for-these-tests").Value;
        return User.Register(email, password, "Ayesha", "Khan", branchId: 1).Value;
    }

    [Fact]
    public void Register_raises_a_UserRegisteredEvent()
    {
        var user = CreateUser();

        user.DomainEvents.Should().ContainSingle(e => e.GetType().Name == "UserRegisteredEvent");
    }

    [Fact]
    public void Register_fails_when_first_name_is_missing()
    {
        var email = Email.Create("someone@retailsphere.pk").Value;
        var password = HashedPassword.FromHash("hash").Value;

        var result = User.Register(email, password, firstName: "", lastName: "Khan", branchId: null);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("User.FirstNameRequired");
    }

    [Fact]
    public void AssignRole_twice_is_rejected_as_a_conflict()
    {
        var user = CreateUser();

        var first = user.AssignRole(roleId: 5);
        var second = user.AssignRole(roleId: 5);

        first.IsSuccess.Should().BeTrue();
        second.IsFailure.Should().BeTrue();
        second.Error.Code.Should().Be("User.RoleAlreadyAssigned");
    }

    [Fact]
    public void Refresh_token_reuse_revokes_the_entire_active_token_family()
    {
        var user = CreateUser();

        var first = user.IssueRefreshToken("hash-1", DateTime.UtcNow.AddDays(30), "127.0.0.1");
        var rotated = user.RotateRefreshToken(first, "hash-2", DateTime.UtcNow.AddDays(30), "127.0.0.1");

        // Simulate the (already-rotated) first token being replayed by an attacker.
        user.RevokeRefreshTokenFamily(first, "Reuse detected in test.");

        rotated.IsActive.Should().BeFalse("rotating the family must revoke every other active token too");
        first.IsRevoked.Should().BeTrue();
    }
}
