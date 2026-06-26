var builder = WebApplication.CreateBuilder(args);

// --- YARP Reverse Proxy ---
// appsettings.json'daki "ReverseProxy" bölümünden Routes/Clusters tanýmlarýný okur.
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.UseHttpsRedirection();

// Gelen her isteði, appsettings.json'daki Routes/Clusters tanýmlarýna göre
// ilgili mikroservise (AuthService veya InventoryService) yönlendirir.
app.MapReverseProxy();

app.Run();