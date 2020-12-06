using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
        private class MovieDto
        {
            public MovieSelection Status { get; private set; } = MovieSelection.Hyr;
            public Movie TargetMovie { get; set; }
            public MovieDto(Movie movie, Customer activeCustomer)
            {
                TargetMovie = movie;

                if(API.IsRentedByCustomer(State.User, TargetMovie.Id))
                {
                    Status = MovieSelection.Uthyrd;
                }
            }

            public void SwitchHiredStatus()
            {
                if (!API.IsRentedByCustomer(State.User, TargetMovie.Id) )
                    if (Status == MovieSelection.Hyr)
                        Status = MovieSelection.Vald;
                    else
                        Status = MovieSelection.Hyr;
            }
        }

        private class ButtonContainer
        {
            private static readonly Dictionary<MovieSelection, string> _buttonStatuses = new Dictionary<MovieSelection, string>() {
                {MovieSelection.Hyr, MovieSelection.Hyr.ToString() },
                {MovieSelection.Vald, MovieSelection.Vald.ToString() },
                {MovieSelection.Uthyrd, MovieSelection.Uthyrd.ToString() }
            };
            private const int _BUTTONWIDTH = 60;

            private Button _targetButton;

            public ButtonContainer(MovieDto belongingMovie, MouseButtonEventHandler mouseButtonEventHandler)
            {
                _targetButton = new Button();

                if (API.IsRentedByCustomer(State.User, belongingMovie.TargetMovie.Id))
                    _targetButton.IsEnabled = false;

                _targetButton.Content = _buttonStatuses[belongingMovie.Status];

                _targetButton.Width = _BUTTONWIDTH;

                _targetButton.PreviewMouseUp += mouseButtonEventHandler;
            }

            public void AddToStackpanel(ref StackPanel stack) { 

                if(!stack.Children.Contains(_targetButton))

                    stack.Children.Add(_targetButton); 
            }
        }


        private const int NUMOFCOLS = 4;
        private const int NUMOFROWS = 6;

        

        private List<MovieDto> movies = new List<MovieDto>();

        
        public MainWindow()
        {
            InitializeComponent();

            GreetText.Text = $"Välkommen {State.User.FirstName}";


            foreach(var m in API.GetMovieSlice(0,40))
            {
                movies.Add(new MovieDto(m, State.User));
            }

            for (int y = 1; y < NUMOFROWS; y++)
            {
                for (int x = 1; x <= NUMOFCOLS; x++)
                {
                    int i = y * MovieGrid.ColumnDefinitions.Count + x;
                    if (i < movies.Count)
                    {
                        
                        try
                        {
                            var stackPanel = new StackPanel()
                            {
                                Orientation = Orientation.Vertical
                            };

                            //Define image properties
                            var image = new Image() {
                                Cursor = Cursors.Hand,
                                HorizontalAlignment = HorizontalAlignment.Center,
                                VerticalAlignment = VerticalAlignment.Stretch,
                                Source = new BitmapImage(new Uri(movies[i].TargetMovie.ImageURL)),
                                Margin = new Thickness(4, 4, 4, 4)
                            };
                            image.Height = 140;

                            stackPanel.Children.Add(image);


                            //Define button properties
                            var buttonContainer = new ButtonContainer(movies[i], ButtonClicked);

                            buttonContainer.AddToStackpanel(ref stackPanel);

                            MovieGrid.Children.Add(stackPanel);
                            Grid.SetRow(stackPanel, y);
                            Grid.SetColumn(stackPanel, x);

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
                var selectedMovie = movies[y * MovieGrid.ColumnDefinitions.Count + x];


                //Behöver kolla om redan vald.
                //Behöver kunna plocka bort.
                //Behöver se till så redan hyrda inte går att klicka.
                if (selectedMovie.Status == MovieSelection.Vald)
                {
                    ChosenMoviesStack.Children.Remove(selectedMovies[selectedMovie]);
                    selectedMovies.Remove(selectedMovie);
                    if(ChosenMoviesStack.Children.Count < 1)
                        ChosenMovieScrollViewer.Visibility = Visibility.Hidden;
                }
                else if (selectedMovie.Status == MovieSelection.Hyr)
                {
                    selectedMovies.Add(selectedMovie, new TextBox()
                    {
                        //Name = MOVIEIDPREFIX + selectedMovie.TargetMovie.Id.ToString(), //Will be used for removing movie.
                        Text = selectedMovie.TargetMovie.Title
                    });
                    ChosenMoviesStack.Children.Add(selectedMovies[selectedMovie]);
                    ChosenMovieScrollViewer.Visibility = Visibility.Visible;
                }
                else
                    throw new Exception("Hiring logic invalid ; Unexpected error occured.");

                selectedMovie.SwitchHiredStatus();
                (sender as Button).Content = selectedMovie.Status.ToString();

            }
        }
    }
}
