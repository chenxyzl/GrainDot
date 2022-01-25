using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ExcelDataReader;
using ExcelMapper.Abstractions;

namespace ExcelMapper.Readers;

/// <summary>
///     Reads the value of a cell and produces multiple values by splitting the string value
///     using the given separators.
/// </summary>
public abstract class SplitCellValueReader : IMultipleCellValuesReader
{
    private ISingleCellValueReader _cellReader;

    /// <summary>
    ///     Constructs a reader that reads the string value of a cell and produces multiple values
    ///     by splitting it.
    /// </summary>
    /// <param name="cellReader">The ICellValueReader that reads the string value of the cell before it is split.</param>
    public SplitCellValueReader(ISingleCellValueReader cellReader)
    {
        CellReader = cellReader ?? throw new ArgumentNullException(nameof(cellReader));
    }

    /// <summary>
    ///     Gets or sets the options used to split the string value of the cell.
    /// </summary>
    public StringSplitOptions Options { get; set; }

    /// <summary>
    ///     Gets or sets the ICellValueReader that reads the string value of the cell
    ///     before it is split.
    /// </summary>
    public ISingleCellValueReader CellReader
    {
        get => _cellReader;
        set => _cellReader = value ?? throw new ArgumentNullException(nameof(value));
    }

    public bool TryGetValues(ExcelSheet sheet, int rowIndex, IExcelDataReader reader,
        out IEnumerable<ReadCellValueResult> result, MemberInfo member)
    {
        if (!CellReader.TryGetValue(sheet, rowIndex, reader, out var readResult))
        {
            result = default;
            return false;
        }

        if (readResult.StringValue == null)
        {
            result = Enumerable.Empty<ReadCellValueResult>();
            return true;
        }

        var splitStringValues = GetValues(readResult.StringValue);
        result = splitStringValues.Select(s =>
            new ReadCellValueResult(readResult.ColumnIndex, readResult.ColumnName, s));
        return true;
    }

    protected abstract string[] GetValues(string value);
}