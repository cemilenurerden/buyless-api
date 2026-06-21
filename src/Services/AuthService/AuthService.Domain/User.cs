namespace AuthService.Domain;

public enum UserRole
{
    User,
    Admin
}

public class User
{
    public Guid Id { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string? PhoneNumber { get; private set; }
    public string PasswordHash { get; private set; } = string.Empty;
    public string FullName { get; private set; } = string.Empty;
    public string? ProfileImageUrl { get; private set; }
    public UserRole Role { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public bool IsActive { get; private set; }

    private User() { }

    public User(string email, string passwordHash, string fullName)
    {
        Id = Guid.NewGuid();
        Email = email;
        PasswordHash = passwordHash;
        FullName = fullName;
        Role = UserRole.User;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateProfile(string? phoneNumber, string? profileImageUrl)
    {
        PhoneNumber = phoneNumber;
        ProfileImageUrl = profileImageUrl;
        UpdatedAt = DateTime.UtcNow;
    }
}