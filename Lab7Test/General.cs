using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Lab7Test
{
    internal class General
    {
        internal static void CheckOOP<M, A>(M mark, A alyssa)
        {
            var m = mark.GetType();
            var a = alyssa.GetType();
            FieldInfo[] mFields = m.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            FieldInfo[] aFields = a.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            PropertyInfo[] mProperties = m.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            PropertyInfo[] aProperties = a.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            MethodInfo[] mMethods = m.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            MethodInfo[] aMethods = a.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            Check(mFields, aFields);
            Check(mProperties, aProperties);
            Check(mMethods, aMethods);
            var mTypes = GetUsedTypes(mFields, mProperties, mMethods);
            var aTypes = GetUsedTypes(aFields, aProperties, aMethods);

            Assert.IsFalse(mTypes.Any(t => t.Namespace == "System.Collections.Generic"));
            Assert.IsFalse(aTypes.Any(t => t.Namespace == "System.Collections.Generic"));
        }
        private static void Check(FieldInfo[] m, FieldInfo[] a)
        {
            Assert.AreEqual(m.Length, a.Length);
            if (m.Length > 0 && a.Length > 0)
            {
                m = m.OrderBy(_ => _.Name).ToArray();
                a = a.OrderBy(_ => _.Name).ToArray();
            }
            for (int i = 0; i < m.Length; i++)
            {
                Assert.AreEqual(m[i].Name, a[i].Name);
                Assert.AreEqual(m[i].IsPublic, a[i].IsPublic);
            }
        }
        private static void Check(PropertyInfo[] m, PropertyInfo[] a)
        {
            Assert.AreEqual(m.Length, a.Length);
            if (m.Length > 0 && a.Length > 0)
            {
                m = m.OrderBy(_ => _.Name).ToArray();
                a = a.OrderBy(_ => _.Name).ToArray();
            }
            for (int i = 0; i < m.Length; i++)
            {
                Assert.AreEqual(m[i].Name, a[i].Name);
                Assert.AreEqual(m[i].CanRead, a[i].CanRead);
                Assert.AreEqual(m[i].CanWrite && m[i].SetMethod != null && m[i].SetMethod.IsPublic, a[i].CanWrite && a[i].SetMethod != null && a[i].SetMethod.IsPublic);
                Assert.AreEqual(m[i].PropertyType.Name, a[i].PropertyType.Name);
            }
        }
        private static void Check(MethodInfo[] m, MethodInfo[] a)
        {
            Assert.AreEqual(m.Length, a.Length);
            if (m.Length > 0 && a.Length > 0)
            {
                m = m.OrderBy(_ => _.Name).ThenBy(_ => _.ReturnType).ToArray();
                a = a.OrderBy(_ => _.Name).ThenBy(_ => _.ReturnType).ToArray();
            }
            for (int i = 0; i < m.Length; i++)
            {
                Assert.AreEqual(m[i].Name, a[i].Name);
                Assert.AreEqual(m[i].IsPublic, a[i].IsPublic);
                Assert.AreEqual(m[i].IsVirtual, a[i].IsVirtual);
                Assert.AreEqual(m[i].IsAbstract, a[i].IsAbstract);
                Assert.AreEqual(m[i].ReturnParameter.ParameterType.Name, a[i].ReturnParameter.ParameterType.Name);
            }
        }
        private static IEnumerable<Type> GetUsedTypes(FieldInfo[] fields, PropertyInfo[] properties, MethodInfo[] methods)
        {
            var mTypes = new List<Type>();

            foreach (var field in fields)
            {
                mTypes.Add(field.FieldType);
            }
            foreach (var property in properties)
            {
                mTypes.Add(property.PropertyType);
            }
            foreach (var method in methods)
            {
                mTypes.Add(method.ReturnType);
                foreach (var parameter in method.GetParameters())
                {
                    mTypes.Add(parameter.ParameterType);
                }
            }
            return mTypes.Distinct();
        }
    }
}
