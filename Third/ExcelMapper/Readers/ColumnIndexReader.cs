using System;
using ExcelDataReader;
using ExcelMapper.Abstractions;

namespace ExcelMapper.Readers;

/// <summary>
///     Reads the value of a single cell given the zero-based index of it's column.
/// </summary>
public sealed class ColumnIndexValueReader : ISingleCellValueReader
{
    /// <summary>
    ///     Constructs a reader that reads the value of a single cell given the zero-based index of it's column.
    /// </summary>
    /// <param name="columnIndex">The zero-based index of the column to read.</param>
    public ColumnIndexValueReader(int columnIndex)
    {
        if (columnIndex < 0)
            throw new ArgumentOutOfRangeException(nameof(columnIndex), columnIndex,
                $"Column index {columnIndex} must be greater or equal to zero.");

        ColumnIndex = columnIndex;
    }

    /// <summary>
    ///     The zero-based index of the column to read.
    /// </summary>
    public int ColumnIndex { get; }

    public bool TryGetValue(ExcelSheet sheet, int rowIndex, IExcelDataReader reader, out ReadCellValueResult result)
    {
        if (ColumnIndex >= reader.FieldCount)
        {
            result = default;
            return false;
        }

        var value = reader[ColumnIndex]?.ToString();
        var name = sheet.Heading.GetColumnName(ColumnIndex);
        result = new ReadCellValueResult(ColumnIndex, name, value);
        return true;
    }
}