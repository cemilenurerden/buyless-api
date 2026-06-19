namespace AuthService.Domain;

public class User
{
    public Guid Id { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string FullName { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }
    public bool IsActive { get; private set; }

    private User() { } // EF Core için gerekli (parametresiz constructor)

    public User(string email, string passwordHash, string fullName)
    {
        Id = Guid.NewGuid();
        Email = email;
        PasswordHash = passwordHash;
        FullName = fullName;
        CreatedAt = DateTime.UtcNow;
        IsActive = true;
    }

    public void Deactivate() => IsActive = false;
}