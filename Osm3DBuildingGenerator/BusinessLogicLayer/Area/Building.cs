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
    /// This class defines an area specified as a building
    /// </summary>
    class Building : Area.Area {
        private List<long> nodes;
        private int height;
        private string kind;
        private double zoom;

        /// <summary>
        /// Non standard constructor
        /// </summary>
        /// <param name="nodes">the nodes (points) that specifies the area of this building</param>
        /// <param name="height">the height of this building</param>
        /// <param name="kind">the kind of building</param>
        /// <param name="zoom">the zoom facor of this building</param>
        public Building(List<long> nodes, int height, string kind, double zoom) : base(nodes, height, kind, zoom) {
            this.nodes = nodes;
            this.height = height;
            this.kind = kind;
            this.zoom = zoom;
        }

        /// <summary>
        /// This method returns a 3D representation of this building surface (roof)
        /// </summary>
        /// <param name="nodesDict">List of all the nodes on the map</param>
        /// <param name="map">bounds of the map</param>
        /// <param name="brush">Color of this surface</param>
        /// <returns>ModelUIElement3D of this surface</returns>
        public override ModelUIElement3D get3DSurface(Dictionary<long, OsmSharp.Osm.Node> nodesDict, Map map, System.Windows.Media.SolidColorBrush brush) {
            return base.get3DSurface(nodesDict, map, brush);
        }

        /// <summary>
        /// This method returns a 3D representation of this building walls
        /// </summary>
        /// <param name="nodesDict">List of all the nodes on the map</param>
        /// <param name="map">bounds of the map</param>
        /// <param name="brush">Color of these walls</param>
        /// <returns>ModelUIElement3D of these walls</returns>
        public ModelUIElement3D get3DWalls(Dictionary<long, OsmSharp.Osm.Node> nodesDict, Map map, ImageBrush brush)
        {
            // Surrounding tags of the mesh
            ModelUIElement3D model = new ModelUIElement3D();
            GeometryModel3D geometryModel = new GeometryModel3D();

            // Mesh and his his properties
            MeshGeometry3D mesh = new MeshGeometry3D();
            DiffuseMaterial material = new DiffuseMaterial((System.Windows.Media.Brush)brush);
            Point3DCollection positions = new Point3DCollection();
            PointCollection texturePoints = new PointCollection();
            Int32Collection indices = new Int32Collection();

            // Add Points of surface and with add points of surface with height 0
            positions = getScaledPositionsWall(nodesDict, map);

            // Add indices to the collection
            for (int i = 0; i < positions.Count - 2; i += 2) {
                indices.Add(i);
                indices.Add(i + 2);
                indices.Add(i + 1);
                indices.Add(i + 3);
                indices.Add(i + 1);
                indices.Add(i + 2);

                // Get the width and height of a wall
                float widthWall = (float)Math.Sqrt(Math.Pow(positions[i].X - positions[i + 2].X, 2) + Math.Pow(positions[i].Y - positions[i + 2].Y, 2));
                int imageWidth = (int)(brush.ImageSource.Width * widthWall);
                int imageHeight = (int)(brush.ImageSource.Height * height);

                // Add texture coordinates
                texturePoints.Add(new System.Windows.Point(0, imageHeight));
                texturePoints.Add(new System.Windows.Point(0, 0));
                texturePoints.Add(new System.Windows.Point(imageWidth, imageHeight));
                texturePoints.Add(new System.Windows.Point(imageWidth, 0));
            }

            // Add these collections to the mesh
            mesh.Positions = positions;
            mesh.TriangleIndices = indices;
            mesh.TextureCoordinates = texturePoints;

            // Set the color of front and back of the triangle
            geometryModel.Material = material;
            geometryModel.BackMaterial = material;

            // Add the mesh to the model
            geometryModel.Geometry = mesh;
            model.Model = geometryModel;

            return model;
        }

        /// <summary>
        /// Returns the scaled points of this surface (scaled by Web Mercator Projection: http://en.wikipedia.org/wiki/Web_Mercator)
        /// </summary>
        /// <param name="nodesDict">List of all the nodes on the map</param>
        /// <param name="map">bounds of the map</param>
        /// <returns>List of scaled points</returns>
        protected override List<PointF> getScaledPointsSurface(Dictionary<long, OsmSharp.Osm.Node> nodesDict, Map map) {
            return base.getScaledPointsSurface(nodesDict, map);
        }

        /// <summary>
        /// Returns the scaled positions of this wall (scaled by Web Mercator Projection: http://en.wikipedia.org/wiki/Web_Mercator)
        /// </summary>
        /// <param name="nodesDict">List of all the nodes on the map</param>
        /// <param name="map">bounds of the map</param>
        /// <returns>List of scaled positions</returns>
        private Point3DCollection getScaledPositionsWall(Dictionary<long, OsmSharp.Osm.Node> nodesDict, Map map) {
            Point3DCollection positions = new Point3DCollection();

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

                // Add the point on the ground and the point on the roof
                positions.Add(new Point3D(x, y, 0));
                positions.Add(new Point3D(x, y, height));
            }

            return positions;
        }

        public string Kind {
            get {
                return kind;
            }
        }
    }
}
