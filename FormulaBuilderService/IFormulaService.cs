using System.ServiceModel;

namespace FormulaBuilderService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IFormulaService" in both code and config file together.
    [ServiceContract]
    public interface IFormulaService
    {
        [OperationContract]
        string GetFormulasStringRepresentation(string value);
    }
}