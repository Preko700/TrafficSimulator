using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using TrafficSimulator.Core.Models;

namespace TrafficSimulator.UI.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private TrafficGraph _trafficGraph;
        private ObservableCollection<Vehicle> _vehicles;
        private SimulationSettings _simulationSettings;
        private Vehicle _selectedVehicle;
        private bool _isSimulationRunning;
        private EditorMode _currentEditorMode;

        public TrafficGraph TrafficGraph
        {
            get => _trafficGraph;
            set
            {
                if (_trafficGraph != value)
                {
                    _trafficGraph = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<Vehicle> Vehicles
        {
            get => _vehicles;
            set
            {
                if (_vehicles != value)
                {
                    _vehicles = value;
                    OnPropertyChanged();
                }
            }
        }

        public SimulationSettings SimulationSettings
        {
            get => _simulationSettings;
            set
            {
                if (_simulationSettings != value)
                {
                    _simulationSettings = value;
                    OnPropertyChanged();
                }
            }
        }

        public Vehicle SelectedVehicle
        {
            get => _selectedVehicle;
            set
            {
                if (_selectedVehicle != value)
                {
                    _selectedVehicle = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsSimulationRunning
        {
            get => _isSimulationRunning;
            set
            {
                if (_isSimulationRunning != value)
                {
                    _isSimulationRunning = value;
                    OnPropertyChanged();
                }
            }
        }

        public EditorMode CurrentEditorMode
        {
            get => _currentEditorMode;
            set
            {
                if (_currentEditorMode != value)
                {
                    _currentEditorMode = value;
                    OnPropertyChanged();
                }
            }
        }

        public MainViewModel()
        {
            TrafficGraph = new TrafficGraph();
            Vehicles = new ObservableCollection<Vehicle>();
            SimulationSettings = new SimulationSettings();
            CurrentEditorMode = EditorMode.Select;
        }

        public void StartSimulation()
        {
            if (!IsSimulationRunning)
            {
                IsSimulationRunning = true;
                // Aquí iría la lógica para iniciar la simulación
            }
        }

        public void PauseSimulation()
        {
            if (IsSimulationRunning)
            {
                // Aquí iría la lógica para pausar la simulación
            }
        }

        public void StopSimulation()
        {
            if (IsSimulationRunning)
            {
                IsSimulationRunning = false;
                // Aquí iría la lógica para detener la simulación
            }
        }

        public void NewGraph()
        {
            TrafficGraph = new TrafficGraph();
            Vehicles.Clear();
        }

        public void SaveGraph(string filePath)
        {
            // Aquí iría la lógica para guardar el grafo
        }

        public void LoadGraph(string filePath)
        {
            // Aquí iría la lógica para cargar un grafo
        }

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }

    public enum EditorMode
    {
        Select,
        AddCity,
        AddRoad
    }
}