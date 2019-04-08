using GlobalLib.Extensions;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace GlobalLib.Extensions
{
    public class Field
    {
        public object FieldValue;
        public string FieldName;

        public Field(object value, string n)
        {
            FieldValue = value;
            FieldName = n;
        }
    }

    public class ObjectMirrow
    {
        /// <summary>
        /// Criar e devolve um Objecto com os com os campos e valores
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        public static object CreateFromFields(List<Field> fields)
        {
            var myType = CompileResultType(fields);
            var myObject = Activator.CreateInstance(myType);

            foreach (var field in fields)
            {
                PropertyInfo prop = myObject.GetType().GetProperty(field.FieldName);
                if (prop != null && prop.CanWrite)
                {
                    prop.SetValue(myObject, field.FieldValue);
                }
            }

            return myObject;
        }

        /// <summary>
        /// Criar e devolve um Objecto com os com os campos e valores e remove os mencionados no parametro remove
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="remove"></param>
        /// <returns></returns>
        public static object CreateFromObject(Object obj, List<string> remove)
        {
            List<Field> _fields = new List<Field>();

            var myType = obj.GetType();
            PropertyInfo[] properties = myType.GetProperties();

            foreach (var prop in properties)
            {
                if (remove.Contains(prop.Name) == false)
                {
                    Field field = new Field(prop.GetValue(obj, null), prop.Name);

                    _fields.Add(field);
                }
            }

            if (!_fields.IsNullOrEmpty())
            {
                return CreateFromFields(_fields);
            }

            return null;
        }

        public static Type CompileResultType(List<Field> fields)
        {
            TypeBuilder tb = GetTypeBuilder();

            ConstructorBuilder constructor = tb.DefineDefaultConstructor(MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);

            string typeAsString = "";
            foreach (var field in fields)
            {
                if (field.FieldValue != null)
                {
                    CreateProperty(tb, field.FieldName, field.FieldValue.GetType(), field.FieldValue);
                }
                else
                {
                    CreateProperty(tb, field.FieldName, typeAsString.GetType(), field.FieldValue);
                }
            }

            Type objectType = tb.CreateTypeInfo();
            return objectType;
        }


        private static TypeBuilder GetTypeBuilder()
        {
            var typeSignature = "Transformed";
            var an = new AssemblyName(typeSignature);
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(Guid.NewGuid().ToString()), AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");
            TypeBuilder tb = moduleBuilder.DefineType(typeSignature,
                    TypeAttributes.Public |
                    TypeAttributes.Class |
                    TypeAttributes.AutoClass |
                    TypeAttributes.AnsiClass |
                    TypeAttributes.BeforeFieldInit |
                    TypeAttributes.AutoLayout,
                    null);
            return tb;
        }


        private static void CreateProperty(TypeBuilder tb, string propertyName, Type propertyType, object propertValue)
        {
            FieldBuilder fieldBuilder = tb.DefineField("_" + propertyName, propertyType, FieldAttributes.Private);

            PropertyBuilder propertyBuilder = tb.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null);
            MethodBuilder getPropMthdBldr = tb.DefineMethod("get_" + propertyName, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, propertyType, Type.EmptyTypes);
            ILGenerator getIl = getPropMthdBldr.GetILGenerator();

            getIl.Emit(OpCodes.Ldarg_0);
            getIl.Emit(OpCodes.Ldfld, fieldBuilder);
            getIl.Emit(OpCodes.Ret);

            MethodBuilder setPropMthdBldr =
                tb.DefineMethod("set_" + propertyName,
                  MethodAttributes.Public |
                  MethodAttributes.SpecialName |
                  MethodAttributes.HideBySig,
                  null, new[] { propertyType });

            ILGenerator setIl = setPropMthdBldr.GetILGenerator();
            Label modifyProperty = setIl.DefineLabel();
            Label exitSet = setIl.DefineLabel();

            setIl.MarkLabel(modifyProperty);
            setIl.Emit(OpCodes.Ldarg_0);
            setIl.Emit(OpCodes.Ldarg_1);
            setIl.Emit(OpCodes.Stfld, fieldBuilder);

            setIl.Emit(OpCodes.Nop);
            setIl.MarkLabel(exitSet);
            setIl.Emit(OpCodes.Ret);

            propertyBuilder.SetGetMethod(getPropMthdBldr);
            propertyBuilder.SetSetMethod(setPropMthdBldr);
        }
    }
}