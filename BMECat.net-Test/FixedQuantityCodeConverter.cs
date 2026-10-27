using BMECat.net;

namespace BMECat.net_Test
{
    /// <summary>
    /// Converts every quantity code into the same, fixed clear text, e.g. to detect whether a converter is used at all
    /// </summary>
    internal class FixedQuantityCodeConverter : IQuantityCodeConverter
    {
        private readonly string _clearText;


        public FixedQuantityCodeConverter(string clearText)
        {
            _clearText = clearText;
        }


        public QuantityCodes Convert(string input)
        {
            return QuantityCodes.Unknown;
        }


        public string Convert(QuantityCodes? code, string defaultValue = null!)
        {
            return _clearText;
        }
    }
}
