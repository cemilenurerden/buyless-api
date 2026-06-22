namespace InventoryService.Domain;

public class Category
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public int? ParentCategoryId { get; private set; }
    public string? IconUrl { get; private set; }

    private Category() { }

    public Category(string name, int? parentCategoryId = null, string? iconUrl = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Kategori adı boş olamaz.", nameof(name));

        Name = name;
        ParentCategoryId = parentCategoryId;
        IconUrl = iconUrl;
    }

    public void UpdateDetails(string name, string? iconUrl)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Kategori adı boş olamaz.", nameof(name));

        Name = name;
        IconUrl = iconUrl;
    }

    public void SetParent(int? parentCategoryId)
    {
        if (parentCategoryId.HasValue && parentCategoryId.Value == Id)
            throw new InvalidOperationException("Bir kategori kendi alt kategorisi olamaz.");

        ParentCategoryId = parentCategoryId;
    }
}