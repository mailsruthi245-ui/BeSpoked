import { useState } from 'react';

export function useEditState<T>() {
  const [editing, setEditing] = useState<T | null>(null);
  const set = (key: keyof T, val: unknown) =>
    setEditing(prev => prev ? { ...prev, [key]: val } : null);
  return { editing, setEditing, set };
}
