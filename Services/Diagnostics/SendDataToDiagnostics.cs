﻿using Microsoft.Azure.IoTSolutions.DeviceSimulation.Services.Http;
using Microsoft.Azure.IoTSolutions.DeviceSimulation.Services.Runtime;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Microsoft.Azure.IoTSolutions.DeviceSimulation.Services.Diagnostics
{
    public class SendDataToDiagnostics
    {
        private readonly IHttpClient httpClient;
        
        public SendDataToDiagnostics(ILogger logger)
        {
            this.httpClient = new HttpClient(logger);
        }

        public async void SendDiagnosticsData(string eventType, string message = "")
        {
            dynamic jobj = new JObject();
            jobj.Timestamp = DateTime.Now;
            jobj.EventType = eventType;
            if (!string.IsNullOrEmpty(message))
            {
                message = "{ ErrorMessage: " + message + "}";
                jobj.EventProperties(message);
            }
            var response = await httpClient.PostAsync(this.PrepareRequest("http://localhost:9006/v1/diagnosticsevents", jobj));
        }

        private HttpRequest PrepareRequest(string path, object obj=null)
        {
            var request = new HttpRequest();
            request.AddHeader(HttpRequestHeader.Accept.ToString(), "application/json");
            request.SetUriFromString(path);
            request.SetContent(obj);
            return request;
        }

    }
}