using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
namespace WpfSlideshowApp
{
    public class OpacityEffect : ISlideshowEffect
    {
        public string Name => "Opacity effect";

        public void PlaySlideshow(Image imageIn, Image imageOut, double windowWidth, double windowHeight)
        {
            
        }
    }
}
