﻿using CommonLib.Extensions;
using CommonLib.GamePong;
using CommonLib.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace CommonLib.Slim {

    /// <summary>
    /// Creates a server of objects of a given type
    /// </summary>
    /// <typeparam name="T">The type of the object 
    /// being received/sent from/to the client</typeparam>
    public class SlimServer<T> : SlimServer {

        public delegate void DataReceivedEventHandler(object sender, Events.DataReceivedEventArgs<T> e);

        /// <summary>
        /// Raised when an object is received from the client
        /// </summary>
        public new event DataReceivedEventHandler DataReceived;

        public SlimServer() : base() {
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
