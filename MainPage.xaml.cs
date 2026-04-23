namespace ConstructionStore.Admin;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
#if WINDOWS
		ConfigureWebView2();
#endif
	}

#if WINDOWS
	private void ConfigureWebView2()
	{
		blazorWebView.HandlerChanged += async (s, e) =>
		{
			if (blazorWebView.Handler?.PlatformView is
				Microsoft.UI.Xaml.Controls.WebView2 wv2)
			{
				await wv2.EnsureCoreWebView2Async();
				wv2.CoreWebView2.Settings.AreDefaultContextMenusEnabled = true;
			}
		};
	}
#endif
}
