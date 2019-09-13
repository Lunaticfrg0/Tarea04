using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tarea04.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Tarea04.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class YoutubePage : ContentPage
    {
        public YoutubePage()
        {
            InitializeComponent();
            BindingContext = new YoutubePageViewModel();
        }
    }
}