using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Econova.ViewModels;
using Econova.Models;

namespace Econova.Views.Pages
{
    public partial class PaginaVerReservas : Page
    {
        private const int HoraInicio = 6;
        private const double AlturaHora = 56.0;

        public PaginaVerReservas()
        {
            InitializeComponent();
            var vm = new PaginaVerReservasViewModel();
            DataContext = vm;
            vm.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(PaginaVerReservasViewModel.ReservasDelDia))
                {
                    DibujarEventosCalendario();
                }
            };
            DibujarEventosCalendario();
        }

        private void DibujarEventosCalendario()
        {
            if (CanvasEventos == null) return;
            CanvasEventos.Children.Clear();

            var vm = DataContext as PaginaVerReservasViewModel;
            if (vm == null || vm.ReservasDelDia == null) return;

            var reservasDelDia = vm.ReservasDelDia;

            string[] fondos = { "#DBEAFE", "#DCFCE7", "#FEF3C7", "#FCE7F3", "#EDE9FE" };
            string[] bordes = { "#93C5FD", "#86EFAC", "#FCD34D", "#F9A8D4", "#C4B5FD" };
            string[] textos = { "#1D4ED8", "#15803D", "#92400E", "#9D174D", "#6D28D9" };

            for (int i = 0; i < reservasDelDia.Count; i++)
            {
                var r = reservasDelDia[i];
                int color = i % fondos.Length;

                double horaInicioPx = (r.FechaEntradaDt.Hour + r.FechaEntradaDt.Minute / 60.0 - HoraInicio) * AlturaHora;
                double duracionHoras = (r.FechaSalidaDt - r.FechaEntradaDt).TotalHours;
                double alturaEvento = Math.Max(duracionHoras * AlturaHora, 40);

                var borde = new Border
                {
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(fondos[color])),
                    BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(bordes[color])),
                    BorderThickness = new Thickness(3, 1, 1, 1),
                    CornerRadius = new CornerRadius(6),
                    Padding = new Thickness(8, 4, 8, 4),
                    Margin = new Thickness(4, 0, 8, 0),
                    Cursor = System.Windows.Input.Cursors.Hand,
                    Tag = r.Id,
                    Height = alturaEvento
                };

                var stack = new StackPanel();
                stack.Children.Add(new TextBlock
                {
                    Text = r.Sala,
                    FontFamily = new FontFamily("Segoe UI"),
                    FontSize = 12,
                    FontWeight = FontWeights.SemiBold,
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(textos[color])),
                    TextTrimming = TextTrimming.CharacterEllipsis
                });

                stack.Children.Add(new TextBlock
                {
                    Text = $"{r.HoraEntrada} – {r.HoraSalida}",
                    FontFamily = new FontFamily("Segoe UI"),
                    FontSize = 11,
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(textos[color])),
                    Opacity = 0.8
                });

                stack.Children.Add(new TextBlock
                {
                    Text = r.Cliente,
                    FontFamily = new FontFamily("Segoe UI"),
                    FontSize = 11,
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(textos[color])),
                    Opacity = 0.7,
                    TextTrimming = TextTrimming.CharacterEllipsis
                });

                borde.Child = stack;
                borde.MouseLeftButtonUp += (s, e) => MostrarDetalle(r);

                Canvas.SetTop(borde, Math.Max(horaInicioPx, 0));
                Canvas.SetLeft(borde, 0);

                borde.SetBinding(Border.WidthProperty, new System.Windows.Data.Binding("ActualWidth") { Source = CanvasEventos });

                CanvasEventos.Children.Add(borde);
            }
        }

        private void MostrarDetalle(Reserva r)
        {
            MessageBox.Show(
                $"Detalle de la reserva #{r.Numero}\n\n" +
                $"Sala: {r.Sala}\n" +
                $"Cliente: {r.Cliente}\n" +
                $"Cédula: {r.Cedula}\n" +
                $"Entrada: {r.FechaEntrada} {r.HoraEntrada}\n" +
                $"Salida:  {r.FechaSalida} {r.HoraSalida}",
                "Información de Reserva",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
    }
}
