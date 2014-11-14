using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for GameWindow.xaml
    /// </summary>
    public partial class GameWindow : Window
    {
        #region FIELDS

        private readonly Random _r = new Random(); //random number generator for placing the images
        private const int ImageHeight = 150;
        private const int ImageWidth = 150;
        private readonly DispatcherTimer _timer;
        private int _time = 12;
        private int _totalTime;
        protected GameState state = GameState.Running;
        protected List<Picture> GamePictures;
        private Stack<KeyValuePair<Picture, Rectangle>> candidateStack = new Stack<KeyValuePair<Picture, Rectangle>>();
        protected Random random = new Random(DateTime.Now.Millisecond);
        protected Grid gameGrid;
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
            Background2.Width = SystemParameters.PrimaryScreenWidth;
            Background2.Height = SystemParameters.PrimaryScreenHeight;
            _timer = new DispatcherTimer {Interval = TimeSpan.FromSeconds(0.9)}; // Starts timer.
            _timer.Tick += Timer_Tick;Height = SystemParameters.PrimaryScreenHeight;
            _timer.Start();
            _totalTime = _time; // Makes a copy for background to divide from. 
            Timer.Text = _time.ToString();
        }

        #endregion

        #region LOAD METHODS

        /// <summary>
        /// Loads the Pictures and displays them on the grid
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
                    //load the image with a random Picture
                    Source = new BitmapImage(new Uri(imagePaths[_r.Next(0, imagePaths.Length)])),
                    Width = ImageWidth,
                    Height = ImageHeight
                };
                var imageX = 0;
                var imageY = 0;
            }
        }

        /*      do
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
        */
        /// <summary>
        /// prevents clicking of several layers.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void GridMousedDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true; 

        }
        /// <summary>
        /// Presents a timer in the packground. 
        /// </summary>
        /// <param name="sendor"></param>
        /// <param name="e"></param>
        
        private void Timer_Tick(Object sendor, EventArgs e)
        {
            _time--;
            Timer.Text = _time.ToString(); //Running a ticking clock.
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
                Timer.Width = Timer.Width - (MainCanvas.Width/_totalTime); // provides a progress bar of the time left.
            }
        }

        protected void PushPictureOnCandidateStack(Rectangle PictureRectangle)
        {
            candidateStack.Push(new KeyValuePair<Picture, Rectangle>((Picture) PictureRectangle.DataContext, PictureRectangle));
        }

        protected int PicturesOnStack
        {
            get { return candidateStack.Count; }
        }

        private List<Picture> AssignPicturesToGameGrid(Grid gameGrid, List<Picture> initialPictureCollection)
        {
            List<Picture> gamePictureCollection = new List<Picture>();

            for (int row = 0; row < 5; row++)
            {
                for (int col = 0; col < 5; col++)
                {
                    Rectangle rectangle = gameGrid.Children.OfType<Rectangle>().Single(
                        x =>
                            (int) x.GetValue(Grid.RowProperty) == row &&
                            (int) x.GetValue(Grid.ColumnProperty) == col);

                    int randomPictureNumber = random.Next(0, initialPictureCollection.Count);

                    Picture picture = initialPictureCollection[randomPictureNumber];

                    gamePictureCollection.Add(picture);
                    rectangle.DataContext = picture;

                    initialPictureCollection.RemoveAt(randomPictureNumber);
                }
            }

            return gamePictureCollection;
        }

        protected virtual void OnPicturePicked(Picture picture)
        {
            //Nothing to do here. A child can overide this.
        }

        protected virtual void OnMatch(string pictureName)
        {
            //Nothing to do here. A child can overide this.
        }

        protected virtual void OnMiss()
        {
            //Nothing to do here. A child can overide this.
        }

        /// <summary>
        /// Match images on current grid to new generated grid. 
        /// </summary>
        /// <param name="rectangle"></param>
      protected void MatchImage(Rectangle rectangle)   // dont know how to do this yet.
        {
            var picture = rectangle.DataContext as Picture;
            if (picture != null) picture.Match();
        }

        /// <summary>
        /// Method for starting the game. 
        /// </summary>
        public void StartGame()
        {
            gameGrid.Children.OfType<Rectangle>().ToList().ForEach(rec => rec.DataContext = null);
            List<Picture> initialPictures = LoadPictures();
            GamePictures = AssignPicturesToGameGrid(gameGrid, initialPictures);
            state = GameState.Running;
            //Inform, that game has started.
        }
/**
        private List<Picture> CreatePictures()
        {
            var pictures = new List<Picture>();
            for (int x = 1; x < 7; x++)
            {
             
            }
        }
     */
        /// <summary>
        /// Gets an image from the image foler. 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private BitmapImage GetPicture(string path)
        {
            var image = new BitmapImage();
            image.BeginInit();
            var streamResourceInfo = Application.GetResourceStream(new Uri(path, UriKind.Relative));
            if (streamResourceInfo != null)
                image.StreamSource = streamResourceInfo.Stream;
            image.EndInit();
            return image;
        }

        #endregion
    }
}