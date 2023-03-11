using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.IO;
using System.Reflection;
namespace WpfSlideshowApp
{
    public interface ISlideshowEffect
    {
        string Name { get; }
        void PlaySlideshow(Image imageIn, Image imageOut, double windowWidth, double windowHeight);
    }

    // source: https://www.codeproject.com/Articles/1052356/Creating-a-Simple-Plugin-System-with-NET
}

