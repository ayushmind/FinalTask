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
            if (commandPart.Length != 2)
                throw new ArgumentException(
                    "incorrect number of parameters. Expected 1, but is " + (commandPart.Length - 1));
            tableName = commandPart[1];
            IList<IDataSet> data = manager.GetTableData(tableName);
            view.Write(GetTableString(data));
        }

        private string GetTableString(IList<IDataSet> data)
        {
            int maxColumnSize = GetMaxColumnSize(data);
            if (maxColumnSize == 0)
                return GetEmptyTable(tableName);
            else
                return GetHeaderOfTheTable(data) + GetStringTableData(data);
        }

        private string GetEmptyTable(string tableName)
        {
            string textEmptyTable = "║ Table '" + tableName + "' is empty or does not exist ║";
            StringBuilder result = new StringBuilder("╔");

            SetStringBuilderInLoop(0, textEmptyTable.Length - 2, "═", ref result);

            result.Append("╗\n");
            result.Append(textEmptyTable + "\n");
            result.Append("╚");

            SetStringBuilderInLoop(0, textEmptyTable.Length - 2, "═", ref result);

            result.Append("╝\n");

            return result.ToString();
        }

        private void SetStringBuilderInLoop(int start , int final , string key , ref StringBuilder sb)
        {
            for (int i = start; i < final; i++)
                sb.Append(key);
            
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
        private string GetStringTableData(IList<IDataSet> dataSets)
        {
            int rowsCount = dataSets.Count;
            int maxColumnSize = GetMaxColumnSize(dataSets);
            StringBuilder result = new StringBuilder();
            if (maxColumnSize % 2 == 0)
                maxColumnSize += 2;
            else
                maxColumnSize += 3;
            int columnCount = GetColumnCount(dataSets);
            for (int row = 0; row < rowsCount; row++)
            {
                IList<object> values = dataSets[row].GetValues();
                result.Append("║");
                for (int column = 0; column < columnCount; column++)
                {
                    int valuesLength = values[column].ToString().Length;
                    if (valuesLength % 2 == 0)
                    {
                        for (int j = 0; j < (maxColumnSize - valuesLength) / 2; j++)
                            result.Append(" ");
                        result.Append(values[column].ToString());
                        for (int j = 0; j < (maxColumnSize - valuesLength) / 2; j++)
                            result.Append(" ");
                        result.Append("║");
                    }
                    else
                    {
                        for (int j = 0; j < (maxColumnSize - valuesLength) / 2; j++)
                            result.Append(" ");
                        result.Append(values[column].ToString());
                        for (int j = 0; j <= (maxColumnSize - valuesLength) / 2; j++)
                            result.Append(" ");
                        result.Append("║");
                    }
                }

                result.Append("\n");
                if (row < rowsCount - 1)
                {
                    result.Append("╠");
                    for (int j = 1; j < columnCount; j++)
                    {
                        for (int i = 0; i < maxColumnSize; i++)
                            result.Append("═");
                        result.Append("╬");
                    }

                    for (int i = 0; i < maxColumnSize; i++)
                        result.Append("═");
                    result.Append("╣\n");
                }
            }

            result.Append("╚");
            for (int j = 1; j < columnCount; j++)
            {
                for (int i = 0; i < maxColumnSize; i++)
                    result.Append("═");
                result.Append("╩");
            }

            for (int i = 0; i < maxColumnSize; i++)
                result.Append("═");
            result.Append("╝\n");
            return result.ToString();
        }

        private int GetColumnCount(IList<IDataSet> dataSets)
        {
            int result = 0;
            if (dataSets.Count > 0)
                return dataSets[0].GetColumnNames().Count;
            return result;
        }

        private string GetHeaderOfTheTable(IList<IDataSet> dataSets)
        {
            int maxColumnSize = GetMaxColumnSize(dataSets);
            StringBuilder result = new StringBuilder();
            int columnCount = GetColumnCount(dataSets);
            if (maxColumnSize % 2 == 0)
                maxColumnSize += 2;
            else
                maxColumnSize += 3;
            result.Append("╔");
            for (int j = 1; j < columnCount; j++)
            {
                for (int i = 0; i < maxColumnSize; i++)
                    result.Append("═");
                result.Append("╦");
            }

            for (int i = 0; i < maxColumnSize; i++)
                result.Append("═");
            result.Append("╗\n");
            IList<string> columnNames = dataSets[0].GetColumnNames();
            for (int column = 0; column < columnCount; column++)
            {
                result.Append("║");
                int columnNamesLength = columnNames[column].Length;
                if (columnNamesLength % 2 == 0)
                {
                    for (int j = 0; j < (maxColumnSize - columnNamesLength) / 2; j++)
                        result.Append(" ");
                    result.Append(columnNames[column]);
                    for (int j = 0; j < (maxColumnSize - columnNamesLength) / 2; j++)
                        result.Append(" ");
                }
                else
                {
                    for (int j = 0; j < (maxColumnSize - columnNamesLength) / 2; j++)
                        result.Append(" ");
                    result.Append(columnNames[column]);
                    for (int j = 0; j <= (maxColumnSize - columnNamesLength) / 2; j++)
                        result.Append(" ");
                }
            }

            result.Append("║\n");

            //last string of the header
            if (dataSets.Count > 0)
            {
                result.Append("╠");
                for (int j = 1; j < columnCount; j++)
                {
                    for (int i = 0; i < maxColumnSize; i++)
                        result.Append("═");
                    result.Append("╬");
                }

                for (int i = 0; i < maxColumnSize; i++)
                    result.Append("═");
                result.Append("╣\n");
            }
            else
            {
                result.Append("╚");
                for (int j = 1; j < columnCount; j++)
                {
                    for (int i = 0; i < maxColumnSize; i++)
                        result.Append("═");
                    result.Append("╩");
                }

                for (int i = 0; i < maxColumnSize; i++)
                    result.Append("═");
                result.Append("╝\n");
            }

            return result.ToString();
        }
    }
}
