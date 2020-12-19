using System.Collections.Generic;
using Yarhl.FileFormat;

namespace Pleinair.Text.DAT.TALK
{
    public class Talk : Format
    {
        public int Count { get; set; }
        public int HeaderSize { get; set; }
        public int[] Positions { get; set; }
        public int[] Sizes { get; set; }
        public byte[][] Blocks { get; set; }
        public byte[][] HeaderEntries { get; set; }
        public List<TalkTextEntry> TextEntries { get; }

        public Talk()
        {
            TextEntries = new List<TalkTextEntry>();
        }

        public void InitializeArrays()
        {
            Positions = new int[Count];
            Sizes = new int[Count];
            Blocks = new byte[Count][];
            HeaderEntries = new byte[Count][];
        }
    }

    public class TalkTextEntry
    {
        public int TalkEntryId { get; set; }
        public int OffsetStart { get; set; }
        public int OffsetEnd { get; set; }
        public string Text { get; set; }
    }
}
