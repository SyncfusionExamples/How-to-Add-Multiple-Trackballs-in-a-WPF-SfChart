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

**Step 2:** Create instances of ChartTrackBallBehaviorExt, and add them to the [Behaviors](https://help.syncfusion.com/cr/wpf/Syncfusion.UI.Xaml.Charts.SfChart.html#Syncfusion_UI_Xaml_Charts_SfChart_Behaviors) collection, assigning specific names to each.

XAML

 ```XAML
<chart:SfChart.Behaviors>
    <local:ChartTrackBallBehaviorExt x:Name="trackball1"/>
    <local:ChartTrackBallBehaviorExt x:Name="trackball2"/>
</chart:SfChart.Behaviors> 
 ```

**Step 3:** Override the **OnContentRendered** method to run the asynchronous task that calls ShowTrackball().

C#

 ```csharp
protected override void OnContentRendered(EventArgs e)
{
    base.OnContentRendered(e);

    Task.Run(async () =>
    {
        await ShowTrackball();
    });
} 
 ```

**Step 4:** Implement the **ShowTrackball** method to calculate positions and display the trackballs at load time by using the Show method.

C#

 ```csharp
async Task ShowTrackball()
{
    await Task.Delay(1000);
    Application.Current.Dispatcher.Invoke(() =>
    {
        float xPosition = (float)chart.ValueToPoint(chart.PrimaryAxis, 1);
        float yPosition = (float)chart.ValueToPoint(chart.SecondaryAxis, 169);
        
        float xPosition1 = (float)chart.ValueToPoint(chart.PrimaryAxis, 6);
        float yPosition1 = (float)chart.ValueToPoint(chart.SecondaryAxis, 170);

        // Show the first trackball
        trackball1.Show(xPosition, yPosition);

        // Show the second trackball
        trackball2.Show(xPosition1, yPosition1);
    });
} 
 ```

**Step 5:** Interact with multiple trackballs by overriding the **Mouse Event handlers** of [ChartTrackBallBehavior](https://help.syncfusion.com/cr/wpf/Syncfusion.UI.Xaml.Charts.ChartTrackBallBehavior.html) class. The **isActivated** variable is used to track whether a specific trackball is currently active, ensuring that only the relevant trackball responds to mouse events.

C#
 
 ```csharp
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

protected override void OnMouseLeave(MouseEventArgs e)
{
    isActivated = false;
} 
 ```

**Step 6:** The **FindNearestTrackball** method identifies the trackball closest to the user’s touch point, determining which trackball should be activated or moved based on user interaction.

C#
 
 ```csharp
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
 ```

**Step 7:** To control the trackballs, simply hover over them with your mouse. As you move the mouse within the chart area, the trackball will follow the cursor, allowing you to inspect different data points interactively.

For a complete example of this implementation, you can refer to the sample project on GitHub [here](https://github.com/SyncfusionExamples/How-to-Add-Multiple-Trackballs-in-a-WPF-SfChart). This sample demonstrates how to add and interact with multiple trackballs in a WPF SfChart, providing a practical reference to complement the steps outlined in this article.

