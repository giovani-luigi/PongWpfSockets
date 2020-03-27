using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLib.Extensions {
    public static class JsonExtensions {

        public static string ToJSON<T>(this object obj) {
            return JsonConvert.SerializeObject(obj,
                new JsonSerializerSettings() {
                    TypeNameHandling = TypeNameHandling.Auto,
                    NullValueHandling = NullValueHandling.Ignore
                });
        }

        public static T FromJSON<T>(this string json) {
            return JsonConvert.DeserializeObject<T>(json,
                new JsonSerializerSettings() {
                    TypeNameHandling = TypeNameHandling.Auto,
                    NullValueHandling = NullValueHandling.Ignore
                });
        }

    }
}
