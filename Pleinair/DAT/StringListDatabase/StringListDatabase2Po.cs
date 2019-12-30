using Yarhl.FileFormat;
using Yarhl.Media.Text;

namespace Pleinair.DAT.StringListDatabase
{
    public class StringListDatabase2Po : IConverter<Stringlistdatabase, Po>
    {
        private Po _po;

        public StringListDatabase2Po()
        {
            //Read the language used by the user' OS, this way the editor can spellcheck the translation. - Thanks Liquid_S por the code
            System.Globalization.CultureInfo currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
            _po = new Po
            {
                Header = new PoHeader("Disgaea 2", "dummy@dummy.com", currentCulture.Name)
            };
        }
        
        public Po Convert(Stringlistdatabase source)
        {
            for (int i = 0; i < source.Count; i++)
            {
                PoEntry entry = new PoEntry(); //Generate the entry on the po file
                entry.Original = !string.IsNullOrWhiteSpace(source.Strings[i]) ? source.Strings[i] : "<!null>";
                entry.Context = i.ToString();
                _po.Add(entry);
            }
            
            return _po;
        }
    }
}