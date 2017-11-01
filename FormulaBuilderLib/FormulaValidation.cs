using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Schema;

namespace FormulaBuilderLib
{
    public class FormulaValidation
    {
        public static bool ValidateFormulaXML(string xml, out List<string> error)
        {
            if (string.IsNullOrEmpty(xml))
            {
                throw new ArgumentNullException();
            }

            using (Stream stream = new MemoryStream())
            {
                var writer = new StreamWriter(stream);
                writer.Write(xml);
                writer.Flush();
                stream.Position = 0;
                return ValidateFormulaXML(stream, out error);
            }
        }

        public static bool ValidateFormulaXML(Stream stream, out List<string> errors,
            string schemeName = @"FormulaBuilderLib.RequestSchema.xsd")
        {
            var position = stream.Position;
            if (stream == Stream.Null)
            {
                throw new ArgumentNullException();
            }

            var errorsList = new List<string>();
            try
            {
                var assembly = typeof(Request).Assembly;
                if (!assembly.GetManifestResourceNames().Contains(schemeName))
                {
                    throw new FileNotFoundException("Embedded scheme not found", schemeName);
                }

                using (var xsd = assembly.GetManifestResourceStream(schemeName))
                {
                    var settings = new XmlReaderSettings {ValidationType = ValidationType.Schema};
                    settings.ValidationEventHandler += (sender, args) =>
                    {
                        if (args.Severity == XmlSeverityType.Warning)
                            errorsList.Add(
                                $"No validation occurred. {args.Message} {args.Exception.LineNumber}, {args.Exception.LinePosition}");
                        else
                            errorsList.Add(
                                $"Validation error: {args.Message} {args.Exception.LineNumber}, {args.Exception.LinePosition}");
                    };

                    var schema = XmlSchema.Read(xsd, null);
                    settings.Schemas.Add(schema);
                    var reader = XmlReader.Create(stream, settings);

                    while (reader.Read()) ;
                }
            }
            catch (Exception ex)
            {
                if (ex is XmlException || ex is InvalidOperationException)
                {
                    errorsList.Add($"{ex.Message}");
                }
                else
                {
                    throw;
                }
            }
            finally
            {
                stream.Position = position;
            }
            errors = errorsList;
            Debug.WriteLine(string.Join("#", errors));
            return errors.Count == 0;
        }
    }
}