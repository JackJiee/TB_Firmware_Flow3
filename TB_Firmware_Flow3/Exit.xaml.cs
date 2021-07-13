using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace TB_Firmware_Flow3
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Exit : Page
    {
        public Exit()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        private void BTN_Yes_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            IMG_Yes.Visibility = Visibility.Visible;
        }

        private void BTN_Yes_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            IMG_Yes.Visibility = Visibility.Collapsed;
        }

        private void BTN_Yes_GotFocus(object sender, RoutedEventArgs e)
        {
            IMG_Yes.Visibility = Visibility.Visible;
        }

        private void BTN_Yes_LostFocus(object sender, RoutedEventArgs e)
        {
            IMG_Yes.Visibility = Visibility.Collapsed;
        }

        private void BTN_No_GotFocus(object sender, RoutedEventArgs e)
        {
            IMG_No.Visibility = Visibility.Visible;
        }

        private void BTN_No_LostFocus(object sender, RoutedEventArgs e)
        {
            IMG_No.Visibility = Visibility.Collapsed;
        }

        private void BTN_No_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            IMG_No.Visibility = Visibility.Visible;
        }

        private void BTN_No_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            IMG_No.Visibility = Visibility.Collapsed;
        }

        private void BTN_No_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        private void BTN_Yes_Click(object sender, RoutedEventArgs e)
        {
            CoreApplication.Exit();
        }
    }
}
