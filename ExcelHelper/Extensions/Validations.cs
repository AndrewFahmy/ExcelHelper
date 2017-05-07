using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ExcelHelper.Extensions
{
    public static class Validations
    {
        public static bool IsValid(this TextBox tb)
        {
            if (!string.IsNullOrWhiteSpace(tb.Text)) return true;

            tb.Style = (Style)Application.Current.Resources["TextBoxRequiredStyle"];
            Keyboard.ClearFocus();
            return false;
        }

        public static bool IsValid(this ComboBox cb)
        {
            if (cb.SelectedValue != null) return true;

            cb.Style = (Style)Application.Current.Resources["ComboBoxRequiredStyle"];
            Keyboard.ClearFocus();
            return false;
        }

        public static bool IsValid(this DatePicker dp)
        {
            if (dp.SelectedDate.HasValue) return true;

            dp.Style = (Style)Application.Current.Resources["DatePickerRequiredStyle"];
            Keyboard.ClearFocus();
            return false;
        }

        public static bool IsValid(this PasswordBox pb)
        {
            if (!string.IsNullOrWhiteSpace(pb.Password)) return true;

            pb.Style = (Style)Application.Current.Resources["PasswordBoxRequiredStyle"];
            Keyboard.ClearFocus();
            return false;
        }


        public static void ResetValidation(this TextBox tb)
        {
            tb.Style = (Style)Application.Current.Resources["CrudTextBoxStyle"];
        }

        public static void ResetValidation(this ComboBox cb)
        {
            cb.Style = (Style)Application.Current.Resources["CrudComboBoxStyle"];
        }

        public static void ResetValidation(this DatePicker db)
        {
            db.Style = (Style)Application.Current.Resources["CrudDatePickerStyle"];
        }

        public static void ResetValidation(this PasswordBox pb)
        {
            pb.Style = (Style)Application.Current.Resources["PasswordBoxStyle"];
        }
    }
}