using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure static files (maps to your "static" directory in Flask)
app.UseStaticFiles();

// Map routes
app.MapGet("/", async context =>
{
    await context.Response.WriteAsync(await RenderView("html/index.html"));
});

app.MapGet("/index.html", async context =>
{
    await context.Response.WriteAsync(await RenderView("html/index.html"));
});

app.MapGet("/user-cart", async context =>
{
    await context.Response.WriteAsync(await RenderView("html/user-cart.html"));
});

app.MapGet("/html/user-cart.html", async context =>
{
    await context.Response.WriteAsync(await RenderView("html/user-cart.html"));
});

app.MapGet("/product", async context =>
{
    var productId = context.Request.Query["id"];
    var categoryId = context.Request.Query["c_id"];
    await context.Response.WriteAsync(await RenderView("html/product.html", new { productId, categoryId }));
});

app.MapGet("/html/product.html", async context =>
{
    var productId = context.Request.Query["id"];
    var categoryId = context.Request.Query["c_id"];
    await context.Response.WriteAsync(await RenderView("html/product.html", new { productId, categoryId }));
});

app.MapGet("/product_detail", async context =>
{
    var productId = context.Request.Query["p_id"];
    await context.Response.WriteAsync(await RenderView("html/product_detail.html", new { productId }));
});

app.MapGet("/html/product_detail.html", async context =>
{
    var productId = context.Request.Query["p_id"];
    await context.Response.WriteAsync(await RenderView("html/product_detail.html", new { productId }));
});

app.MapGet("/html/blog.html", async context =>
{
    await context.Response.WriteAsync(await RenderView("html/blog.html"));
});

// Serve static files from "static/images/json-url"
app.MapGet("/assets/images/json-url/{filename}", async context =>
{
    var fileName = context.Request.RouteValues["filename"]?.ToString();
    var filePath = Path.Combine(app.Environment.WebRootPath, "images/json-url", fileName);
    if (File.Exists(filePath))
    {
        await context.Response.SendFileAsync(filePath);
    }
    else
    {
        context.Response.StatusCode = 404;
    }
});

// Default static file serving
app.MapGet("/static/{filename}", async context =>
{
    var fileName = context.Request.RouteValues["filename"]?.ToString();
    var filePath = Path.Combine(app.Environment.WebRootPath, fileName);
    if (File.Exists(filePath))
    {
        await context.Response.SendFileAsync(filePath);
    }
    else
    {
        context.Response.StatusCode = 404;
    }
});

app.Run();

async Task<string> RenderView(string viewPath, object model = null)
{
    var viewFile = Path.Combine(app.Environment.ContentRootPath, "Views", viewPath);
    if (!File.Exists(viewFile)) return "404 Not Found";

    var content = await File.ReadAllTextAsync(viewFile);
    // Replace placeholders like {{ variable }} with actual model data
    if (model != null)
    {
        var modelProperties = model.GetType().GetProperties();
        foreach (var prop in modelProperties)
        {
            content = content.Replace($"{{{{ {prop.Name} }}}}", prop.GetValue(model)?.ToString());
        }
    }

    return content;
}
