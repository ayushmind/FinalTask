using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using FinalTask.Task1.ThirdParty;

namespace FinalTask.Task1
{
    public class Print : ICommand
    {
        private readonly IDatabaseManager manager;
        private readonly IView view;
        private string tableName;

        public Print(IView view, IDatabaseManager manager)
        {
            this.view = view;
            this.manager = manager;
        }

        public bool CanProcess(string command)
        {
            return command.StartsWith("print ");
        }

        public void Process(string command)
        {
            string[] commandPart = command.Split(' ');
            ValidateCommand(commandPart);
            tableName = commandPart[1];
            IList<IDataSet> data = manager.GetTableData(tableName);
            view.Write(GetTableData(data));
        }

        private void ValidateCommand(string[] commandPart)
        {
            if (commandPart.Length != 2)
                throw new ArgumentException($"incorrect number of parameters. Expected 1, but is {commandPart.Length - 1}");
        }

        private string GetTableData(IList<IDataSet> data)
        {
            int maxColumnSize = GetMaxColumnSize(data);
            if (maxColumnSize == 0)
                return GetEmptyTable(tableName);
            else
                return GetHeaderOfTheTable(data, maxColumnSize) + GetTableRowData(data, maxColumnSize);
        }

        private string GetEmptyTable(string tableName)
        {
            string textEmptyTable = "║ Table '" + tableName + "' is empty or does not exist ║";
            StringBuilder result = new StringBuilder();

            result.Append("╔");
            SetStringBuilderInLoop(textEmptyTable.Length - 2, "═", ref result);

            result.Append("╗\n");
            result.Append(textEmptyTable + "\n");
            result.Append("╚");

            SetStringBuilderInLoop(textEmptyTable.Length - 2, "═", ref result);

            result.Append("╝\n");

            return result.ToString();
        }

        private void SetStringBuilderInLoop(int repeatCount, string stringToRepeat, ref StringBuilder stringBuilder)
        {
            for (int i = 0; i < repeatCount; i++)
                stringBuilder.Append(stringToRepeat);
        }

        private int GetMaxColumnSize(IList<IDataSet> dataSets)
        {
            int maxColumnSize = 0;
            if (dataSets.Count > 0)
            {
                maxColumnSize = GetMaxNumber(GetMaxColumnSizeByHeader(dataSets[0]), GetMaxColumnSizeByValue(dataSets));
            }
            return maxColumnSize;
        }

        private int GetMaxNumber(int number1, int number2)
        {
            return number1 > number2 ? number1 : number2;
        }

        private int GetMaxColumnSizeByHeader(IDataSet dataSet)
        {
            IList<string> columnNames = dataSet.GetColumnNames();
            return columnNames.Max(column => column.Length);
        }

        private int GetMaxColumnSizeByValue(IList<IDataSet> dataSets)
        {
            return dataSets.Max(dataSet => dataSet.GetValues().Max(x => x.ToString().Length));                  
        }

        private string GetTableRowData(IList<IDataSet> dataSets, int maxColumnSize)
        {
            
            StringBuilder result = new StringBuilder();
            maxColumnSize = GetMaxColumnSizeForCell(maxColumnSize);

            SetTableRowData(dataSets, maxColumnSize, ref result);

            SetTableFooterData(maxColumnSize, GetColumnCount(dataSets), ref result);

            return result.ToString();
        }

        public void SetTableRowData(IList<IDataSet> dataSets, int maxColumnSize, ref StringBuilder result)
        {
            int rowsCount = dataSets.Count;
            int columnCount = GetColumnCount(dataSets);

            for (int row = 0; row < rowsCount; row++)
            {
                IList<object> values = dataSets[row].GetValues();
                result.Append("║");
                for (int column = 0; column < columnCount; column++)
                {
                    int valuesLength = values[column].ToString().Length;

                    SetStringBuilderInLoop((maxColumnSize - valuesLength) / 2, " ", ref result);
                    result.Append(values[column].ToString());

                    SetStringBuilderInLoop(GetSize(maxColumnSize, valuesLength), " ", ref result);
                    result.Append("║");

                }
                result.Append("\n");

                if (row < rowsCount - 1)
                {
                    result.Append("╠");
                    for (int j = 1; j < columnCount; j++)
                    {
                        SetStringBuilderInLoop(maxColumnSize, "═", ref result);
                        result.Append("╬");
                    }

                    SetStringBuilderInLoop(maxColumnSize, "═", ref result);
                    result.Append("╣\n");
                }
            }
        }

        private int GetMaxColumnSizeForCell(int maxColumnSize)
        {
            if (maxColumnSize % 2 == 0)
                maxColumnSize += 2;
            else
                maxColumnSize += 3;

            return maxColumnSize;
        }

        private void SetTableFooterData(int maxColumnSize, int columnCount, ref StringBuilder result)
        {
            result.Append("╚");
            for (int j = 1; j < columnCount; j++)
            {
                SetStringBuilderInLoop(maxColumnSize, "═", ref result);
                result.Append("╩");
            }

            SetStringBuilderInLoop(maxColumnSize, "═", ref result);
            result.Append("╝\n");
        }

        private int GetColumnCount(IList<IDataSet> dataSets)
        {
            int result = 0;
            if (dataSets.Count > 0)
                return dataSets[0].GetColumnNames().Count;
            return result;
        }

        private string GetHeaderOfTheTable(IList<IDataSet> dataSets ,int maxColumnSize)
        {
            StringBuilder result = new StringBuilder();
            
            maxColumnSize = GetMaxColumnSizeForCell(maxColumnSize);

            SetHeaderData(dataSets, maxColumnSize, ref result);

            SetLastHeader(maxColumnSize, GetColumnCount(dataSets), ref result);

            return result.ToString();
        }

        private void SetHeaderData(IList<IDataSet> dataSets, int maxColumnSize, ref StringBuilder result)
        {
            int columnCount = GetColumnCount(dataSets);

            result.Append("╔");
            for (int j = 1; j < columnCount; j++)
            {
                SetStringBuilderInLoop(maxColumnSize, "═", ref result);
                result.Append("╦");
            }

            SetStringBuilderInLoop(maxColumnSize, "═", ref result);
            result.Append("╗\n");

            IList<string> columnNames = dataSets[0].GetColumnNames();
            for (int column = 0; column < columnCount; column++)
            {
                result.Append("║");
                int columnNamesLength = columnNames[column].Length;

                SetStringBuilderInLoop((maxColumnSize - columnNamesLength) / 2, " ", ref result);

                result.Append(columnNames[column]);

                SetStringBuilderInLoop(GetSize(maxColumnSize, columnNamesLength), " ", ref result);

            }
            result.Append("║\n");
        }

        private void SetLastHeader(int maxColumnSize, int columnCount, ref StringBuilder result)
        {
            result.Append("╠");
            for (int j = 1; j < columnCount; j++)
            {
                SetStringBuilderInLoop(maxColumnSize, "═", ref result);
                result.Append("╬");
            }

            SetStringBuilderInLoop(maxColumnSize, "═", ref result);
            result.Append("╣\n");
        }

        private int GetSize(int maxColumnSize , int value)
        {
            if (value % 2 == 0)
                return (maxColumnSize - value) / 2;
            return ((maxColumnSize - value) / 2) + 1;
        }
    }
}
