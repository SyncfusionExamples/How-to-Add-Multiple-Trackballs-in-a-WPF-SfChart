namespace SfChartMultipleTrackball
{
    public class ViewModel
    {
        public List<Model> Data { get; set; }

        public ViewModel()
        {
            Data = new List<Model>
            {
                new Model { Day = "Monday", CPULoad = 35 },
                new Model { Day = "Tuesday", CPULoad = 42 },
                new Model { Day = "Wednesday", CPULoad = 18 },
                new Model { Day = "Thursday", CPULoad = 30 },
                new Model { Day = "Friday", CPULoad = 64 },
                new Model { Day = "Saturday", CPULoad = 22 },
                new Model { Day = "Sunday", CPULoad = 10 }
            };
        }
    }
}
