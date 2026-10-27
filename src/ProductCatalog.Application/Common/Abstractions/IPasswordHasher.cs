namespace ProductCatalog.Application.Common.Abstractions;

public enum PasswordCheck
{
    Failed,
    Success,
    SuccessRehashNeeded
}

public interface IPasswordHasher
{
    string Hash(string password);

    PasswordCheck Verify(
        string passwordHash,
        string password);
}
