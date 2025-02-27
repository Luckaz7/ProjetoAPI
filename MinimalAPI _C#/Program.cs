using Microsoft.AspNetCore.Http;
using UglyToad.PdfPig;
using System.Text;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Http.Features;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

// Permitir requisições grandes (100MB)
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 1024 * 1024 * 100; // 100MB
});

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 1024 * 1024 * 100; // 100MB para arquivos multipart/form-data
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "API PDF Groq", Version = "v1" });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

const string URL_API_GROQ = "https://api.groq.com/openai/v1/chat/completions";
const string GROQ_API_KEY = "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";

app.MapPost("/upload", async (IFormFile file) =>
{
    if (file == null || file.Length == 0)
        return Results.BadRequest(new { message = "Nenhum arquivo enviado!" });

    using var stream = file.OpenReadStream();
    using var pdfDocument = PdfDocument.Open(stream);

    var fullText = new StringBuilder();
    foreach (var page in pdfDocument.GetPages())
    {
        fullText.AppendLine(page.Text);
    }

    var httpClient = new HttpClient();
    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {GROQ_API_KEY}");

    var requestData = new
    {
        model = "mixtral-8x7b-32768",
        messages = new[]
        {
            new { role = "system", content = "Extraia o número do processo e as partes envolvidas do seguinte texto." },
            new { role = "user", content = fullText.ToString() }
        }
    };

    var response = await httpClient.PostAsJsonAsync(URL_API_GROQ, requestData);
    if (!response.IsSuccessStatusCode)
        return Results.StatusCode((int)response.StatusCode);

    var jsonResponse = await response.Content.ReadAsStringAsync();
    var jsonDoc = JsonDocument.Parse(jsonResponse);
    var content = jsonDoc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();

    var filteredData = new
    {
        NumeroProcesso = content?.Split('\n')[0],
        Partes = content?.Split('\n')[1..]
    };

    return Results.Json(filteredData);
})
.DisableAntiforgery();

app.Run();
