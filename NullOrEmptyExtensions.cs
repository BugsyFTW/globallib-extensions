using System.Collections;
using System.Collections.Generic;

namespace GlobalLib.Extensions
{
    public static class NullOrEmptyExtensions
    {
        #region isNullOrEmpty

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this IEnumerable source)
        {
            if (source != null)
            {
                foreach (object obj in source)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
        {
            if (source != null)
            {
                foreach (T obj in source)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty<T>(this object[] source)
        {
            if (source != null)
            {
                foreach (T obj in source)
                {
                    return false;
                }
            }
            return true;
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Obj"></param>
        /// <param name="ValNull"></param>
        /// <returns></returns>
        public static object NullOrValue(this object Obj, object ValNull)
        {
            if (Obj == null)
            {
                return ValNull;
            }
            else
            {
                return Obj;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Obj"></param>
        /// <param name="ValNull"></param>
        /// <returns></returns>
        public static string NullOrValue(this string Obj, string ValNull)
        {
            if (Obj == null)
            {
                return ValNull;
            }
            else
            {
                return Obj;
            }
        }
    }
}
