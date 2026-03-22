using ConstructionStore.Admin.Services;
using Microsoft.Extensions.Logging;

namespace ConstructionStore.Admin;

public static class MauiProgram
{
	private const string LocalApiBaseUrl = "https://localhost:7242/";
	private const string ProductionApiBaseUrl = "https://constructionstore-api.onrender.com/";

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
			BaseAddress = new Uri(GetApiBaseUrl())
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

	private static string GetApiBaseUrl()
	{
#if DEBUG
		return LocalApiBaseUrl;
#else
		return ProductionApiBaseUrl;
#endif
	}
}
