using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NB.Core.Attributes
{
    public class CacheAttribute : Attribute
    {
        /// <summary>
        /// 缓存时长(秒)
        /// </summary>
        public int Duration { get; set; }

        /// <summary>
        /// 缓存Key
        /// </summary>
        public string Key { get; set; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="duration">缓存时长</param>
        public CacheAttribute(int duration)
        {
            Duration = duration;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="duration">缓存时长</param>
        /// <param name="key">缓存Key</param>
        public CacheAttribute(int duration, string key)
        {
            Duration = duration;
            Key = key;
        }
    }
}
