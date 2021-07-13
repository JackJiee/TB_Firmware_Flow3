using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using TB_Firmware_Flow3.Manager;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace TB_Firmware_Flow3
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Velocity : Page
    {

        XboxOneGip xbg;
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter.GetType().Equals(typeof(XboxOneGip)))
            {
                xbg = (XboxOneGip)e.Parameter;
            }
            else if (e.Parameter.GetType().Equals(typeof(string)))
            {
                ;
            }
            NavigationCacheMode = NavigationCacheMode.Enabled;

        }
        public Velocity()
        {
            this.InitializeComponent();
           
           


        }

        public Image Display { get; set; }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
        
         
        }
        int progressValue = 0;
        void timerUpdateProgress(object sender, object e)
        {
            progressValue = xbg.GetProgressValue();
            progressBar1.Value = progressValue;
            if (xbg == null)
            {
                return; //UpBack.Focus(FocusState.Programmatic);
            }
            if (xbg.GetProgressValue() >= 100 && xbg.GetFinishedState())
            {
                Debug.WriteLine("Update Suceessed!!");
                //Thread.Sleep(3000);
                Back.Visibility = Visibility.Collapsed;
                Over.Visibility = Visibility.Visible;
                (sender as DispatcherTimer).Tick -= timerUpdateProgress;
                (sender as DispatcherTimer).Stop();
            }
            return;
            progressBar1.Value++;
            percentage.Text = progressBar1.Value.ToString() + "%";
            if (progressBar1.Value == 100)
            {
                Back.Visibility = Visibility.Collapsed;
                Over.Visibility = Visibility.Visible;
                BTN_Over.Focus(FocusState.Programmatic);
            }
        }

        private void BTN_Upgrade_PointerEntered(object sender, PointerRoutedEventArgs e)
        {

         //   BTN_Upgrade_IMG.Source = BTN_IMG.Source;
            BTN_Upgrade_IMG.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/Update_font.gif"));



        }

        private void BTN_Upgrade_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            BTN_Upgrade_IMG.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/Update_back.gif"));

        }

        private void Image_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                Image image = sender as Image;

                BitmapImage newImage = new BitmapImage();
                newImage.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                newImage.UriSource = (image.Source as BitmapImage).UriSource;

                image.Source = newImage;
            }
        }

        private void BTN_Upgrade_Click(object sender, RoutedEventArgs e)
        {
            xbg.StateUpgrade(1);
            Front.Visibility = Visibility.Collapsed;
            Back.Visibility = Visibility.Visible;
            DispatcherTimer timerUpdate = new DispatcherTimer();
            timerUpdate.Interval = new TimeSpan(0, 0, 0, 0, 100);
            timerUpdate.Tick += timerUpdateProgress;
            timerUpdate.Start();
        }

        private void BTN_Over_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage), xbg, new SuppressNavigationTransitionInfo());
        }

        private void BTN_Over_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            BTN_Over_IMG.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/ButtonHouseIN.gif"));
        }

        private void BTN_Over_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            BTN_Over_IMG.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/ButtonHouseOut.gif"));
        }

        private void Grid_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Escape)
            {
                e.Handled = true;
                Frame.Navigate(typeof(MainPage), xbg, new SuppressNavigationTransitionInfo());
            }
        }

        private void BTN_Upgrade_GotFocus(object sender, RoutedEventArgs e)
        {
            Light.Visibility = Visibility.Visible;
        }

        private void BTN_Upgrade_LostFocus(object sender, RoutedEventArgs e)
        {
            Light.Visibility = Visibility.Collapsed;
        }

        private void BTN_Over_GotFocus(object sender, RoutedEventArgs e)
        {
            Light1.Visibility = Visibility.Visible;
        }

        private void BTN_Over_LostFocus(object sender, RoutedEventArgs e)
        {
            Light1.Visibility = Visibility.Collapsed;
        }
    }
}
