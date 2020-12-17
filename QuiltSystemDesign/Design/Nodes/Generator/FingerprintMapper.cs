//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

namespace RichTodd.QuiltSystem.Design.Nodes.Generator
{
    internal class FingerprintMapper<T> where T : Enum
    {
        private readonly int[,] m_fingerprintIndexes;
        private readonly PatchMapper<T>[,] m_patchMapppers;

        public FingerprintMapper(int[,] fingerprintIndexes, PatchMapper<T>[,] patchMappers)
        {
            m_fingerprintIndexes = fingerprintIndexes;
            m_patchMapppers = patchMappers;
        }

        public int ColumnCount
        {
            get { return m_fingerprintIndexes.GetLength(1); }
        }

        public int RowCount
        {
            get { return m_fingerprintIndexes.GetLength(0); }
        }

        public T Map(Fingerprint<T> fingerprint, int row, int column)
        {
            var fingerprintPatch = fingerprint.Values[m_fingerprintIndexes[row, column]];
            var patch = m_patchMapppers[row, column].Map(fingerprintPatch);
            return patch;
        }
    }
}