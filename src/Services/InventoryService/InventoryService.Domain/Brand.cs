namespace InventoryService.Domain;

public class Brand
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public string? LogoUrl { get; private set; }
    public bool IsVerified { get; private set; }

    private Brand() { }

    public Brand(string name, string? logoUrl = null, bool isVerified = false)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Marka adı boş olamaz.", nameof(name));

        Name = name;
        LogoUrl = logoUrl;
        IsVerified = isVerified;
    }

    public void UpdateDetails(string name, string? logoUrl)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Marka adı boş olamaz.", nameof(name));

        Name = name;
        LogoUrl = logoUrl;
    }

    public void Verify() => IsVerified = true;
    public void Unverify() => IsVerified = false;
}