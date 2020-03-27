using CommonLib.Extensions;
using CommonLib.GamePong;
using CommonLib.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace CommonLib.Slim {

    public class SlimClientOfObject<T> : SlimClient {

        
        public new delegate void DataReceivedEventHandler(object sender, Events.DataReceivedEventArgs<T> args);

        /// <summary>
        /// Return the received object from server
        /// </summary>
        public new event DataReceivedEventHandler DataReceived;

        public SlimClientOfObject() {
            base.DataReceived += OnRawDataReceived;
        }

        private void OnRawDataReceived(object sender, Events.DataReceivedEventArgs e) {
            try {
                // de-serialize to an object of type T
                T data = e.Data.FromJSON<T>();
                // invoke callbacks if any
                DataReceived?.Invoke(this, new Events.DataReceivedEventArgs<T>(data));
            } catch (Exception ex) {
                Log.WriteLine(ex.ToString());
            }
        }

        public void Send(T data) {
            if (RemoteSocket == null) return; // no one connected
            try {
                // serialize the object of type T to a JSON string then to byte array
                Send(data.ToJSON<T>());
            } catch (Exception ex) {
                Log.WriteLine(ex.ToString());
            }
        }
    }
}
