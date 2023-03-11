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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Drawing;

namespace WpfSlideshowApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        object currdir = Directory.GetCurrentDirectory();
        static bool pic_opened = false;
        static bool no_pics = true;
        static List<string> files_to_present = new List<string>();
        static List<ISlideshowEffect> LibraryEffects = new List<ISlideshowEffect>();

        public MainWindow()
        {
            InitializeComponent();
            LibraryEffects = ReadExtensions();
            foreach (ISlideshowEffect effect in LibraryEffects)
            {
                System.Windows.Controls.MenuItem newItem = new System.Windows.Controls.MenuItem();
                newItem.Header = effect.Name;
                newItem.Click += NewItem_Click;
                Menu_slideshow.Items.Add(newItem);
                effects.Items.Add(newItem.Header);
            }
            if (LibraryEffects.Count == 0) effects.Text = "";
            else effects.Text = LibraryEffects[0].Name;
        }

        private void NewItem_Click(object sender, RoutedEventArgs e)
        {
            if (no_pics) System.Windows.MessageBox.Show("The selected folder does not contain any images to start a slideshow!", "An error occured", MessageBoxButton.OK, MessageBoxImage.Warning);
            else
            {
                System.Windows.Controls.MenuItem chosenname = (System.Windows.Controls.MenuItem)sender;
                ISlideshowEffect chosen = LibraryEffects.Find(st => st.Name == chosenname.Header.ToString());
                SlideShow slideShow = new SlideShow(chosen,files_to_present);
                slideShow.ShowDialog();
            }
        }

        static List<ISlideshowEffect> ReadExtensions()
        {
            var pluginsLists = new List<ISlideshowEffect>();
            var files = Directory.GetFiles("Plugins", "*.dll");

            foreach (var file in files)
            {
                var assembly = Assembly.LoadFile(System.IO.Path.Combine(Directory.GetCurrentDirectory(), file));

                var pluginTypes = assembly.GetTypes().Where(t => typeof(ISlideshowEffect).IsAssignableFrom(t) && !t.IsInterface).ToArray();

                foreach (var pluginType in pluginTypes)
                {
                    var pluginInstance = Activator.CreateInstance(pluginType) as ISlideshowEffect;
                    pluginsLists.Add(pluginInstance);
                }
            }

            return pluginsLists;
        }
        // source: https://www.youtube.com/watch?v=qDrPCMoqxdA

        private void About_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("This is a simple image slideshow application.", "About", MessageBoxButton.OK, MessageBoxImage.Asterisk);

        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void Explorer_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (string drive in Environment.GetLogicalDrives())
            {

                TreeViewItem item = new TreeViewItem();
                item.Header = drive;
                item.Tag = drive;
                item.Items.Add(currdir);
                item.Expanded += direxplore_recurrency;
                //item.Selected += new RoutedEventHandler(direxplore_selection);
                Explorer_Tree.Items.Add(item);

            }
        }

        private void Open_Folder_Click(object sender, RoutedEventArgs e)
        {
            files_to_present.Clear();
            using (var exp_dialog = new FolderBrowserDialog())
            {
                DialogResult result = exp_dialog.ShowDialog();

                //System.Windows.MessageBox.Show("Selected " + item.Header);
                int i = 0;
                foreach (string file in Directory.GetFiles(exp_dialog.SelectedPath, "*", SearchOption.TopDirectoryOnly).Where(s => s.EndsWith(".jpg") || s.EndsWith(".png") || s.EndsWith(".bmp")))
                {
                    i++;
                }
                no_pics = i == 0 ? true : false;
                System.Drawing.Image[] images = new System.Drawing.Image[i];
                i = 0;
                foreach (string file in Directory.GetFiles(exp_dialog.SelectedPath, "*", SearchOption.TopDirectoryOnly).Where(s => s.EndsWith(".jpg") || s.EndsWith(".png") || s.EndsWith(".bmp")))
                {
                    images[i] = System.Drawing.Image.FromFile(file);
                    images[i].Tag = file;
                    i++;
                    files_to_present.Add(file);
                }
                IEnumerable<PictureTile> pictureTiles = images.Select(img =>
                {
                    string tmp = img.Tag.ToString();
                    float Bytes = new FileInfo(tmp).Length;
                    return new PictureTile()
                    {
                        Name = tmp.Substring(tmp.LastIndexOf("\\") + 1),
                        Image = img,
                        Height = img.Height,
                        Width = img.Width,
                        KBytes = Bytes / 1024,
                        picture = tmp
                    };

                });
                DataContext = pictureTiles;
                e.Handled = true;
            }

        }
        void direxplore_recurrency(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)sender;

            if (item.Items.Count == 1 && item.Items[0] == currdir)
            {
                item.Items.Clear();
                    foreach (string dir in Directory.GetDirectories(item.Tag.ToString()))
                    {
                        TreeViewItem subdir = new TreeViewItem();
                        subdir.Header = dir.Substring(dir.LastIndexOf("\\") + 1);
                        subdir.Tag = dir;
                        subdir.Items.Add(currdir);
                        subdir.Expanded += direxplore_recurrency;
                        subdir.Selected += direxplore_selection;
                        subdir.Unselected += direxplore_unselection;
                        item.Items.Add(subdir);
                    }
            }
        }

        void direxplore_unselection(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)sender;
            //System.Windows.MessageBox.Show("Unselected " + item.Header);
            no_pics = true;
            DataContext = null;
            e.Handled = true;
        }

        void direxplore_selection(object sender, RoutedEventArgs e)
        {
            files_to_present.Clear();
            TreeViewItem item = (TreeViewItem)sender;
            //System.Windows.MessageBox.Show("Selected " + item.Header);
            int i = 0;
            foreach (string file in Directory.GetFiles(item.Tag.ToString(), "*", SearchOption.TopDirectoryOnly).Where(s => s.EndsWith(".jpg") || s.EndsWith(".png") || s.EndsWith(".bmp")))
            {
                i++;
            }
            no_pics = i == 0 ? true : false;

            System.Drawing.Image[] images = new System.Drawing.Image[i];
            i = 0;
            foreach (string file in Directory.GetFiles(item.Tag.ToString(), "*", SearchOption.TopDirectoryOnly).Where(s => s.EndsWith(".jpg") || s.EndsWith(".png") || s.EndsWith(".bmp")))
            {
                images[i] = System.Drawing.Image.FromFile(file);
                images[i].Tag = file;
                i++;
                files_to_present.Add(file);
            }
            var pictureTiles = images.Select(img =>
            {
                string tmp = img.Tag.ToString();
                float Bytes = new FileInfo(tmp).Length;


                return new PictureTile()
                {
                    Name = tmp.Substring(tmp.LastIndexOf("\\") + 1),
                    Image = img,
                    Height = img.Height,
                    Width = img.Width,
                    KBytes = Bytes / 1024,
                    //bitmap = bitmap
                    picture = tmp

                };
            });
            DataContext = pictureTiles;
            e.Handled = true;
        }

        private void StackPanel_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {

        }

        private void StackPanel_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            rightscroll.ScrollToVerticalOffset(rightscroll.VerticalOffset - (e.Delta / 2));
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if(File_info.IsExpanded)
            //Fileinfo_grid.DataContext = null;
            var item = (System.Windows.Controls.ListView)sender;
            PictureTile tile = (PictureTile)item.SelectedItem;
            Fileinfo_grid.Children.Clear();
            //System.Windows.MessageBox.Show("Selected " + tile.Name);
            if (tile != null)
            {
                TextBlock[] just_text = new TextBlock[4];
                for (int i = 0; i < 4; i++)
                {
                    just_text[i] = new TextBlock();
                }
                TextBlock filename_value = new TextBlock();
                TextBlock width_value = new TextBlock();
                TextBlock height_value = new TextBlock();
                TextBlock size_value = new TextBlock();

                filename_value.Text = tile.Name;
                width_value.Text = tile.Width.ToString() + " px";
                height_value.Text = tile.Height.ToString() + " px";
                size_value.Text = tile.KBytes.ToString("0.##") + " KB";


                filename_value.Style = (Style)FindResource("valuetext");
                width_value.Style = (Style)FindResource("valuetext");
                height_value.Style = (Style)FindResource("valuetext");
                size_value.Style = (Style)FindResource("valuetext");

                just_text[0].Text = "File name:";
                just_text[1].Text = "Width:";
                just_text[2].Text = "Height:";
                just_text[3].Text = "Size:";
                for (int i = 0; i < 4; i++)
                {
                    just_text[i].Style = (Style)FindResource("justtext");
                    //just_text[i].Width = Fileinfo_grid.ActualWidth / 2;
                    Grid.SetColumn(just_text[i], 0);
                    Grid.SetRow(just_text[i], i);
                    Fileinfo_grid.Children.Add(just_text[i]);
                }
                Grid.SetColumn(filename_value, 1);
                Grid.SetRow(filename_value, 0);
                Grid.SetColumn(width_value, 1);
                Grid.SetRow(width_value, 1);
                Grid.SetColumn(height_value, 1);
                Grid.SetRow(height_value, 2);
                Grid.SetColumn(size_value, 1);
                Grid.SetRow(size_value, 3);
                Fileinfo_grid.Children.Add(filename_value);
                Fileinfo_grid.Children.Add(width_value);
                Fileinfo_grid.Children.Add(height_value);
                Fileinfo_grid.Children.Add(size_value);

                //System.Windows.MessageBox.Show("Selected 1 " + tile.Name);

            }
            else
            {
                System.Windows.Controls.Label just_text = new System.Windows.Controls.Label();
                just_text.Content = "No files selected!";
                just_text.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                just_text.VerticalAlignment = VerticalAlignment.Center;
                just_text.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
                just_text.VerticalContentAlignment = VerticalAlignment.Center;
                just_text.Height = 30;
                just_text.Width = Fileinfo_grid.ActualWidth;
                Grid.SetColumnSpan(just_text, 2);
                Grid.SetRowSpan(just_text, 4);

                Fileinfo_grid.Children.Add(just_text);


                //System.Windows.MessageBox.Show("Unselected");
            }

            pic_opened = true;
            e.Handled = true;
        }

        private void Fileinfo_grid_Loaded(object sender, RoutedEventArgs e)
        {
            //Fileinfo_grid.Children.Clear();
            if (pic_opened == false)
            {
                System.Windows.Controls.Label text = new System.Windows.Controls.Label();
                text.Content = "No files selected!";
                text.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                text.VerticalAlignment = VerticalAlignment.Center;
                text.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
                text.VerticalContentAlignment = VerticalAlignment.Center;
                text.Height = 30;
                Grid.SetColumnSpan(text, 2);
                Grid.SetRowSpan(text, 4);

                text.Width = Fileinfo_grid.ActualWidth;
                Fileinfo_grid.Children.Add(text);
                //System.Windows.MessageBox.Show("Loaded the grid");
                e.Handled = true;
            }
        }


        private void ListView_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void Menu_slideshow_Click(object sender, RoutedEventArgs e)
        {

        }

        private void StartSlideshow(object sender, RoutedEventArgs e)
        {
            if (no_pics) System.Windows.MessageBox.Show("The selected folder does not contain any images to start a slideshow!", "An error occured", MessageBoxButton.OK, MessageBoxImage.Warning);
            else
            {
                string chosenname = effects.Text;
                ISlideshowEffect chosen = LibraryEffects.Find(st => st.Name == chosenname);
                SlideShow slideShow = new SlideShow(chosen, files_to_present);
                slideShow.ShowDialog();
            }
        }
    }
}
