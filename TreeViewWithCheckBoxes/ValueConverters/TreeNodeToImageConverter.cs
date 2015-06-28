using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Bornander.UI.ViewModels;

namespace TreeViewWithCheckBoxes.ValueConverters {
    //[ValueConversion(typeof(EntityViewModel<>), typeof(ImageSource))]
    //public class TreeNodeToImageConverter : IValueConverter
    //{
    //    private const string UriFormat = "pack://application:,,,/Resources/Images/{0}";
    //    private static readonly IDictionary<string, ImageSource> SuffixToImageMap = new Dictionary<string, ImageSource>();

    //    private static readonly ImageSource FolderSource = new BitmapImage(new Uri(String.Format(UriFormat, "Folder.png")));

    //    static TreeNodeToImageConverter()
    //    {
    //        SuffixToImageMap[".exe"] = new BitmapImage(new Uri(String.Format(UriFormat, "Executable.png")));
    //        SuffixToImageMap[".zip"] = new BitmapImage(new Uri(String.Format(UriFormat, "Archive.png")));
    //        SuffixToImageMap[".png"] = SuffixToImageMap[".jpeg"] = SuffixToImageMap[".jpg"] = new BitmapImage(new Uri(String.Format(UriFormat, "Picture.png")));
    //        SuffixToImageMap[".txt"] = new BitmapImage(new Uri(String.Format(UriFormat, "Text.png")));
    //    }

    //    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    //    {
    //        var viewModel = value as EntityViewModel;
    //        if (viewModel == null)
    //            return Binding.DoNothing;

    //        if (!viewModel.IsLeaf)
    //            return FolderSource;
    //        else
    //        {
    //            var source = SuffixToImageMap.Where(kvp => viewModel.Name.EndsWith(kvp.Key)).Select(kvp => kvp.Value).FirstOrDefault();
    //            return source ?? Binding.DoNothing;
    //        }
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}
