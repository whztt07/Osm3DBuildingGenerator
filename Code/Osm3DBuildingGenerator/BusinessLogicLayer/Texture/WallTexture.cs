using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Osm3DBuildingGenerator.BusinessLogicLayer.Texture
{

    /// <summary>
    /// Klasse die de logica voor het creëeren van een texture die op de muren van de gebouwen komt bevat
    /// </summary>
    class WallTexture
    {
        private int side;
        private int windowsSide;
        private int windowSize;
        private int marginSize;
        private int rest;
        private Bitmap bitmap;

        private const double MARGINWINDOW = 0.4;

        /// <summary>
        /// Niet-standaard constructor van deze klasse
        /// </summary>
        /// <param name="side">lengte van een zijde</param>
        /// <param name="windowsSide">aantal ramen die in die zijde moeten zitten</param>
        public WallTexture(int side, int windowsSide)
        {
            this.side = side;
            this.windowsSide = windowsSide;
            bitmap = new Bitmap(side, side);

            int windowAndMargin = (int)(Math.Floor((decimal)(side / windowsSide)));
            rest = side % windowsSide;

            if (windowAndMargin <= 1)
            {
                throw new ArgumentException("Too much windows for this side");
            }

            windowSize = (int)((double)windowAndMargin * (1.0 - MARGINWINDOW));
            marginSize = windowAndMargin - windowSize;
        }

        /// <summary>
        /// Methode die moet opgeropen worden als de texture moet getekend worden
        /// </summary>
        /// <returns>geeft de texture in bitmap-formaat terug</returns>
        public Bitmap DrawTexture()
        {
            FillRectangle(Color.Black, 0, 0, side);

            DrawWindow(rest / 2 + marginSize / 2, rest / 2 + marginSize / 2, windowsSide - 1);

            return bitmap;
        }

        /// <summary>
        /// Recursieve methode die zichzelf 3 maal oproept
        /// 1. het venster onder dit venster
        /// 2. het venster rechts van dit venster
        /// 3. het venster schuin onder/rechts van dit venster
        /// </summary>
        /// <param name="x">x coordinaat van de bovenkant van het venster</param>
        /// <param name="y">x coordinaat van de linker kant van het venster</param>
        /// <param name="depth">Het aantal keren deze methode nog moet itereren</param>
        private void DrawWindow(int x, int y, int depth)
        {
            if (depth >= 0)
            {
                FillRectangle(Color.LightBlue, x, y, windowSize);

                DrawWindow(x, y + windowSize + marginSize, depth - 1);
                DrawWindow(x + windowSize + marginSize, y, depth - 1);
                DrawWindow(x + windowSize + marginSize, y + windowSize + marginSize, depth - 1);
            }
        }

        /// <summary>
        /// Vul dit venster met een kleur
        /// </summary>
        /// <param name="color">kleur</param>
        /// <param name="x">x coordinaat van de bovenkant van het venster</param>
        /// <param name="y">x coordinaat van de linker kant van het venster</param>
        /// <param name="length">de lengte van de zijde van dit venster</param>
        private void FillRectangle(Color color, int x, int y, int length)
        {
            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    bitmap.SetPixel(x + i, y + j, color);
                }
            }
        }
    }
}
