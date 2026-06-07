import { useState, useEffect, useRef } from 'react';

export function useFetch<T>(fetchFn: () => Promise<T>) {
  const [data, setData]     = useState<T | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError]   = useState('');
  const ref = useRef(fetchFn);

  useEffect(() => {
    ref.current()
      .then(setData)
      .catch((e: unknown) => setError(e instanceof Error ? e.message : 'Failed to load.'))
      .finally(() => setLoading(false));
  }, []);

  return { data, loading, error, setData, setError };
}
