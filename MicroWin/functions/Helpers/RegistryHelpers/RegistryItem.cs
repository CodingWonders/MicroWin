namespace MicroWin.functions.Helpers.RegistryHelpers
{
    public class RegistryItem
    {
        public string Name { get; set; }
        public ValueKind Kind { get; set; }
        public object Data { get; set; }

        public RegistryItem(string name, ValueKind kind, object data)
        {
            Name = name;
            Kind = kind;
            Data = data;
        }
    }
}