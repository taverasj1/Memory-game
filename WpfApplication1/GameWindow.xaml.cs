using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for GameWindow.xaml
    /// </summary>
    public partial class GameWindow : Window
    {
#region FIELDS
        readonly Random _r = new Random(); //random number generator for placing the images
        private const int ImageHeight = 150;
        private const int ImageWidth = 150;
        private readonly DispatcherTimer _timer;
        private int _time = 12;
        private int _totalTime;
        #endregion

#region CONSTRUCTORS
        /// <summary>
        /// Default constructor
        /// </summary>
        public GameWindow()
        {
            InitializeComponent();
            MainCanvas.Width = SystemParameters.PrimaryScreenWidth;
            MainCanvas.Height = SystemParameters.PrimaryScreenHeight;
            Timer.Width = SystemParameters.PrimaryScreenWidth;
            Timer.Height = SystemParameters.PrimaryScreenHeight;
            _timer = new DispatcherTimer {Interval = TimeSpan.FromSeconds(0.9)}; // Starts timer.
            _timer.Tick += Timer_Tick;
            _timer.Start();
            _totalTime = _time; // Makes a copy for background to divide from. 
            Timer.Text = _time.ToString();
            LoadPictures(5);
        }
#endregion


#region LOAD METHODS
        /// <summary>
        /// Loads the pictures and displays them on the grid
        /// </summary>
        public void LoadPictures(int numberOfPictures)
        {
            //set images to build type content
            //set images to copy always
            var imagePaths = Directory.GetFiles(Environment.CurrentDirectory + "/images");

            for (var x = 0; x < numberOfPictures; x++)
            {
                var validPlacement = true;
                //create the image
                var i = new Image
                {
                    //load the image with a random picture
                    Source = new BitmapImage(new Uri(imagePaths[_r.Next(0, imagePaths.Length)])),
                    Width = ImageWidth,
                    Height = ImageHeight
                };
                var imageX = 0;
                var imageY = 0;
                
                do
                {
                    //place the image on a random spot
                    imageX = _r.Next(0, ((int) SystemParameters.PrimaryScreenWidth) - ImageWidth);
                    Debug.WriteLine(imageX + "");
                    imageY = _r.Next(0, ((int)SystemParameters.PrimaryScreenHeight) - ImageHeight);
                    Debug.WriteLine(imageY + "");

                    var hitTestParams = new GeometryHitTestParameters(new RectangleGeometry(new Rect(imageX,imageY,ImageWidth,ImageHeight)));

                    var resultCallback = new HitTestResultCallback(res => HitTestResultBehavior.Continue);
                    
                    var selectedElements = new List<DependencyObject>();

                    var filterCallback = new HitTestFilterCallback(
                        element =>
                        {
                            if (Equals(VisualTreeHelper.GetParent(element), MainCanvas))
                            {
                                selectedElements.Add(element);
                                Debug.WriteLine("Collsion");
                            }
                            return HitTestFilterBehavior.Continue;
                        });

                    //perform hit test
                    VisualTreeHelper.HitTest(MainCanvas, filterCallback, resultCallback, hitTestParams);


                    foreach (var u in selectedElements)
                    {
                        Debug.WriteLine(u.ToString());
                        validPlacement = false;
                    }  

                } while (!validPlacement);
                //add to the canvas
                MainCanvas.Children.Add(i);
                Canvas.SetLeft(i, imageX);
                Canvas.SetTop(i,imageY);
            }
        }
        private void GridMousedDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true; // prevents clicking of several layers.

        }
        /* Running a ticking clock
         **/
        private void Timer_Tick(Object sendor, EventArgs e)
        {
            _time--;
            Timer.Text = _time.ToString();
            if (_time > 0)
            {
                if (_time <= 10)
                {
                    if (_time%2 == 0)
                    {
                        Timer.Background = Brushes.DarkRed;
                    }
                    else
                    {
                       Timer.Foreground = Brushes.MintCream;
                    }

                    Timer.Text = String.Format(_time.ToString()); // display timer
                }
                else
                {

                    Timer.Text = String.Format(_time.ToString());
                }
            }
            else
            {
                _timer.Stop();
                MessageBox.Show("Remember Faster!");
                MessageBox.Show(" OR ELSE {!!} ");
                Environment.Exit(0);
            }
            if (Timer.Width > 0)
            {
                Timer.Width = Timer.Width - (MainCanvas.MaxWidth/_totalTime);  // provides a progress bar of the time left.
            }
         }

#endregion
    }
}

/*
 *          clickedPoint = e.GetPosition((UIElement)sender);

            


            

            
        }
 */
