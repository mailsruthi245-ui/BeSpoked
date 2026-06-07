interface Props {
  label: string;
  value: string | number;
  onChange: (val: string) => void;
  type?: string;
  min?: string | number;
  max?: string | number;
  step?: string | number;
}

export default function FormField({ label, value, onChange, type = 'text', min, max, step }: Props) {
  return (
    <div className="form-group">
      <label>{label}</label>
      <input
        type={type}
        value={value}
        min={min}
        max={max}
        step={step}
        onChange={e => onChange(e.target.value)}
        onFocus={e => { if (type === 'number' && +e.target.value === 0) e.target.select(); }}
      />
    </div>
  );
}
