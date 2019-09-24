// Copyright (C) 2019 Pedro Garau Martínez
//
// This file is part of Pleinair.
//
// Pleinair is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// Pleinair is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with Pleinair. If not, see <http://www.gnu.org/licenses/>.
//

using Yarhl.FileFormat;
using Yarhl.FileSystem;

namespace Pleinair.FAD
{
    class Fad2NodeContainer : IConverter<FAD, NodeContainerFormat>
    {

        public NodeContainerFormat Convert(FAD source)
        {
            NodeContainerFormat container = new NodeContainerFormat();

            for (int i = 0; i < source.ImagesCount; i++)
            {
                Node child = NodeFactory.FromMemory(i.ToString().PadLeft(2, '0') + ".YKCMPC");
                child.Stream.Write(source.Containers[i], 0, source.Containers[i].Length);
                container.Root.Add(child);
            }

            return container;
        }
    }
}
