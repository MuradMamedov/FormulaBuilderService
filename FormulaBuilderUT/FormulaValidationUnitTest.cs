using System;
using System.Collections.Generic;
using System.IO;
using FormulaBuilderLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FormulaBuilderUT
{
    [TestClass]
    public class FormulaValidationUnitTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FormulaValidation_NullStringArgument()
        {
            List<string> error;
            FormulaValidation.ValidateFormulaXML(null, out error);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FormulaValidation_NullStreamArgument()
        {
            List<string> error;
            FormulaValidation.ValidateFormulaXML(Stream.Null, out error);
        }

#if DEBUG

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void FormulaValidation_EmbeddedSchemeNotFound()
        {
            List<string> error;
            FormulaValidation.ValidateFormulaXML(new MemoryStream(), out error, @"FormulaBuilderLib.Schema.xsd");
        }

#endif
    }
}