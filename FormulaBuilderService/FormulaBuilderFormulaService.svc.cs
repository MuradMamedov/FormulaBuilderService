using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using FormulaBuilderLib;

namespace FormulaBuilderService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service.svc or Service.svc.cs at the Solution Explorer and start debugging.
    public class FormulaBuilderFormulaService : IFormulaService
    {
        public string GetFormulasStringRepresentation(string value)
        {
            var response = new Response();
            RequestWrapper request;
            List<string> errors;
            using (Stream stream = new MemoryStream())
            {
                var writer = new StreamWriter(stream);
                writer.Write(value);
                writer.Flush();
                stream.Position = 0;
                request = WrapperFactory.CreateRequestWrapper(stream, out errors);
            }

            response.Result = request?.StringForm ?? "";
            response.Errors = errors.ToArray();

            var xsSubmit = new XmlSerializer(typeof(Response));
            using (var sww = new StringWriter())
            {
                using (var writer = XmlWriter.Create(sww))
                {
                    xsSubmit.Serialize(writer, response);
                    return sww.ToString();
                }
            }
        }
    }
}