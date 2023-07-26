using System.Collections.Generic;

namespace FinalTask.Task1.ThirdParty
{
    public interface IDataSet
    {
        void Put(string columnName, object value);

        IList<string> GetColumnNames();

        IList<object> GetValues();
    }
}
