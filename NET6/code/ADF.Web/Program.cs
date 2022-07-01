using ADF.Utility;
using Microsoft.Extensions.FileProviders;
using Microsoft.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.SetBasePath(Directory.GetCurrentDirectory()).AddCommandLine(args).Build();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<ICustomMemoryCache, CustomMemoryCache>();
builder.Services.AddSession();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(new StaticFileOptions()
{
    // ����wwwroot�µľ�̬�ļ�
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot")),
    OnPrepareResponse = context =>
    {
        // �������ʱ��Ϊ60s
        context.Context.Response.Headers[HeaderNames.CacheControl] = "public,max-age=10";
        context.Context.Response.Headers[HeaderNames.CacheControl] = "no-cache";//
        context.Context.Response.Headers[HeaderNames.CacheControl] = "no-store";//
    }
});

// ����ʱʹ��session
app.UseSession();

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
});

app.Run();
