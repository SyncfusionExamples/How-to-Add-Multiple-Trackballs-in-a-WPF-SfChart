using Syncfusion.UI.Xaml.Charts;
using System.Reflection;
using System.Windows;
using System.Windows.Input;

namespace SfChartMultipleTrackball
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            trackball1.SfChart = this.chart;
            trackball2.SfChart = this.chart;
        }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);

            // Run the ShowTrackball method asynchronously
            Task.Run(async () =>
            {
                await ShowTrackball();
            });
        }

        async Task ShowTrackball()
        {
            // Wait for 1 second before executing the rest of the method
            await Task.Delay(1000);
            Application.Current.Dispatcher.Invoke(() =>
            {
                // Calculated positions for the first trackball
                float xPosition = (float)chart.ValueToPoint(chart.PrimaryAxis, 1);
                float yPosition = (float)chart.ValueToPoint(chart.SecondaryAxis, 169);

                // Calculated positions for the second trackball
                float xPosition1 = (float)chart.ValueToPoint(chart.PrimaryAxis, 6);
                float yPosition1 = (float)chart.ValueToPoint(chart.SecondaryAxis, 170);

                // Display the first trackball
                trackball1.Display(xPosition, yPosition);

                // Display the second trackball
                trackball2.Display(xPosition1, yPosition1);
            });
        }
    }

    public class ChartTrackBallBehaviorExt : ChartTrackBallBehavior
    {
        private bool isTrackballActive = false;

        public SfChart? SfChart { get; set; }

        public double X { get; set; }
        public double Y { get; set; }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            // Get the position of the mouse pointer
            var touchPoint = e.GetPosition(null);

            // Find the nearest trackball to the mouse pointer
            var trackball = FindNearestTrackball(touchPoint);

            // Activate the trackball if it is the nearest one
            if (trackball == this)
            {
                isTrackballActive = true;
                base.OnMouseEnter(e);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            // Check if the trackball is activated
            if (isTrackballActive)
            {
                // Get the position of the mouse pointer
                var touchPoint = e.GetPosition(null);

                // Display the trackball at the current mouse position
                Display((float)touchPoint.X, (float)touchPoint.Y);
                base.OnMouseMove(e);
            }
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            // Deactivate the trackball
            isTrackballActive = false;
        }

        private ChartTrackBallBehavior? FindNearestTrackball(Point touchPoint)
        {
            ChartTrackBallBehavior? nearestTrackball = null;
            double minDistance = double.MaxValue;

            // Iterate through all trackball behaviors to find the nearest one
            if (SfChart != null)
            {
                foreach (var trackballBehaviour in SfChart.Behaviors)
                {
                    if (trackballBehaviour is ChartTrackBallBehaviorExt trackball)
                    {
                        // Calculate the distance between the trackball and the touch point 
                        double distance = Math.Sqrt(Math.Pow(trackball.X - touchPoint.X, 2) + Math.Pow(trackball.Y - touchPoint.Y, 2));

                        // Update the nearest trackball if the current one is closer
                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            nearestTrackball = trackball;
                        }
                    }
                }
            }

            return nearestTrackball;
        }

        public void Display(float x, float y)
        {
            X = x; Y = y;
            IsActivated = true;
            var point = new Point(x, y);

            // Set the internal property for the current point
            SetInternalProperty(typeof(ChartTrackBallBehavior), this, point, "CurrentPoint");

            // Trigger the pointer position changed event
            base.OnPointerPositionChanged();

            // Activate the trackball
            InvokeInternalMethod(typeof(ChartTrackBallBehavior), this, "Activate", IsActivated);
        }

        // Sets an internal property of an object using reflection.
        internal static void SetInternalProperty(Type type, object obj, object value, string propertyName)
        {
            var properties = type.GetRuntimeProperties();

            foreach (var item in properties)
            {
                if (item.Name == propertyName)
                {
                    item.SetValue(obj, value);
                    break;
                }
            }
        }

        // Invokes an internal method of an object using reflection.
        internal static object? InvokeInternalMethod(Type type, object obj, string methodName, params object[] args)
        {
            var method = type.GetTypeInfo().GetDeclaredMethod(methodName);
            return method?.Invoke(obj, args);
        }
    }
}