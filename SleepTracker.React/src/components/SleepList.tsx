import {useState, useEffect} from 'react';
import { getSleeps } from '../services/sleep';
import type { SleepReadDto } from '../services/sleep';

export function SleepList() {
  const [sleeps, setSleeps] = useState<SleepReadDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [pageNumber, setPageNumber] = useState(1);
  const [pageSize] = useState(10);
  const [totalRecords, setTotalRecords] = useState(0);
  const [totalPages, setTotalPages] = useState(0);
  const [filterStartDate, setFilterStartDate] = useState<Date | null>(null);
  const [filterEndDate, setFilterEndDate] = useState<Date | null>(null);

  async function loadSleeps() {
    setLoading(true);
    setError(null);
    try {
      const response = await getSleeps(
        pageNumber, 
        pageSize,
        filterStartDate ?? undefined,
        filterEndDate ?? undefined
      );
      if (response.status === 0 && response.data) {
        setSleeps(response.data);
        setTotalRecords(response.totalRecords);
        setTotalPages(response.totalPages);
      } else {
        setError(response.message || 'Failed to load sleep records');
      }
    } catch (err) {
      setError('Failed to load sleep records. Please try again.');
      console.error('Error loading sleeps: ', err);
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    loadSleeps();
  }, [pageNumber, filterStartDate, filterEndDate]);

  function formatDateTime(dateTimeString: string): string {
    const date = new Date(dateTimeString);
    return date.toLocaleString('en-US', {
      month: 'short',
      day: 'numeric',
      year: 'numeric',
      hour: 'numeric',
      minute: '2-digit',
      hour12: true
    });
  }

  function formatDuration(start: string, end: string): string {
    const startDate = new Date(start);
    const endDate = new Date(end);
    const diffMs = endDate.getTime() - startDate.getTime();
    const diffSeconds = Math.floor(diffMs / 1000);
    const diffMinutes = Math.floor(diffSeconds / 60);
    const hours = Math.floor(diffMinutes / 60);
    const minutes = diffMinutes % 60;
    const seconds = diffSeconds % 60;
    if (diffSeconds < 60) {
      return `${seconds} second${seconds !== 1 ? 's' : ''}`;
    } else if (hours === 0) {
      return `${minutes} minute${minutes !== 1 ? 's' : ''}`;
    } else if (minutes === 0) {
      return `${hours} hour${hours !== 1 ? 's' : ''}`;
    } else {
      return `${hours} hour${hours !== 1 ? 's' : ''} and ${minutes} minute${minutes !== 1 ? 's' : ''}`;
    }
  }

  function openEditModal(_sleep: SleepReadDto) {
    // TODO: wire up in next step
  }

  function deleteSleepRecord(_sleep: SleepReadDto) {
    // TODO: wire up in next step
  }

  function goToFirstPage() {
    if (pageNumber <= 1) return;
    setPageNumber(1);
  }
  function goToPreviousPage() {
    if (pageNumber <= 1) return;
    setPageNumber(pageNumber - 1);
  }
  function goToNextPage() {
    if (pageNumber >= totalPages) return;
    setPageNumber(pageNumber + 1);
  }
  function goToLastPage() {
    if (pageNumber >= totalPages) return;
    setPageNumber(totalPages);
  }

  return (
    <div className="p-4 sleep-list-container">
      <div className="header-section flex flex-col gap-4 md:flex-row md:items-center md:justify-between mb-6">
        <div className="filter-section">
          <div className="filter-fields flex flex-wrap gap-2 items-center">
            <div className="flex flex-col gap-1 w-36 min-w-0 md:w-auto">
              <label htmlFor="filter-start">Sleep Date From</label>
              <input
                id="filter-start"
                type="date"
                className="input input-bordered pr-2 w-full"
                value={filterStartDate ? filterStartDate.toISOString().slice(0, 10) : ''}
                onChange={(e) => {
                  const val = e.target.value;
                  const nextStart = val ? new Date(val) : null;
                  setFilterStartDate(nextStart);
                  setPageNumber(1);
                  loadSleeps();
                }}
              />
            </div>
            <div className="flex flex-col gap-1 w-36 min-w-0 md:w-auto">
              <label htmlFor="filter-end">Sleep Date To</label>
              <input
                id="filter-end"
                type="date"
                className="input input-bordered pr-2 w-full"
                value={filterEndDate ? filterEndDate.toISOString().slice(0, 10) : ''}
                onChange={(e) => {
                  const val = e.target.value;
                  const nextEnd = val ? new Date(val) : null;
                  setFilterEndDate(nextEnd);
                  setPageNumber(1);
                  loadSleeps();
                }}
              />
            </div>
            {(filterStartDate || filterEndDate) && (
              <button
                type="button"
                className="btn btn-ghost btn-sm btn-circle text-error flex-shrink-0 ring-1 ring-error/25 !w-7 !h-7 !min-h-0 !min-w-0 p-0 flex items-center justify-center"
                onClick={() => {
                  setFilterStartDate(null);
                  setFilterEndDate(null);
                  setPageNumber(1);
                  loadSleeps();
                }}
                aria-label="Clear filters"
              >
                <svg xmlns="http://www.w3.org/2000/svg" className="w-3 h-3" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M6 18L18 6M6 6l12 12" />
                </svg>
              </button>
            )}
          </div>
        </div>
      </div>

      {loading && <p>Loading sleep records...</p>}
      {error && <p className="text-error">{error}</p>}
      {!loading && !error && sleeps.length === 0 && (
        <p>No sleep records found.</p>
      )}
      {!loading && !error && sleeps.length > 0 && (
        <>
          <div className="table-container overflow-x-auto mb-4 hidden md:block">
            <table className="table table-zebra">
              <thead>
                <tr>
                  <th>Sleep Date/Time</th>
                  <th>Wake Date/Time</th>
                  <th>Duration</th>
                  <th className="actions-column">Actions</th>
                </tr>
              </thead>
              <tbody>
                {sleeps.map((sleep) => (
                  <tr key={sleep.id}>
                    <td>{formatDateTime(sleep.start)}</td>
                    <td>{formatDateTime(sleep.end)}</td>
                    <td>{formatDuration(sleep.start, sleep.end)}</td>
                    <td className="actions-column">
                      <button type="button" onClick={() => openEditModal(sleep)} aria-label="Edit" className="btn btn-sm btn-ghost btn-circle">
                        <svg xmlns="http://www.w3.org/2000/svg" className="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M15.232 5.232l3.536 3.536m-2.036-5.036a2.5 2.5 0 113.536 3.536L6.5 21.036H3v-3.572L16.732 3.732z" />
                        </svg>
                      </button>
                      <button type="button" onClick={() => deleteSleepRecord(sleep)} aria-label="Delete" className="btn btn-sm btn-error btn-circle">
                        <svg xmlns="http://www.w3.org/2000/svg" className="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
                        </svg>
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>

          <div className="block md:hidden space-y-4">
            {sleeps.map((sleep) => (
              <div key={sleep.id} className="card bg-base-200 shadow compact">
                <div className="card-body gap-1">
                  <div className="flex justify-between gap-2">
                    <span className="font-medium text-base-content/70">Sleep Date/Time</span>
                    <span>{formatDateTime(sleep.start)}</span>
                  </div>
                  <div className="flex justify-between gap-2">
                    <span className="font-medium text-base-content/70">Wake Date/Time</span>
                    <span>{formatDateTime(sleep.end)}</span>
                  </div>
                  <div className="flex justify-between gap-2">
                    <span className="font-medium text-base-content/70">Duration</span>
                    <span>{formatDuration(sleep.start, sleep.end)}</span>
                  </div>
                  <div className="flex justify-end gap-2 pt-2">
                    <button type="button" className="btn btn-sm btn-ghost btn-circle" aria-label="Edit sleep record" onClick={() => openEditModal(sleep)}>
                      <svg xmlns="http://www.w3.org/2000/svg" className="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M15.232 5.232l3.536 3.536m-2.036-5.036a2.5 2.5 0 113.536 3.536L6.5 21.036H3v-3.572L16.732 3.732z" />
                      </svg>
                    </button>
                    <button type="button" className="btn btn-sm btn-error btn-circle" aria-label="Delete sleep record" onClick={() => deleteSleepRecord(sleep)}>
                      <svg xmlns="http://www.w3.org/2000/svg" className="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
                      </svg>
                    </button>
                  </div>
                </div>
              </div>
            ))}
          </div>

          <div className="pagination-info mt-4 text-center text-sm text-base-content/70">
            <p>Showing {sleeps.length} of {totalRecords} records (Page {pageNumber} of {totalPages})</p>
            {totalPages > 1 && (
              <div className="pagination-buttons flex gap-2 justify-center mt-2">
                <button type="button" disabled={pageNumber <= 1} onClick={goToFirstPage} className="btn btn-sm">First</button>
                <button type="button" disabled={pageNumber <= 1} onClick={goToPreviousPage} className="btn btn-sm">Previous</button>
                <button type="button" disabled={pageNumber >= totalPages} onClick={goToNextPage} className="btn btn-sm">Next</button>
                <button type="button" disabled={pageNumber >= totalPages} onClick={goToLastPage} className="btn btn-sm">Last</button>
              </div>
            )}
          </div>
        </>
      )}
    </div>
  );
}