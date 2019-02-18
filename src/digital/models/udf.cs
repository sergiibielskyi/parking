using System.Collections.Generic;

namespace digital
{
    public class UserDefinedFunction
    {
        public string Id { get; set; }
        public IEnumerable<Matcher> Matchers { get; set; }
        public string Name { get; set; }
        public string SpaceId { get; set; }
    }
}