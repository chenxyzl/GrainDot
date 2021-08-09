using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ExcelDataReader;
using ExcelMapper.Abstractions;
using ExcelMapper.Readers;

namespace ExcelMapper
{
    public delegate IDictionary<TKey, TValue> CreateDictionaryFactory<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> elements);

    /// <summary>
    /// A map that reads one or more values from one or more cells and maps these values to the type of the
    /// property or field. This is used to map IDictionary properties and fields.
    /// </summary>
    /// <typeparam name="TValue">The element type of the IDictionary property or field.</typeparam>
    public class ManyToOneDictionaryMap<TKey, TValue> : IMap
    {
        /// <summary>
        /// Constructs a map reads one or more values from one or more cells and maps these values as element
        /// contained by the property or field.
        /// </summary>
        /// <param name="valuePipeline">The map that maps the value of a single cell to an object of the element type of the property or field.</param>
        public ManyToOneDictionaryMap(IMultipleCellValuesReader cellValuesReader, IValuePipeline<TKey> keyPipeline, IValuePipeline<TValue> valuePipeline, CreateDictionaryFactory<TKey, TValue> createDictionaryFactory)
        {
            CellValuesReader = cellValuesReader ?? throw new ArgumentNullException(nameof(cellValuesReader));
            KeyPipeline = keyPipeline ?? throw new ArgumentNullException(nameof(keyPipeline));
            ValuePipeline = valuePipeline ?? throw new ArgumentNullException(nameof(valuePipeline));
            CreateDictionaryFactory = createDictionaryFactory ?? throw new ArgumentNullException(nameof(createDictionaryFactory));
        }

        /// <summary>
        /// Gets the map that maps the value of a single cell to an object of the element type of the property
        /// or field.
        /// </summary>
        public IValuePipeline<TKey> KeyPipeline { get; private set; }
        public IValuePipeline<TValue> ValuePipeline { get; private set; }

        /// <summary>
        /// Gets the reader that reads one or more values from one or more cells used to map each
        /// element of the property or field.
        /// </summary>
        public IMultipleCellValuesReader _cellValuesReader;

        public IMultipleCellValuesReader CellValuesReader
        {
            get => _cellValuesReader;
            set => _cellValuesReader = value ?? throw new ArgumentNullException(nameof(value));
        }

        public CreateDictionaryFactory<TKey, TValue> CreateDictionaryFactory { get; }

        public bool TryGetValue(ExcelSheet sheet, int rowIndex, IExcelDataReader reader, MemberInfo member, out object value)
        {
            if (!CellValuesReader.TryGetValues(sheet, rowIndex, reader, out IEnumerable<ReadCellValueResult> valueResults, member))
            {
                throw new ExcelMappingException($"Could not read value for \"{member.Name}\"", sheet, rowIndex, -1);
            }

            ExcelColumnMultiAttribute excelColumnMultiAttribute = member.GetCustomAttribute<ExcelColumnMultiAttribute>();
            if (excelColumnMultiAttribute == null)
            {
                throw new ExcelMappingException($"Could not find ExcelColumnMultiAttribute for \"{member.Name}\"", sheet, rowIndex, -1);
            }

            ExcelColumnNameAttribute excelColumnNameAttribute = member.GetCustomAttribute<ExcelColumnNameAttribute>();
            if (excelColumnMultiAttribute == null)
            {
                throw new ExcelMappingException($"Could not find ExcelColumnNameAttribute for \"{member.Name}\"", sheet, rowIndex, -1);
            }

            var columnNames = sheet.Heading.ColumnNames.ToArray();
            var columnNamesDic = new Dictionary<string, int>();
            for (var idx = 0; idx < columnNames.Length; idx++)
            {
                columnNamesDic.Add(columnNames[idx], idx);
            }

            var keyIndex = new List<ReadCellValueResult>();
            var valueIndex = new List<ReadCellValueResult>();
            for (int idx = 1; idx <= excelColumnMultiAttribute.Count; idx++)
            {
                var keyName = $"{excelColumnNameAttribute.Name}{idx}";
                var valueName = $"{excelColumnNameAttribute.ValueName}{idx}";
                if (columnNamesDic.ContainsKey(keyName))
                {
                    if (!columnNamesDic.ContainsKey(valueName))
                    {
                        throw new ExcelMappingException($"Could not find Value for \"{member.Name}\"", sheet, rowIndex, -1);
                    }
                    keyIndex.Add(valueResults.ToArray()[columnNamesDic[keyName]]);
                    valueIndex.Add(valueResults.ToArray()[columnNamesDic[valueName]]);
                }
            }

            var keys = new List<TKey>();
            foreach (ReadCellValueResult valueResult in keyIndex)
            {
                TKey keyValue = (TKey)ExcelMapper.ValuePipeline.GetPropertyValue(KeyPipeline, sheet, rowIndex, reader, valueResult, member);
                keys.Add(keyValue);
            }
            var values = new List<TValue>();
            foreach (ReadCellValueResult valueResult in valueIndex)
            {
                TValue keyValue = (TValue)ExcelMapper.ValuePipeline.GetPropertyValue(ValuePipeline, sheet, rowIndex, reader, valueResult, member);
                values.Add(keyValue);
            }

            IEnumerable<KeyValuePair<TKey, TValue>> elements = keys.Zip(values, (key, keyValue) => new KeyValuePair<TKey, TValue>(key, keyValue));
            value = CreateDictionaryFactory(elements);
            return true;
        }
    }
}
