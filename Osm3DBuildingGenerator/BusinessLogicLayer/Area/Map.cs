using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Osm3DBuildingGenerator.BusinessLogicLayer {

    /// <summary>
    /// This class defines the bounds of the map
    /// </summary>
    class Map {
        private float minLat;
        private float minLong;
        private float maxLat;
        private float maxLong;

        /// <summary>
        /// Non standard constructor
        /// </summary>
        /// <param name="minLat">Minimum latitude of the map</param>
        /// <param name="minLong">Minimum longitude of the map</param>
        /// <param name="maxLat">Maximum latitude of the map</param>
        /// <param name="maxLong">Maximum longitude of the map</param>
        public Map(float minLat, float minLong, float maxLat, float maxLong) {
            this.minLat = minLat;
            this.minLong = minLong;
            this.maxLat = maxLat;
            this.maxLong = maxLong;
        }

        public float MinLat {
            get {
                return minLat;
            }
        }

        public float MinLong {
            get {
                return minLong;
            }
        }

        public float MaxLat {
            get {
                return maxLat;
            }
        }

        public float MaxLong {
            get {
                return maxLong;
            }
        }
    }
}