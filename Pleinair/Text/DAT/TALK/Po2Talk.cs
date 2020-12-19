using System.IO;
using Yarhl.FileFormat;
using Yarhl.Media.Text;

namespace Pleinair.Text.DAT.TALK
{
    public class Po2Talk : IConverter<Po, Talk>
    {
        public BinaryFormat OriginalFile { get; set; }
        private Talk talk;

        public Talk Convert(Po source)
        {
            if (OriginalFile == null)
                throw new FileNotFoundException("It's necessary the original file for import the texts.");

            InstanceTalk();
            ParseTextEntries(source);

            return talk;
        }

        private void InstanceTalk()
        {
            var converter = new Binary2Talk()
            {
                GetText = false
            };
            talk = converter.Convert(OriginalFile);
        }

        private void ParseTextEntries(Po po)
        {
            foreach (var entry in po.Entries)
            {
                var info = entry.Context.Split('|');
                talk.TextEntries.Add(new TalkTextEntry()
                {
                    Text = entry.Text,
                    TalkEntryId = System.Convert.ToInt32(info[0]),
                    OffsetStart = System.Convert.ToInt32(info[1]),
                    OffsetEnd = System.Convert.ToInt32(info[2]),
                });
            }
        }
    }
}
