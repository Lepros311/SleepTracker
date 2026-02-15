import {useState, useEffect} from 'react'
import { ToastContainer } from './components/ToastContainer';
import { SleepList } from './components/SleepList';

function App() {
  const [theme, setTheme] = useState<'light' | 'dark'>('light');

  useEffect(() => {
    const saved = localStorage.getItem('theme');
    if (saved === 'light' || saved === 'dark') {
      setTheme(saved);
      document.documentElement.setAttribute('data-theme', saved);
    } else {
      document.documentElement.setAttribute('data-theme', 'light');
    }
  }, []);

  function toggleTheme() {
    const next = theme === 'light' ? 'dark' : 'light';
    setTheme(next);
    document.documentElement.setAttribute('data-theme', next);
    localStorage.setItem('theme', next);
  }

  return (
    <div className="min-h-screen w-full flex flex-col">
      <header className="navbar bg-base-200 px-6 py-5 shadow-md border-b border-base-300">
        <div className="flex-1">
          <h1 className="text-3xl font-bold tracking-tight">Sleep Tracker</h1>
        </div>
        <div className="flex-none">
          <div className="flex items-center gap-2">
            <span className="text-base-content/70" aria-hidden="true">
              <svg xmlns="http://www.w3.org/2000/svg" className="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M12 3v1m0 16v1m9-9h-1M4 12H3m15.364 6.364l-.707-.707M6.343 6.343l-.707-.707m12.728 0l-.707.707M6.343 17.657l-.707.707M16 12a4 4 0 11-8 0 4 4 0 018 0z" />
              </svg>
            </span>
            <input
              type="checkbox"
              className="toggle toggle-primary"
              checked={theme === 'dark'}
              onChange={toggleTheme}
              aria-label={theme === 'light' ? 'Switch to dark mode' : 'Switch to light mode'}
            />
            <span className="text-base-content/70" aria-hidden="true">
              <svg xmlns="http://www.w3.org/2000/svg" className="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M20.354 15.354A9 9 0 018.646 3.646 9.003 9.003 0 0012 21a9.003 9.003 0 008.354-5.646z" />
              </svg>
            </span>
          </div>
        </div>
      </header>
      <main className="flex-1">
        <SleepList />
      </main>
      <ToastContainer />
    </div>
  );
}

export default App
