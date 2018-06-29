using System;
using System.Collections.Generic;
using System.Text;
using System.CommandLine;
using Newtonsoft.Json;

namespace HelixTestListParser.JSONFormat
{
    public class HelixTestDefinition
    {
        public string command;
        public string WorkItemId;
        public string TimeoutInSeconds;
        public string[] CorrelationPayloadUris;
        public string PayloadUri;
    }
}
