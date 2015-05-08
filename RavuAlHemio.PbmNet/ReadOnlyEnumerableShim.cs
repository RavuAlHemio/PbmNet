using System.Collections;
using System.Collections.Generic;

namespace RavuAlHemio.PbmNet
{
    public class ReadOnlyEnumerableShim<T> : IEnumerable<T>
    {
        private readonly IEnumerable<T> _enumerable;

        class Enumerator : IEnumerator<T>
        {
            private readonly IEnumerator<T> _enumerator;

            internal Enumerator(IEnumerator<T> enumerator)
            {
                _enumerator = enumerator;
            }

            public bool MoveNext()
            {
                return _enumerator.MoveNext();
            }

            public void Reset()
            {
                _enumerator.Reset();
            }

            public T Current { get { return _enumerator.Current; } }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            public void Dispose()
            {
                _enumerator.Dispose();
            }
        }

        public ReadOnlyEnumerableShim(IEnumerable<T> enumerable)
        {
            _enumerable = enumerable;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new Enumerator(_enumerable.GetEnumerator());
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
