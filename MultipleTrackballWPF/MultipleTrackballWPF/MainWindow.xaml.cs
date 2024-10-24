using Syncfusion.UI.Xaml.Charts;
using System.Reflection;
using System.Windows;
using System.Windows.Input;

namespace MultipleTrackballWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        internal ChartTrackBallBehaviorExt chartTrackBallBehavior;

        public MainWindow()
        {
            InitializeComponent();
            trackball1.sfChart = this.chart;
            trackball2.sfChart = this.chart;
        }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);

            Task.Run(async () =>
            {
                await ShowTrackball();
            });
        }

        async Task ShowTrackball()
        {
            await Task.Delay(1000);
            Application.Current.Dispatcher.Invoke(() =>
            {
                // Calculated positions for the first trackball
                float xPosition = (float)chart.ValueToPoint(chart.PrimaryAxis, 1);
                float yPosition = (float)chart.ValueToPoint(chart.SecondaryAxis, 169);

                // Calculated positions for the second trackball
                float xPosition1 = (float)chart.ValueToPoint(chart.PrimaryAxis, 6);
                float yPosition1 = (float)chart.ValueToPoint(chart.SecondaryAxis, 170);

                // Show the first trackball
                trackball1.Show(xPosition, yPosition);

                // Show the second trackball
                trackball2.Show(xPosition1, yPosition1);
            });
        }
    }

    public class Person
    {
        public string Name { get; set; }
        public double Height { get; set; }
        public double Weight { get; set; }
    }

    public class ViewModel
    {
        public List<Person> Data { get; set; }

        public ViewModel()
        {
            Data = new List<Person>
            {
                new Person { Name = "James", Height = 175 },
                new Person { Name = "John", Height = 169 },
                new Person { Name = "Paul", Height = 178 },
                new Person { Name = "Peter", Height = 165 },
                new Person { Name = "Chris", Height = 180 },
                new Person { Name = "Tom", Height = 160 },
                new Person { Name = "Harry", Height = 170 },
                new Person { Name = "George", Height = 176 }
            };
        }
    }

    public class ChartTrackBallBehaviorExt : ChartTrackBallBehavior
    {
        bool isActivated = false;

        public SfChart sfChart { get; set; }

        public double X { get; set; }
        public double Y { get; set; }

        public ChartTrackBallBehaviorExt()
        {

        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            var touchPoint = e.GetPosition(null);
            var trackball = FindNearestTrackball(touchPoint);  

            if (trackball == this)
            {
                isActivated = true;
                base.OnMouseEnter(e);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (isActivated)
            {
                var touchPoint = e.GetPosition(null);
                Show((float)touchPoint.X, (float)touchPoint.Y);
                base.OnMouseMove(e);
            }
        }

        private ChartTrackBallBehavior FindNearestTrackball(Point touchPoint)
        {
            ChartTrackBallBehavior nearestTrackball = null;
            double minDistance = double.MaxValue;

            foreach (var trackballBehavior in sfChart.Behaviors)
            {
                if (trackballBehavior is ChartTrackBallBehaviorExt trackballExt)
                {
                    var distance = Math.Sqrt(Math.Pow(trackballExt.X - touchPoint.X, 2) + Math.Pow(trackballExt.Y - touchPoint.Y, 2));

                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        nearestTrackball = trackballExt;
                    }
                }
            }
            return nearestTrackball;
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            isActivated = false;
        }

        public void Show(float x, float y)
        {
            X = x; Y = y;
            IsActivated = true;
            var point = new Point(X, Y);

            SetInternalProperty(typeof(ChartTrackBallBehavior), this, point, "CurrentPoint");
            base.OnPointerPositionChanged();
            InvokeInternalMethod(typeof(ChartTrackBallBehavior), this, "Activate", IsActivated);
        }

        public void Hide()
        {
            IsActivated = false;
        }

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

        internal static object? InvokeInternalMethod(Type type, object obj, string methodName, params object[] args)
        {
            var method = type.GetTypeInfo().GetDeclaredMethod(methodName);
            return method?.Invoke(obj, args);
        }
    }
}