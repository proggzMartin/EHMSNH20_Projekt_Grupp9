using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DatabaseConnection;

namespace Store
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Knapp kan ha 3 lägen; uthyrd, hyra, vald.
        public enum MovieStatus
        {
            Hyr, Vald, Uthyrd
        }

        private class SelectMovieContainer {

            public Movie TargetMovie { get; set; }
            public MovieStatus Status { get; private set; } = MovieStatus.Hyr;

            private StackPanel _stackPanel;
            private MovieImageContainer _imageContainer;
            private MovieButtonContainer _buttonContainer;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="targetElement">The element to be contained by the selectmovie-box.</param>
            /// <param name="stack">The stackpanel the element shall belong to.</param>
            public SelectMovieContainer(Movie movie, MouseButtonEventHandler mouseButtonEventHandler)
            {
                if (!_TargetMovieIsValid(movie))
                    throw new Exception("Invalid use o SelectMovieContainer.");

                TargetMovie = movie;

                if (API.IsRentedByCustomer(State.User, TargetMovie.Id))
                    Status = MovieStatus.Uthyrd;

                _imageContainer = new MovieImageContainer(TargetMovie.ImageURL);
                _buttonContainer = new MovieButtonContainer(Status, TargetMovie, mouseButtonEventHandler);

                _stackPanel = new StackPanel();
                _stackPanel.Children.Add(_imageContainer.Image);
                _stackPanel.Children.Add(_buttonContainer.Button);

            }

            public void SetNewMovie(Movie movie)
            {
                TargetMovie = movie;
                if (API.IsRentedByCustomer(State.User, movie.Id))
                    Status = MovieStatus.Uthyrd;
                else if (selectedMovies.ContainsKey(movie))
                    Status = MovieStatus.Uthyrd;
                else
                    Status = MovieStatus.Hyr;

                _imageContainer.SetActiveMovie(movie.ImageURL);
                _buttonContainer.SetAppearance(Status);
            }

            public StackPanel GetStackPanel()
            {
                return _stackPanel;
            }

            private bool _TargetMovieIsValid(Movie movie)
            {
                if (movie == null ||
                   movie.Id < 0 ||
                   string.IsNullOrEmpty(movie.ImageURL) ||
                   string.IsNullOrEmpty(movie.Title))
                    return false;
                return true;
            }

            public void SwitchMovieStatus()
            {
                if (!API.IsRentedByCustomer(State.User, TargetMovie.Id))
                    if (Status == MovieStatus.Hyr)
                        Status = MovieStatus.Vald;
                    else
                        Status = MovieStatus.Hyr;

                _buttonContainer.SetAppearance(Status);
            }

            private class MovieButtonContainer
            {
                private static readonly Dictionary<MovieStatus, (string buttonText, SolidColorBrush buttonBackgroundColor, SolidColorBrush buttonTextColor)> _buttonStatusesAndColor =
                                    new Dictionary<MovieStatus, (string buttonText, SolidColorBrush buttonBackgroundColor, SolidColorBrush buttonTextColor)>() {

                {MovieStatus.Hyr,
                        ( buttonText: MovieStatus.Hyr.ToString(),
                          buttonBackgroundColor: new SolidColorBrush(Color.FromRgb(0x00, 0xCC, 0x00)),
                          buttonTextColor: new SolidColorBrush(Color.FromRgb(255,255,255))
                        )
                },
                {MovieStatus.Vald,
                        ( buttonText: MovieStatus.Vald.ToString(),
                          buttonBackgroundColor: new SolidColorBrush(Color.FromRgb(0x22, 0x74, 0xA5)),
                          buttonTextColor: new SolidColorBrush(Color.FromRgb(255,255,255))
                        )
                },
                {MovieStatus.Uthyrd,
                        ( buttonText: MovieStatus.Uthyrd.ToString(),
                          buttonBackgroundColor: new SolidColorBrush(Color.FromRgb(0xCC, 0, 0)),
                          buttonTextColor: new SolidColorBrush(Color.FromRgb(0,0,0))
                        )
                }
                };

                private const int _BUTTONWIDTH = 60;
                public Button Button;
                public MovieButtonContainer(MovieStatus movieStatus, Movie movie,
                                            MouseButtonEventHandler mouseButtonEventHandler)
                {
                    Button = new Button();
                    if (API.IsRentedByCustomer(State.User, movie.Id))
                        Button.IsEnabled = false;

                    SetAppearance(movieStatus);

                    Button.Width = _BUTTONWIDTH;

                    Button.PreviewMouseUp += mouseButtonEventHandler;
                }

                public void SetAppearance(MovieStatus movieStatus)
                {
                    if (movieStatus.Equals(MovieStatus.Uthyrd))
                        Button.IsEnabled = false;

                    Button.Content =    _buttonStatusesAndColor[movieStatus].buttonText;
                    Button.Background = _buttonStatusesAndColor[movieStatus].buttonBackgroundColor;
                    Button.Foreground = _buttonStatusesAndColor[movieStatus].buttonTextColor;
                }
            }

            private class MovieImageContainer
            {
                public Image Image;
                public MovieImageContainer(string movieImageURI)
                {
                    Image = new Image();

                    Image.Cursor = Cursors.Hand;
                    Image.HorizontalAlignment = HorizontalAlignment.Center;
                    Image.VerticalAlignment = VerticalAlignment.Stretch;
                    Image.Margin = new Thickness(4, 4, 4, 4);
                    Image.Height = 140;

                    SetActiveMovie(movieImageURI);
                }

                public void SetActiveMovie(string movieImageURI)
                {
                    Image.Source = new BitmapImage(new Uri(movieImageURI));
                }
            }
        }

        private const int NUMOFCOLS = 3;
        private const int NUMOFROWS = 3;
        private const int SLICESIZE = NUMOFROWS * NUMOFCOLS;
        private int pageIndex = 0;


        private List<SelectMovieContainer> moviesForRent = new List<SelectMovieContainer>();
        public static Dictionary<Movie, UIElement> selectedMovies = new Dictionary<Movie, UIElement>();

        public MainWindow()
        {
            InitializeComponent();

            RentMoviesButton.Visibility = Visibility.Hidden;
            RentMoviesButton.PreviewMouseUp += _rentMovies;

            LogoutButton.PreviewMouseUp += _logout;

            PageDisplay.Text = $"Sida {pageIndex} of {Math.Ceiling((double)API.GetNumOfMovies() / (double)(NUMOFCOLS * NUMOFROWS))}"; //round up to nearest integer

            PageSelector.PreviewKeyUp += _changePage;

            GreetText.Text = $"Välkommen {State.User.FirstName}";

            foreach (var m in API.GetMovieSlice(pageIndex, SLICESIZE))
                moviesForRent.Add(new SelectMovieContainer(m, ButtonClicked));

            for (int y = 0; y < NUMOFROWS; y++)
            {
                for (int x = 0; x < NUMOFCOLS; x++)
                {
                    int i = y * (MovieGrid.ColumnDefinitions.Count-1) + x; //First columndef doesn't count.
                    //if (i < moviesForRent.Count)
                    //{
                        try
                        {
                            MovieGrid.Children.Add(moviesForRent[i].GetStackPanel());

                            Grid.SetRow(moviesForRent[i].GetStackPanel(), y+1);
                            Grid.SetColumn(moviesForRent[i].GetStackPanel(), x+1);
                        }
                        catch (Exception e) when
                            (e is ArgumentNullException ||
                             e is System.IO.FileNotFoundException ||
                             e is UriFormatException)
                        {
                            continue;
                        }
                    //}
                }
            }
        }


        private void _changePage(object sender, KeyEventArgs e)
        {
            if (e.Key != System.Windows.Input.Key.Enter) 
                return;

            int x;

            if (int.TryParse(PageSelector.Text, out x))
            {
                //load correct page.
                //is index inside spectrum?
                //change appearance of pagenumber - pageIndex
            }
        }


        private void _logout(object sender, MouseButtonEventArgs e)
        {
            var x = API.GetMovieSlice(30, 1).First();

            moviesForRent[0].SetNewMovie(x);




            //State.User = null;

            //var next_window = new LoginWindow();
            //next_window.Show();
            //Close();
        }

        private void _setMoviesForRent(int page)
        {
            //foreach (var m in API.GetMovieSlice(pageIndex, SLICESIZE))
                //moviesForRent.Add(new SelectMovieContainer(m, ButtonClicked));
        }

        private void _rentMovies(object sender, MouseButtonEventArgs e)
        {
            ChosenMoviesStack.Children.Clear();
            ChosenMovieScrollViewer.Visibility = Visibility.Hidden;

            var movieIds = selectedMovies.Select(sm => sm.Key.Id).ToList();

            foreach(var movieId in movieIds)
                API.RentMovie(State.User, movieId);

            //Uppdatera buttons.
        }


        private void ButtonClicked(object sender, MouseButtonEventArgs e) 
        {
            if(sender is Button)
            {
                var buttonParentStackPanel = (sender as Button).Parent;
                var x = Grid.GetColumn(buttonParentStackPanel as UIElement)-1;
                var y = Grid.GetRow(buttonParentStackPanel as UIElement)-1;

                //int i = y * MovieGrid.ColumnDefinitions.Count + x;
                var selectedMovieContainer = moviesForRent[y * (MovieGrid.ColumnDefinitions.Count - 1) + x];

                if (selectedMovieContainer.Status == MovieStatus.Vald)
                {
                    ChosenMoviesStack.Children.Remove(selectedMovies[selectedMovieContainer.TargetMovie]);
                    selectedMovies.Remove(selectedMovieContainer.TargetMovie);
                    if(ChosenMoviesStack.Children.Count < 1)
                    {
                        ChosenMovieScrollViewer.Visibility = Visibility.Hidden;
                        RentMoviesButton.Visibility = Visibility.Hidden;

                    }
                }
                else if (selectedMovieContainer.Status == MovieStatus.Hyr)
                {
                    selectedMovies.Add(selectedMovieContainer.TargetMovie, new TextBox()
                    {
                        Text = selectedMovieContainer.TargetMovie.Title
                    });
                    ChosenMoviesStack.Children.Add(selectedMovies[selectedMovieContainer.TargetMovie]);
                    ChosenMovieScrollViewer.Visibility = Visibility.Visible; //skips if-statement; would still require 1 check.
                    RentMoviesButton.Visibility = Visibility.Visible;
                }
                else
                    throw new Exception("Hiring logic invalid ; Unexpected error occured.");

                selectedMovieContainer.SwitchMovieStatus();

                //(sender as Button).Content = selectedMovie.movieDto.Status.ToString();
            } else 
                throw new Exception("ButtonClicked used by component that shouldn't");
        }
    }
}
