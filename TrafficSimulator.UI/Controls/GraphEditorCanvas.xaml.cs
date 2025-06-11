using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using TrafficSimulator.Core.Models;
using TrafficSimulator.UI.ViewModels;
using DrawingPoint = System.Drawing.Point;
using WpfPoint = System.Windows.Point;

namespace TrafficSimulator.UI.Controls
{
    public partial class GraphEditorCanvas : UserControl
    {
        private MainViewModel? _viewModel;
        private UIElement? _draggedElement;
        private CityControl? _selectedSourceCity;
        private WpfPoint _dragStartPosition;
        private Dictionary<Guid, CityControl> _cityControlsMap = new Dictionary<Guid, CityControl>();
        private Dictionary<Guid, RoadControl> _roadControlsMap = new Dictionary<Guid, RoadControl>();

        public MainViewModel? ViewModel
        {
            get => _viewModel;
            set
            {
                if (_viewModel != value)
                {
                    if (_viewModel != null)
                    {
                        UnsubscribeFromViewModelEvents();
                    }

                    _viewModel = value;

                    if (_viewModel != null)
                    {
                        SubscribeToViewModelEvents();
                        UpdateCanvasFromViewModel();
                    }
                }
            }
        }

        public GraphEditorCanvas()
        {
            InitializeComponent();
        }

        private void SubscribeToViewModelEvents()
        {
            // Aquí nos suscribiremos a eventos del ViewModel cuando los tengamos
        }

        private void UnsubscribeFromViewModelEvents()
        {
            // Aquí nos desuscribiremos de eventos del ViewModel cuando los tengamos
        }

        public void UpdateCanvasFromViewModel()
        {
            if (_viewModel == null || _viewModel.TrafficGraph == null)
                return;

            EditorCanvas.Children.Clear();
            _cityControlsMap.Clear();
            _roadControlsMap.Clear();

            foreach (var road in _viewModel.TrafficGraph.Roads)
            {
                AddRoadToCanvas(road);
            }

            foreach (var city in _viewModel.TrafficGraph.Cities)
            {
                AddCityToCanvas(city);
            }
        }

        public void UpdateCity(City city)
        {
            if (_cityControlsMap.TryGetValue(city.Id, out var cityControl))
            {
                Canvas.SetLeft(cityControl, city.Position.X - cityControl.Width / 2);
                Canvas.SetTop(cityControl, city.Position.Y - cityControl.Height / 2);

                cityControl.City = city;
                UpdateConnectedRoads(city);
            }
        }

        public void UpdateRoad(Road road)
        {
            if (_roadControlsMap.TryGetValue(road.Id, out var roadControl))
            {
                roadControl.Road = road;
                UpdateRoadPosition(roadControl);
            }
        }

        public void AddCityToCanvas(City city)
        {
            var cityControl = new CityControl(city);
            cityControl.MouseLeftButtonDown += City_MouseLeftButtonDown;
            cityControl.MouseLeftButtonUp += City_MouseLeftButtonUp;
            cityControl.MouseRightButtonDown += City_MouseRightButtonDown;

            Canvas.SetLeft(cityControl, city.Position.X - cityControl.Width / 2);
            Canvas.SetTop(cityControl, city.Position.Y - cityControl.Height / 2);

            EditorCanvas.Children.Add(cityControl);
            _cityControlsMap[city.Id] = cityControl;
        }

        public void AddRoadToCanvas(Road road)
        {
            var roadControl = new RoadControl(road);
            roadControl.MouseLeftButtonDown += Road_MouseLeftButtonDown;

            EditorCanvas.Children.Add(roadControl);
            _roadControlsMap[road.Id] = roadControl;

            UpdateRoadPosition(roadControl);
        }

        public void UpdateRoadPosition(RoadControl roadControl)
        {
            var road = roadControl.Road;
            var source = road.SourceCity.Position;
            var target = road.DestinationCity.Position;

            roadControl.X1 = source.X;
            roadControl.Y1 = source.Y;
            roadControl.X2 = target.X;
            roadControl.Y2 = target.Y;
        }

