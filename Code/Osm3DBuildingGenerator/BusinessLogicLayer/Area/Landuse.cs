using Osm3DBuildingGenerator.BusinessLogicLayer.PolygonTriangulation;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Osm3DBuildingGenerator.BusinessLogicLayer {

    /// <summary>
    /// This class defines an area specified as a landuse
    /// </summary>
    class Landuse : Area.Area {
        private List<long> nodes;
        private int height;
        private string kind;
        private double zoom;

        /// <summary>
        /// Non standard constructor
        /// </summary>
        /// <param name="nodes">the nodes (points) that specifies the area of this landuse</param>
        /// <param name="height">the height of this landuse (wich always will be defined as zero)</param>
        /// <param name="kind">the kind of landuse</param>
        /// <param name="zoom">the zoom facor of this landuse</param>
        public Landuse(List<long> nodes, int height, string kind, double zoom) : base(nodes, height, kind, zoom){
            this.nodes = nodes;
            this.height = height;
            this.kind = kind;
            this.zoom = zoom;
        }

        /// <summary>
        /// This method returns a 3D representation of this landuse
        /// </summary>
        /// <param name="nodesDict">List of all the nodes on the map</param>
        /// <param name="map">bounds of the map</param>
        /// <param name="brush">Color of this landuse</param>
        /// <returns>ModelUIElement3D of this landuse</returns>
        public override ModelUIElement3D get3DSurface(Dictionary<long, OsmSharp.Osm.Node> nodesDict, Map map, System.Windows.Media.SolidColorBrush brush) {
            return base.get3DSurface(nodesDict, map, brush);
        }

        /// <summary>
        /// Returns the scaled points of this landuse (scaled by Web Mercator Projection: http://en.wikipedia.org/wiki/Web_Mercator)
        /// </summary>
        /// <param name="nodesDict">List of all the nodes on the map</param>
        /// <param name="map">bounds of the map</param>
        /// <returns>List of scaled points</returns>
        protected override List<PointF> getScaledPointsSurface(Dictionary<long, OsmSharp.Osm.Node> nodesDict, Map map) {
            return base.getScaledPointsSurface(nodesDict, map);
        }

        public string Kind {
            get {
                return kind;
            }
        }
    }
}
