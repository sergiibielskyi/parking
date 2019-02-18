using System.Collections.Generic;

namespace digital
{
    public class UserDefinedFunctionUpdate
    {
        public string Id { get; set; }
        public IEnumerable<string> Matchers { get; set; }
        public string Name { get; set; }
        public string SpaceId { get; set; }
    }
}