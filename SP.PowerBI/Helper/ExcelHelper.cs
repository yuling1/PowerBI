using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using SP.PowerBI.DB.Entities;
using SP.PowerBI.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SP.PowerBI.Helper
{
    public class ExcelHelper : IExcelService, IDisposable
    {
        private string fileName = null; //文件名
        private IWorkbook workbook = null;
        private FileStream fs = null;
        private bool disposed;

        public ExcelHelper()
        { }


        public ExcelHelper(string fileName)
        {
            this.fileName = fileName;
            disposed = false;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (fs != null)
                        fs.Close();
                }

                fs = null;
                disposed = true;
            }
        }

        public List<List<object>> TableToEntities(string targetNameSpace, DataTable dt)
        {
            ExcelHelper excelHelper = new ExcelHelper();
            List<List<object>> entityList = new List<List<object>>();
            //List<Type> types1 = typeof(db.DBContext.Basic_DbContext).Assembly.GetTypes().ToList();
            List<Type> types = typeof(DB.DBContext.Basic_DbContext).Assembly.GetTypes().Where(x => x.Namespace.Contains(targetNameSpace)).ToList();//获取名为targetNameSpace的NameSpace下的所有Type
            List<List<object>> entityLists = new List<List<object>>();
            foreach (Type type in types)
            {
                entityList.Add(excelHelper.TableToEntity(dt, type));
                //Console.WriteLine(entityList.Count);
            }

            return entityList;
        }


        //类型转换 将string转换成指定类型
        private object ToType(Type type, string value)
        {
            //TypeConverter t = System.ComponentModel.TypeDescriptor.GetConverter(type);
            //t.ConvertFrom(value);
            return TypeDescriptor.GetConverter(type).ConvertFrom(value);
        }

        //将DataTable映射成一个指定实体类型的List
        public List<object> TableToEntity(DataTable dt, Type entityType)
        {
            ExcelHelper excelHelper = new ExcelHelper();

            DataColumnCollection heads = dt.Columns;
            string[] headNames = new string[heads.Count];
            int i = 0;
            foreach (DataColumn head in heads)
            {//获取列名
                headNames[i++] = head.ColumnName;
            }

            List<PropertyInfo> properties = entityType.GetProperties().ToList();//获取Entity的properties
            List<object> entityList = new List<object>();//创建要返回的实体列表
            foreach (DataRow dr in dt.Rows)
            {//依次遍历DataTable的每一行
                object e = entityType.GetConstructor(new Type[0]).Invoke(new object[0]);
                foreach (string headName in headNames)
                {//遍历一条数据的每个单元格
                    string matchingColumnName = headName;
                    bool result = false;
                    foreach (var property in properties)
                    {
                        var customAttribute = property.GetCustomAttribute<ColumnNameAttribute>();

                        if (customAttribute != null && customAttribute.Name == headName)
                        {//如果列名和ColumnNameAttribute相同，则返回true
                            matchingColumnName = property.Name;
                            result = true;
                            break;
                        }
                        else if (customAttribute == null && headName == property.Name)
                        {//如果列名和属性名相同，则返回true
                            result = true;
                            break;
                        }
                    }

                    if (result)
                    {//如果单元格的列名是实体属性的话，将单元格的数据存到对象对应的属性值
                        PropertyInfo property = e.GetType().GetProperty(matchingColumnName);
                        object value = excelHelper.ToType(property.PropertyType, dr[headName].ToString());
                        if (!string.IsNullOrEmpty(value.ToString()))
                        {
                            property.SetValue(e, value);
                        }
                    }
                    else if (properties.Any(x => x.Name == "MetaData"))
                    {//如果单元格的列名是MetaData的话，将单元格的数据存到对象MetaData属性中
                        PropertyInfo property = e.GetType().GetProperty("MetaData");
                        object value = property.GetValue(e) + "\t" + headName + ":" + dr[headName];
                        property.SetValue(e, value);
                    }

                }
                entityList.Add(e);
            }
            if (entityList != null)
            {
                return entityList;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 将excel中的数据导入到DataTable中
        /// </summary>
        /// <param name="sheetName">excel工作薄sheet的名称</param>
        /// <returns>返回的DataTable</returns>
        public DataTable ExcelToDataTable(string sheetName = null)
        {
            ISheet sheet = null;
            DataTable dt = new DataTable();
            int startRow = 0;
            try
            {
                fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);//读取文件流
                workbook = new XSSFWorkbook(fs);

                if (workbook == null)
                {
                    Console.WriteLine("Table doesn't exsit!");
                }
                else if (sheetName == null)  //如果没有指定sheetName就获取第一个sheet
                {
                    sheet = workbook.GetSheetAt(0);
                    dt.TableName = sheet.SheetName;
                }
                else
                {
                    sheet = workbook.GetSheet(sheetName);//根据sheetName获取sheet
                    if (sheet == null) //如果没有找到指定的sheetName对应的sheet，则尝试获取第一个sheet
                    {
                        sheet = workbook.GetSheetAt(0);
                    }
                    dt.TableName = sheet.SheetName;
                }
                if (sheet != null)
                {
                    IRow firstRow = sheet.GetRow(0);//第一行 即表头
                    int cellCount = firstRow.LastCellNum; //一行最后一个cell的编号 即总的列数

                    //将非空的表头存入DataTable
                    for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                    {
                        ICell cell = firstRow.GetCell(i);
                        if (cell != null)
                        {
                            string cellValue = cell.StringCellValue;
                            if (cellValue != null)
                            {
                                DataColumn column = new DataColumn(cellValue);
                                dt.Columns.Add(column);
                            }
                        }
                    }

                    startRow = sheet.FirstRowNum + 1;//数据起始行 即表头下一行                    
                    int rowCount = sheet.LastRowNum;//最后一行的行号
                    //将数据
                    for (int i = startRow; i <= rowCount; ++i)
                    {
                        IRow row = sheet.GetRow(i);
                        if (row == null) break; //如果是没有数据的行 就退出循环

                        DataRow dataRow = dt.NewRow();//如果有数据 就在DataTable中新建一行
                        for (int j = row.FirstCellNum; j < cellCount; ++j)
                        {
                            if (row.GetCell(j) != null) //没有数据的单元格都默认是null
                                dataRow[j] = row.GetCell(j).ToString();
                        }
                        dt.Rows.Add(dataRow);//将新建的行加入DataTable
                    }
                }

                return dt;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
                return null;
            }
        }

    }
}
