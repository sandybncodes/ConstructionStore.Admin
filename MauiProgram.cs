using ConstructionStore.Admin.Services;
using Microsoft.Extensions.Logging;

namespace ConstructionStore.Admin;

public static class MauiProgram
{
	// Change this to match your API's base URL
	private const string ApiBaseUrl = "https://localhost:7242/";

	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			});

		builder.Services.AddMauiBlazorWebView();

		builder.Services.AddSingleton(_ => new HttpClient
		{
			BaseAddress = new Uri(ApiBaseUrl)
		});
		builder.Services.AddSingleton<LocalizationService>();
		builder.Services.AddSingleton<AuthStateService>();
		builder.Services.AddSingleton<Services.ProductService>();
		builder.Services.AddSingleton<Services.CategoryService>();
		builder.Services.AddSingleton<Services.OrderService>();

#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
