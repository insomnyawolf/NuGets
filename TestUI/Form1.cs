using LowLevelHooks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestUI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            var keysTracked = new List<KeyTracked>()
            {
                new KeyTracked()
                {
                    Key = 'Z',
                },
                new KeyTracked()
                {
                    Key = 'C',
                },
            };

            var keyboardHook = new KeyboardHook();

            keyboardHook.OnKeyDown += (character) =>
            {
                var ch = (char)character;

                Output.Text = "";

                for (int i = 0; i < keysTracked.Count; i++)
                {
                    var current = keysTracked[i];
                    if (current.Key == ch)
                    {
                        current.Count++;
                    }

                    Output.Text += $"Key: '{current.Key}' => {current.Count}\r\n";
                }

            };
        }

        class KeyTracked
        {
            public char Key { get; set; }
            public long Count { get; set; }
        }
    }
}
