using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Osm3DBuildingGenerator.BusinessLogicLayer.Texture
{

    /// <summary>
    /// Klasse die de logica van het diamond-square algoritme bevat
    /// </summary>
    class DiamondSquare
    {
        private double roughness;
        private double displacementFactor;
        private int side;
        int[,] colors;

        /// <summary>
        /// Niet-standaard constructor van deze klasse
        /// </summary>
        /// <param name="roughness">De ruwheid</param>
        /// <param name="displacementFactor">De mate van verplaatsing van een punt</param>
        /// <param name="side">de lengte van een zijde</param>
        public DiamondSquare(double roughness, double displacementFactor, int side)
        {
            this.roughness = roughness;
            this.displacementFactor = displacementFactor;
            this.side = side;
        }

        /// <summary>
        /// Methode die opgeropen moet worden als men de bitmap na het algoritme wil krijgen
        /// </summary>
        /// <param name="color">Het kleur waarin het resultaat getekend moet worden</param>
        /// <returns>Bitmap met het resultaat van het algoritme</returns>
        public Bitmap getPoints(string color)
        {
            Random rand = new Random();
            colors = new int[side + 1, side + 1];
            int depth = 0;
            displacementFactor *= roughness;

            colors[0, 0] = (int)(128);
            colors[side, 0] = (int)(128);
            colors[0, side] = (int)(128);
            colors[side, side] = (int)(128);

            while (Math.Pow(2, depth) != side)
            {
                int distance = (int)(side / Math.Pow(2, depth));

                for (int i = distance / 2; i <= side; i += distance)
                {
                    for (int j = distance / 2; j <= side; j += distance)
                    {
                        Square(j, i, distance / 2, rand.NextDouble() * displacementFactor * 2.0 - displacementFactor);
                    }
                }

                for (int i = 0; i <= side; i += distance / 2)
                {
                    for (int j = (i + distance / 2) % distance; j <= side; j += distance)
                    {
                        Diamond(j, i, distance / 2, rand.NextDouble() * displacementFactor * 2.0 - displacementFactor);
                    }
                }

                depth++;
            }

            return ToBitmap(color);
        }

        /// <summary>
        /// Methode die het square deel van het algoritme behandelt
        /// </summary>
        /// <param name="x">x-coördinaat</param>
        /// <param name="y">y-coördinaat</param>
        /// <param name="half">halve afstand tussen hoekpunten</param>
        /// <param name="offset">random getal</param>
        private void Square(int x, int y, int half, double offset)
        {
            double topLeft = colors[x - half, y - half];
            double topRight = colors[x + half, y - half];
            double bottomLeft = colors[x - half, y + half];
            double bottomRight = colors[x + half, y + half];

            double average = (topLeft + topRight + bottomLeft + bottomRight) / 4.0;

            colors[x, y] = (int)(average + offset);
        }

        /// <summary>
        /// Methode die het diamond deel van het algoritme behandelt
        /// </summary>
        /// <param name="x">x-coördinaat</param>
        /// <param name="y">y-coördinaat</param>
        /// <param name="half">halve afstand tussen hoekpunten</param>
        /// <param name="offset">random getal</param>
        private void Diamond(int x, int y, int half, double offset)
        {
            double top = 0, left = 0, right = 0, bottom = 0;
            double divide = 4.0;

            if (y - half >= 0) top = colors[x, y - half];
            else divide = 3.0;

            if (x - half >= 0) left = colors[x - half, y];
            else divide = 3.0;

            if (x + half <= side) right = colors[x + half, y];
            else divide = 3.0;

            if (y + half <= side) bottom = colors[x, y + half];
            else divide = 3.0;

            double average = (top + left + right + bottom) / divide;

            colors[x, y] = (int)(average + offset);
        }

        /// <summary>
        /// Methode die de punten omvormt naar een bitmap
        /// </summary>
        /// <param name="color">kleur waarin het resultaat van het algoritme moet getekend worden</param>
        /// <returns>resultaat van het algoritme in bitmap-formaat</returns>
        private Bitmap ToBitmap(string color)
        {
            Bitmap bitmap = new Bitmap(side, side);
            int max = 0;
            int min = 255;
            double factor = 1;

            // Het maximum en minimum van de array zoeken
            for (int i = 0; i < side; i++)
            {
                for (int j = 0; j < side; j++)
                {
                    if (colors[i, j] > max) max = colors[i, j];
                    if (colors[i, j] < min) min = colors[i, j];
                }
            }

            // Zorgen dat de parameters voor de transformatie juist staan
            if (min < 0)
            {
                min = Math.Abs(min);
                factor = (double)(max + min) / 255.0;
            }
            else
            {
                min = 0;
            }

            if (max > 255)
            {
                factor = (double)(max + min) / 255.0;
            }

            for (int i = 0; i < side; i++)
            {
                for (int j = 0; j < side; j++)
                {
                    if (color == "blue") bitmap.SetPixel(j, i, Color.FromArgb((int)((colors[i, j] + min) / factor), (int)((colors[i, j] + min) / factor), 255));
                    if (color == "green") bitmap.SetPixel(j, i, Color.FromArgb((int)((colors[i, j] + min) / factor), 255, (int)((colors[i, j] + min) / factor)));
                }
            }

            return bitmap;
        }
    }
}
