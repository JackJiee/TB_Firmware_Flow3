using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

using TB_Firmware_Flow3.Manager;
using System.Diagnostics;
using System.Threading;
using Windows.System;
using D4XGaming.Devices;
// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace TB_Firmware_Flow3
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Recon_White : Page
    {
        XboxOneGip xbg;
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter.GetType().Equals(typeof(XboxOneGip)))
            {
                xbg = (XboxOneGip)e.Parameter;

                D4XDevice[] device = xbg.GetD4XDevices();
                //显示版本号
                if(device[1]!=null)
                    Current_Version.Text = "CURRENT FIRMWARE: V" +
                    device[1].FirmwareVersionInfo.Build.ToString().Insert(1, ".").Insert(3, ".");
                else if(device[3]!=null)
                    Current_Version.Text = "CURRENT FIRMWARE: V" +
                        device[3].FirmwareVersionInfo.Build.ToString().Insert(1, ".").Insert(3, ".");
            }
            else if (e.Parameter.GetType().Equals(typeof(string)))
            {
                ;
            }
            NavigationCacheMode = NavigationCacheMode.Enabled;

        }
        public Recon_White()
        {
            this.InitializeComponent();

            setVerion_4.Text = LaunchScreen.sowfwareVersion;
            setVerion_5.Text = LaunchScreen.sowfwareVersion;
            setVerion_6.Text = LaunchScreen.sowfwareVersion;
            // NavigationCacheMode = NavigationCacheMode.Enabled;
            //xbg = new XboxOneGip();
            //xbg.InitxboxGip();
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
                MainPage.typeControoler[1] = false;
                Back.Visibility = Visibility.Collapsed;
                Over.Visibility = Visibility.Visible;
                (sender as DispatcherTimer).Tick -= timerUpdateProgress;
                (sender as DispatcherTimer).Stop();
                BTN_Over.Focus(FocusState.Programmatic);
            }
            if (xbg.GetErrorState())
            {
                (sender as DispatcherTimer).Tick -= timerUpdateProgress;
                (sender as DispatcherTimer).Stop();
                Frame.Navigate(typeof(Fail), xbg, new SuppressNavigationTransitionInfo());
            }
            return;

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
            DispatcherTimer timer4 = new DispatcherTimer();
            timer4.Interval = new TimeSpan(0, 0, 0, 0, 100);
            timer4.Tick += timerUpdateProgress;
            timer4.Start();
        }

        private void BTN_Over_Click(object sender, RoutedEventArgs e)
        {
           
            Frame.Navigate(typeof(MainPage), xbg, new SuppressNavigationTransitionInfo());
            xbg.ConnceNotice = true;
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
