import React from 'react';

interface ModalProps {
  title: string;
  onClose: () => void;
  onSave: () => void;
  saveLabel?: string;
  children: React.ReactNode;
}

export default function Modal({ title, onClose, onSave, saveLabel = 'Save', children }: ModalProps) {
  return (
    <div className="modal-overlay" onClick={onClose}>
      <div className="modal" onClick={e => e.stopPropagation()}>
        <h2>{title}</h2>
        <div className="form-grid">{children}</div>
        <div className="modal-actions">
          <button className="btn btn-secondary" onClick={onClose}>Cancel</button>
          <button className="btn btn-primary" onClick={onSave}>{saveLabel}</button>
        </div>
      </div>
    </div>
  );
}
