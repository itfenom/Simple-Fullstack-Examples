namespace Playground.Winforms.Forms.Examples.WorldTreeViewEx
{
    public abstract class TreeDataObject
    {
        public int Id { get; set; }
        public string FieldName { get; set; }
        public int ParentId { get; set; }
    }

    public class ContinentObject : TreeDataObject
    {

    }

    public class CountryObject : TreeDataObject
    {

    }

    public class StateObject : TreeDataObject
    {

    }

    public class CityObject : TreeDataObject
    {

    }
}
