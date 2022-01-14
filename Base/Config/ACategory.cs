using ExcelMapper;
using Message;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Base
{
    public abstract class ACategory : ISupportInitialize
    {
        public abstract Type ConfigType { get; }

        public virtual void BeginInit()
        {
        }

        public virtual void EndInit()
        {
        }
    }

    /// <summary>
    /// 管理该所有的配置
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ACategory<K, T> : ACategory where T : IConfig<K>
    {
        protected Dictionary<K, T> dict;

        public override void BeginInit()
        {
            //typeof(T).GetTypeInfo().IsAssignableFrom(typeof(IExcelConfig).Ge‌​tTypeInfo())
            var fileName = this.GetType().GetCustomAttribute<ConfigAttribute>().FileName;
            if (typeof(IExcelConfig<K>).IsAssignableFrom(typeof(T)))
            {
                FromExcel(fileName);
            }
            else
            {
                FromTextJson(fileName);
            }
        }

        private void FromTextJson(string fileName)
        {
            string path = $"../Config/{fileName}.txt";
            string configStr = ConfigHelper.GetText(path);

            try
            {
                this.dict = ConfigHelper.ToObject<Dictionary<K, T>>(configStr);
            }
            catch (Exception e)
            {
                throw new Exception($"parser json fail: {configStr}", e);
            }
        }
        private void FromExcel(string fileName)
        {
            this.dict = new Dictionary<K, T>();
            string path = $"../Config/{fileName}.xlsx";
            try
            {
                using (var stream = File.OpenRead(path))
                {
                    using var importer = new ExcelImporter(stream);
                    var sheetNames = typeof(T).GetCustomAttribute<SheetNameAttribute>().SheetName;
                    foreach (var sheetName in sheetNames)
                    {
                        ExcelSheet sheet = importer.ReadSheet(sheetName);
                        T[] president = sheet.ReadRows<T>().ToArray();
                        foreach (var v in president)
                        {
                            this.dict[v.Id] = v;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception($"parser excel fail: {path}", e);
            }


        }

        public override Type ConfigType
        {
            get
            {
                return typeof(T);
            }
        }

        public override void EndInit()
        {
            
        }

        public T Get(K id)
        {
            T t;
            if (!this.dict.TryGetValue(id, out t))
            {
                A.RequireNotNull(t, Code.ConfigNotFound, $"not found config: {typeof(T)} id: {id}");
            }
            return t;
        }

        public Dictionary<K, T> GetAll()
        {
            return this.dict;
        }

        public T GetOne()
        {
            return this.dict.Values.First();
        }
    }

    /// <summary>
    /// 管理该所有的配置
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ACategory<K, T, V> : ACategory where T : ExcelClassMap<V> where V : IConfig<K>
    {
        protected Dictionary<K, V> dict;

        public override void BeginInit()
        {
            //typeof(T).GetTypeInfo().IsAssignableFrom(typeof(IExcelConfig).Ge‌​tTypeInfo())
            var fileName = this.GetType().GetCustomAttribute<ConfigAttribute>().FileName;
            FromExcel(fileName);
        }
        private void FromExcel(string fileName)
        {
            this.dict = new Dictionary<K, V>();
            string path = $"../Config/{fileName}.xlsx";
            try
            {
                using (var importer = new ExcelImporter(File.OpenRead(path)))
                {
                    importer.Configuration.RegisterClassMap(Activator.CreateInstance<T>());
                    var sheetNames = typeof(V).GetCustomAttribute<SheetNameAttribute>().SheetName;
                    foreach (var sheetName in sheetNames)
                    {
                        ExcelSheet sheet = importer.ReadSheet(sheetName);
                        V[] president = sheet.ReadRows<V>().ToArray();
                        foreach (var v in president)
                        {
                            this.dict[v.Id] = v;
                        }
                    }
                    Console.WriteLine(dict);
                }
            }
            catch (Exception e)
            {
                throw new Exception($"parser excel fail: {path}", e);
            }


        }

        public override Type ConfigType
        {
            get
            {
                return typeof(V);
            }
        }

        public override void EndInit()
        {
        }

        public V Get(K id)
        {
            V t;
            if (!this.dict.TryGetValue(id, out t))
            {
                A.RequireNotNull(t, Code.ConfigNotFound, $"not found config: {typeof(V)} id: {id}");
            }
            return t;
        }

        public Dictionary<K, V> GetAll()
        {
            return this.dict;
        }

        public V GetOne()
        {
            return this.dict.Values.First();
        }
    }
}