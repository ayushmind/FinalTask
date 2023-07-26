using System.Collections.Generic;

namespace FinalTask.Task1.ThirdParty
{
    public interface IDatabaseManager
    {
        IList<IDataSet> GetTableData(string tableName);
    }
}
