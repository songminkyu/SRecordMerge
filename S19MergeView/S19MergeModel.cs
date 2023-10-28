using S19Merge.BindServices;

namespace S19Merge.Model
{
    public class SRecord : BindableBase
    {
        public string? Type
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }
        public string? Address
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }
        public string? DataLen
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }
        public string? DataString
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }     
    }
}
