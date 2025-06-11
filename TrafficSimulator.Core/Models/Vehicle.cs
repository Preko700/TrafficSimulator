using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

namespace TrafficSimulator.Core.Models
{
    public class Vehicle : INotifyPropertyChanged
    {
        private Guid _id;
        private City _source;
        private City _destination;
        private Point _currentPosition;
        private double _speed;
        private List<Road> _route;
        private int _currentRouteIndex;
        private double _distanceTraveledOnCurrentRoad;

        public Guid Id
        {
            get => _id;
            set
            {
                if (_id != value)
                {
                    _id = value;
                    OnPropertyChanged();
                }
            }
        }

        public City Source
        {
            get => _source;
            set
            {
                if (_source != value)
                {
                    _source = value;
                    OnPropertyChanged();
                }
            }
        }

        public City Destination
        {
            get => _destination;
            set
            {
                if (_destination != value)
                {
                    _destination = value;
                    OnPropertyChanged();
                }
            }
        }

        public Point CurrentPosition
        {
            get => _currentPosition;
            set
            {
                if (_currentPosition != value)
                {
                    _currentPosition = value;
                    OnPropertyChanged();
                }
            }
        }

        public double Speed
        {
            get => _speed;
            set
            {
                if (_speed != value)
                {
                    _speed = value;
                    OnPropertyChanged();
                }
            }
        }

        public List<Road> Route
        {
            get => _route;
            set
            {
                if (_route != value)
                {
                    _route = value;
                    OnPropertyChanged();
                }
            }
        }

        public int CurrentRouteIndex
        {
            get => _currentRouteIndex;
            set
            {
                if (_currentRouteIndex != value)
                {
                    _currentRouteIndex = value;
                    OnPropertyChanged();
                }
            }
        }

        public double DistanceTraveledOnCurrentRoad
        {
            get => _distanceTraveledOnCurrentRoad;
            set
            {
                if (_distanceTraveledOnCurrentRoad != value)
                {
                    _distanceTraveledOnCurrentRoad = value;
                    OnPropertyChanged();
                }
            }
        }

        public Vehicle()
        {
            Id = Guid.NewGuid();
            Route = new List<Road>();
            CurrentRouteIndex = 0;
            DistanceTraveledOnCurrentRoad = 0;
            Speed = 1.0; // Velocidad por defecto
        }

        public Vehicle(City source, City destination) : this()
        {
            Source = source;
            Destination = destination;
            CurrentPosition = source.Position; // Inicia en la posición de origen
        }

        // Marca un vehículo como que ha llegado a su destino
        public bool IsAtDestination()
        {
            if (Route == null || Route.Count == 0)
                return false;

            return CurrentRouteIndex >= Route.Count;
        }

        // Avanza el vehículo a lo largo de la ruta actual
        public void AdvanceAlongRoad(double distance)
        {
            if (IsAtDestination() || Route == null || Route.Count == 0)
                return;

            Road currentRoad = Route[CurrentRouteIndex];
            DistanceTraveledOnCurrentRoad += distance;

            // Si hemos completado esta carretera, pasamos a la siguiente
            if (DistanceTraveledOnCurrentRoad >= currentRoad.Distance)
            {
                CurrentRouteIndex++;

                if (!IsAtDestination())
                {
                    // Iniciar la siguiente carretera
                    DistanceTraveledOnCurrentRoad = 0;
                    CurrentPosition = Route[CurrentRouteIndex].SourceCity.Position;
                }
                else
                {
                    // Llegamos al destino
                    CurrentPosition = Destination.Position;
                }

                return;
            }

            // Calcular la posición actual en la carretera usando interpolación lineal
            double ratio = DistanceTraveledOnCurrentRoad / currentRoad.Distance;
            double newX = currentRoad.SourceCity.Position.X +
                         (currentRoad.DestinationCity.Position.X - currentRoad.SourceCity.Position.X) * ratio;
            double newY = currentRoad.SourceCity.Position.Y +
                         (currentRoad.DestinationCity.Position.Y - currentRoad.SourceCity.Position.Y) * ratio;

            CurrentPosition = new Point((int)newX, (int)newY);
        }

        public bool HasCompletedCurrentSegment()
        {
            if (IsAtDestination() || Route == null || Route.Count == 0)
                return false;

            return DistanceTraveledOnCurrentRoad >= Route[CurrentRouteIndex].Distance;
        }

        // Este método será implementado cuando tengamos la clase TrafficGraph
        public void CalculateShortestPath()
        {
            // La implementación real dependerá del algoritmo Dijkstra que implementaremos más adelante
            // Por ahora, solo dejamos un stub
        }

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}