        private void EditorCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_viewModel?.CurrentEditorMode == EditorMode.AddCity)
            {
                var position = e.GetPosition(EditorCanvas);
                AddNewCity(position);
            }
            else if (_viewModel?.CurrentEditorMode == EditorMode.Select)
            {
                _selectedSourceCity = null;
                ConnectionLine.Visibility = Visibility.Collapsed;
            }
        }

        private void EditorCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (_draggedElement != null && e.LeftButton == MouseButtonState.Pressed)
            {
                var position = e.GetPosition(EditorCanvas);
                var offset = position - _dragStartPosition;

                if (_draggedElement is CityControl cityControl)
                {
                    var left = Canvas.GetLeft(cityControl) + offset.X;
                    var top = Canvas.GetTop(cityControl) + offset.Y;
                    Canvas.SetLeft(cityControl, left);
                    Canvas.SetTop(cityControl, top);

                    cityControl.City.Position = new DrawingPoint(
                        (int)(left + cityControl.Width / 2),
                        (int)(top + cityControl.Height / 2)
                    );

                    UpdateConnectedRoads(cityControl.City);

                    _dragStartPosition = position;
                }
            }

            if (_viewModel?.CurrentEditorMode == EditorMode.AddRoad && _selectedSourceCity != null)
            {
                var position = e.GetPosition(EditorCanvas);
                ConnectionLine.X2 = position.X;
                ConnectionLine.Y2 = position.Y;
            }
        }

        private void EditorCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _draggedElement = null;
        }

        private void EditorCanvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            _selectedSourceCity = null;
            ConnectionLine.Visibility = Visibility.Collapsed;
            _draggedElement = null;
        }

        private void City_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var cityControl = sender as CityControl;
            if (cityControl == null) return;

            if (_viewModel?.CurrentEditorMode == EditorMode.Select)
            {
                _draggedElement = cityControl;
                _dragStartPosition = e.GetPosition(EditorCanvas);
                e.Handled = true;
            }
            else if (_viewModel?.CurrentEditorMode == EditorMode.AddRoad)
            {
                if (_selectedSourceCity == null)
                {
                    _selectedSourceCity = cityControl;
                    var sourcePosition = cityControl.City.Position;
                    ConnectionLine.X1 = sourcePosition.X;
                    ConnectionLine.Y1 = sourcePosition.Y;
                    ConnectionLine.X2 = sourcePosition.X;
                    ConnectionLine.Y2 = sourcePosition.Y;
                    ConnectionLine.Visibility = Visibility.Visible;
                }
                else
                {
                    var sourceCity = _selectedSourceCity.City;
                    var targetCity = cityControl.City;

                    if (sourceCity.Id != targetCity.Id)
                    {
                        AddNewRoad(sourceCity, targetCity);
                    }

                    _selectedSourceCity = null;
                    ConnectionLine.Visibility = Visibility.Collapsed;
                }
                e.Handled = true;
            }
        }

        private void City_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // Manejar si es necesario
        }

        private void City_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var cityControl = sender as CityControl;
            if (cityControl == null) return;

            e.Handled = true;
        }

        private void Road_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var roadControl = sender as RoadControl;
            if (roadControl == null) return;

            if (_viewModel?.CurrentEditorMode == EditorMode.Select)
            {
                e.Handled = true;
            }
        }

        private void UpdateConnectedRoads(City city)
        {
            foreach (var road in _viewModel?.TrafficGraph?.Roads ?? new List<Road>())
            {
                if (road.SourceCity.Id == city.Id || road.DestinationCity.Id == city.Id)
                {
                    if (_roadControlsMap.TryGetValue(road.Id, out var roadControl))
                    {
                        UpdateRoadPosition(roadControl);
                    }
                }
            }
        }

        private void AddNewCity(WpfPoint position)
        {
            var cityName = $"City_{_viewModel?.TrafficGraph?.Cities.Count + 1}";
            var newCity = new City(cityName, new DrawingPoint((int)position.X, (int)position.Y));
            _viewModel?.TrafficGraph?.AddCity(newCity);
            AddCityToCanvas(newCity);
        }

        private void AddNewRoad(City source, City destination)
        {
            var existingRoad = _viewModel?.TrafficGraph?.Roads.Find(r =>
                (r.SourceCity.Id == source.Id && r.DestinationCity.Id == destination.Id) ||
                (r.SourceCity.Id == destination.Id && r.DestinationCity.Id == source.Id));

            if (existingRoad != null)
                return;

            var roadName = $"Road_{source.Name}_to_{destination.Name}";
            var newRoad = new Road(source, destination, roadName);
            _viewModel?.TrafficGraph?.AddRoad(newRoad);
            AddRoadToCanvas(newRoad);
        }
    }
}
