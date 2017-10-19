using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Awesomium.Windows.Forms;

namespace ZaraCut
{
    class NavigationOnWebControl
    {
        Dictionary<GetElementBy, string> Elements;
        public NavigationOnWebControl()
        {
            Elements = new Dictionary<GetElementBy, string>();
            AddElements();
        }
        public enum GetElementBy
        {
            ClassName = 0,
            Id = 1,
            Name = 2
        }
        private void AddElements()
        {
            Elements.Add(GetElementBy.ClassName, "getElementsByClassName");
            Elements.Add(GetElementBy.Id, "getElementById");
            Elements.Add(GetElementBy.Name, "getElementsByName");
        }

        /// <summary>
        /// Нажимает кнопку на сайте
        /// </summary>
        /// <param name="webControl">webControl на котором будет выполнять метод</param>
        /// <param name="getElementBy">Атрибут поиска в теге</param>
        /// <param name="ElementValue">Значение атрибута, по которому будет происходить поиск</param>
        public void PressButton(WebControl webControl, GetElementBy getElementBy, string ElementValue)
        {
            webControl.ExecuteJavascript("document." + Elements[getElementBy] + "('" + ElementValue + "')[0].click()");
        }

        /// <summary>
        /// Обновит страницу на webControl
        /// </summary>
        /// <param name="webControl">webControl на котором будет выполнять метод</param>
        public void RefreshPage(WebControl webControl)
        {
            webControl.Reload(false);
        }

        /// <summary>
        /// Вернет индекс nameValueComboBox
        /// </summary>
        /// <param name="namesValueComboBox">Имена значений comboBox(по порядку, как на сайте)</param> // Так же можно найти скрипт на считывание этих значений
        /// <param name="nameValueComboBox">Имя значения, по которому будет сравнивать с массивом имен значений comboBox, для получения индекса значения в comboBox</param>
        /// <returns>Индекс, соответствующий значению nameValueComboBox из массива значений namesValueComboBox</returns>
        public int RetIndex(string[] namesValueComboBox, string nameValueComboBox)
        {

            for (int i = 0; i < namesValueComboBox.Length; i++)
            {
                if (nameValueComboBox == namesValueComboBox[i])
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Выбирает значение из comboBox на сайте
        /// </summary>
        /// <param name="webControl">webControl на котором будет выполнять метод</param>
        /// <param name="getElementBy">Атрибут поиска в теге</param>
        /// <param name="nuberComboBoxValue">Индекс, по которому выберет значение из comboBox</param>
        public void SelectComboBoxValue(WebControl webControl, GetElementBy getElementBy, string ElementValue, int nuberComboBoxValue)
        {
            webControl.ExecuteJavascript("document." + Elements[getElementBy] + "('" + ElementValue + "').selectedIndex = " + nuberComboBoxValue);
        }

        /// <summary>
        /// Записывает в поле на сайта
        /// </summary>
        /// <param name="webControl">webControl на котором будет выполнять метод</param>
        /// <param name="getElementBy">Атрибут поиска в теге</param>
        /// <param name="AttributeValue">Значение атрибута, по которому происходит поиск элемента</param>
        /// <param name="value">Значение, которое запишет в поле на сайте</param>
        public void WriteInField(WebControl webControl, GetElementBy getElementBy, string AttributeValue, string value)
        {
            webControl.ExecuteJavascript("document." + Elements[getElementBy] + "('" + AttributeValue + "')[0].value = " + "'" + value + "'");
        }
    }
}
