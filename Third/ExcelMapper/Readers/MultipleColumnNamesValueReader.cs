﻿using System;
using System.Collections.Generic;
using System.Reflection;
using ExcelDataReader;
using ExcelMapper.Abstractions;
using ExcelMapper.Utilities;

namespace ExcelMapper.Readers;

/// <summary>
///     Reads a multiple values of one or more columns given the name of each column.
/// </summary>
public sealed class MultipleColumnNamesValueReader : IMultipleCellValuesReader
{
    /// <summary>
    ///     Constructs a reader that reads the values of one or more columns with a given name
    ///     and returns the string value of for each column.
    /// </summary>
    /// <param name="columnNames">The names of each column to read.</param>
    public MultipleColumnNamesValueReader(params string[] columnNames)
    {
        if (columnNames == null) throw new ArgumentNullException(nameof(columnNames));

        if (columnNames.Length == 0) throw new ArgumentException("Column names cannot be empty.", nameof(columnNames));

        foreach (var columnName in columnNames)
            if (columnName == null)
                throw new ArgumentException($"Null column name in {columnNames.ArrayJoin()}.", nameof(columnNames));

        ColumnNames = columnNames;
    }

    /// <summary>
    ///     Gets the names of each column to read.
    /// </summary>
    public string[] ColumnNames { get; }

    public bool TryGetValues(ExcelSheet sheet, int rowIndex, IExcelDataReader reader,
        out IEnumerable<ReadCellValueResult> result, MemberInfo member)
    {
        if (sheet.Heading == null)
            throw new ExcelMappingException(
                $"The sheet \"{sheet.Name}\" does not have a heading. Use a column index mapping instead.");

        var values = new ReadCellValueResult[ColumnNames.Length];
        for (var i = 0; i < ColumnNames.Length; i++)
        {
            if (!sheet.Heading.TryGetColumnIndex(ColumnNames[i], out var index))
            {
                result = default;
                return false;
            }

            var value = reader[index]?.ToString();
            values[i] = new ReadCellValueResult(index, ColumnNames[index], value);
        }

        result = values;
        return true;
    }
}