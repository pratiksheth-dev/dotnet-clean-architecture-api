namespace CleanArchitecture.Application.Interfaces;

/// <summary>
/// Issues signed JWT access tokens. The implementation in Infrastructure is a
/// demo stub — production systems should validate users against a real
/// identity store (e.g. Microsoft Entra ID) instead.
/// </summary>
public interface IJwtTokenService
{
    /// <summary>Creates a signed token for the given user name.</summary>
    string CreateToken(string userName);
}
