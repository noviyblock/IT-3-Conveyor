using Avalonia.Controls;
using Avalonia.Controls.Templates;
using System;
using System.Linq;
using MyApp.ViewModels;

namespace MyApp
{
    public class ViewLocator : IDataTemplate
    {
        public Control Build(object data)
        {
            if (data is null)
                return null;

            var name = data.GetType().FullName!
                .Replace("ViewModel", "View")
                .Replace("ViewModels", "Views");

            var type = Type.GetType(name);

            if (type != null)
            {
                return (Control)Activator.CreateInstance(type)!;
            }
            else
            {
                // Fallback for generic view models
                var baseName = name.Split('`').First();
                var fallbackType = Type.GetType(baseName);
                if (fallbackType != null)
                {
                    return (Control)Activator.CreateInstance(fallbackType)!;
                }
            }

            return new TextBlock { Text = $"View Not Found: {name}" };
        }

        public bool Match(object data)
        {
            return data is ViewModelBase;
        }
    }
}