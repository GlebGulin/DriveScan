using DTOs.Responces;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace BLL.Helper
{
    public class ListFileComparer : IEqualityComparer<FileDiskDTO>
    {
        public bool Equals([AllowNull] FileDiskDTO x, [AllowNull] FileDiskDTO y)
        {
            return x.Name == y.Name;
        }

        public int GetHashCode([DisallowNull] FileDiskDTO obj)
        {
            return obj.Name.GetHashCode();
        }
    }
}
