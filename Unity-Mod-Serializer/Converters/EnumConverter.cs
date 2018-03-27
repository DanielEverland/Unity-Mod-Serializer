using System;
using System.Collections.Generic;
using System.Text;

namespace UMS.Converters
{
    /// <summary>
    /// Serializes and deserializes enums by their current name.
    /// </summary>
    public class EnumConverter : Converter
    {
        public override bool CanProcess(Type type)
        {
            return type.Resolve().IsEnum;
        }

        public override bool RequestCycleSupport(Type storageType)
        {
            return false;
        }

        public override bool RequestInheritanceSupport(Type storageType)
        {
            return false;
        }

        public override object CreateInstance(Data data, Type storageType)
        {
            // In .NET compact, Enum.ToObject(Type, Object) is defined but the
            // overloads like Enum.ToObject(Type, int) are not -- so we get
            // around this by boxing the value.
            return Enum.ToObject(storageType, (object)0);
        }

        public override Result TrySerialize(object instance, out Data serialized, Type storageType)
        {
            if (Serializer.Config.SerializeEnumsAsInteger)
            {
                serialized = new Data(Convert.ToInt64(instance));
            }
            else if (PortableReflection.GetAttribute<FlagsAttribute>(storageType) != null)
            {
                long instanceValue = Convert.ToInt64(instance);
                var result = new StringBuilder();

                bool first = true;
                foreach (var value in Enum.GetValues(storageType))
                {
                    long integralValue = Convert.ToInt64(value);
                    bool isSet = (instanceValue & integralValue) != 0;

                    if (isSet)
                    {
                        if (first == false) result.Append(",");
                        first = false;
                        result.Append(value.ToString());
                    }
                }

                serialized = new Data(result.ToString());
            }
            else
            {
                serialized = new Data(Enum.GetName(storageType, instance));
            }
            return Result.Success;
        }

        public override Result TryDeserialize(Data data, ref object instance, Type storageType)
        {
            if (data.IsString)
            {
                string[] enumValues = data.AsString.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                long instanceValue = 0;
                for (int i = 0; i < enumValues.Length; ++i)
                {
                    string enumValue = enumValues[i];

                    // Verify that the enum name exists; Enum.TryParse is only
                    // available in .NET 4.0 and above :(.
                    if (ArrayContains(Enum.GetNames(storageType), enumValue) == false)
                    {
                        return Result.Fail("Cannot find enum name " + enumValue + " on type " + storageType);
                    }

                    long flagValue = (long)Convert.ChangeType(Enum.Parse(storageType, enumValue), typeof(long));
                    instanceValue |= flagValue;
                }
                
                instance = Enum.ToObject(storageType, instanceValue);
                
                return Result.Success;
            }
            else if (data.IsInt64)
            {
                int enumValue = (int)data.AsInt64;

                // In .NET compact, Enum.ToObject(Type, Object) is defined but
                // the overloads like Enum.ToObject(Type, int) are not -- so we
                // get around this by boxing the value.
                instance = Enum.ToObject(storageType, (object)enumValue);

                return Result.Success;
            }

            return Result.Fail("EnumConverter encountered an unknown JSON data type");
        }

        /// <summary>
        /// Returns true if the given value is contained within the specified
        /// array.
        /// </summary>
        private static bool ArrayContains<T>(T[] values, T value)
        {
            // note: We don't use LINQ because this function will *not* allocate
            for (int i = 0; i < values.Length; ++i)
            {
                if (EqualityComparer<T>.Default.Equals(values[i], value))
                {
                    return true;
                }
            }

            return false;
        }
    }
}