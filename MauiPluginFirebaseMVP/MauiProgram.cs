using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
using Plugin.Firebase.Auth;
using Plugin.Firebase.Bundled.Shared;
using Plugin.Firebase.Crashlytics;

#if ANDROID
using Plugin.Firebase.Bundled.Platforms.Android;
#elif IOS
using Plugin.Firebase.Bundled.Platforms.iOS;
#endif

namespace MauiPluginFirebaseMVP;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.RegisterFirebaseServices()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			});

		builder.Services.AddMauiBlazorWebView();

#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
	
	private static MauiAppBuilder RegisterFirebaseServices(this MauiAppBuilder builder)
	{
		builder.ConfigureLifecycleEvents(events => {
#if IOS
            events.AddiOS(iOS => iOS.FinishedLaunching((app, launchOptions) => {
                CrossFirebase.Initialize(CreateCrossFirebaseSettings());
                CrossFirebaseCrashlytics.Current.SetCrashlyticsCollectionEnabled(true);
                return false;
            }));
#else
			events.AddAndroid(android => android.OnCreate((activity, _) =>
				CrossFirebase.Initialize(activity, CreateCrossFirebaseSettings())));
			CrossFirebaseCrashlytics.Current.SetCrashlyticsCollectionEnabled(true);
#endif
		});

		builder.Services.AddSingleton(_ => CrossFirebaseAuth.Current);
		return builder;
	}

	private static CrossFirebaseSettings CreateCrossFirebaseSettings()
	{
		return new CrossFirebaseSettings(
			isAnalyticsEnabled: true,
			isAuthEnabled: true,
			isCloudMessagingEnabled: true,
			isCrashlyticsEnabled: true);
	}
}
