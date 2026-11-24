using System.Drawing;
using System.Drawing.Drawing2D;
using BuildingPatternsLib;

namespace BuildingVisualApp
{
    public static class BuildingRenderer
    {
        public static void DrawSingleBuilding(Graphics g, MultiStoryBuilding? b, Rectangle bounds, bool isSelected, bool showLabels)
        {
            if (b == null) return;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            int groundY = bounds.Bottom - 36;
            int centerX = bounds.Left + bounds.Width / 2;

            // Масштабування висоти залежно від кількості поверхів
            int visualFloors = b.FloorsCount <= 9 ? 6 : (b.FloorsCount <= 16 ? 9 : 12);
            int maxAvailableH = Math.Max(80, bounds.Height - 95);
            int floorH = Math.Clamp(maxAvailableH / visualFloors, 10, 22);
            int totalH = visualFloors * floorH;
            int bWidth = Math.Clamp(bounds.Width - 46, 56, 150);
            int bLeft = centerX - bWidth / 2;
            int bTop = groundY - totalH;

            // Крок 1: Фундамент (під рівнем землі)
            if (b.HasFoundation)
            {
                int foundH = b.BuildingClass == "Преміум-клас" ? 22 : (b.BuildingClass == "Комфорт-клас" ? 16 : 12);
                using var foundBrush = new SolidBrush(Color.FromArgb(110, 118, 128));
                using var hatchBrush = new HatchBrush(HatchStyle.DiagonalCross, Color.FromArgb(80, 86, 94), Color.FromArgb(130, 138, 148));
                var foundRect = new Rectangle(bLeft - 8, groundY, bWidth + 16, foundH);
                g.FillRectangle(hatchBrush, foundRect);
                g.DrawRectangle(Pens.DimGray, foundRect);

                // Палі для преміум-класу
                if (b.BuildingClass == "Преміум-клас")
                {
                    using var pilePen = new Pen(Color.FromArgb(75, 82, 90), 4);
                    for (int px = bLeft; px <= bLeft + bWidth; px += bWidth / 4)
                    {
                        g.DrawLine(pilePen, px, groundY + foundH, px, groundY + foundH + 10);
                    }
                }
            }

            // Крок 2: Каркас, поверхи та вікна
            if (b.HasStructure)
            {
                Color wallColor = ColorTranslator.FromHtml(b.WallColorHex);
                using var wallBrush = new SolidBrush(wallColor);
                using var wallPen = new Pen(Color.FromArgb(55, 65, 75), 1.5f);
                var bodyRect = new Rectangle(bLeft, bTop, bWidth, totalH);
                g.FillRectangle(wallBrush, bodyRect);
                g.DrawRectangle(wallPen, bodyRect);

                // Сітка вікон
                int cols = bWidth >= 110 ? 5 : 3;
                int marginX = 8;
                int winSpacing = (bWidth - marginX * 2) / cols;
                int winW = Math.Max(6, winSpacing - 6);
                int winH = Math.Max(5, floorH - 6);

                // Якщо крок 5 (комунікації/освітлення) виконано — частина вікон світиться теплим жовтим
                Color litWindow = Color.FromArgb(255, 232, 120);
                Color darkWindow = Color.FromArgb(175, 215, 238);

                for (int r = 0; r < visualFloors; r++)
                {
                    int wy = bTop + r * floorH + 3;
                    for (int c = 0; c < cols; c++)
                    {
                        // Залишаємо місце для ліфтової шахти по центру, якщо ліфти встановлено
                        if (b.HasElevators && c == cols / 2) continue;

                        int wx = bLeft + marginX + c * winSpacing + 2;
                        bool isLit = b.HasUtilities && ((r + c + b.Id) % 2 == 0);
                        using var wb = new SolidBrush(isLit ? litWindow : darkWindow);
                        g.FillRectangle(wb, wx, wy, winW, winH);
                        g.DrawRectangle(Pens.SlateGray, wx, wy, winW, winH);
                    }
                }
            }

            // Крок 3: Покрівля
            if (b.HasRoof && b.HasStructure)
            {
                if (b.RoofKind == RoofStyle.FlatPanel)
                {
                    using var roofBrush = new SolidBrush(Color.FromArgb(95, 102, 110));
                    g.FillRectangle(roofBrush, bLeft - 3, bTop - 8, bWidth + 6, 8);
                    g.DrawRectangle(Pens.Black, bLeft - 3, bTop - 8, bWidth + 6, 8);
                }
                else if (b.RoofKind == RoofStyle.MembraneModern)
                {
                    using var roofBrush = new SolidBrush(Color.FromArgb(150, 72, 56));
                    Point[] pts = {
                        new Point(bLeft - 4, bTop),
                        new Point(bLeft + 10, bTop - 13),
                        new Point(bLeft + bWidth - 10, bTop - 13),
                        new Point(bLeft + bWidth + 4, bTop)
                    };
                    g.FillPolygon(roofBrush, pts);
                    g.DrawPolygon(Pens.Black, pts);
                }
                else // GreenTerrace
                {
                    using var deckBrush = new SolidBrush(Color.FromArgb(70, 88, 105));
                    g.FillRectangle(deckBrush, bLeft - 2, bTop - 7, bWidth + 4, 7);
                    using var grassBrush = new SolidBrush(Color.FromArgb(76, 166, 92));
                    g.FillRectangle(grassBrush, bLeft + 2, bTop - 11, bWidth - 4, 5);
                    // Деревця на терасі
                    using var treeBrush = new SolidBrush(Color.FromArgb(46, 133, 64));
                    g.FillEllipse(treeBrush, bLeft + 8, bTop - 21, 12, 12);
                    g.FillEllipse(treeBrush, bLeft + bWidth - 20, bTop - 21, 12, 12);
                }
            }

            // Крок 4: Скляна ліфтова шахта та кабіна
            if (b.HasElevators && b.HasStructure)
            {
                int shaftW = Math.Max(14, bWidth / 6);
                int shaftX = centerX - shaftW / 2;
                using var shaftBrush = new SolidBrush(Color.FromArgb(200, 228, 246, 255));
                using var shaftPen = new Pen(Color.SteelBlue, 1.5f);
                g.FillRectangle(shaftBrush, shaftX, bTop + 2, shaftW, totalH - 4);
                g.DrawRectangle(shaftPen, shaftX, bTop + 2, shaftW, totalH - 4);

                // Кабіна ліфта
                int cabinY = bTop + totalH / 3;
                using var cabinBrush = new SolidBrush(Color.FromArgb(235, 165, 52));
                g.FillRectangle(cabinBrush, shaftX + 2, cabinY, shaftW - 4, Math.Max(10, floorH - 2));
                g.DrawRectangle(Pens.DarkGoldenrod, shaftX + 2, cabinY, shaftW - 4, Math.Max(10, floorH - 2));
            }

            // Крок 5: Комунікації (антена / дахова котельня / індикатор Smart Home)
            if (b.HasUtilities && b.HasStructure)
            {
                int roofTopY = bTop - (b.RoofKind == RoofStyle.GreenTerrace ? 12 : 10);
                using var utilPen = new Pen(Color.DarkSlateGray, 2f);
                g.DrawLine(utilPen, centerX, roofTopY, centerX, roofTopY - 14);
                using var signalBrush = new SolidBrush(Color.Crimson);
                g.FillEllipse(signalBrush, centerX - 3, roofTopY - 18, 6, 6);
            }

            // Крок 6: Паркінг (підземний або наземний біля будинку)
            if (b.HasParking)
            {
                if (b.BuildingClass == "Преміум-клас")
                {
                    // Підземний паркінг із позначкою P
                    using var parkBrush = new SolidBrush(Color.FromArgb(45, 55, 68));
                    var pRect = new Rectangle(bLeft - 4, groundY + 3, bWidth + 8, 16);
                    g.FillRectangle(parkBrush, pRect);
                    g.DrawRectangle(Pens.LightSteelBlue, pRect);
                    using var pFont = new Font("Segoe UI", 7.5f, FontStyle.Bold);
                    g.DrawString("P  [EV]", pFont, Brushes.LightSkyBlue, pRect.Left + 6, pRect.Top + 1);
                }
                else
                {
                    // Наземна стоянка праворуч/поруч
                    int pW = Math.Min(34, bounds.Right - (bLeft + bWidth) - 4);
                    if (pW >= 16)
                    {
                        using var asphBrush = new SolidBrush(Color.FromArgb(88, 92, 98));
                        g.FillRectangle(asphBrush, bLeft + bWidth + 3, groundY - 6, pW, 6);
                        // Схематичне авто
                        using var carBrush = new SolidBrush(Color.FromArgb(220, 82, 74));
                        g.FillRectangle(carBrush, bLeft + bWidth + 6, groundY - 13, Math.Min(18, pW - 4), 7);
                    }
                    else
                    {
                        using var pFont = new Font("Segoe UI", 7.5f, FontStyle.Bold);
                        g.DrawString("P", pFont, Brushes.DarkBlue, bLeft + 4, groundY + 2);
                    }
                }
            }

            // Виділення обраного будинку у списку прототипів
            if (isSelected)
            {
                using var selPen = new Pen(Color.DodgerBlue, 2.5f) { DashStyle = DashStyle.Dash };
                g.DrawRectangle(selPen, bounds.Left + 3, bounds.Top + 3, bounds.Width - 6, bounds.Height - 6);
            }

            // Підпис під будинком
            if (showLabels)
            {
                using var lblFont = new Font("Segoe UI", 8.0f, FontStyle.Bold);
                string title = b.IsClone ? $"Копія #{b.Id} (з #{b.ClonedFromId})" : $"Оригінал #{b.Id}";
                var textSize = g.MeasureString(title, lblFont);
                g.DrawString(title, lblFont, b.IsClone ? Brushes.DarkMagenta : Brushes.DarkSlateBlue,
                    centerX - textSize.Width / 2, bounds.Bottom - 16);
            }
        }
    }
}
