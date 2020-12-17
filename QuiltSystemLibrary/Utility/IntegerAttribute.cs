//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//

//using System.ComponentModel.DataAnnotations;

//namespace RichTodd.QuiltSystem.Utility
//{
//    public class IntegerAttribute : ValidationAttribute
//    {
//        private int m_maxLength;

//        public IntegerAttribute(int maxLength)
//            : base()
//        {
//            m_maxLength = maxLength;
//        }

//        public int MaxLength
//        {
//            get { return m_maxLength; }
//        }

//        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
//        {
//            if (value == null)
//            {
//                return ValidationResult.Success;
//            }

//            int intValue;
//            if (value is int)
//            {
//                intValue = (int)value;
//            }
//            else if (!int.TryParse(value.ToString(), out intValue))
//            {
//                return new ValidationResult("Value must be an integer.");
//            }

//            if (intValue.ToString().Length > MaxLength)
//            {
//                return new ValidationResult("Value must be an integer.");
//            }

//            return ValidationResult.Success;
//        }
//    }
//}
