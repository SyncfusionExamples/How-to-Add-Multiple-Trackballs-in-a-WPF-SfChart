# How to add multiple trackballs in a WPF SfChart.
This article provides a detailed walkthrough on how to add multiple trackballs in an [SfChart](https://help.syncfusion.com/wpf/charts/getting-started) in WPF, allowing you to hover over the trackball with your mouse and move them independently to view the information of different data points simultaneously.

Learn step-by-step instructions and gain insights to add multiple trackballs in a WPF SfChart.

**Step 1:** Create a custom ChartTrackBallBehaviorExt class, which is inherited from [ChartTrackballBehavior](https://help.syncfusion.com/cr/wpf/Syncfusion.UI.Xaml.Charts.ChartTrackBallBehavior.html#).

C#

 ```csharp
public class ChartTrackBallBehaviorExt : ChartTrackBallBehavior
{

} 
 ```

**Step 2:** In the constructor of your MainWindow class, initialize the trackballs by setting their SfChart property to the chart defined in your XAML. This ensures that the trackballs are associated with the correct chart instance and can be accessed in other classes.

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
}

public class ChartTrackBallBehaviorExt : ChartTrackBallBehavior
{
    . . .
    public SfChart SfChart { get; set; }
    . . .
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

**Step 4:** Override the **OnContentRendered** method to run the asynchronous task that calls ShowTrackball().

C#

 ```csharp
public partial class MainWindow : Window
{
    . . .
    protected override void OnContentRendered(EventArgs e)
    {
        base.OnContentRendered(e);
    
        // Run the ShowTrackball method asynchronously
        Task.Run(async () =>
        {
            await ShowTrackball();
        });
    }
    . . .
}
 ```

**Step 5:** Implement the **ShowTrackball** method to calculate positions and display the trackballs at load time by using the Display method.

C#
 
 ```csharp
public partial class MainWindow : Window
{
    . . .
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
    . . .
 }
 ```

**Step 6:** Implement the **Display** method, which is responsible for displaying the trackball at specified coordinates . It updates the trackball’s position, sets the IsActivated protected property of the ChartTrackBallBehavior class to true, and triggers the necessary internal methods to reflect these changes on the UI.

C#
 
 ```csharp
public class ChartTrackBallBehaviorExt : ChartTrackBallBehavior
{
    . . .
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
    . . .
}
 ```

**Step 7:** The **SetInternalProperty** method uses reflection to set an internal property of an object. This is useful for accessing and modifying properties that are not publicly accessible.

C#

 ```csharp
public class ChartTrackBallBehaviorExt : ChartTrackBallBehavior
{
    . . .
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
    . . .
}
 ```

**Step 8:** The **InvokeInternalMethod** method uses reflection to invoke an internal method of an object. This allows calling methods that are not publicly accessible.

C#

 ```csharp
public class ChartTrackBallBehaviorExt : ChartTrackBallBehavior
{
    . . .
    internal static object? InvokeInternalMethod(Type type, object obj, string methodName, params object[] args)
    {
        var method = type.GetTypeInfo().GetDeclaredMethod(methodName);
        return method?.Invoke(obj, args);
    } 
    . . .
}
 ```

**Step 9:** Interact with multiple trackballs by overriding the **Mouse Event handlers** of [ChartTrackBallBehavior](https://help.syncfusion.com/cr/wpf/Syncfusion.UI.Xaml.Charts.ChartTrackBallBehavior.html) class. The **FindNearestTrackball** method is called in **OnMouseEnter** method to find the nearest trackball to the mouse pointer. The **isTrackballActive** variable is used to track whether a specific trackball is currently active, ensuring that only the relevant trackball responds to mouse events.

C#

 ```csharp
public class ChartTrackBallBehaviorExt : ChartTrackBallBehavior
{
    private bool isTrackballActive = false;
    . . .
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
    . . .
}
 ```

**Step 10:** The **FindNearestTrackball** method identifies the trackball closest to the user’s touch point, determining which trackball should be activated or moved based on user interaction.

C#

 ```csharp
public class ChartTrackBallBehaviorExt : ChartTrackBallBehavior
{
    . . .
    private ChartTrackBallBehavior FindNearestTrackball(Point touchPoint)
    {
        ChartTrackBallBehavior nearestTrackball = null;
        double minDistance = double.MaxValue;
    
        // Iterate through all trackball behaviors to find the nearest one
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
    
        return nearestTrackball;
    }
    . . .
 }
 ```

**Step 11:** To control the trackballs, simply hover over them with your mouse. As you move the mouse within the chart area, the trackball will follow the cursor, allowing you to inspect different data points interactively.

**Output:**

![trackballDemo](https://github.com/user-attachments/assets/0a01c337-49a5-479b-b04f-1719eac9a37e)


## Troubleshooting
### Path too long exception
If you are facing a path too long exception when building this example project, close Visual Studio and rename the repository to short and build the project.

For more details, refer to the KB on [How to add multiple trackballs in a WPF SfChart?](https://support.syncfusion.com/agent/kb/17741/edit).



