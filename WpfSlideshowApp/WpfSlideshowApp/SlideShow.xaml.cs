using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Timers;

namespace WpfSlideshowApp
{
    /// <summary>
    /// Interaction logic for SlideShow.xaml
    /// </summary>
    public partial class SlideShow : Window
    {
        public bool pause = false;
        public Timer timer;
        public int index = 0;
        static bool pic_zero = false;
        ISlideshowEffect effect_buf;
        List<string> files_buf;
        public SlideShow(ISlideshowEffect effect, List<string> files)
        {
            InitializeComponent();
            effect_buf = effect;
            files_buf = files;
            timer = new Timer();
            Loaded += new RoutedEventHandler(StartTheShow);
        }

        private void Window_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            var Contextmenu = Resources["slideshowmenu"] as ContextMenu;
            Contextmenu.IsOpen = true;
        }
        private void StartTheShow(object sender, RoutedEventArgs e)
        {
            timer.Elapsed += Timer_Elapsed;
            timer.Interval = 3500;
            timer.Start();
            index = 0;
            pic_zero = false;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if(pause==false)
            {
                Action animation = new Action(()=>
                    { 
                        int newindex;
                        if (index == files_buf.Count() - 1) newindex = 0;
                        else newindex = index + 1;
                        if(pic_zero==false)
                        {
                            pic_zero = true;
                            before.Source = new BitmapImage();
                            after.Source = new BitmapImage(new Uri(files_buf[newindex]));
                            effect_buf.PlaySlideshow(after, before, ActualWidth, ActualHeight);
                        }
                        else
                        {
                            before.Source = new BitmapImage(new Uri(files_buf[index]));
                            after.Source = new BitmapImage(new Uri(files_buf[newindex]));
                            effect_buf.PlaySlideshow(after, before, ActualWidth, ActualHeight);
                        }

                        if (index == files_buf.Count() -1) index = 0;
                        else index++;
                    });
                Dispatcher.Invoke(animation);
            }
        }

        private void menupause_Click(object sender, RoutedEventArgs e)
        {
            pause = true ? false : true;
        }

        private void menuquit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
