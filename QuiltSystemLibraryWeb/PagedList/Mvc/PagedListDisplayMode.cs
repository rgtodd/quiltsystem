//
// Source code derived from the GitHub project at https://github.com/troygoode/PagedList
// Licensed under the MIT license.  See LICENSE.txt project file for more information.
//
namespace PagedList.Mvc
{
    /// <summary>
    /// A tri-state enum that controls the visibility of portions of the PagedList paging control.
    /// </summary>
    public enum PagedListDisplayMode
    {
        /// <summary>
        /// Always render.
        /// </summary>
        Always,

        /// <summary>
        /// Never render.
        /// </summary>
        Never,

        /// <summary>
        /// Only render when there is data that makes sense to show (context sensitive).
        /// </summary>
        IfNeeded
    }
}