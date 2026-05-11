using Android.Webkit;
using Microsoft.AspNetCore.Components.WebView.Maui;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;

namespace PocketPikachuMaui.Platforms.Android;

public class CustomBlazorWebViewHandler : BlazorWebViewHandler
{
    public static global::Android.Webkit.WebView? CurrentWebView { get; private set; }

    protected override void ConnectHandler(global::Android.Webkit.WebView platformView)
    {
        base.ConnectHandler(platformView);
        CurrentWebView = platformView;

        // Enable JavaScript
        platformView.Settings.JavaScriptEnabled = true;
        platformView.Settings.DomStorageEnabled = true;
    }

    protected override void DisconnectHandler(global::Android.Webkit.WebView platformView)
    {
        if (CurrentWebView == platformView)
        {
            CurrentWebView = null;
        }
        base.DisconnectHandler(platformView);
    }

    public static void ExecuteJavaScript(string script)
    {
        if (CurrentWebView != null)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                CurrentWebView.EvaluateJavascript(script, null);
            });
        }
    }
}
