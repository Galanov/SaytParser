using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ZaraCut.Core
{
    public class Result 
    {
        public void Message(Color colorMessage, string text)
        {
            this.MessageColor = colorMessage;
            this.MessageText  = text;
        }
        public Result(Label label)
        {
            this.label = label;
        }
        private Label label;
        public string MessageText
        {
            set
            {
                label.Text = value;
            }
        }
        public Color MessageColor
        {
            set
            {
                label.ForeColor = value;
            }
        }
    }
}
