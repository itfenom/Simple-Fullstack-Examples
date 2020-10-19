namespace Playground.WpfApp.Forms.OtherEx.DataTemplating
{
    public enum TaskType
    {
        Home,
        Work
    }

    public class DataTemplatingModel
    {
        public string TaskName { get; set; }
        public string Description { get; set; }
        public int Priority { get; set; }
        public TaskType TaskType { get; set; }

        public override string ToString()
        {
            return TaskName;
        }
    }
}
