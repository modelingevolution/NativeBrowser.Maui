namespace NativeBrowser.Maui.Example
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
            nv.On("yes").Event += (s,e) => Task.Run(async () =>  await this.DisplayAlert("Alert", "Yes", "OK", "Cancel"));
        }

      
    }

}
