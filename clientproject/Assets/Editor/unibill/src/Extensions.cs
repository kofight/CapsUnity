using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unibill.Impl {
    public static class UnibillExtensions {

        public static string nJson(this Dictionary<string, object> dic) {
            return Newtonsoft.Json.JsonConvert.SerializeObject(dic, Newtonsoft.Json.Formatting.Indented);
        }
    }

}
