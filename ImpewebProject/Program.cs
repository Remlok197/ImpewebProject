using ImpewebProject.Components;
using ImpewebProject.Data;
using ImpewebProject.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Diagnostics;
using System.Security.Claims;


QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddControllers();

builder.Services.AddDbContextFactory<ImpewebContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("LocalConnection")));

builder.Services.AddScoped<ICotizacionService, CotizacionService>();

builder.Services.AddScoped<IFacturaService, FacturaService>();

builder.Services.AddScoped<IOrdenesCompraService, OrdenesCompraService>();

builder.Services.AddScoped<ICartaPorteService, CartaPorteService>();

builder.Services.AddScoped<IComplementosPagoService, ComplementosPagoService>();

builder.Services.AddScoped<IPedidosService, PedidoService>();

builder.Services.AddScoped<IAuditoriaService, AuditoriaService>();

builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();

builder
    .Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
    });

builder.Services.AddAuthenticationCore();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapGet("/api/descargar/xml/{tipo}/{uuid}", async (string tipo, string uuid, IConfiguration config) =>
{
    string? connectionString = config.GetConnectionString("ServerConnection");

    using var connection = new Microsoft.Data.SqlClient.SqlConnection(connectionString);
    await connection.OpenAsync();

    using var command = new Microsoft.Data.SqlClient.SqlCommand("SELECT CfdiTimbrado FROM Comprobantes WHERE UUID = @id", connection);
    command.Parameters.AddWithValue("@id", uuid);

    var result = await command.ExecuteScalarAsync();

    if (result != null && result != DBNull.Value)
    {
        string xmlBase64 = result.ToString()!;

        // Traducimos la sopa de letras Base64 a bytes crudos
        byte[] bytesComprimidos = Convert.FromBase64String(xmlBase64);

        // Saltamos los primeros 4 bytes de basura y leemos el archivo desde la posición 4 hasta el final.
        using var inputStream = new MemoryStream(bytesComprimidos, 4, bytesComprimidos.Length - 4);

        // 3. Pasamos esos bytes limpios por la lavadora de GZip de .NET
        using var gzipStream = new System.IO.Compression.GZipStream(inputStream, System.IO.Compression.CompressionMode.Decompress);
        using var outputStream = new MemoryStream();
        await gzipStream.CopyToAsync(outputStream);

        // 4. Recogemos el XML puro
        byte[] bytesFinales = outputStream.ToArray();

        return Results.File(bytesFinales, "application/xml", $"{tipo}_{uuid}.xml");
    }

    return Results.NotFound("No se encontró el XML en la base de datos.");
});


app.MapControllers();

app.Run();