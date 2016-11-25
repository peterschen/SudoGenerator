using System.Collections.Generic;

namespace SudoGenerator.Classes
{
    public class ProductVersion
    {
        public string Id { get; set; }
        public string Value { get; set; }

        public static readonly ProductVersion OM2012RTM = new ProductVersion("2012-rtm", "2012 RTM");
        public static readonly ProductVersion OM2012R2 = new ProductVersion("2012-r2", "2012 SP1 / 2012 R2");
        public static readonly ProductVersion OM2016RTM = new ProductVersion("2016-rtm", "2016 RTM (Beta)");

        public static readonly List<ProductVersion> ALL = new List<ProductVersion> { OM2012RTM, OM2012R2, OM2016RTM };

        public ProductVersion(string id, string value)
        {
            Id = id;
            Value = value;
        }
    }
}