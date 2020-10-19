using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Playground.Winforms.HelperForms
{
    public sealed partial class MessageBoxWithTextBox : BaseForm
    {
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private List<Button> _buttonList;

        public MessageBoxWithTextBox(String text, String caption, List<Button> buttons, Icon icon)
        {
            InitializeComponent();

            Text = caption;

            lblIcon.Text = null;
            // ReSharper disable once MergeConditionalExpression
            lblIcon.Image = icon != null ? icon.ToBitmap() : null;

            rtbMessage.Text = text;

            _buttonList = buttons;
            for (int i = 0; i < _buttonList.Count; i++)
            {
                _buttonList[i].Click += button_Click;
                tlpButtons.ColumnStyles.Insert(i + 1, new ColumnStyle(SizeType.AutoSize));
                tlpButtons.ColumnCount++;
                tlpButtons.Controls.Add(_buttonList[i], i + 1, 0);
            }
        }
        private void button_Click(Object sender, EventArgs e)
        {
            Button button = (Button)sender;
            DialogResult = GetDialogResultFromString(button.Text);
            Close();
        }

        public static DialogResult Show(String text)
        {
            return Show(text, "", MessageBoxButtons.OK, MessageBoxIcon.None);
        }
        public static DialogResult Show(String text, String caption)
        {
            return Show(text, caption, MessageBoxButtons.OK, MessageBoxIcon.None);
        }
        public static DialogResult Show(String text, String caption, MessageBoxButtons buttons)
        {
            return Show(text, caption, buttons, MessageBoxIcon.None);
        }
        public static DialogResult Show(String text, String caption, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            Icon actualIcon = GetIcon(icon);

            MessageBoxWithTextBox popup = new MessageBoxWithTextBox(text, caption, GetButtonsFromMessageBoxButtons(buttons), actualIcon);

            return popup.ShowDialog();
        }

        private static List<Button> GetButtonsFromMessageBoxButtons(MessageBoxButtons buttons)
        {
            List<Button> list = new List<Button>();
            Button button;
            switch (buttons)
            {
                case MessageBoxButtons.AbortRetryIgnore:
                    button = new Button();
                    button.Text = @"Abort";
                    list.Add(button);
                    button = new Button();
                    button.Text = @"Retry";
                    list.Add(button);
                    button = new Button();
                    button.Text = @"Ignore";
                    list.Add(button);
                    break;
                case MessageBoxButtons.OK:
                    button = new Button();
                    button.Text = @"OK";
                    list.Add(button);
                    break;
                case MessageBoxButtons.OKCancel:
                    button = new Button();
                    button.Text = @"OK";
                    list.Add(button);
                    button = new Button();
                    button.Text = @"Cancel";
                    list.Add(button);
                    break;
                case MessageBoxButtons.RetryCancel:
                    button = new Button();
                    button.Text = @"Retry";
                    list.Add(button);
                    button = new Button();
                    button.Text = @"Cancel";
                    list.Add(button);
                    break;
                case MessageBoxButtons.YesNo:
                    button = new Button();
                    button.Text = @"Yes";
                    list.Add(button);
                    button = new Button();
                    button.Text = @"No";
                    list.Add(button);
                    break;
                case MessageBoxButtons.YesNoCancel:
                    button = new Button();
                    button.Text = @"Yes";
                    list.Add(button);
                    button = new Button();
                    button.Text = @"No";
                    list.Add(button);
                    button = new Button();
                    button.Text = @"Cancel";
                    list.Add(button);
                    break;
            }
            return list;
        }
        private static DialogResult GetDialogResultFromString(String text)
        {
            switch (text)
            {
                case "Abort":
                    return DialogResult.Abort;
                case "Cancel":
                    return DialogResult.Cancel;
                case "Ignore":
                    return DialogResult.Ignore;
                case "No":
                    return DialogResult.No;
                case "OK":
                    return DialogResult.OK;
                case "Retry":
                    return DialogResult.Retry;
                case "Yes":
                    return DialogResult.Yes;
                default:
                    return DialogResult.None;
            }
        }
        private static Icon GetIcon(MessageBoxIcon icon)
        {
            switch (icon)
            {
                case MessageBoxIcon.Error:
                    return SystemIcons.Error;
                case MessageBoxIcon.Exclamation:
                    return SystemIcons.Exclamation;
                case MessageBoxIcon.Information:
                    return SystemIcons.Asterisk;
                case MessageBoxIcon.Question:
                    return SystemIcons.Question;
                default:
                    return null;
            }
        }
    }
}
