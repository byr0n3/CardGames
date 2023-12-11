using CardGames.Components;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents().AddInteractiveServerComponents();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseHttpsRedirection();
}
else
{
	app.UseExceptionHandler("/Error", true);
	app.UseHsts();
}

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

app.Run();
