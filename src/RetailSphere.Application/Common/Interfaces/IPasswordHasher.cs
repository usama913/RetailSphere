namespace RetailSphere.Application.Common.Interfaces;

/// <summary>Implemented in Infrastructure (Argon2id/PBKDF2 — never a custom scheme).</summary>
public interface IPasswordHasher
{
    string Hash(string plainTextPassword);

    bool Verify(string plainTextPassword, string hash);
}
