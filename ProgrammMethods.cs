using System.Windows.Forms;
using Awesomium.Windows.Forms;


namespace ZaraCut
{
    class ProgrammMethods
    {
        NavigationOnWebControl NOW;
        public TabControlAwesomium TCA;

        public ProgrammMethods(TabControl tabControl, WebSessionProvider webSessionProvider)
        {
            NOW = new NavigationOnWebControl();
            TCA = new TabControlAwesomium(tabControl, webSessionProvider);
        }
    }
}
