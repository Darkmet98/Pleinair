using Yarhl.FileFormat;
using Yarhl.Media.Text;

namespace Pleinair.Text.DAT.TALK
{
    public class Talk2Po : IConverter<Talk, Po>
    {
        public Po Convert(Talk source)
        {
            //Read the language used by the user' OS, this way the editor can spellcheck the translation. - Thanks Liquid_S por the code
            var currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
            var po = new Po
            {
                Header = new PoHeader("Disgaea", "dummy@dummy.com", currentCulture.Name)
            };

            foreach (var entry in source.TextEntries)
            {
                po.Add(new PoEntry(entry.Text)
                {
                    Context = $"{entry.TalkEntryId}|{entry.OffsetStart}|{entry.OffsetEnd}"
                });
            }

            return po;
        }
    }
}
