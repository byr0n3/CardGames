using CardGames;
using CardGames.Frontend;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Services
	   .AddRazorComponents()
	   .AddInteractiveServerComponents();

builder.Services.AddSingleton<GameManager>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseHttpsRedirection();
}
else
{
	app.UseExceptionHandler("/error", true);
	app.UseHsts();
}

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

app.Run();
