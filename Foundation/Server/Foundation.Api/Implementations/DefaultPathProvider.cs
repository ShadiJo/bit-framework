﻿using System;
using System.IO;
using Foundation.Core.Contracts;

namespace Foundation.Api.Implementations
{
    public class DefaultPathProvider : IPathProvider
    {
        private static IPathProvider _current;

        protected DefaultPathProvider()
        {
        }

        public static IPathProvider Current
        {
            get
            {
                if (_current == null)
                    _current = new DefaultPathProvider();
                return _current;
            }
            set { _current = value; }
        }

        public virtual string GetCurrentAppPath()
        {
            return AppContext.BaseDirectory;
        }

        public virtual string MapPath(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            return Path.Combine(GetCurrentAppPath(), path);
        }
    }
}