using System.Collections.Generic;
using FormulaBuilderLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FormulaBuilderUT
{
    [TestClass]
    public class XMLValidationUnitTest
    {
        [TestMethod]
        public void XML_Correct()
        {
            var validXml = @"<request>
	                                <expression>
                                        <operation>minus</operation>
		                                <operand>
			                                <const>20</const>
		                                </operand>
		                                <operand>
			                                <expression>
				                                <operation>minus</operation>
				                                <operand>
					                                <const>10</const>
				                                </operand>
				                                <operand>
					                                <const>5</const>
				                                </operand>
			                                </expression>
		                                </operand>
	                                </expression>
                                </request>";
            List<string> errorList;
            var isValid = FormulaValidation.ValidateFormulaXML(validXml, out errorList);
            var error = string.Join("#", errorList);
            Assert.AreEqual(true, isValid, error);
        }

        [TestMethod]
        public void XML_CorrectWithSwappedTags()
        {
            var validXml = @"<request>
	                                <expression>
                                        <operation>minus</operation>
		                                <operand>
			                                <const>20</const>
		                                </operand>
		                                <operand>
			                                <expression>
				                                <operand>
					                                <const>10</const>
				                                </operand>
				                                <operand>
					                                <const>5</const>
				                                </operand>
				                                <operation>minus</operation>
			                                </expression>
		                                </operand>
	                                </expression>
                                </request>";
            List<string> errorList;
            var isValid = FormulaValidation.ValidateFormulaXML(validXml, out errorList);
            var error = string.Join("#", errorList);
            Assert.AreEqual(true, isValid, error);
        }

        [TestMethod]
        public void XML_EmptyRequest()
        {
            var validXml = @"<request>
                            </request>";
            List<string> errorList;
            FormulaValidation.ValidateFormulaXML(validXml, out errorList);
            var error = string.Join("#", errorList);
            Assert.IsTrue(error.Contains(
                    @"The element 'request' has incomplete content. List of possible elements expected: 'expression'."),
                error);
        }

        [TestMethod]
        public void XML_InvalidOperation()
        {
            var invalidXml = @"<request>
	                                <expression>
		                                <operation>pow</operation>
		                                <operand>
			                                <const>20</const>
		                                </operand>
		                                <operand>
			                               	<const>20</const>
		                                </operand>
	                                </expression>
                                </request>";
            List<string> errorList;
            FormulaValidation.ValidateFormulaXML(invalidXml, out errorList);
            var error = string.Join("#", errorList);
            Assert.IsTrue(error.Contains(
                    @"The 'operation' element is invalid - The value 'pow' is invalid according to its datatype 'Operation' - The Enumeration constraint failed."),
                error);
        }

        [TestMethod]
        public void XML_InvalidMultipleOperations()
        {
            var invalidXml = @"<request>
	                                <expression>
		                                <operation>minus</operation>
		                                <operation>minus</operation>
                                        <operand>
			                                <const>20</const>
		                                </operand>
	                                </expression>
                                </request>";
            List<string> errorList;
            FormulaValidation.ValidateFormulaXML(invalidXml, out errorList);
            var error = string.Join("#", errorList);
            Assert.IsTrue(error.Contains(
                @"The element 'expression' has invalid child element 'operand'."), error);
        }

        [TestMethod]
        public void XML_InvalidNoOperation()
        {
            var invalidXml = @"<request>
	                                <expression>
		                                <operand>
			                                <const>20</const>
		                                </operand>
                                        <operand>
			                                <const>20</const>
		                                </operand>
	                                </expression>
                                </request>";
            List<string> errorList;
            FormulaValidation.ValidateFormulaXML(invalidXml, out errorList);
            var error = string.Join("#", errorList);
            Assert.IsTrue(error.Contains(
                    @"The element 'expression' has incomplete content. List of possible elements expected: 'operation, operand'."),
                error);
        }

        [TestMethod]
        public void XML_InvalidOneOperand()
        {
            var invalidXml = @"<request>
	                                <expression>
		                                <operation>minus</operation>
		                                <operand>
			                                <const>20</const>
		                                </operand>
	                                </expression>
                                </request>";
            List<string> errorList;
            FormulaValidation.ValidateFormulaXML(invalidXml, out errorList);
            var error = string.Join("#", errorList);
            Assert.IsTrue(error.Contains(
                    @"The element 'expression' has incomplete content. List of possible elements expected: 'operand'."),
                error);
        }

        [TestMethod]
        public void XML_InvalidThreeOperands()
        {
            var invalidXml = @"<request>
	                                <expression>
		                                <operation>minus</operation>
		                                <operand>
			                                <const>20</const>
		                                </operand>
                                        <operand>
			                                <const>20</const>
		                                </operand>
                                        <operand>
			                                <const>20</const>
		                                </operand>
	                                </expression>
                                </request>";
            List<string> errorList;
            FormulaValidation.ValidateFormulaXML(invalidXml, out errorList);
            var error = string.Join("#", errorList);
            Assert.IsTrue(error.Contains(
                @"The element 'expression' has invalid child element 'operand'."), error);
        }

        [TestMethod]
        public void XML_InvalidOperandWithValueAndExpression()
        {
            var invalidXml = @"<request>
	                                <expression>
		                                <operation>mul</operation>
		                                <operand>
			                                <const>20</const>
                                            <expression>
				                                <operation>minus</operation>
				                                <operand>
					                                <const>10</const>
				                                </operand>
				                                <operand>
					                                <const>5</const>
				                                </operand>
			                                </expression>
		                                </operand>
	                                </expression>
                                </request>";
            List<string> errorList;
            FormulaValidation.ValidateFormulaXML(invalidXml, out errorList);
            var error = string.Join("#", errorList);
            Assert.IsTrue(error.Contains(
                @"The element 'operand' has invalid child element 'expression'."), error);
        }

        [TestMethod]
        public void XML_InvalidOperandValue()
        {
            var invalidXml = @"<request>
	                                <expression>
		                                <operation>mul</operation>
		                                <operand>
			                                <const>twenty</const>
		                                </operand>
	                                </expression>
                                </request>";
            List<string> errorList;
            FormulaValidation.ValidateFormulaXML(invalidXml, out errorList);
            var error = string.Join("#", errorList);
            Assert.IsTrue(error.Contains(
                    @"The 'const' element is invalid - The value 'twenty' is invalid according to its datatype 'http://www.w3.org/2001/XMLSchema:decimal' - The string 'twenty' is not a valid Decimal value."),
                error);
        }
    }
}