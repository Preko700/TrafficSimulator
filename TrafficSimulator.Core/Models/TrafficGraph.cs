using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace TrafficSimulator.Core.Models
{
    public class TrafficGraph : INotifyPropertyChanged
    {
        private List<City> _cities;
        private List<Road> _roads;

        public List<City> Cities
        {
            get => _cities;
            set
            {
                if (_cities != value)
                {
                    _cities = value;
                    OnPropertyChanged();
                }
            }
        }

        public List<Road> Roads
        {
            get => _roads;
            set
            {
                if (_roads != value)
                {
                    _roads = value;
                    OnPropertyChanged();
                }
            }
        }

        public TrafficGraph()
        {
            Cities = new List<City>();
            Roads = new List<Road>();
        }

        public void AddCity(City city)
        {
            if (city == null)
                throw new ArgumentNullException(nameof(city));

            if (!Cities.Any(c => c.Id == city.Id))
            {
                Cities.Add(city);
                OnPropertyChanged(nameof(Cities));
            }
        }

        public void RemoveCity(City city)
        {
            if (city == null)
                throw new ArgumentNullException(nameof(city));

            // Primero eliminamos todas las carreteras conectadas a esta ciudad
            var roadsToRemove = Roads.Where(r =>
                r.SourceCity.Id == city.Id || r.DestinationCity.Id == city.Id).ToList();

            foreach (var road in roadsToRemove)
            {
                Roads.Remove(road);
            }

            if (Roads.Count != roadsToRemove.Count)
            {
                OnPropertyChanged(nameof(Roads));
            }

            // Luego eliminamos la ciudad
            if (Cities.Remove(city))
            {
                OnPropertyChanged(nameof(Cities));
            }
        }

        public void AddRoad(Road road)
        {
            if (road == null)
                throw new ArgumentNullException(nameof(road));

            // Verificar que ambas ciudades existen en el grafo
            if (!Cities.Any(c => c.Id == road.SourceCity.Id) ||
                !Cities.Any(c => c.Id == road.DestinationCity.Id))
            {
                throw new ArgumentException("Ambas ciudades deben existir en el grafo.");
            }

            // Verificar que no existe ya una carretera igual
            if (!Roads.Any(r =>
                (r.SourceCity.Id == road.SourceCity.Id && r.DestinationCity.Id == road.DestinationCity.Id) ||
                (r.SourceCity.Id == road.DestinationCity.Id && r.DestinationCity.Id == road.SourceCity.Id)))
            {
                Roads.Add(road);
                OnPropertyChanged(nameof(Roads));
            }
        }

        public void RemoveRoad(Road road)
        {
            if (road == null)
                throw new ArgumentNullException(nameof(road));

            if (Roads.Remove(road))
            {
                OnPropertyChanged(nameof(Roads));
            }
        }

        // Método inicial para encontrar la ruta más corta
        // La implementación completa se hará cuando agreguemos el algoritmo Dijkstra
        public List<Road> GetShortestPath(City source, City destination)
        {
            // Por ahora, sólo devolvemos una lista vacía
            return new List<Road>();
        }

        // Actualizamos la carga de tráfico en todas las carreteras
        public void UpdateTrafficLoad()
        {
            // La implementación real se hará más adelante
        }

        // Encontrar puntos críticos en el grafo
        public List<CriticalPoint> FindCriticalPoints()
        {
            // La implementación real se hará más adelante
            return new List<CriticalPoint>();
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