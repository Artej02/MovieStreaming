using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieStreaming.Custom.DatabaseHelpers
{
    public class DatabaseSingleResult<T>
    {
        public bool HasError = false;
        public T Result = default(T);
        public string ErrorMessage = string.Empty;
        public bool HasData => this.Result == null ? false : true;
    }
}
