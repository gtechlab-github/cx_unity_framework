﻿#if UNITY_EDITOR

using System.Collections.Generic;
using System.IO;
using System.Data;
using ExcelDataReader;

public class m9ExcelReader
{
    public class SheetData
    {
        public int rowCount = 0;
        public int columnCount = 0;
        private List<List<string>> table = new List<List<string>>();
        public List<List<string>> Table
        {
            get { return table; }
        }

        public string At(int row, int column)
        {
            return table[row][column];
        }

    }

    /// <summary>
    /// get data from Excel file by sheet.
    /// </summary>
    /// <param name="filePath">absolute file path on the disk</param>
    /// <param name="sheet">sheet index, from 0 to max</param>
    /// <returns></returns>
    public static SheetData AsStringArray(string filePath, int sheet = 0)
    {
        FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
        IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);

        SheetData sheetData = new SheetData();

        int sheetIndex = 0;
        do
        {
            if (sheetIndex != sheet)
            {
                sheetIndex++;
                continue;
            }

            // read rows
            int rowIndex = 0;
            while (excelReader.Read())
            {
                List<string> rowData = new List<string>();
                // read columns
                for (int col = 0; col < excelReader.FieldCount; col++)
                {
                    rowData.Add(excelReader.IsDBNull(col) ? "" : excelReader.GetString(col));
                }
                sheetData.Table.Add(rowData);
                rowIndex++;
            }

            sheetData.rowCount = rowIndex;
            sheetData.columnCount = excelReader.FieldCount;

            break;

        } while (excelReader.NextResult());

        excelReader.Close();

        return sheetData;
    }

    public static System.Data.DataTable GetDataTable(string filePath)
    {
        FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
        IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
        DataSet result = excelReader.AsDataSet();
        return result.Tables[0];
    }

}

#endif