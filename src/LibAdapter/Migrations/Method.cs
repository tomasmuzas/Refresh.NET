using System.Collections.Generic;

namespace LibAdapter.Migrations
{
    public class Method
    {
        public string Type { get; set; }

        public string Name { get; set; }

        public IList<Argument> Arguments { get; set; }
    }
}
