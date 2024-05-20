namespace NativeBrowser.Maui;

public interface IWebView
{
    Task InvokeVoidAsync(string identifier, params object[] args);
    Task<T> InvokeAsync<T>(string identifier, params object[] args);
}
public partial interface INativeViewHandler
{

}