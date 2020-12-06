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
        public enum MovieSelection
        {
            Hyr, Vald, Uthyrd
        }
        public class MovieDto
        {
            public MovieSelection Status { get; private set; } = MovieSelection.Hyr;
            public Movie TargetMovie { get; set; }
            public MovieDto(Movie movie, Customer activeCustomer) //activeCustomer not neccesary?
            {
                if (movie == null)
                    throw new Exception("MovieDto: Movie was null; this class is not allowed to be used like this; dependencies in other classes."); //See MovieButtonContainer
                if (activeCustomer == null)
                    throw new Exception("MovieDto: activeCustomer was null; this class is not allowed to be used like this; dependencies in other classes.");

                TargetMovie = movie;

                if (API.IsRentedByCustomer(State.User, TargetMovie.Id))
                {
                    Status = MovieSelection.Uthyrd;
                }
            }

            public void SwitchHiredStatus()
            {
                if (!API.IsRentedByCustomer(State.User, TargetMovie.Id))
                    if (Status == MovieSelection.Hyr)
                        Status = MovieSelection.Vald;
                    else
                        Status = MovieSelection.Hyr;
            }
        }


        private class SelectMovieContainer {

            public MovieDto movieDto { get; set; }

            private MovieImageContainer _imageContainer;
            private MovieButtonContainer _buttonContainer;
            private StackPanel _stackPanel;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="targetElement">The element to be contained by the selectmovie-box.</param>
            /// <param name="stack">The stackpanel the element shall belong to.</param>
            public SelectMovieContainer(Movie movie, MouseButtonEventHandler mouseButtonEventHandler)
            {
                if (!_TargetMovieIsValid(movie))
                    throw new Exception("Invalid use o SelectMovieContainer.");

                movieDto = new MovieDto(movie, State.User);

                _stackPanel = new StackPanel()
                {
                    Orientation = Orientation.Vertical
                };

                _imageContainer = new MovieImageContainer(movieDto);
                _imageContainer.AddToStack(ref _stackPanel);

                _buttonContainer = new MovieButtonContainer(movieDto, mouseButtonEventHandler);
                _buttonContainer.AddToStack(ref _stackPanel);
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

            public void AddStackToGridLocation(ref Grid MovieGrid, int x, int y)
            {
                MovieGrid.Children.Add(_stackPanel);
                Grid.SetRow(_stackPanel, y);
                Grid.SetColumn(_stackPanel, x);
            }

            public void SwitchHiredStatus()
            {
                movieDto.SwitchHiredStatus();
                _buttonContainer.SetAppearance(movieDto);
            }

            private interface MovieContainerElement
            {
                public void AddToStack(ref StackPanel stack);
            }

            private class MovieButtonContainer : MovieContainerElement //TODO: Check only SelectMovieContainer may use this.
            {
                private static readonly Dictionary<MovieSelection, (string buttonText, SolidColorBrush buttonBackgroundColor, SolidColorBrush buttonTextColor)> _buttonStatusesAndColor =
                                    new Dictionary<MovieSelection, (string buttonText, SolidColorBrush buttonBackgroundColor, SolidColorBrush buttonTextColor)>() {

                {MovieSelection.Hyr,
                        ( buttonText: MovieSelection.Hyr.ToString(),
                          buttonBackgroundColor: new SolidColorBrush(Color.FromArgb(0xFF, 0x00, 0xCC, 0x00)),
                          buttonTextColor: new SolidColorBrush(Color.FromRgb(255,255,255))
                        )
                },
                {MovieSelection.Vald,
                        ( buttonText: MovieSelection.Vald.ToString(),
                          buttonBackgroundColor: new SolidColorBrush(Color.FromArgb(0xFF, 0x22, 0x74, 0xA5)),
                          buttonTextColor: new SolidColorBrush(Color.FromRgb(255,255,255))
                        )
                },
                {MovieSelection.Uthyrd,
                        ( buttonText: MovieSelection.Uthyrd.ToString(),
                          buttonBackgroundColor: new SolidColorBrush(Color.FromArgb(0xFF, 0xCC, 0, 0)),
                          buttonTextColor: new SolidColorBrush(Color.FromRgb(255,255,255))
                        )
                }
                };

                private const int _BUTTONWIDTH = 60;
                private Button _button;
                public MovieButtonContainer(MovieDto belongingMovie,
                                               MouseButtonEventHandler mouseButtonEventHandler)
                {
                    _button = new Button();
                    if (API.IsRentedByCustomer(State.User, belongingMovie.TargetMovie.Id))
                        _button.IsEnabled = false;

                    SetAppearance(belongingMovie);

                    _button.Width = _BUTTONWIDTH;

                    _button.PreviewMouseUp += mouseButtonEventHandler;
                }

                public void SetAppearance(MovieDto belongingMovie)
                {
                    _button.Content = _buttonStatusesAndColor[belongingMovie.Status].buttonText;
                    _button.Background = _buttonStatusesAndColor[belongingMovie.Status].buttonBackgroundColor;
                    _button.Foreground = _buttonStatusesAndColor[belongingMovie.Status].buttonTextColor;
                }

                public void AddToStack(ref StackPanel stack)
                {
                    if (!stack.Children.Contains(_button))
                        stack.Children.Add(_button);
                }
            }

            private class MovieImageContainer : MovieContainerElement
            {
                private Image _image;
                public MovieImageContainer(MovieDto movieDto)
                {
                    _image = new Image();

                    _image.Cursor = Cursors.Hand;
                    _image.HorizontalAlignment = HorizontalAlignment.Center;
                    _image.VerticalAlignment = VerticalAlignment.Stretch;
                    _image.Source = new BitmapImage(new Uri(movieDto.TargetMovie.ImageURL));
                    _image.Margin = new Thickness(4, 4, 4, 4);
                    _image.Height = 140;
                }

                public void AddToStack(ref StackPanel stack)
                {
                    if (!stack.Children.Contains(_image))
                        stack.Children.Add(_image);
                }
            }
        }


        private const int NUMOFCOLS = 4;
        private const int NUMOFROWS = 6;



        private List<SelectMovieContainer> moviesForRent = new List<SelectMovieContainer>();
        
        public MainWindow()
        {
            InitializeComponent();

            GreetText.Text = $"Välkommen {State.User.FirstName}";

            foreach(var m in API.GetMovieSlice(0,40))
                moviesForRent.Add(new SelectMovieContainer(m, ButtonClicked));

            for (int y = 1; y < NUMOFROWS; y++)
            {
                for (int x = 1; x <= NUMOFCOLS; x++)
                {
                    int i = y * MovieGrid.ColumnDefinitions.Count + x;
                    if (i < moviesForRent.Count)
                    {
                        try
                        {
                            moviesForRent[i].AddStackToGridLocation(ref MovieGrid, x, y);

                        }
                        catch (Exception e) when 
                            (e is ArgumentNullException || 
                             e is System.IO.FileNotFoundException || 
                             e is UriFormatException)
                        {
                            continue;
                        }
                    }
                }
            }
        }

        private void _rentMovies(Movie[] movies)
        {
            throw new NotImplementedException();
        }


        private const int LISTSPACING = 20;
        private const int LEFTMARGIN = 20;
        private const int SELECTEDMOVIELISTSTARTY = 120;
        private const string MOVIEIDPREFIX = "ID";

        private Dictionary<MovieDto, UIElement> selectedMovies = new Dictionary<MovieDto, UIElement>();

        private void ButtonClicked(object sender, MouseButtonEventArgs e) 
        {
            if(sender is Button)
            {
                var buttonParentStackPanel = (sender as Button).Parent;
                var x = Grid.GetColumn(buttonParentStackPanel as UIElement);
                var y = Grid.GetRow(buttonParentStackPanel as UIElement);

                //int i = y * MovieGrid.ColumnDefinitions.Count + x;
                var selectedMovie = moviesForRent[y * MovieGrid.ColumnDefinitions.Count + x];

                //Behöver se till så redan hyrda inte går att klicka.
                if (selectedMovie.movieDto.Status == MovieSelection.Vald)
                {
                    ChosenMoviesStack.Children.Remove(selectedMovies[selectedMovie.movieDto]);
                    selectedMovies.Remove(selectedMovie.movieDto);
                    if(ChosenMoviesStack.Children.Count < 1)
                        ChosenMovieScrollViewer.Visibility = Visibility.Hidden;
                }
                else if (selectedMovie.movieDto.Status == MovieSelection.Hyr)
                {
                    selectedMovies.Add(selectedMovie.movieDto, new TextBox()
                    {
                        //Name = MOVIEIDPREFIX + selectedMovie.TargetMovie.Id.ToString(), //Will be used for removing movie.
                        Text = selectedMovie.movieDto.TargetMovie.Title
                    });
                    ChosenMoviesStack.Children.Add(selectedMovies[selectedMovie.movieDto]);
                    ChosenMovieScrollViewer.Visibility = Visibility.Visible; //skips if-statement; would still require 1 check.
                }
                else
                    throw new Exception("Hiring logic invalid ; Unexpected error occured.");

                selectedMovie.SwitchHiredStatus();
                (sender as Button).Content = selectedMovie.movieDto.Status.ToString();
            }
        }
    }
}
