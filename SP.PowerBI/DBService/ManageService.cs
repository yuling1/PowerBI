using SP.PowerBI.DB.DBContext;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP.PowerBI.DBService
{
    public class ManageService : BaseService
    {
        const string dimNameSpace = "SP.PowerBI.DB.Entities.Dimension";
        const string fctNameSpace = "SP.PowerBI.DB.Entities.Fact";
        public ManageService(Basic_DbContext context) : base(context)
        {
        }

        public string StoreExcel2Db(List<List<object>> allLists)
        {
            try
            {
                foreach (var lists in allLists)
                {
                    foreach (var property in context.GetType().GetProperties())
                    {
                        if (property.PropertyType.IsGenericType && property.PropertyType.GenericTypeArguments[0] == lists[0].GetType())
                        {
                            var propertyValue = property.GetGetMethod().Invoke(context, new object[0]);
                            lists.ForEach(x => property.PropertyType.GetMethod("Add").Invoke(propertyValue, new[] { x }));
                        }

                    }
                }
                context.SaveChanges();
                return "Success";
            }
            catch (Exception e)
            {
                return "Error: " + e.ToString();
            }
        }

        //判断两个条数据是否相同
        public static bool DataEqual(object targetData, object data)
        {
            foreach (var targetProp in targetData.GetType().GetProperties())
            {//遍历targetData的所有属性
                var dataProp = data.GetType().GetProperty(targetProp.Name);//data中对应的属性
                if (dataProp == null)
                {//如果data中没有这种属性，就直接返回false
                    return false;
                }
                else if (dataProp.DeclaringType == targetData.GetType())
                {
                    var dataPropValue = dataProp.GetGetMethod().Invoke(data, new object[0]);//data的属性值
                    var targetPropValue = targetProp.GetGetMethod().Invoke(targetData, new object[0]);//target的属性值
                    if (targetPropValue == null && targetPropValue != dataPropValue)
                    {//如果target的属性值为空，data的属性值不为空就直接返回false
                        return false;
                    }
                    else if (targetPropValue != null)
                    {
                        var targetPropValueString = targetPropValue.ToString().Replace(" ", "");
                        var dataPropValueString = dataPropValue.ToString().Replace(" ", "");
                        if (!targetPropValue.Equals(dataPropValue) && targetPropValueString != dataPropValueString)
                        {//如果data中有相同属性但和targetData中的值不同，就直接返回false
                            return false;
                        }
                    }
                }
            }

            return true;//如果data的所有属性、属性值都和sampleData一致，就返回true
        }

        //将data转换成目标类型
        public static object DataConvert(object data, Type targetType)
        {
            var target = targetType.GetConstructor(new Type[0]).Invoke(new object[0]);//创建目标类型的实例对象

            foreach (var targetProp in targetType.GetProperties())
            {//遍历data的所有属性
                var dataProp = data.GetType().GetProperty(targetProp.Name);//获取目标类型相同名字的属性
                if (dataProp != null && targetProp.PropertyType.Name == dataProp.PropertyType.Name)
                {//如果目标类型中有同名属性，而且属性类型也相同，就将该属性值赋给实例对象
                    var dataPropValue = dataProp.GetGetMethod().Invoke(data, new object[0]);
                    targetProp.GetSetMethod().Invoke(target, new object[] { dataPropValue });
                }
            }

            return target;
        }

        //判断dimension中是否已经存在data，存在返回true，不存在返回false
        public static bool ExsitInDimension(ref object data, IEnumerable contextPropValues)
        {
            bool result = false;
            foreach (var contextPropValue in contextPropValues)
            {//遍历dimension表
                if (DataEqual(contextPropValue, data))
                {//如果dimension表中有记录与data相同
                    data = contextPropValue;
                    result = true;
                    break;
                }
            }

            return result;
        }


        //将dimension类型的datas和fact中的外键对应
        public static void SaveTofactFK(List<object> datas, object fctData, IEnumerable contextPropValues)
        {
            var props = fctData.GetType().GetProperties();//获取事实表的所有属性
            foreach (var prop in props)
            {//遍历事实表的所有属性
                foreach (var data in datas)
                {//如果datas里面有同名的维度表记录，就存到事实表里
                    if (prop.PropertyType.Name == data.GetType().Name)
                    {
                        prop.GetSetMethod().Invoke(fctData, new object[] { data });
                    }
                }
            }
        }



        /// <summary>
        /// To solve the Foreign Key issue, this method will give a guid of the related context entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool ConvertModel2Context(List<object> billings)
        {
            List<Type> dimTypes = typeof(Basic_DbContext).Assembly.GetTypes().Where(x => x.Namespace.Contains(dimNameSpace)).ToList();//获取名为dimNameSpace的NameSpace下的所有Type，即所有维度表
            List<Type> fctTypes = typeof(Basic_DbContext).Assembly.GetTypes().Where(x => x.Namespace.Contains(fctNameSpace)).ToList();//获取名为fctNameSpace的NameSpace下的所有Type，即所有事实表
            foreach (var billing in billings)
            {//遍历表格的所有记录
                List<object> dimDatas = new List<object>();
                foreach (var dimType in dimTypes)
                {//对每个维度表做以下操作
                    object dimData = DataConvert(billing, dimType);//将billing转换成dimension类型
                    var contextProp = context.GetType().GetProperty(dimType.Name);//获取和当前维度表名字相同的数据库表
                    var contextPropValues = contextProp.GetGetMethod().Invoke(context, new object[0]) as IEnumerable;//获取该数据库表的值，即对应表的所有记录
                    if (!ExsitInDimension(ref dimData, contextPropValues))
                    {//如果dimension里面没有data，就将data存进去
                        contextProp.PropertyType.GetMethod("Add").Invoke(contextPropValues, new[] { dimData });//存入数据库
                        context.SaveChanges();
                    }
                    dimDatas.Add(dimData);

                }

                foreach (var fcttype in fctTypes)
                {
                    object fctData = DataConvert(billing, fcttype);//将billing转换成fact类型
                    var contextProp = context.GetType().GetProperty(fcttype.Name);//获取和当前事实表名字相同的数据库表
                    var contextPropValues = contextProp.GetGetMethod().Invoke(context, new object[0]) as IEnumerable;//获取该数据库表的值，即对应表的所有记录
                    SaveTofactFK(dimDatas, fctData, contextPropValues);//外键存储
                    contextProp.PropertyType.GetMethod("Add").Invoke(contextPropValues, new[] { fctData });//存入数据库
                    context.SaveChanges();
                }
            }

            return true;
        }
    }
}
