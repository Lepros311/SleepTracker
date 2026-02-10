import {useToast} from '../contexts/ToastContext';

export function ToastContainer() {
  const {toasts, dismiss} = useToast();

  return (
    <div className="toast toast-bottom toast-center z-[9999] flex flex-col gap-2 p-4">
      {toasts.map((t) => (
        <div
          key={t.id}
          className={`alert shadow-lg ${t.type === 'success' ? 'alert-success' : 'alert-error'}`}
          role="alert"
        >
          <span>{t.message}</span>
          <button
            type="button"
            className="btn btn-ghost btn-xs"
            aria-label="Dismiss"
            onClick={() => dismiss(t.id)}
          >
            X
          </button>
        </div>
      ))}
    </div>
  );
}