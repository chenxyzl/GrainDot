﻿using System;
using ExcelDataReader;
using ExcelMapper.Abstractions;

namespace ExcelMapper.Readers;

/// <summary>
///     Reads the value of a single cell given the name of it's column.
/// </summary>
public sealed class ColumnNameValueReader : ISingleCellValueReader
{
    /// <summary>
    ///     Constructs a reader that reads the value of a single cell given the name of it's column.
    /// </summary>
    /// <param name="columnName">The name of the column to read.</param>
    public ColumnNameValueReader(string columnName)
    {
        if (columnName == null) throw new ArgumentNullException(nameof(columnName));

        if (columnName.Length == 0) throw new ArgumentException("Column name cannot be empty.", nameof(columnName));

        ColumnName = columnName;
    }

    /// <summary>
    ///     The name of the column to read.
    /// </summary>
    public string ColumnName { get; }

    public bool TryGetValue(ExcelSheet sheet, int rowIndex, IExcelDataReader reader, out ReadCellValueResult result)
    {
        if (sheet.Heading == null)
            throw new ExcelMappingException(
                $"The sheet \"{sheet.Name}\" does not have a heading. Use a column index mapping instead.");

        if (!sheet.Heading.TryGetColumnIndex(ColumnName, out var index))
        {
            result = default;
            return false;
        }

        var value = reader[index]?.ToString();
        result = new ReadCellValueResult(index, ColumnName, value);
        return true;
    }
}