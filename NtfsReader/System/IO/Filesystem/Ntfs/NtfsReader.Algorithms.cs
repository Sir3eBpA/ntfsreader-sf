/*
    The NtfsReader library.

    Copyright (C) 2008 Danny Couture

    This library is free software; you can redistribute it and/or
    modify it under the terms of the GNU Lesser General Public
    License as published by the Free Software Foundation; either
    version 2.1 of the License, or (at your option) any later version.

    This library is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
    Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public
    License along with this library; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
  
    For the full text of the license see the "License.txt" file.

    This library is based on the work of Jeroen Kessels, Author of JkDefrag.
    http://www.kessels.com/Jkdefrag/
    
    Special thanks goes to him.
  
    Danny Couture
    Software Architect
*/
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Cysharp.Text;
using Utils;

namespace System.IO.Filesystem.Ntfs
{
    public partial class NtfsReader
    {
        private const int MAX_BUFFER = 2048;

        /// <summary>
        /// Recurse the node hierarchy and construct its entire name
        /// stopping at the root directory.
        /// </summary>
        private ReadOnlySpan<char> GetNodeFullNameCore(UInt32 nodeIndex)
        {
            UInt32 node = nodeIndex;
            int index = 0;

            var fullPathNodes = ArrayPool<UInt32>.Shared.Rent(256);
            fullPathNodes[index] = node;
            ++index;

            UInt32 lastNode = node;
            while (true)
            {
                UInt32 parent = _nodes[node].ParentNodeIndex;

                //loop until we reach the root directory
                if (parent == ROOTDIRECTORY)
                    break;

                if (parent == lastNode)
                    throw new InvalidDataException("Detected a loop in the tree structure.");

                fullPathNodes[index] = parent;
                ++index;

                lastNode = node;
                node = parent;
            }

            // use fast string builder here to avoid any unnecessary allocations
            // this has proven out to be even faster than ZString
            // trim by max path size but this is a subject to change because of LongPathsEnabled
            // TODO: check unsafe code improvements at some point in future
            var fullPath = new FastStringBuilder(MAX_BUFFER);
            fullPath.Append(_driveInfo.Name.TrimEnd(new char[] { '\\' }).AsSpan());

            for(int i = index-1; i >= 0; i--)
            {
                node = fullPathNodes[i];

                fullPath.Append(@"\".AsSpan());
                fullPath.Append(GetNameFromIndex(_nodes[node].NameIndex).AsSpan());
            }

            ArrayPool<UInt32>.Shared.Return(fullPathNodes);
            return fullPath.GetSpan();
        }
    }
}
