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
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;
using Windows.Web.Http.Headers;
using Windows.UI.Input;
using Windows.Web.Http.Filters;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Dial_tplink
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        public enum CurrentTool
        {
            BrightnessItem,
            ColorItem
        }

        private CurrentTool thisTool;
        private RadialController radialController;
        private RadialControllerConfiguration radialControllerConfig;
        private RadialControllerMenuItem ToggleMenuItem;
        private RadialControllerMenuItem BrightnessMenuItem;
        private RadialControllerMenuItem ColorMenuItem;

        public MainPage()
        {
            this.InitializeComponent();
            InitializeDial();
        }

        private void InitializeDial()
        {
            radialController = RadialController.CreateForCurrentView();

            // Set rotation resolution to 1 degree of sensitivity.
            radialController.RotationResolutionInDegrees = 36;

            // create new dial menu item
            //ToggleMenuItem =
            //    RadialControllerMenuItem.CreateFromFontGlyph("Toggle Light", "\xE793", "Segoe MDL2 Assets");
            //radialController.Menu.Items.Add(ToggleMenuItem);

            BrightnessMenuItem =
                RadialControllerMenuItem.CreateFromFontGlyph("Brightness", "\xE706", "Segoe MDL2 Assets");
            radialController.Menu.Items.Add(BrightnessMenuItem);

            ColorMenuItem =
                RadialControllerMenuItem.CreateFromFontGlyph("Color", "\xE790", "Segoe MDL2 Assets");
            radialController.Menu.Items.Add(ColorMenuItem);

            // clear all default item
            RadialControllerConfiguration config = RadialControllerConfiguration.GetForCurrentView();
            config.SetDefaultMenuItems(new RadialControllerSystemMenuItemKind[] { });

            // bind dial menu item to CurrentTool Enum
            //ToggleMenuItem.Invoked += (sender, args) => thisTool = CurrentTool.ToggleItem;
            BrightnessMenuItem.Invoked += (sender, args) => thisTool = CurrentTool.BrightnessItem;
            ColorMenuItem.Invoked += (sender, args) => thisTool = CurrentTool.ColorItem;

            // subscribe click and rotation of the dial
            radialController.ButtonClicked += (sender, args) => { RadialController_ButtonClicked(sender, args); };
            radialController.RotationChanged += (sender, args) => { RadialController_RotationChanged(sender, args); };
        }

        private void RadialController_ButtonClicked(RadialController sender, RadialControllerButtonClickedEventArgs args)
        {
            switch(thisTool)
            {
                case CurrentTool.BrightnessItem:
                    toggleLight();
                    break;
            }
        }

        public double brightnessValue = 0;
        private void RadialController_RotationChanged(RadialController sender, RadialControllerRotationChangedEventArgs args)
        {
            switch(thisTool)
            {
                case CurrentTool.BrightnessItem:
                    double tempBrightnessValue = brightnessValue;
                    tempBrightnessValue += args.RotationDeltaInDegrees / 36 * 10;
                    if (!(tempBrightnessValue < 0) && !(tempBrightnessValue > 100))
                    {
                        brightnessValue += args.RotationDeltaInDegrees / 36 * 10;

                        string msg = String.Format("Brightness: {0}%", brightnessValue);
                        ShowSlideText.Text = msg;

                        BrightnessSlide.Value = brightnessValue;

                        brightnessChange(Math.Round(brightnessValue).ToString());
                    } else if (tempBrightnessValue < 0)
                    {
                        toggleLight(0);
                    }
                    break;
            }
        }

        private void MenuListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(!mainSplitView.IsPaneOpen)
            {
                mainSplitView.IsPaneOpen = !mainSplitView.IsPaneOpen;
                MenuListBox.SelectedIndex = -1;
            } else
            {
                if (HamburgerItem.IsSelected)
                {
                    MenuListBox.SelectedIndex = -1;
                    mainSplitView.IsPaneOpen = !mainSplitView.IsPaneOpen;
                }
                if (HomeItem.IsSelected)
                {
                    MenuListBox.SelectedIndex = -1;
                    homeGrid.Visibility = Visibility.Visible;
                    brightnessGrid.Visibility = Visibility.Collapsed;
                }
                if (BrightnessItem.IsSelected)
                {
                    MenuListBox.SelectedIndex = -1;
                    homeGrid.Visibility = Visibility.Collapsed;
                    brightnessGrid.Visibility = Visibility.Visible ;
                }
                if (ColorItem.IsSelected)
                {
                    MenuListBox.SelectedIndex = -1;
                }
            }
        }

        private async void toggleLight(int status = 1)
        {
            ToggleText.Text = "GET Response: Loading...";

            HttpBaseProtocolFilter filter = new HttpBaseProtocolFilter();
            // Setting it to .NoCache always forces a new request
            filter.CacheControl.ReadBehavior = HttpCacheReadBehavior.NoCache;
            filter.CacheControl.WriteBehavior = HttpCacheWriteBehavior.Default;

            HttpClient httpClient = new HttpClient(filter);
            var headers = httpClient.DefaultRequestHeaders;
            headers.Add("client","andy-pc");
            if(status == 1)
            {
                headers.Add("action", "toggle");
            } else
            {
                headers.Add("action", "off");
            }
            Uri requestUri = new Uri("http://127.0.0.1:3000");
            var response = await httpClient.GetAsync(requestUri);
            ToggleText.Text = "GET Response: " + await response.Content.ReadAsStringAsync();
        }

        private void ToggleBtn_Click(object sender, RoutedEventArgs e)
        {
            toggleLight();
        }

        private async void brightnessChange(string value)
        {
            ToggleText.Text = "GET Response: Loading...";

            HttpBaseProtocolFilter filter = new HttpBaseProtocolFilter();
            // Setting it to .NoCache always forces a new request
            filter.CacheControl.ReadBehavior = HttpCacheReadBehavior.NoCache;
            filter.CacheControl.WriteBehavior = HttpCacheWriteBehavior.Default;


            HttpClient httpClient = new HttpClient(filter);
            var headers = httpClient.DefaultRequestHeaders;
            headers.Add("client", "andy-pc");
            headers.Add("action", "brightness");
            headers.Add("brightness", value);
            Uri requestUri = new Uri("http://127.0.0.1:3000");
            var response = await httpClient.GetAsync(requestUri);
            ToggleText.Text = "GET Response: " + await response.Content.ReadAsStringAsync();
        }

        private void BrightnessSlide_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            string msg = String.Format("Brightness: {0}%", e.NewValue);
            ShowSlideText.Text = msg;
        }
    }
}
