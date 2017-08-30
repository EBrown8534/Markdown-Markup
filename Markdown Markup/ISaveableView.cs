using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Markdown_Markup
{
    public interface ISaveableView
    {
        /// <summary>
        /// If true, then the view has changes to save.
        /// </summary>
        bool MustSave { get; }
        /// <summary>
        /// Performs what saving the view might need to do and returns true if saved successfully.
        /// </summary>
        /// <returns>Returns true if the view successfully saved the changes, false otherwise.</returns>
        bool Save();
        /// <summary>
        /// Returns a string that represents the changes that must be saved to show to the user.
        /// </summary>
        string UnsavedChanges { get; }
    }
}
