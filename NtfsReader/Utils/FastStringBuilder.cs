using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public ref struct FastStringBuilder
    {
        private Span<char> span;
        private int pos;

        public FastStringBuilder(int maxlength)
        {
            span = new Span<char>(new char[maxlength]);
            pos = 0;
        }

        public void Append(ReadOnlySpan<char> str)
        {
            if (pos + str.Length > span.Length) throw new IndexOutOfRangeException();
            str.CopyTo(span.Slice(pos));
            pos += str.Length;
        }

        public override string ToString()
        {
            return span.Slice(0, pos).ToString();
        }

        public Span<char> GetSpan()
        {
            return span.Slice(0, pos);
        }
    }

}
