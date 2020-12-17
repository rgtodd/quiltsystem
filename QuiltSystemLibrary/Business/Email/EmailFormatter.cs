//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
namespace RichTodd.QuiltSystem.Business.Email
{
    public abstract class EmailFormatter
    {
        public abstract string GetHtml();

        public abstract string GetText();

        public abstract string GetSubject();
    }
}