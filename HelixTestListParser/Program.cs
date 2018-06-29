using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Diagnostics;
using System.IO;
using HelixTestListParser.JSONFormat;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;

namespace HelixTestListParser
{
    class Program
    {
        public static string TestPath;
        public static bool InPlace = false;
        public static string OutputPath;

        static void Main(string[] args)
        {
            ArgumentSyntax syntax = ParseCommandLine(args);
            string outputPath = InPlace ? TestPath : OutputPath;
            if (!InPlace && String.IsNullOrEmpty(OutputPath))
            {
                Console.WriteLine("Provide output path when using in place");
                throw new ArgumentException();
            }

            OutputDefs(StripLogsFromFile(TestPath), outputPath, InPlace);
        }
        
        private static ArgumentSyntax ParseCommandLine(string[] args)
        {

            ArgumentSyntax argSyntax = ArgumentSyntax.Parse(args, syntax =>
            {
                syntax.DefineOption("testlist", ref TestPath, "Path to testlist");
                syntax.DefineOption("inplace", ref InPlace, "Modify in place");
                syntax.DefineOption("outputPath", ref OutputPath, "PathToOutput");

            });

            return argSyntax;
        }

        public static void OutputDefs(List<CLRCIDefinition> defs, string outputPath, bool InPlace)
        {

           
            using (var sw = new StreamWriter(outputPath, InPlace))
            using (var wr = new JsonTextWriter(sw))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(wr, defs);
            }
        }

        public static List<CLRCIDefinition> StripLogsFromFile(string inputPath)
        {
            Debug.Assert(File.Exists(inputPath));

            var fullDefs = new List<HelixTestDefinition>();
            var strippedDefs = new List<CLRCIDefinition>();

            JSchemaGenerator jsonGenerator = new JSchemaGenerator();
            JSchema testDefinitionSchema = jsonGenerator.Generate(typeof(IList<HelixTestDefinition>));

            var validationMessages = new List<string>();

            using (var sr = new StreamReader(inputPath))
            using (var jsonReader = new JsonTextReader(sr))
            using (var jsonValidationReader = new JSchemaValidatingReader(jsonReader))
            {
                // Create schema validator
                jsonValidationReader.Schema = testDefinitionSchema;
                jsonValidationReader.ValidationEventHandler += (o, a) => validationMessages.Add(a.Message);

                JsonSerializer serializer = new JsonSerializer();
                fullDefs = serializer.Deserialize<List<HelixTestDefinition>>(jsonValidationReader);
            }


            return MinifyDefList(fullDefs);
        }

        public static List<CLRCIDefinition> MinifyDefList(List<HelixTestDefinition> testDefList)
        {
            var minifiedList = new List<CLRCIDefinition>();
            
            foreach(HelixTestDefinition testDef in testDefList)
            {
                minifiedList.Add(MinifyDef(testDef));
            }

            return minifiedList;
        }

        public static CLRCIDefinition MinifyDef(HelixTestDefinition testDef)
        {
            return new CLRCIDefinition() { WorkItemId=testDef.WorkItemId, PayloadUri=$"https://cloudcijobs.blob.core.windows.net/coreclrci/CoreFXArchives_OSX/{testDef.WorkItemId}.zip" };
        }
    }
}
