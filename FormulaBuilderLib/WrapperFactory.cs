using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace FormulaBuilderLib
{
    public static class WrapperFactory
    {
        public static RequestWrapper CreateRequestWrapper(Stream stream, out List<string> error)
        {
            if (stream == Stream.Null)
            {
                throw new ArgumentNullException();
            }

            if (FormulaValidation.ValidateFormulaXML(stream, out error))
            {
                var xs = new XmlSerializer(typeof(Request));
                return new RequestWrapper((Request) xs.Deserialize(stream));
            }
            return null;
        }
    }
}