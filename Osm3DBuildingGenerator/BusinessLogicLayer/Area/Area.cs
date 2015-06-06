using Osm3DBuildingGenerator.BusinessLogicLayer.PolygonTriangulation;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Osm3DBuildingGenerator.BusinessLogicLayer.Area {

    /// <summary>
    /// This abstract class (with virtual methods) defines an area and is the parent class of the Building - and Landuse class
    /// </summary>
    abstract class Area {
        private List<long> nodes;
        private int height;
        private string kind;
        private double zoom;

        /// <summary>
        /// Non standard constructor
        /// </summary>
        /// <param name="nodes">the nodes (points) that specifies this area</param>
        /// <param name="height">the height of this area</param>
        /// <param name="kind">the kind of area</param>
        /// <param name="zoom">the zoom facor of this area</param>
        public Area(List<long> nodes, int height, string kind, double zoom) {
            this.nodes = nodes;
            this.height = height;
            this.kind = kind;
            this.zoom = zoom;
        }

        /// <summary>
        /// This method returns a 3D representation of this area
        /// </summary>
        /// <param name="nodesDict">List of all the nodes on the map</param>
        /// <param name="map">bounds of the map</param>
        /// <param name="brush">Color of this area</param>
        /// <returns>ModelUIElement3D of this area</returns>
        public virtual ModelUIElement3D get3DSurface(Dictionary<long, OsmSharp.Osm.Node> nodesDict, Map map, System.Windows.Media.SolidColorBrush brush) {
            List<PointF> ptlist = getScaledPointsSurface(nodesDict, map);

            // Divide the polygons in triangles, this is code (and these two classes) are from: https://polygontriangulation.codeplex.com/
            PolygonData poly = new PolygonData(ptlist);
            List<PointF[]> triangles = Triangulation2D.Triangulate(poly);

            // Surrounding tags of the mesh
            ModelUIElement3D model = new ModelUIElement3D();
            GeometryModel3D geometryModel = new GeometryModel3D();

            // Mesh and his his properties
            MeshGeometry3D mesh = new MeshGeometry3D();
            DiffuseMaterial material = new DiffuseMaterial((System.Windows.Media.Brush)brush);
            Point3DCollection positions = new Point3DCollection();
            Int32Collection indices = new Int32Collection();

            // Add points and indices to their collection
            foreach (PointF[] points in triangles) {
                foreach (PointF point in points) {
                    positions.Add(new Point3D(point.X, point.Y, height));
                }

                int count = positions.Count;
                indices.Add(count - 3);
                indices.Add(count - 2);
                indices.Add(count - 1);
            }

            // Add these collections to the mesh
            mesh.Positions = positions;
            mesh.TriangleIndices = indices;

            // Set the color of front and back of the triangle
            geometryModel.Material = material;
            geometryModel.BackMaterial = material;

            // Add the mesh to the model
            geometryModel.Geometry = mesh;
            model.Model = geometryModel;

            return model;
        }

        /// <summary>
        /// Returns the scaled points of this area (scaled by Web Mercator Projection: http://en.wikipedia.org/wiki/Web_Mercator)
        /// </summary>
        /// <param name="nodesDict">List of all the nodes on the map</param>
        /// <param name="map">bounds of the map</param>
        /// <returns>List of scaled points</returns>
        protected virtual List<PointF> getScaledPointsSurface(Dictionary<long, OsmSharp.Osm.Node> nodesDict, Map map) {
            List<PointF> ptlist = new List<PointF>();

            foreach (long node in nodes) {
                float longitude = (float)nodesDict[node].Longitude;
                float latitude = (float)nodesDict[node].Latitude;

                // Scaled positions
                int x = (int)((128 / Math.PI) * Math.Pow(2, zoom) * (longitude + Math.PI));
                int y = (int)((128 / Math.PI) * Math.Pow(2, zoom) * (Math.PI - Math.Log(Math.Tan(Math.PI / 4 + latitude / 2))));

                // Necessary variables to do a translation
                int minX = (int)((128 / Math.PI) * Math.Pow(2, zoom) * (map.MinLong + Math.PI));
                int minY = (int)((128 / Math.PI) * Math.Pow(2, zoom) * (Math.PI - Math.Log(Math.Tan(Math.PI / 4 + map.MinLat / 2))));
                int maxX = (int)((128 / Math.PI) * Math.Pow(2, zoom) * (map.MaxLong + Math.PI));
                int maxY = (int)((128 / Math.PI) * Math.Pow(2, zoom) * (Math.PI - Math.Log(Math.Tan(Math.PI / 4 + map.MaxLat / 2))));

                // Translate these points so the center of the villaga/city becomes the point (0, 0, 0)
                x = x - (minX + ((maxX - minX) / 2));
                y = y - (minY + ((maxY - minY) / 2));

                ptlist.Add(new PointF(x, y));
            }

            return ptlist;
        }
    }
}
