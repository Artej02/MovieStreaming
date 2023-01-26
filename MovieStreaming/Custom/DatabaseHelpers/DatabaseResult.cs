using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieStreaming.Custom.DatabaseHelpers
{
    public class DatabaseResult<T>
    {
        public bool HasError = false;
        public IEnumerable<T> Result = Enumerable.Empty<T>();
        public string ErrorMessage = string.Empty;
        public int Count => this.Result == null ? 0 : this.Result.Count();
    }
}
