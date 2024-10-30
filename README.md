# How to add multiple trackballs in a WPF SfChart.
This article provides a detailed walkthrough on how to add multiple trackballs in an [SfChart](https://help.syncfusion.com/wpf/charts/getting-started) in WPF, allowing you to hover over the trackball with your mouse and move them independently to view the information of different data points simultaneously.

Learn step-by-step instructions and gain insights to add multiple trackballs in a WPF SfChart.

**Step 1:** Initialize the SfChart with primary and secondary axes. For more detailed steps, refer to the WPF Charts [documentation](https://help.syncfusion.com/wpf/charts/getting-started).

XAML

```XML
<Grid>

    <chart:SfChart x:Name="chart">

        <chart:SfChart.PrimaryAxis>
            <chart:CategoryAxis/>
        </chart:SfChart.PrimaryAxis>

        <chart:SfChart.SecondaryAxis>
            <chart:NumericalAxis/>
        </chart:SfChart.SecondaryAxis>

        <chart:LineSeries ItemsSource="{Binding Data}" 
                          XBindingPath="Day" 
                          YBindingPath="CPULoad" >
         </chart:LineSeries>

    </chart:SfChart>
    
</Grid>
```

**Step 2:** Create a custom ChartTrackBallBehaviorExt class, which is inherited from [ChartTrackballBehavior](https://help.syncfusion.com/cr/wpf/Syncfusion.UI.Xaml.Charts.ChartTrackBallBehavior.html#).

C#

```csharp
public class ChartTrackBallBehaviorExt : ChartTrackBallBehavior
{

} 
```

**Step 3:** Create instances of ChartTrackBallBehaviorExt, and add them to the [Behaviors](https://help.syncfusion.com/cr/wpf/Syncfusion.UI.Xaml.Charts.SfChart.html#Syncfusion_UI_Xaml_Charts_SfChart_Behaviors) collection, assigning specific names to each.

XAML

```XML
<chart:SfChart.Behaviors>
    <local:ChartTrackBallBehaviorExt x:Name="trackball1"/>
    <local:ChartTrackBallBehaviorExt x:Name="trackball2"/>
</chart:SfChart.Behaviors> 
```

**Step 4:** Implement the **ChartTrackBallBehaviorExt** class and its functionalities. Include the **Display** method, which is responsible for displaying the trackball at specified coordinates by setting the IsActivated protected property of the ChartTrackBallBehavior class. Manage multiple trackballs by overriding **mouse event handlers** in ChartTrackBallBehavior, using the **FindNearestTrackball** method in **OnMouseEnter** to locate the closest trackball. The **isTrackballActive** variable ensures only the active trackball responds to the events.

C#

```csharp
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

    private ChartTrackBallBehavior FindNearestTrackball(Point touchPoint)
    {
        ChartTrackBallBehavior nearestTrackball = new ChartTrackBallBehaviorExt();
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
```

**Step 5:** Assign the chart instance to the **SfChart** property in the **ChartTrackBallBehaviorExt** class, and override the **OnContentRendered** method to run the asynchronous task that calls **ShowTrackball** method.

The **ShowTrackballMethod** method calculates the positions and displays the trackballs on the chart.

C#

```csharp
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
```

**Step 6:** To control the trackballs, simply hover over them with your mouse. As you move the mouse within the chart area, the trackball will follow the cursor, allowing you to inspect different data points interactively.

**Output:**

![trackball_demo](https://github.com/user-attachments/assets/2d319594-8d8c-49f1-be27-5d9573b45344)

## Troubleshooting
### Path too long exception
If you are facing a path too long exception when building this example project, close Visual Studio and rename the repository to short and build the project.

For more details, refer to the KB on [How to add multiple trackballs in a WPF SfChart?](https://support.syncfusion.com/kb/article/17741/how-to-add-multiple-trackballs-in-a-wpf-sfchart).
