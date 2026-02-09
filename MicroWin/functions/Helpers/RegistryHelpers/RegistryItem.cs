namespace MicroWin.functions.Helpers.RegistryHelpers
{
    /// <summary>
    /// Class for registry items
    /// </summary>
    public class RegistryItem
    {
        /// <summary>
        /// The name of the registry value
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The kind of the registry value
        /// </summary>
        public ValueKind Kind { get; set; }
        /// <summary>
        /// The data of the registry value
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// Creates a <see cref="RegistryItem"/> instance with name, kind, and data
        /// values.
        /// </summary>
        /// <param name="name">The name of the registry item</param>
        /// <param name="kind">The kind of the registry item</param>
        /// <param name="data">The data of the registry item</param>
        public RegistryItem(string name, ValueKind kind, object data)
        {
            Name = name;
            Kind = kind;
            Data = data;
        }
    }
}