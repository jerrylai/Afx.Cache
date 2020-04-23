using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Afx.Cache.Impl
{
    /// <summary>
    /// BaseCache
    /// </summary>
    public abstract class BaseCache : IBaseCache
    {
        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// Dispose
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {

        }
    }
}
