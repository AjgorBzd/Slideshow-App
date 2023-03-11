using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace WpfSlideshowApp
{
    public class VerticalEffect : ISlideshowEffect
    {
        public string Name => "Vertical effect";

        public void PlaySlideshow(Image imageIn, Image imageOut, double windowWidth, double windowHeight)
        {
            imageIn.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
            imageOut.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            DoubleAnimation d1 = new DoubleAnimation(0,windowHeight,new Duration(new TimeSpan(0,0,3)));
            DoubleAnimation d2 = new DoubleAnimation(windowHeight,0,new Duration(new TimeSpan(0,0,3)));
            Storyboard s1 = new Storyboard();
            Storyboard s2 = new Storyboard();
            //Storyboard.SetTargetProperty(d1,)
            
        }
    }
}
