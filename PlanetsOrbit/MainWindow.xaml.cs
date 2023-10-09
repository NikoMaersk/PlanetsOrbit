using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Threading;

namespace PlanetsOrbit
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private List<Ellipse> planets = new List<Ellipse>();
		private List<Polyline> planetTrails = new List<Polyline>();
		private double sunRadius = 50;

		private double[] planetRadii = { 2, 4.9, 5.2, 2.6, 11.2, 9.4, 8.8, 3.8 };
		private double[] planetDistances = { 75, 110, 150, 200, 250, 300, 350, 400 };
		private double[] planetAngularSpeeds = { 0.017, 0.004, 0.003, 0.0016, 0.000533, 0.000262, 0.000131, 0.000087 };
		private double[] planetAngles;

		private DispatcherTimer timer = new DispatcherTimer();
		//private Timer timerThread;

		public MainWindow()
		{
			InitializeComponent();
			InitializeSolarSystem();
			InitializeStars();

			// Dispatcher timer
			timer.Interval = TimeSpan.FromMilliseconds(5);
			timer.Tick += UpdateSolarSystem;
			timer.Start();

			// Threading.Timer
			//timerThread = new Timer(UpdateSolarSystemInvokeDispatcher, null, 0, 5);
		}

		private void InitializeStars()
		{
			Random rnd = new Random();

			StarsCanvas.Children.Clear();

			for (int i = 0; i < 200; i++)
			{
				int starSize = rnd.Next(1, 4);
				Ellipse star = new Ellipse
				{
					Width = starSize,
					Height = starSize,
					Fill = Brushes.White
				};

				double starX = rnd.Next(0, (int)StarsCanvas.ActualWidth);
				double starY = rnd.Next(0, (int)StarsCanvas.ActualHeight);
				Canvas.SetLeft(star, starX);
				Canvas.SetTop(star, starY);

				StarsCanvas.Children.Add(star);
			}
		}

		private void StarsCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			InitializeStars();
		}

		private void InitializeSolarSystem()
		{
			planetAngles = new double[planetRadii.Length];
			for (int i = 0; i < planetRadii.Length; i++)
			{
				Ellipse planet = CreatePlanet(planetRadii[i], GetPlanetColor(i));
				Canvas.SetLeft(planet, (SolarSystemCanvas.ActualWidth - planetRadii[i] * 2) / 2);
				Canvas.SetTop(planet, (SolarSystemCanvas.ActualHeight - planetRadii[i] * 2) / 2);
				SolarSystemCanvas.Children.Add(planet);
				planets.Add(planet);

				Polyline trail = new Polyline
				{
					Stroke = new SolidColorBrush(GetPlanetColor(i)),
					StrokeThickness = 1
				};
				SolarSystemCanvas.Children.Add(trail);
				planetTrails.Add(trail);
			}
		}

		private void SolarSystemCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			double centerX = SolarSystemCanvas.ActualWidth / 2;
			double centerY = SolarSystemCanvas.ActualHeight / 2;
			Canvas.SetLeft(Sun, centerX - sunRadius);
			Canvas.SetTop(Sun, centerY - sunRadius);
		}


		private Color GetPlanetColor(int index)
		{
			Color[] planetColors = { Colors.Gray, Colors.Orange, Colors.Blue, Colors.Red, Colors.OrangeRed, Colors.Tan, Colors.SaddleBrown, Colors.DeepSkyBlue };
			return planetColors[index];
		}

		private Ellipse CreatePlanet(double radius, Color color)
		{
			Ellipse planet = new Ellipse
			{
				Width = radius * 2,
				Height = radius * 2,
				Fill = new SolidColorBrush(color)
			};
			return planet;
		}

		private void UpdateSolarSystem(object sender, EventArgs e)
		{
			double centerX = SolarSystemCanvas.ActualWidth / 2;
			double centerY = SolarSystemCanvas.ActualHeight / 2;

			for (int i = 0; i < planetRadii.Length; i++)
			{
				double planetX = centerX + planetDistances[i] * Math.Cos(planetAngles[i]);
				double planetY = centerY + planetDistances[i] * Math.Sin(planetAngles[i]);

				Canvas.SetLeft(planets[i], planetX - planetRadii[i]);
				Canvas.SetTop(planets[i], planetY - planetRadii[i]);

				planetTrails[i].Points.Add(new Point(planetX, planetY));

				if (planetTrails[i].Points.Count > 300)
				{
					planetTrails[i].Points.RemoveAt(0);
				}

				planetAngles[i] += planetAngularSpeeds[i];

				if (planetAngles[i] >= 360)
				{
					planetAngles[i] = 0;
				}
			}
		}

		private void UpdateSolarSystemInvokeDispatcher(object state)
		{
			double centerX = SolarSystemCanvas.ActualWidth / 2;
			double centerY = SolarSystemCanvas.ActualHeight / 2;

			Application.Current.Dispatcher.Invoke(new Action(() =>
			{
				for (int i = 0; i < planetRadii.Length; i++)
				{
					double planetX = centerX + planetDistances[i] * Math.Cos(planetAngles[i]);
					double planetY = centerY + planetDistances[i] * Math.Sin(planetAngles[i]);

					Canvas.SetLeft(planets[i], planetX - planetRadii[i]);
					Canvas.SetTop(planets[i], planetY - planetRadii[i]);

					planetTrails[i].Points.Add(new Point(planetX, planetY));

					if (planetTrails[i].Points.Count > 300)
					{
						planetTrails[i].Points.RemoveAt(0);
					}

					planetAngles[i] += planetAngularSpeeds[i];

					if (planetAngles[i] >= 360)
					{
						planetAngles[i] = 0;
					}
				}
			}));
		}
	}
}
