using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Timer.Resources;
using Timer.ViewModels;

namespace Timer
{
    public partial class MainPage : PhoneApplicationPage
    {
        private long tic = 0L, toc = 0L;
        private long all = 0L, lap = 0L;
        private int id = 0;
        private bool paused = false, first = true;
        DateTime date, lapTime, allTime;

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            paused = false;
            first = true;
            id = App.ViewModel.Items.Count;

            DataContext = App.ViewModel;
            //lapButton.Content = App.textContent;

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            }
        }

        public void LapButtonClick(object sender, RoutedEventArgs e)
        {
            date = DateTime.Now;
            if (first)
            {
                /*
                 * Initial start of timer
                 */
                all = 0L;
                lap = 0L;
                //id = 0;
                //App.ViewModel.Items.Clear();
                id = App.ViewModel.Items.Count;
                if (id > 0)
                {
                    string[] lastAllTime = App.ViewModel.Items[id-1].LineTwo.Split(':');
                    if (lastAllTime.Length == 3)
                    {
                        try
                        {
                            long hours = long.Parse(lastAllTime[0]);
                            long minutes = long.Parse(lastAllTime[1]);
                            long seconds = long.Parse(lastAllTime[2].Substring(0, 2));
                            long milliseconds = long.Parse(lastAllTime[2].Substring(3, 3));
                            all = milliseconds + (seconds + (minutes + (hours) * 60L) * 60L) * 1000L;
                        }
                        catch (Exception)
                        {
                        }
                    }
                }

                resetButton.Content = AppResources.PauseMessage;
                resetButton.IsEnabled = true;
                lapButton.Content = AppResources.LapMessage;
                paused = false;
                first = false;
                tic = GetTimeInMilliseconds(date);
                toc = tic;
            }
            else
            {
                if (paused)
                {
                    /*
                     * Continue event
                     */
                    resetButton.Content = AppResources.PauseMessage;
                    lapButton.Content = AppResources.LapMessage;
                    paused = false;
                    tic = GetTimeInMilliseconds(date);
                }
                else
                {
                    /*
                     * Lap event
                     */
                    toc = GetTimeInMilliseconds(date);
                    lap += toc - tic;
                    lapTime = MillisecondsToDateTime(lap);
                    all += lap;
                    allTime = MillisecondsToDateTime(all);
                    lap = 0L;
                    tic = toc;

                    /*
                     * Write data to output
                     */
                    string timeStamp = date.Year.ToString("d4") + "-"
                        + date.Month.ToString("d2") + "-"
                        + date.Day.ToString("d2") + " "
                        + date.Hour.ToString("d2") + ":"
                        + date.Minute.ToString("d2") + ":"
                        + date.Second.ToString("d2") + "."
                        + date.Millisecond.ToString("d3");
                    string laptime = lapTime.Hour.ToString() + ":"
                        + lapTime.Minute.ToString("d2") + ":"
                        + lapTime.Second.ToString("d2") + "."
                        + lapTime.Millisecond.ToString("d3");
                    string alltime = allTime.Hour.ToString() + ":"
                        + allTime.Minute.ToString("d2") + ":"
                        + allTime.Second.ToString("d2") + "."
                        + allTime.Millisecond.ToString("d3");

                    App.ViewModel.Items.Add(new ItemViewModel()
                    {
                        ID = id.ToString(),
                        LineOne = laptime,
                        LineTwo = alltime,
                        //LineThree = timeStamp + " " + laptime + " " + alltime
                        LineThree = timeStamp
                    });
                    id++;
                }
            }
        }

        private void resetButtonClick(object sender, RoutedEventArgs e)
        {
            if (paused)
            {
                /*
                 * Here app must be reset
                 */
                resetButton.Content = AppResources.PauseMessage;
                resetButton.IsEnabled = false;
                lapButton.Content = AppResources.StartMessage;
                App.ViewModel.Items.Clear();
                paused = false;
                first = true;
                id = 0;
            }
            else
            {
                /*
                 * Here app must be paused
                 */
                resetButton.Content = AppResources.ResetMessage;
                lapButton.Content = AppResources.ContinueMessage;
                paused = true;
                // Calculate lap-part time up to this moment
                toc = GetTimeInMilliseconds(DateTime.Now);
                lap += toc - tic;
            }
        }

        private int MonthLengthInDays(DateTime time)
        {
            int monthLength = 0;
            switch (time.Month)
            {
                case 4:
                case 6:
                case 9:
                case 11:
                    monthLength = 30;
                    break;
                case 2:
                    switch (time.Year % 4)
                    {
                        case 0:
                            monthLength = 29;
                            break;
                        default:
                            monthLength = 28;
                            break;
                    }
                    break;
                default:
                    monthLength = 31;
                    break;
            }
            return monthLength;
        }

        private long GetTimeInMilliseconds(DateTime time)
        {
            long monthLength = MonthLengthInDays(time);
            long yearLenght = time.Year % 4 == 0 ? 366L : 365L;
            long result = (time.Day + (time.Month + time.Year * yearLenght) * monthLength) * 24L;
            result = time.Millisecond + (time.Second + (time.Minute + (time.Hour + result) * 60L) * 60L) * 1000L;
            return result;
        }

        private DateTime MillisecondsToDateTime(long time)
        {
            DateTime result = DateTime.Now;
            long h = time / 1000;
            long ms = time % 1000;
            long s = h % 60;
            h /= 60;
            long m = h % 60;
            h /= 60;
            result = new DateTime(result.Year, result.Month, result.Day, (int)h, (int)m, (int)s, (int)ms);
            return result;
        }

        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}