using System.Windows;
using TrafficSimulator.Core.Models;
using TrafficSimulator.UI.Controls;
using TrafficSimulator.UI.ViewModels;

namespace TrafficSimulator.UI
{
    public partial class MainWindow : Window
    {
        private MainViewModel _viewModel;
        private GraphEditorCanvas _graphEditor;
        private PropertiesPanel _propertiesPanel;

        public MainWindow()
        {
            InitializeComponent();

            // Inicializar el ViewModel
            _viewModel = new MainViewModel();
            DataContext = _viewModel;

            // Inicializar los controles
            InitializeControls();

            // Configurar los manejadores de eventos
            SetupEventHandlers();
        }

        private void InitializeControls()
        {
            // Crear el editor de grafos
            _graphEditor = new GraphEditorCanvas
            {
                ViewModel = _viewModel
            };
            graphEditorContainer.Content = _graphEditor;

            // Crear el panel de propiedades
            _propertiesPanel = new PropertiesPanel();
            propertiesPanelContainer.Content = _propertiesPanel;
        }

        private void SetupEventHandlers()
        {
            // Manejadores de eventos para los botones de la barra de herramientas
            btnNew.Click += (s, e) => NewGraph();
            btnOpen.Click += (s, e) => OpenGraph();
            btnSave.Click += (s, e) => SaveGraph();

            // Manejadores para los botones de modo de edición
            tglAddCity.Click += (s, e) => SetEditorMode(EditorMode.AddCity);
            tglAddRoad.Click += (s, e) => SetEditorMode(EditorMode.AddRoad);
            tglSelect.Click += (s, e) => SetEditorMode(EditorMode.Select);

            // Manejadores para los botones de simulación
            btnStartSimulation.Click += (s, e) => _viewModel.StartSimulation();
            btnPauseSimulation.Click += (s, e) => _viewModel.PauseSimulation();
            btnStopSimulation.Click += (s, e) => _viewModel.StopSimulation();

            // Manejadores para eventos del panel de propiedades
            _propertiesPanel.CityPropertiesChanged += (s, city) => UpdateCityInEditor(city);
            _propertiesPanel.RoadPropertiesChanged += (s, road) => UpdateRoadInEditor(road);
            _propertiesPanel.DeleteElementRequested += (s, e) => DeleteSelectedElement();

            // Suscribirse a cambios en el ViewModel
            _viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(MainViewModel.IsSimulationRunning))
                {
                    UpdateSimulationButtonsState();
                }
            };
        }

        private void SetEditorMode(EditorMode mode)
        {
            // Actualizar los botones toggle
            tglAddCity.IsChecked = mode == EditorMode.AddCity;
            tglAddRoad.IsChecked = mode == EditorMode.AddRoad;
            tglSelect.IsChecked = mode == EditorMode.Select;

            // Actualizar el modo en el ViewModel
            _viewModel.CurrentEditorMode = mode;

            // Actualizar el mensaje de estado
            switch (mode)
            {
                case EditorMode.AddCity:
                    txtStatus.Text = "Haga clic en el lienzo para agregar una ciudad";
                    break;
                case EditorMode.AddRoad:
                    txtStatus.Text = "Haga clic en dos ciudades para conectarlas con una carretera";
                    break;
                case EditorMode.Select:
                    txtStatus.Text = "Haga clic en elementos para seleccionarlos o arrastrarlos";
                    break;
            }
        }

        private void NewGraph()
        {
            if (MessageBox.Show("¿Está seguro de que desea crear un nuevo grafo? Se perderán los cambios no guardados.",
                "Nuevo grafo", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                _viewModel.NewGraph();
                _graphEditor.UpdateCanvasFromViewModel();
                UpdateStatusBar();
            }
        }

        private void OpenGraph()
        {
            // Implementar la funcionalidad de abrir un grafo guardado
            // Por ahora solo mostraremos un mensaje
            MessageBox.Show("La funcionalidad de abrir grafo se implementará más adelante.",
                "Abrir grafo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void SaveGraph()
        {
            // Implementar la funcionalidad de guardar el grafo
            // Por ahora solo mostraremos un mensaje
            MessageBox.Show("La funcionalidad de guardar grafo se implementará más adelante.",
                "Guardar grafo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void UpdateCityInEditor(City city)
        {
            // Actualizar la visualización de la ciudad en el editor
            _graphEditor.UpdateCity(city);
        }

        private void UpdateRoadInEditor(Road road)
        {
            // Actualizar la visualización de la carretera en el editor
            _graphEditor.UpdateRoad(road);
        }

        private void DeleteSelectedElement()
        {
            // Implementar la eliminación del elemento seleccionado
            if (_propertiesPanel.SelectedElement is City city)
            {
                if (MessageBox.Show($"¿Está seguro de que desea eliminar la ciudad '{city.Name}'?",
                    "Eliminar ciudad", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    _viewModel.TrafficGraph.RemoveCity(city);
                    _graphEditor.UpdateCanvasFromViewModel();
                    _propertiesPanel.SelectedElement = null;
                    UpdateStatusBar();
                }
            }
            else if (_propertiesPanel.SelectedElement is Road road)
            {
                if (MessageBox.Show($"¿Está seguro de que desea eliminar la carretera '{road.Name}'?",
                    "Eliminar carretera", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    _viewModel.TrafficGraph.RemoveRoad(road);
                    _graphEditor.UpdateCanvasFromViewModel();
                    _propertiesPanel.SelectedElement = null;
                    UpdateStatusBar();
                }
            }
        }

        private void UpdateSimulationButtonsState()
        {
            bool isRunning = _viewModel.IsSimulationRunning;
            btnStartSimulation.IsEnabled = !isRunning;
            btnPauseSimulation.IsEnabled = isRunning;
            btnStopSimulation.IsEnabled = isRunning;
        }

        private void UpdateStatusBar()
        {
            // Actualizar contadores en la barra de estado
            txtCityCount.Text = _viewModel.TrafficGraph.Cities.Count.ToString();
            txtRoadCount.Text = _viewModel.TrafficGraph.Roads.Count.ToString();
            txtVehicleCount.Text = _viewModel.Vehicles.Count.ToString();
        }
    }
}