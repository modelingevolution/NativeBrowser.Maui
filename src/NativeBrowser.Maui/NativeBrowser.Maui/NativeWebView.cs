using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NativeBrowser.Maui
{
    
    public partial class NativeWebView : View
    {
        
        internal bool IsInitialized { get; private set; }

        internal void AckInitialized() => IsInitialized = true;
        public static BindableProperty SourceProperty = BindableProperty.Create(nameof(Source), typeof(string), typeof(NativeWebView));
        public static BindableProperty ScriptProperty = BindableProperty.Create(nameof(Script), typeof(string), typeof(NativeWebView));
        public static BindableProperty NameProperty = BindableProperty.Create(nameof(Name), typeof(string), typeof(NativeWebView));
        public string? Source { get => (string)GetValue(SourceProperty); set => SetValue(SourceProperty, value); }
        public string Name { get => (string)GetValue(NameProperty); set => SetValue(NameProperty, value); }
        public string? Script { get => (string)GetValue(ScriptProperty); set => SetValue(ScriptProperty, value); }

    }

    public partial class NativeWebViewHandler
    {

    }

   
}
